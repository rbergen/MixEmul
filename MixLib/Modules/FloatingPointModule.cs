using MixLib.Instruction;
using MixLib.Misc;
using MixLib.Modules.Settings;
using MixLib.Settings;
using MixLib.Type;

namespace MixLib.Modules
{
	public class FloatingPointModule : ModuleBase
	{
		const string moduleName = "Float";

		const string accSymbol = "ACC";
		const string prenormSymbol = "PRENORM";
		const string exitSymbol = "EXIT";
		const string unexpectedOverflowSymbol = "OFLO";
		const string exponentUnderflowSymbol = "EXPUN";
		const string exponentOverflowSymbol = "EXPOV";
		const string fixOverflowSymbol = "FIXOV";
		const string divisionByZeroSymbol = "DIVZRO";
		readonly Mix mMix;
		readonly Memory mFullMemory;
		readonly IMemory mMemory;
		readonly Registers mRegisters;
		RunStatus mStatus;
		int? mAccAddress;
		int? mPrenormAddress;
		int? mExitAddress;
		int? mUnexpectedOverflowAddress;
		int? mExponentOverflowAddress;
		int? mExponentUnderflowAddress;
		int? mFixOverflowAddress;
		int? mDivisionByZeroAddress;

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

		public override string ModuleName => moduleName;

		public override IMemory Memory => mMemory;

		public override IMemory FullMemory => mFullMemory;

		public override Registers Registers => mMix.Registers;

		public override void AddLogLine(LogLine line)
		{
			mMix.AddLogLine(line);
		}

		public override void ResetProfilingCounts()
		{
			mFullMemory.ResetProfilingCounts();
		}

		public override RunStatus Status
		{
			get => mStatus;
			protected set => mStatus = value;
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

		int? GetSymbolAddress(string name)
		{
			if (Symbols == null)
			{
				return null;
			}

			var symbol = Symbols[name] as IValueSymbol;
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
			mAccAddress = GetSymbolAddress(accSymbol);
			mPrenormAddress = GetSymbolAddress(prenormSymbol);
			mExitAddress = GetSymbolAddress(exitSymbol);
			mUnexpectedOverflowAddress = GetSymbolAddress(unexpectedOverflowSymbol);
			mExponentOverflowAddress = GetSymbolAddress(exponentOverflowSymbol);
			mExponentUnderflowAddress = GetSymbolAddress(exponentUnderflowSymbol);
			mFixOverflowAddress = GetSymbolAddress(fixOverflowSymbol);
			mDivisionByZeroAddress = GetSymbolAddress(divisionByZeroSymbol);

			ValidateAddresses(Severity.Warning);

			return true;
		}

		bool ValidateAddress(Severity severity, string name, int address)
		{
			if (address < 0 || address >= ModuleSettings.FloatingPointMemoryWordCount)
			{
				AddLogLine(new LogLine(moduleName, severity, "Address invalid", string.Format("Symbol {0} contains invalid address value {1}", name, address)));
				return false;
			}

			return true;
		}

		bool ValidateAddresses(Severity severity)
		{
			if (mAccAddress == null || mPrenormAddress == null || mExitAddress == null)
			{
				AddLogLine(new LogLine(moduleName, severity, "Symbols unset", string.Format("Symbols {0}, {1} and/or {2} are unset", accSymbol, prenormSymbol, exitSymbol)));
				return false;
			}

			return ValidateAddress(severity, accSymbol, mAccAddress.Value) && ValidateAddress(severity, prenormSymbol, mPrenormAddress.Value) && ValidateAddress(severity, exitSymbol, mExitAddress.Value);
		}

		public bool ValidateCall(string mnemonic)
		{
			if (!ValidateAddresses(Severity.Error))
			{
				return false;
			}

			long? instructionValue = GetSymbolAddress(mnemonic);
			if (instructionValue == null)
			{
				AddLogLine(new LogLine(moduleName, Severity.Error, "Symbol unset", string.Format("Symbol for mnemonic {0} unset", mnemonic)));
				return false;
			}

			return ValidateAddress(Severity.Error, mnemonic, (int)instructionValue.Value);
		}

		public bool PreparePrenorm(IFullWord rAValue)
		{
			Registers.RA.LongValue = rAValue.LongValue;
			ProgramCounter = mPrenormAddress.Value;

			if (IsBreakpointSet(ProgramCounter))
			{
				ReportBreakpointReached();
				return true;
			}

			return false;
		}

		bool PrepareCall(string mnemonic)
		{
			ProgramCounter = GetSymbolAddress(mnemonic).Value;

			if (IsBreakpointSet(ProgramCounter))
			{
				ReportBreakpointReached();
				return true;
			}

			return false;
		}

		public bool PrepareCall(string mnemonic, IFullWord rAValue)
		{
			Registers.RA.LongValue = rAValue.LongValue;

			return PrepareCall(mnemonic);
		}

		public bool PrepareCall(string mnemonic, IFullWord rAValue, IFullWord paramValue)
		{

			Memory[mAccAddress.Value].LongValue = rAValue.LongValue;
			Registers.RA.LongValue = paramValue.LongValue;

			return PrepareCall(mnemonic);
		}

		public override RunMode Mode
		{
			get => RunMode.Normal;
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

			if (mUnexpectedOverflowAddress != null && ProgramCounter == mUnexpectedOverflowAddress.Value)
			{
				ReportRuntimeError(string.Format("Overflow set at instruction start"));
				return true;
			}

			if (mExponentOverflowAddress != null && ProgramCounter == mExponentOverflowAddress.Value)
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
			var instruction = InstructionSet.Instance.GetInstruction(instructionWord[MixInstruction.OpcodeByte], new FieldSpec(instructionWord[MixInstruction.FieldSpecByte]));

			if (instruction == null)
			{
				ReportInvalidInstruction("Opcode (and field) do not encode an instruction");
				return false;
			}

			var instance = instruction.CreateInstance(instructionWord);
			var errors = instance.Validate();
			if (errors != null)
			{
				ReportInvalidInstruction(errors);
				return false;
			}

			var increasePC = instance.Execute(this);

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
	}
}
