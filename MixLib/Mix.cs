using System;
using System.Collections;
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
		private const string moduleName = "Mix";
		private const int loaderStartAddress = 16;
		private const int memoryMinIndex = -3999;
		private const int memoryMaxIndex = 3999;
		private const int timerMemoryAddress = -10;
		private static readonly long[] loaderInstructionValues = { 1060, 4457506, 103 };

		private Devices mDevices = new Devices();
		private Memory mFullMemory = new Memory(memoryMinIndex, memoryMaxIndex);
		private Registers mRegisters = new Registers();
		private MemoryView mMemory;
		private Queue<Interrupt> mInterruptQueue = new Queue<Interrupt>();

		private int mCurrentInstructionAddress;
		private int mCurrentInstructionTicksLeft;
		private string mCurrentInstructionMnemonic;

		private Queue mLog;
		private bool mRunDetached;
		private TimeSpan mRunSpan;
		private AutoResetEvent mStartStep;
		private RunStatus mStatus;
		private RunMode mMode;
		private bool mInterruptExecuted;
		private object mSyncRoot = new object();

        public FloatingPointModule FloatingPointModule { get; private set; }
        public long TickCounter { get; private set; }

        public event EventHandler LogLineAdded;
		public event EventHandler StepPerformed;

		public Mix()
		{
			SetFloatingPointModuleEnabled(ModuleSettings.FloatingPointEnabled);

			mMemory = new MemoryView(mFullMemory);

			mDevices.DeviceReportingEvent += deviceReporting;
			mLog = Queue.Synchronized(new Queue());
			ProgramCounter = 0;
			TickCounter = 0L;
			mStatus = RunStatus.Idle;
			mMode = RunMode.Normal;

			setMemoryBounds();

			mCurrentInstructionAddress = int.MinValue;
			mCurrentInstructionMnemonic = null;
			mCurrentInstructionTicksLeft = int.MinValue;
			mInterruptExecuted = false;

			BreakpointManager = null;
			mStartStep = new AutoResetEvent(false);
			mRunDetached = false;

			Thread thread = new Thread(stepRunEntryPoint);
			thread.IsBackground = true;
			thread.Name = "Run thread";
			thread.Start();
		}

        public override Devices Devices => mDevices;

        public bool IsError => Status == RunStatus.InvalidInstruction || Status == RunStatus.RuntimeError;

        public bool IsExecuting => IsRunning || Status == RunStatus.Stepping;

        public bool IsRunning => Status == RunStatus.Running || Status == RunStatus.Halted;

        public override IMemory FullMemory => mFullMemory;

        public override IMemory Memory => mMemory;

        public override Registers Registers => mRegisters;

        public override string ModuleName => moduleName;

        public void QueueInterrupt(Interrupt interrupt) => mInterruptQueue.Enqueue(interrupt);

        public void ContinueRun() => ContinueRun(new TimeSpan(0L));

        public LogLine GetLogLine() => mLog.Count <= 0 ? null : (LogLine)mLog.Dequeue();

        public void RequestStop() => RequestStop(true);

        private void deviceReporting(object sender, DeviceReportingEventArgs args) =>
            AddLogLine(new LogLine(ModuleName, args.Severity, "Device message", string.Concat(new object[] { "Device ", args.ReportingDevice.Id, ": ", args.Message })));

        private void setMemoryBounds()
		{
			mMemory.IndexOffset = 0;
			mMemory.MinWordIndex = Mode == RunMode.Control ? mFullMemory.MinWordIndex : 0;
			mMemory.MaxWordIndex = mFullMemory.MaxWordIndex;

			if (ProgramCounter < mMemory.MinWordIndex)
			{
				ProgramCounter = mMemory.MinWordIndex;
			}
		}

		public void SetFloatingPointModuleEnabled(bool enabled)
		{
			if (enabled && FloatingPointModule == null)
			{
				FloatingPointModule = new FloatingPointModule(this);
			}
			else if (!enabled && FloatingPointModule != null)
			{
				FloatingPointModule = null;
			}
		}

        public override void AddLogLine(LogLine line)
		{
			mLog.Enqueue(line);

            LogLineAdded?.Invoke(this, new EventArgs());
        }

        public void ContinueRun(TimeSpan runSpan)
		{
			mRunSpan = runSpan;
			mStartStep.Set();
		}

        public override void Halt(int code)
		{
			AddLogLine(new LogLine(ModuleName, Severity.Info, ProgramCounter, "Halted", string.Format("System halted with code {0}", code)));
			Status = RunStatus.Halted;
		}

		public void RequestStop(bool reportStop)
		{
			if (reportStop)
			{
				AddLogLine(new LogLine(ModuleName, Severity.Info, ProgramCounter, "Stopped", "Stop requested"));
			}

			Status = RunStatus.Idle;
		}

		public override void Reset()
		{
			mFullMemory.Reset();
			mRegisters.Reset();
			mDevices.Reset();

			ProgramCounter = 0;
			TickCounter = 0L;
			mCurrentInstructionAddress = int.MinValue;
			mCurrentInstructionMnemonic = null;
			mCurrentInstructionTicksLeft = int.MinValue;
			mInterruptExecuted = false;

			mInterruptQueue.Clear();

			AddLogLine(new LogLine(ModuleName, Severity.Info, "Reset", "System reset"));

			Mode = RunMode.Normal;
			Status = RunStatus.Idle;
		}

		public void ResetTickCounter()
		{
			TickCounter = 0L;
		}

		public void StartRun()
		{
			Status = RunStatus.Running;
			mRunSpan = new TimeSpan(0L);
			mStartStep.Set();
		}

		public void StartStep()
		{
			Status = RunStatus.Stepping;
			mStartStep.Set();
		}

		private void stepRunEntryPoint()
		{
			DateTime stepStartTime = new DateTime();
			bool suspendRun = true;

			while (true)
			{
				if (suspendRun)
				{
					mStartStep.WaitOne();
					stepStartTime = DateTime.Now;
				}
				else
				{
					mStartStep.Reset();
				}

				do
				{
					Tick();
				}
				while ((mCurrentInstructionAddress == ProgramCounter) && IsExecuting);

				suspendRun = !IsRunning || (DateTime.Now - stepStartTime > mRunSpan && !RunDetached);

				if (suspendRun && StepPerformed != null)
				{
					StepPerformed(this, new EventArgs());
				}
			}
		}

		private void increaseTickCounter()
		{
			TickCounter++;
			if (TickCounter % 1000 == 0)
			{
				long timerValue = mFullMemory[timerMemoryAddress].LongValue;
				if (timerValue > 0)
				{
					mFullMemory[timerMemoryAddress].LongValue = timerValue - 1;

					QueueInterrupt(new Interrupt(Interrupt.Types.Timer));
				}
			}

			mDevices.Tick();
		}

		public void PrepareLoader()
		{
			FullWord word = new FullWord(1060);
			MixInstruction instruction = InstructionSet.Instance.GetInstruction(IOInstructions.INOpCode, new FieldSpec(Devices.CardReaderUnitCode));
			MixInstruction.Instance instance = instruction.CreateInstance(word);

			int ticksLeft = instruction.TickCount;
			while (ticksLeft-- > 0)
			{
				increaseTickCounter();
			}

			instance.Execute(this);

			while (mDevices[Devices.CardReaderUnitCode].Busy)
			{
				increaseTickCounter();
			}

			ProgramCounter = 0;

			increaseTickCounter();

			mRegisters.rJ.LongValue = 0;

			increaseTickCounter();
		}

		public void SignalInterruptExecuted()
		{
			mInterruptExecuted = true;
		}

		public override void ResetProfilingCounts()
		{
			mFullMemory.ResetProfilingCounts();
			if (FloatingPointModule != null)
			{
				FloatingPointModule.ResetProfilingCounts();
			}
		}

		public void Tick()
		{
			if (Mode != RunMode.Module)
			{
				if (mInterruptQueue.Count > 0 && mCurrentInstructionTicksLeft == 0 && Mode != RunMode.Control && (Status == RunStatus.Running || Status == RunStatus.Stepping || Status == RunStatus.Idle))
				{
					if (mInterruptExecuted)
					{
						mInterruptExecuted = false;
					}
					else
					{
						InterruptHandler.HandleInterrupt(this, mInterruptQueue.Dequeue());
					}
				}

				increaseTickCounter();

				if (Status == RunStatus.Halted)
				{
					if (!mDevices.IsAnyBusy)
					{
						Status = RunStatus.Idle;
					}
					return;
				}
			}

			IMemoryFullWord instructionWord = Memory[ProgramCounter];
			MixInstruction instruction = InstructionSet.Instance.GetInstruction(instructionWord[MixInstruction.OpcodeByte], new FieldSpec(instructionWord[MixInstruction.FieldSpecByte]));

			if (instruction == null)
			{
				ReportInvalidInstruction("Opcode (and field) do not encode an instruction");
				if (Mode == RunMode.Module)
				{
					resetMode();
				}

				return;
			}

			MixInstruction.Instance instance = instruction.CreateInstance(instructionWord);
			InstanceValidationError[] errors = instance.Validate();
			if (errors != null)
			{
				ReportInvalidInstruction(errors);
				if (Mode == RunMode.Module)
				{
					resetMode();
				}

				return;
			}

			if (mCurrentInstructionAddress != ProgramCounter || mCurrentInstructionMnemonic != instruction.Mnemonic || (mCurrentInstructionTicksLeft <= 0 && Mode != RunMode.Module))
			{
				mCurrentInstructionAddress = ProgramCounter;
				mCurrentInstructionTicksLeft = instruction.TickCount;
				mCurrentInstructionMnemonic = instruction.Mnemonic;
				if (Mode == RunMode.Module)
				{
					resetMode();
				}
			}

			if (Mode != RunMode.Module && mCurrentInstructionTicksLeft > 0)
			{
				mCurrentInstructionTicksLeft--;

				if (ExecutionSettings.ProfilingEnabled)
				{
					instructionWord.IncreaseProfilingTickCount(1);
				}
			}

			int programCounter;

			if (Mode == RunMode.Module || mCurrentInstructionTicksLeft == 0)
			{
				bool increasePC = instance.Execute(this);

				programCounter = ProgramCounter;

				if (increasePC)
				{
					programCounter++;
				}

				if (ExecutionSettings.ProfilingEnabled && (Mode != RunMode.Module || increasePC))
				{
					instructionWord.IncreaseProfilingExecutionCount();
				}
			}
			else
			{
				programCounter = ProgramCounter;
			}

			if (programCounter > mMemory.MaxWordIndex)
			{
				ProgramCounter = mMemory.MaxWordIndex;
				ReportRuntimeError("Program counter overflow");
			}
			else
			{
				ProgramCounter = programCounter;
				if (Status == RunStatus.Running && IsBreakpointSet(programCounter))
				{
					ReportBreakpointReached();
				}
			}
		}

		private void resetMode()
		{
			Mode = ProgramCounter < 0 ? RunMode.Control : RunMode.Normal;
		}

		public bool RunDetached
		{
			get
			{
				lock (mSyncRoot)
				{
					return mRunDetached;
				}
			}
			set
			{
				lock (mSyncRoot)
				{
					mRunDetached = value;
				}
			}
		}

		public override RunMode Mode
		{
			get
			{
				return mMode;
			}
			set
			{
				if (mMode != value)
				{
					mMode = value;

					setMemoryBounds();
				}
			}
		}

		public override RunStatus Status
		{
			get
			{
				lock (mSyncRoot)
				{
					return mStatus;
				}
			}

			protected set
			{
				lock (mSyncRoot)
				{
					mStatus = value;
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
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    mStartStep?.Dispose();
                }

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
