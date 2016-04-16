using MixLib.Instruction;
using MixLib.Misc;
using MixLib.Type;

namespace MixLib.Modules
{
	public abstract class ModuleBase
	{
		private int mProgramCounter;

		public IBreakpointManager BreakpointManager { protected get; set; }
        public abstract string ModuleName { get; }
        public abstract IMemory FullMemory { get; }
        public abstract IMemory Memory { get; }
        public abstract MixLib.Registers Registers { get; }
        public abstract RunStatus Status { get; protected set; }
        public abstract RunMode Mode { get; set; }

        public abstract void ResetProfilingCounts();

		protected bool IsBreakpointSet(int address)
		{
			return BreakpointManager != null && BreakpointManager.IsBreakpointSet(address);
		}

		public void ReportBreakpointReached()
		{
			AddLogLine(new LogLine(ModuleName, Severity.Info, ProgramCounter, "Breakpoint", "Reached breakpoint"));
			Status = RunStatus.BreakpointReached;
		}

		public abstract void AddLogLine(LogLine line);

		public virtual MixLib.Devices Devices
		{
			get
			{
				return null;
			}
		}

		public int ProgramCounter
		{
			get
			{
				return mProgramCounter;
			}
			set
			{
				if (mProgramCounter == value)
				{
					return;
				}

				if (value < Memory.MinWordIndex)
				{
					value = Memory.MinWordIndex;
				}
				else if (value > Memory.MaxWordIndex)
				{
					value = Memory.MaxWordIndex;
				}

				mProgramCounter = value;
			}
		}

		private int loadInstructionInstance(LoaderInstruction.Instance instance, int locationCounter)
		{
			FullMemory[locationCounter].SourceLine = instance.SourceLine;

			switch (((LoaderInstruction)instance.Instruction).Operation)
			{
				case LoaderInstruction.Operations.SetLocationCounter:
					{
						int desiredLC = (int)instance.Value.LongValue;
						if (desiredLC >= FullMemory.MinWordIndex && desiredLC <= FullMemory.MaxWordIndex)
						{
							return desiredLC;
						}

						reportLoadError(locationCounter, string.Format("Attempt to set location counter to invalid value {0}", desiredLC));

						return locationCounter;
					}
				case LoaderInstruction.Operations.SetMemoryWord:
					FullMemory[locationCounter].MagnitudeLongValue = instance.Value.MagnitudeLongValue;
					FullMemory[locationCounter].Sign = instance.Value.Sign;

					return (locationCounter + 1);

				case LoaderInstruction.Operations.SetProgramCounter:
					{
						int desiredPC = (int)instance.Value.LongValue;
						if (desiredPC < 0 && Mode != RunMode.Control)
						{
							AddLogLine(new LogLine(ModuleName, Severity.Info, "Mode switch", "Attempting to switch to Control mode to set program counter to negative value"));
							Mode = RunMode.Control;
						}

						if (desiredPC >= Memory.MinWordIndex && desiredPC <= Memory.MaxWordIndex)
						{
							ProgramCounter = desiredPC;
							return locationCounter;
						}

						reportLoadError(locationCounter, string.Format("Attempt to set program counter to invalid value {0}", desiredPC));

						return locationCounter;
					}
			}

			return locationCounter;
		}

		private int loadInstructionInstance(MixInstruction.Instance instance, int locationCounter)
		{
			InstanceValidationError[] validationErrors = instance.Validate();
			if (validationErrors != null)
			{
				reportLoadInstanceErrors(locationCounter, validationErrors);
			}

			FullMemory[locationCounter].MagnitudeLongValue = instance.InstructionWord.MagnitudeLongValue;
			FullMemory[locationCounter].Sign = instance.InstructionWord.Sign;
			FullMemory[locationCounter].SourceLine = instance.SourceLine;

			return (locationCounter + 1);
		}

		public virtual bool LoadInstructionInstances(InstructionInstanceBase[] instances, SymbolCollection symbols)
		{
			Memory.ClearSourceLines();
			ResetProfilingCounts();

			int locationCounter = 0;

			foreach (InstructionInstanceBase instance in instances)
			{
				if (instance is MixInstruction.Instance)
				{
					locationCounter = loadInstructionInstance((MixInstruction.Instance)instance, locationCounter);
				}
				else if (instance is LoaderInstruction.Instance)
				{
					locationCounter = loadInstructionInstance((LoaderInstruction.Instance)instance, locationCounter);
				}

				if (locationCounter > FullMemory.MaxWordIndex)
				{
					reportLoadError(FullMemory.MaxWordIndex, "Location counter overflow; loading aborted");
					Status = RunStatus.Idle;

					return false;
				}
			}

			AddLogLine(new LogLine(ModuleName, Severity.Info, "Loader info", "Program loaded"));
			Status = RunStatus.Idle;

			return true;
		}

		protected void ReportInvalidInstruction(InstanceValidationError[] errors)
		{
			foreach (InstanceValidationError error in errors)
			{
				ReportInvalidInstruction(error.CompiledMessage);
			}
		}

		protected void ReportInvalidInstruction(string message)
		{
			AddLogLine(new LogLine(ModuleName, Severity.Error, ProgramCounter, "Invalid instruction", message));
			Status = RunStatus.InvalidInstruction;
		}

		private void reportLoadError(int locationCounter, string message)
		{
			AddLogLine(new LogLine(ModuleName, Severity.Error, locationCounter, "Loader error", message));
		}

		private void reportLoadInstanceErrors(int counter, InstanceValidationError[] errors)
		{
			foreach (InstanceValidationError error in errors)
			{
				AddLogLine(new LogLine(ModuleName, Severity.Warning, counter, "Loaded invalid instruction", error.CompiledMessage));
			}
		}

		public void ReportOverflow()
		{
			AddLogLine(new LogLine(ModuleName, Severity.Info, ProgramCounter, "Overflow", "Overflow occured"));
			Registers.OverflowIndicator = true;
		}

		public void ReportRuntimeError(string message)
		{
			AddLogLine(new LogLine(ModuleName, Severity.Error, ProgramCounter, "Runtime error", message));
			Status = RunStatus.RuntimeError;
		}

		public enum RunStatus
		{
			Idle,
			Stepping,
			Running,
			Halted,
			InvalidInstruction,
			RuntimeError,
			BreakpointReached
		}

		public enum RunMode
		{
			Normal = 0,
			Control,
			Module
		}

		public abstract void Halt(int code);

		public abstract void Reset();
	}
}
