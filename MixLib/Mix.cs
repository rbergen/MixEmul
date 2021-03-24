using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using MixLib.Events;
using MixLib.Instruction;
using MixLib.Interrupts;
using MixLib.Misc;
using MixLib.Modules;
using MixLib.Modules.Settings;
using MixLib.Settings;
using MixLib.Type;

namespace MixLib
{
	public class Mix : ModuleBase, IDisposable
	{
		private const string MyModuleName = "Mix";
		private const int MemoryMinIndex = -3999;
		private const int MemoryMaxIndex = 3999;
		private const int TimerMemoryAddress = -10;

		private readonly Devices _devices = new();
		private readonly Memory _fullMemory = new(MemoryMinIndex, MemoryMaxIndex);
		private readonly Registers _registers = new();
		private readonly MemoryView _memory;
		private readonly Queue<Interrupt> _interruptQueue = new();

		private int _currentInstructionAddress;
		private int _currentInstructionTicksLeft;
		private string _currentInstructionMnemonic;
		private readonly ConcurrentQueue<LogLine> _log;
		private bool _runDetached;
		private TimeSpan _runSpan;
		private readonly AutoResetEvent _startStep;
		private RunStatus _status;
		private RunMode _mode;
		private bool _interruptExecuted;
		private readonly object _syncRoot = new();

		public FloatingPointModule FloatingPointModule { get; private set; }
		public long TickCounter { get; private set; }

		public event EventHandler LogLineAdded;
		public event EventHandler StepPerformed;

		public Mix()
		{
			SetFloatingPointModuleEnabled(ModuleSettings.FloatingPointEnabled);

			_memory = new MemoryView(_fullMemory);

			_devices.DeviceReportingEvent += DeviceReporting;
			_log = new ConcurrentQueue<LogLine>();
			ProgramCounter = 0;
			TickCounter = 0L;
			_status = RunStatus.Idle;
			_mode = RunMode.Normal;

			SetMemoryBounds();

			_currentInstructionAddress = int.MinValue;
			_currentInstructionMnemonic = null;
			_currentInstructionTicksLeft = int.MinValue;
			_interruptExecuted = false;

			BreakpointManager = null;
			_startStep = new AutoResetEvent(false);
			_runDetached = false;

			var thread = new Thread(StepRunEntryPoint)
			{
				IsBackground = true,
				Name = "Run thread"
			};

			thread.Start();
		}

		public override Devices Devices
			=> _devices;

		public bool IsError
			=> Status == RunStatus.InvalidInstruction || Status == RunStatus.RuntimeError;

		public bool IsExecuting
			=> IsRunning || Status == RunStatus.Stepping;

		public bool IsRunning
			=> Status == RunStatus.Running || Status == RunStatus.Halted;

		public override IMemory FullMemory
			=> _fullMemory;

		public override IMemory Memory
			=> _memory;

		public override Registers Registers
			=> _registers;

		public override string ModuleName
			=> MyModuleName;

		public void QueueInterrupt(Interrupt interrupt)
			=> _interruptQueue.Enqueue(interrupt);

		public void ContinueRun()
			=> ContinueRun(new TimeSpan(0L));

		public LogLine GetLogLine()
			=> _log.TryDequeue(out LogLine line) ? line : null;

		public void RequestStop()
			=> RequestStop(true);

		private void DeviceReporting(object sender, DeviceReportingEventArgs args)
			=> AddLogLine(new LogLine(ModuleName, args.Severity, "Device message", string.Concat(new object[] { "Device ", args.ReportingDevice.Id, ": ", args.Message })));

		private void SetMemoryBounds()
		{
			_memory.IndexOffset = 0;
			_memory.MinWordIndex = Mode == RunMode.Control ? _fullMemory.MinWordIndex : 0;
			_memory.MaxWordIndex = _fullMemory.MaxWordIndex;

			if (ProgramCounter < _memory.MinWordIndex)
				ProgramCounter = _memory.MinWordIndex;
		}

		public void SetFloatingPointModuleEnabled(bool enabled)
		{
			if (enabled && FloatingPointModule == null)
				FloatingPointModule = new FloatingPointModule(this);

			else if (!enabled && FloatingPointModule != null)
				FloatingPointModule = null;
		}

		public override void AddLogLine(LogLine line)
		{
			_log.Enqueue(line);

			LogLineAdded?.Invoke(this, new EventArgs());
		}

		public void ContinueRun(TimeSpan runSpan)
		{
			_runSpan = runSpan;
			_startStep.Set();
		}

		public override void Halt(int code)
		{
			AddLogLine(new LogLine(ModuleName, Severity.Info, ProgramCounter, "Halted", string.Format("System halted with code {0}", code)));
			Status = RunStatus.Halted;
		}

		public void RequestStop(bool reportStop)
		{
			if (reportStop)
				AddLogLine(new LogLine(ModuleName, Severity.Info, ProgramCounter, "Stopped", "Stop requested"));

			Status = RunStatus.Idle;
		}

		public override void Reset()
		{
			_fullMemory.Reset();
			_registers.Reset();
			_devices.Reset();

			ProgramCounter = 0;
			TickCounter = 0L;
			_currentInstructionAddress = int.MinValue;
			_currentInstructionMnemonic = null;
			_currentInstructionTicksLeft = int.MinValue;
			_interruptExecuted = false;

			_interruptQueue.Clear();

			AddLogLine(new LogLine(ModuleName, Severity.Info, "Reset", "System reset"));

			Mode = RunMode.Normal;
			Status = RunStatus.Idle;
		}

		public void ResetTickCounter()
			=> TickCounter = 0L;

		public void StartRun()
		{
			Status = RunStatus.Running;
			_runSpan = new TimeSpan(0L);
			_startStep.Set();
		}

		public void StartStep()
		{
			Status = RunStatus.Stepping;
			_startStep.Set();
		}

		private void StepRunEntryPoint()
		{
			var stepStartTime = new DateTime();
			bool suspendRun = true;

			while (true)
			{
				if (suspendRun)
				{
					_startStep.WaitOne();
					stepStartTime = DateTime.Now;
				}
				else
					_startStep.Reset();

				do Tick();
				while ((_currentInstructionAddress == ProgramCounter) && IsExecuting);

				suspendRun = !IsRunning || (DateTime.Now - stepStartTime > _runSpan && !RunDetached);

				if (suspendRun)
					StepPerformed?.Invoke(this, new EventArgs());
			}
		}

		private void IncreaseTickCounter()
		{
			TickCounter++;
			if (TickCounter % 1000 == 0)
			{
				long timerValue = _fullMemory[TimerMemoryAddress].LongValue;
				if (timerValue > 0)
				{
					_fullMemory[TimerMemoryAddress].LongValue = timerValue - 1;

					QueueInterrupt(new Interrupt(Interrupt.Types.Timer));
				}
			}

			_devices.Tick();
		}

		public void PrepareLoader()
		{
			var instruction = InstructionSet.Instance.GetInstruction(IOInstructions.INOpCode, new FieldSpec(Devices.CardReaderUnitCode));

			var instructionWord = new FullWord
			{
				[MixInstruction.OpcodeByte] = IOInstructions.INOpCode,
				[MixInstruction.FieldSpecByte] = Devices.CardReaderUnitCode
			};
			var instance = instruction.CreateInstance(instructionWord);

			int ticksLeft = instruction.TickCount;
			while (ticksLeft-- > 0)
				IncreaseTickCounter();

			instance.Execute(this);

			while (_devices[Devices.CardReaderUnitCode].Busy)
				IncreaseTickCounter();

			ProgramCounter = 0;

			IncreaseTickCounter();

			_registers.RJ.LongValue = 0;

			IncreaseTickCounter();
		}

		public void SignalInterruptExecuted()
			=> _interruptExecuted = true;

		public override void ResetProfilingCounts()
		{
			_fullMemory.ResetProfilingCounts();
			if (FloatingPointModule != null)
				FloatingPointModule.ResetProfilingCounts();
		}

		public void Tick()
		{
			if (Mode != RunMode.Module)
			{
				if (_interruptQueue.Count > 0 && _currentInstructionTicksLeft == 0 && Mode != RunMode.Control && (Status == RunStatus.Running || Status == RunStatus.Stepping || Status == RunStatus.Idle))
				{
					if (_interruptExecuted)
						_interruptExecuted = false;

					else
						InterruptHandler.HandleInterrupt(this, _interruptQueue.Dequeue());
				}

				IncreaseTickCounter();

				if (Status == RunStatus.Halted)
				{
					if (!_devices.IsAnyBusy)
						Status = RunStatus.Idle;

					return;
				}
			}

			IMemoryFullWord instructionWord = Memory[ProgramCounter];
			var instruction = InstructionSet.Instance.GetInstruction(instructionWord[MixInstruction.OpcodeByte], new FieldSpec(instructionWord[MixInstruction.FieldSpecByte]));

			if (instruction == null)
			{
				ReportInvalidInstruction("Opcode (and field) do not encode an instruction");
				if (Mode == RunMode.Module)
					ResetMode();

				return;
			}

			var instance = instruction.CreateInstance(instructionWord);
			var errors = instance.Validate();
			if (errors != null)
			{
				ReportInvalidInstruction(errors);
				if (Mode == RunMode.Module)
					ResetMode();

				return;
			}

			if (_currentInstructionAddress != ProgramCounter || _currentInstructionMnemonic != instruction.Mnemonic || (_currentInstructionTicksLeft <= 0 && Mode != RunMode.Module))
			{
				_currentInstructionAddress = ProgramCounter;
				_currentInstructionTicksLeft = instruction.TickCount;
				_currentInstructionMnemonic = instruction.Mnemonic;

				if (Mode == RunMode.Module)
					ResetMode();
			}

			if (Mode != RunMode.Module && _currentInstructionTicksLeft > 0)
			{
				_currentInstructionTicksLeft--;

				if (ExecutionSettings.ProfilingEnabled)
					instructionWord.IncreaseProfilingTickCount(1);
			}

			int programCounter;

			if (Mode == RunMode.Module || _currentInstructionTicksLeft == 0)
			{
				var increasePC = instance.Execute(this);

				programCounter = ProgramCounter;

				if (increasePC)
					programCounter++;

				if (ExecutionSettings.ProfilingEnabled && (Mode != RunMode.Module || increasePC))
					instructionWord.IncreaseProfilingExecutionCount();
			}
			else
			{
				programCounter = ProgramCounter;
			}

			if (programCounter > _memory.MaxWordIndex)
			{
				ProgramCounter = _memory.MaxWordIndex;
				ReportRuntimeError("Program counter overflow");
			}
			else
			{
				ProgramCounter = programCounter;
				if (Status == RunStatus.Running && IsBreakpointSet(programCounter))
					ReportBreakpointReached();
			}
		}

		private void ResetMode()
			=> Mode = ProgramCounter < 0 ? RunMode.Control : RunMode.Normal;

		public bool RunDetached
		{
			get
			{
				lock (_syncRoot)
				{
					return _runDetached;
				}
			}
			set
			{
				lock (_syncRoot)
				{
					_runDetached = value;
				}
			}
		}

		public override RunMode Mode
		{
			get => _mode;
			set
			{
				if (_mode != value)
				{
					_mode = value;

					SetMemoryBounds();
				}
			}
		}

		public override RunStatus Status
		{
			get
			{
				lock (_syncRoot)
				{
					return _status;
				}
			}

			protected set
			{
				lock (_syncRoot)
				{
					_status = value;
				}
			}
		}

		public bool NeutralizeStatus()
		{
			if (Status == RunStatus.BreakpointReached || IsError)
			{
				Status = RunStatus.Idle;
				return true;
			}

			return false;
		}

		#region IDisposable Support
		private bool disposedValue;

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
					_startStep?.Dispose();

				disposedValue = true;
			}
		}

		~Mix()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);

			GC.SuppressFinalize(this);
		}
		#endregion
	}
}
