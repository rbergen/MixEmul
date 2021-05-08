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

		private readonly Devices devices = new();
		private readonly Memory fullMemory = new(MemoryMinIndex, MemoryMaxIndex);
		private readonly Registers registers = new();
		private readonly MemoryView memory;
		private readonly Queue<Interrupt> interruptQueue = new();

		private int currentInstructionAddress;
		private int currentInstructionTicksLeft;
		private string currentInstructionMnemonic;
		private readonly ConcurrentQueue<LogLine> log;
		private bool runDetached;
		private TimeSpan runSpan;
		private readonly AutoResetEvent startStep;
		private RunStatus status;
		private RunMode mode;
		private bool interruptExecuted;
		private readonly object syncRoot = new();

		public FloatingPointModule FloatingPointModule { get; private set; }
		public long TickCounter { get; private set; }

		public event EventHandler LogLineAdded;
		public event EventHandler StepPerformed;

		public Mix()
		{
			SetFloatingPointModuleEnabled(ModuleSettings.FloatingPointEnabled);

			this.memory = new MemoryView(this.fullMemory);

			this.devices.DeviceReportingEvent += DeviceReporting;
			this.log = new ConcurrentQueue<LogLine>();
			ProgramCounter = 0;
			TickCounter = 0L;
			this.status = RunStatus.Idle;
			this.mode = RunMode.Normal;

			SetMemoryBounds();

			this.currentInstructionAddress = int.MinValue;
			this.currentInstructionMnemonic = null;
			this.currentInstructionTicksLeft = int.MinValue;
			this.interruptExecuted = false;

			BreakpointManager = null;
			this.startStep = new AutoResetEvent(false);
			this.runDetached = false;

			var thread = new Thread(StepRunEntryPoint)
			{
				IsBackground = true,
				Name = "Run thread"
			};

			thread.Start();
		}

		public override Devices Devices
			=> this.devices;

		public bool IsError
			=> Status == RunStatus.InvalidInstruction || Status == RunStatus.RuntimeError;

		public bool IsExecuting
			=> IsRunning || Status == RunStatus.Stepping;

		public bool IsRunning
			=> Status == RunStatus.Running || Status == RunStatus.Halted;

		public override IMemory FullMemory
			=> this.fullMemory;

		public override IMemory Memory
			=> this.memory;

		public override Registers Registers
			=> this.registers;

		public override string ModuleName
			=> MyModuleName;

		public void QueueInterrupt(Interrupt interrupt)
			=> this.interruptQueue.Enqueue(interrupt);

		public void ContinueRun()
			=> ContinueRun(new TimeSpan(0L));

		public LogLine GetLogLine()
			=> this.log.TryDequeue(out LogLine line) ? line : null;

		public void RequestStop()
			=> RequestStop(true);

		private void DeviceReporting(object sender, DeviceReportingEventArgs args)
			=> AddLogLine(new LogLine(ModuleName, args.Severity, "Device message", string.Concat(new object[] { "Device ", args.ReportingDevice.Id, ": ", args.Message })));

		private void SetMemoryBounds()
		{
			this.memory.IndexOffset = 0;
			this.memory.MinWordIndex = Mode == RunMode.Control ? this.fullMemory.MinWordIndex : 0;
			this.memory.MaxWordIndex = this.fullMemory.MaxWordIndex;

			if (ProgramCounter < this.memory.MinWordIndex)
				ProgramCounter = this.memory.MinWordIndex;
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
			this.log.Enqueue(line);

			LogLineAdded?.Invoke(this, new EventArgs());
		}

		public void ContinueRun(TimeSpan runSpan)
		{
			this.runSpan = runSpan;
			this.startStep.Set();
		}

		public override void Halt(int code)
		{
			AddLogLine(new LogLine(ModuleName, Severity.Info, ProgramCounter, "Halted", $"System halted with code {code}"));
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
			this.fullMemory.Reset();
			this.registers.Reset();
			this.devices.Reset();

			ProgramCounter = 0;
			TickCounter = 0L;
			this.currentInstructionAddress = int.MinValue;
			this.currentInstructionMnemonic = null;
			this.currentInstructionTicksLeft = int.MinValue;
			this.interruptExecuted = false;

			this.interruptQueue.Clear();

			AddLogLine(new LogLine(ModuleName, Severity.Info, "Reset", "System reset"));

			Mode = RunMode.Normal;
			Status = RunStatus.Idle;
		}

		public void ResetTickCounter()
			=> TickCounter = 0L;

		public void StartRun()
		{
			Status = RunStatus.Running;
			this.runSpan = new TimeSpan(0L);
			this.startStep.Set();
		}

		public void StartStep()
		{
			Status = RunStatus.Stepping;
			this.startStep.Set();
		}

		private void StepRunEntryPoint()
		{
			var stepStartTime = new DateTime();
			bool suspendRun = true;

			while (true)
			{
				if (suspendRun)
				{
					this.startStep.WaitOne();
					stepStartTime = DateTime.Now;
				}
				else
					this.startStep.Reset();

				do Tick();
				while ((this.currentInstructionAddress == ProgramCounter) && IsExecuting);

				suspendRun = !IsRunning || (DateTime.Now - stepStartTime > this.runSpan && !RunDetached);

				if (suspendRun)
					StepPerformed?.Invoke(this, new EventArgs());
			}
		}

		private void IncreaseTickCounter()
		{
			TickCounter++;
			if (TickCounter % 1000 == 0)
			{
				long timerValue = this.fullMemory[TimerMemoryAddress].LongValue;
				if (timerValue > 0)
				{
					this.fullMemory[TimerMemoryAddress].LongValue = timerValue - 1;

					QueueInterrupt(new Interrupt(Interrupt.Types.Timer));
				}
			}

			this.devices.Tick();
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

			while (this.devices[Devices.CardReaderUnitCode].Busy)
				IncreaseTickCounter();

			ProgramCounter = 0;

			IncreaseTickCounter();

			this.registers.RJ.LongValue = 0;

			IncreaseTickCounter();
		}

		public void SignalInterruptExecuted()
			=> this.interruptExecuted = true;

		public override void ResetProfilingCounts()
		{
			this.fullMemory.ResetProfilingCounts();
			if (FloatingPointModule != null)
				FloatingPointModule.ResetProfilingCounts();
		}

		public void Tick()
		{
			if (Mode != RunMode.Module)
			{
				if (this.interruptQueue.Count > 0 && this.currentInstructionTicksLeft == 0 && Mode != RunMode.Control && (Status == RunStatus.Running || Status == RunStatus.Stepping || Status == RunStatus.Idle))
				{
					if (this.interruptExecuted)
						this.interruptExecuted = false;

					else
						InterruptHandler.HandleInterrupt(this, this.interruptQueue.Dequeue());
				}

				IncreaseTickCounter();

				if (Status == RunStatus.Halted)
				{
					if (!this.devices.IsAnyBusy)
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

			if (this.currentInstructionAddress != ProgramCounter || this.currentInstructionMnemonic != instruction.Mnemonic || (this.currentInstructionTicksLeft <= 0 && Mode != RunMode.Module))
			{
				this.currentInstructionAddress = ProgramCounter;
				this.currentInstructionTicksLeft = instruction.TickCount;
				this.currentInstructionMnemonic = instruction.Mnemonic;

				if (Mode == RunMode.Module)
					ResetMode();
			}

			if (Mode != RunMode.Module && this.currentInstructionTicksLeft > 0)
			{
				this.currentInstructionTicksLeft--;

				if (ExecutionSettings.ProfilingEnabled)
					instructionWord.IncreaseProfilingTickCount(1);
			}

			int programCounter;

			if (Mode == RunMode.Module || this.currentInstructionTicksLeft == 0)
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

			if (programCounter > this.memory.MaxWordIndex)
			{
				ProgramCounter = this.memory.MaxWordIndex;
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
				lock (this.syncRoot)
				{
					return this.runDetached;
				}
			}
			set
			{
				lock (this.syncRoot)
				{
					this.runDetached = value;
				}
			}
		}

		public override RunMode Mode
		{
			get => this.mode;
			set
			{
				if (this.mode != value)
				{
					this.mode = value;

					SetMemoryBounds();
				}
			}
		}

		public override RunStatus Status
		{
			get
			{
				lock (this.syncRoot)
				{
					return this.status;
				}
			}

			protected set
			{
				lock (this.syncRoot)
				{
					this.status = value;
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
					this.startStep?.Dispose();

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
