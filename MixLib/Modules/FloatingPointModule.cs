using MixLib.Instruction;
using MixLib.Misc;
using MixLib.Modules.Settings;
using MixLib.Settings;
using MixLib.Type;

namespace MixLib.Modules
{
	public class FloatingPointModule : ModuleBase
	{
		private const string moduleName = "Float";

		private const string accSymbol = "ACC";
		private const string prenormSymbol = "PRENORM";
		private const string exitSymbol = "EXIT";
		private const string unexpectedOverflowSymbol = "OFLO";
		private const string exponentUnderflowSymbol = "EXPUN";
		private const string exponentOverflowSymbol = "EXPOV";
		private const string fixOverflowSymbol = "FIXOV";
		private const string divisionByZeroSymbol = "DIVZRO";

		private Mix mMix;
		private Memory mFullMemory;
		private IMemory mMemory;
		private Registers mRegisters;
		private RunStatus mStatus;
		private int? mAccAddress;
		private int? mPrenormAddress;
		private int? mExitAddress;
		private int? mUnexpectedOverflowAddress;
		private int? mExponentOverflowAddress;
		private int? mExponentUnderflowAddress;
		private int? mFixOverflowAddress;
		private int? mDivisionByZeroAddress;

        public SymbolCollection Symbols { get; private set; }

        public FloatingPointModule(Mix mix)
		{
			mMix = mix;
			mFullMemory = new Memory(0, ModuleSettings.FloatingPointMemoryWordCount + 8);
			mMemory = new MemoryView(mFullMemory, 0, ModuleSettings.FloatingPointMemoryWordCount - 1, 0);
			mRegisters = new Registers();
			ProgramCounter = 0;
			mStatus = RunStatus.Idle;
		}

		public override void AddLogLine(Misc.LogLine line)
		{
			mMix.AddLogLine(line);
		}

		public override string ModuleName
		{
			get
			{
				return moduleName;
			}
		}

		public override IMemory Memory
		{
			get
			{
				return mMemory;
			}
		}

		public override IMemory FullMemory
		{
			get
			{
				return mFullMemory;
			}
		}

		public override Registers Registers
		{
			get
			{
				return mMix.Registers;
			}
		}

		public override RunStatus Status
		{
			get
			{
				return mStatus;
			}
			protected set
			{
				mStatus = value;
			}
		}

		public override void Halt(int code)
		{
			Status = RunStatus.Idle;
			AddLogLine(new LogLine(ModuleName, Severity.Info, ProgramCounter, "Halt requested", "Halting Mix"));
			mMix.Halt(code);
		}

		public override void Reset()
		{
			mRegisters.Reset();

			ProgramCounter = 0;
			AddLogLine(new LogLine(ModuleName, Severity.Info, "Reset", "Module reset"));
			Status = RunStatus.Idle;
		}

		private int? getSymbolAddress(string name)
		{
			if (Symbols == null)
			{
				return null;
			}

			IValueSymbol symbol = Symbols[name] as IValueSymbol;
			return symbol == null ? null : (int?)symbol.Value;
		}

		public override bool LoadInstructionInstances(InstructionInstanceBase[] instances, SymbolCollection symbols)
		{
			mMemory.Reset();
			Symbols = null;
			mAccAddress = null;
			mPrenormAddress = null;
			mExitAddress = null;
			mUnexpectedOverflowAddress = null;
			mExponentOverflowAddress = null;
			mExponentUnderflowAddress = null;
			mFixOverflowAddress = null;
			mDivisionByZeroAddress = null;

			if (!base.LoadInstructionInstances(instances, symbols))
			{
				return false;
			}

			Symbols = symbols;
			mAccAddress = getSymbolAddress(accSymbol);
			mPrenormAddress = getSymbolAddress(prenormSymbol);
			mExitAddress = getSymbolAddress(exitSymbol);
			mUnexpectedOverflowAddress = getSymbolAddress(unexpectedOverflowSymbol);
			mExponentOverflowAddress = getSymbolAddress(exponentOverflowSymbol);
			mExponentUnderflowAddress = getSymbolAddress(exponentOverflowSymbol);
			mFixOverflowAddress = getSymbolAddress(fixOverflowSymbol);
			mDivisionByZeroAddress = getSymbolAddress(divisionByZeroSymbol);

			validateAddresses(Severity.Warning);

			return true;
		}

		private bool validateAddress(Severity severity, string name, int address)
		{
			if (address < 0 || address >= ModuleSettings.FloatingPointMemoryWordCount)
			{
				AddLogLine(new LogLine(moduleName, severity, "Address invalid", string.Format("Symbol {0} contains invalid address value {1}", name, address)));
				return false;
			}

			return true;
		}

		private bool validateAddresses(Severity severity)
		{
			if (mAccAddress == null || mPrenormAddress == null || mExitAddress == null)
			{
				AddLogLine(new LogLine(moduleName, severity, "Symbols unset", string.Format("Symbols {0}, {1} and/or {2} are unset", accSymbol, prenormSymbol, exitSymbol)));
				return false;
			}

			return validateAddress(severity, accSymbol, mAccAddress.Value) && validateAddress(severity, prenormSymbol, mPrenormAddress.Value) && validateAddress(severity, exitSymbol, mExitAddress.Value);
		}

		public bool ValidateCall(string mnemonic)
		{
			if (!validateAddresses(Severity.Error))
			{
				return false;
			}

			long? instructionValue = getSymbolAddress(mnemonic);
			if (instructionValue == null)
			{
				AddLogLine(new LogLine(moduleName, Severity.Error, "Symbol unset", string.Format("Symbol for mnemonic {0} unset", mnemonic)));
				return false;
			}

			if (!validateAddress(Severity.Error, mnemonic, (int)instructionValue.Value))
			{
				return false;
			}

			return true;
		}

		public bool PreparePrenorm(IFullWord rAValue)
		{
			Registers.rA.LongValue = rAValue.LongValue;
			ProgramCounter = mPrenormAddress.Value;

			if (IsBreakpointSet(ProgramCounter))
			{
				ReportBreakpointReached();
				return true;
			}

			return false;
		}

		private bool prepareCall(string mnemonic)
		{
			ProgramCounter = getSymbolAddress(mnemonic).Value;

			if (IsBreakpointSet(ProgramCounter))
			{
				ReportBreakpointReached();
				return true;
			}

			return false;
		}

		public bool PrepareCall(string mnemonic, IFullWord rAValue)
		{
			Registers.rA.LongValue = rAValue.LongValue;

			return prepareCall(mnemonic);
		}

		public bool PrepareCall(string mnemonic, IFullWord rAValue, IFullWord paramValue)
		{

			Memory[mAccAddress.Value].LongValue = rAValue.LongValue;
			Registers.rA.LongValue = paramValue.LongValue;

			return prepareCall(mnemonic);
		}

		public override RunMode Mode
		{
			get
			{
				return RunMode.Normal;
			}
			set
			{
				if (value != RunMode.Normal)
				{
					AddLogLine(new LogLine(ModuleName, Severity.Warning, "Invalid mode", string.Format("Mode {0} not supported by this module, change request ignored", value)));
				}
			}
		}

		public bool Tick()
		{
			bool overflowDetected = false;

			if (ProgramCounter == mExitAddress.Value)
			{
				Status = RunStatus.Idle;
				return false;
			}
			else if (mUnexpectedOverflowAddress != null && ProgramCounter == mUnexpectedOverflowAddress.Value)
			{
				ReportRuntimeError(string.Format("Overflow set at instruction start"));
				return true;
			}
			else if (mExponentOverflowAddress != null && ProgramCounter == mExponentOverflowAddress.Value)
			{
				AddLogLine(new LogLine(moduleName, Severity.Info, "Overflow", "Exponent overflow detected"));
				overflowDetected = true;
			}
			else if (mExponentUnderflowAddress != null && ProgramCounter == mExponentUnderflowAddress.Value)
			{
				AddLogLine(new LogLine(moduleName, Severity.Info, "Underflow", "Exponent underflow detected"));
				overflowDetected = true;
			}
			else if (mFixOverflowAddress != null && ProgramCounter == mFixOverflowAddress.Value)
			{
				AddLogLine(new LogLine(moduleName, Severity.Info, "Overflow", "Float to fix overflow detected"));
				overflowDetected = true;
			}
			else if (mDivisionByZeroAddress != null && ProgramCounter == mDivisionByZeroAddress.Value)
			{
				AddLogLine(new LogLine(moduleName, Severity.Info, "Overflow", "Division by zero detected"));
				overflowDetected = true;
			}

			Status = RunStatus.Running;

			IMemoryFullWord instructionWord = Memory[ProgramCounter];
			MixInstruction instruction = InstructionSet.Instance.GetInstruction(instructionWord[MixInstruction.OpcodeByte], new FieldSpec(instructionWord[MixInstruction.FieldSpecByte]));

			if (instruction == null)
			{
				ReportInvalidInstruction("Opcode (and field) do not encode an instruction");
				return false;
			}

			MixInstruction.Instance instance = instruction.CreateInstance(instructionWord);
			InstanceValidationError[] errors = instance.Validate();
			if (errors != null)
			{
				ReportInvalidInstruction(errors);
				return false;
			}

			bool increasePC = instance.Execute(this);

			if (ExecutionSettings.ProfilingEnabled)
			{
				instructionWord.IncreaseProfilingTickCount(instruction.TickCount);
				instructionWord.IncreaseProfilingExecutionCount();
			}

			int programCounter = ProgramCounter;

			if (increasePC)
			{
				programCounter++;
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

			return overflowDetected;
		}

		public override void ResetProfilingCounts()
		{
			mFullMemory.ResetProfilingCounts();
		}
	}
}
