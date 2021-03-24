using MixLib.Instruction;
using MixLib.Misc;
using MixLib.Modules.Settings;
using MixLib.Settings;
using MixLib.Type;

namespace MixLib.Modules
{
	public class FloatingPointModule : ModuleBase
	{
		private const string MyModuleName = "Float";
		private const string AccSymbol = "ACC";
		private const string PrenormSymbol = "PRENORM";
		private const string ExitSymbol = "EXIT";
		private const string UnexpectedOverflowSymbol = "OFLO";
		private const string ExponentUnderflowSymbol = "EXPUN";
		private const string ExponentOverflowSymbol = "EXPOV";
		private const string FixOverflowSymbol = "FIXOV";
		private const string DivisionByZeroSymbol = "DIVZRO";

		private readonly Mix _mix;
		private readonly Memory _fullMemory;
		private readonly IMemory _memory;
		private readonly Registers _registers;
		private RunStatus _status;
		private int? _accAddress;
		private int? _prenormAddress;
		private int? _exitAddress;
		private int? _unexpectedOverflowAddress;
		private int? _exponentOverflowAddress;
		private int? _exponentUnderflowAddress;
		private int? _fixOverflowAddress;
		private int? _divisionByZeroAddress;

		public SymbolCollection Symbols { get; private set; }

		public FloatingPointModule(Mix mix)
		{
			_mix = mix;
			_fullMemory = new Memory(0, ModuleSettings.FloatingPointMemoryWordCount + 8);
			_memory = new MemoryView(_fullMemory, 0, ModuleSettings.FloatingPointMemoryWordCount - 1, 0);
			_registers = new Registers();
			ProgramCounter = 0;
			_status = RunStatus.Idle;
		}

		public override string ModuleName 
			=> MyModuleName;

		public override IMemory Memory 
			=> _memory;

		public override IMemory FullMemory 
			=> _fullMemory;

		public override Registers Registers 
			=> _mix.Registers;

		public override void AddLogLine(LogLine line) 
			=> _mix.AddLogLine(line);

		public override void ResetProfilingCounts() 
			=> _fullMemory.ResetProfilingCounts();

		public override RunStatus Status
		{
			get => _status;
			protected set => _status = value;
		}

		public override void Halt(int code)
		{
			Status = RunStatus.Idle;
			AddLogLine(new LogLine(ModuleName, Severity.Info, ProgramCounter, "Halt requested", "Halting Mix"));
			_mix.Halt(code);
		}

		public override void Reset()
		{
			_registers.Reset();

			ProgramCounter = 0;
			AddLogLine(new LogLine(ModuleName, Severity.Info, "Reset", "Module reset"));
			Status = RunStatus.Idle;
		}

		private int? GetSymbolAddress(string name)
			=> Symbols[name] is not IValueSymbol symbol ? null : (int?)symbol.Value;

		public override bool LoadInstructionInstances(InstructionInstanceBase[] instances, SymbolCollection symbols)
		{
			_memory.Reset();
			Symbols = null;
			_accAddress = null;
			_prenormAddress = null;
			_exitAddress = null;
			_unexpectedOverflowAddress = null;
			_exponentOverflowAddress = null;
			_exponentUnderflowAddress = null;
			_fixOverflowAddress = null;
			_divisionByZeroAddress = null;

			if (!base.LoadInstructionInstances(instances, symbols))
				return false;

			Symbols = symbols;
			_accAddress = GetSymbolAddress(AccSymbol);
			_prenormAddress = GetSymbolAddress(PrenormSymbol);
			_exitAddress = GetSymbolAddress(ExitSymbol);
			_unexpectedOverflowAddress = GetSymbolAddress(UnexpectedOverflowSymbol);
			_exponentOverflowAddress = GetSymbolAddress(ExponentOverflowSymbol);
			_exponentUnderflowAddress = GetSymbolAddress(ExponentUnderflowSymbol);
			_fixOverflowAddress = GetSymbolAddress(FixOverflowSymbol);
			_divisionByZeroAddress = GetSymbolAddress(DivisionByZeroSymbol);

			ValidateAddresses(Severity.Warning);

			return true;
		}

		private bool ValidateAddress(Severity severity, string name, int address)
		{
			if (address < 0 || address >= ModuleSettings.FloatingPointMemoryWordCount)
			{
				AddLogLine(new LogLine(MyModuleName, severity, "Address invalid", string.Format("Symbol {0} contains invalid address value {1}", name, address)));
				return false;
			}

			return true;
		}

		private bool ValidateAddresses(Severity severity)
		{
			if (_accAddress == null || _prenormAddress == null || _exitAddress == null)
			{
				AddLogLine(new LogLine(MyModuleName, severity, "Symbols unset", string.Format("Symbols {0}, {1} and/or {2} are unset", AccSymbol, PrenormSymbol, ExitSymbol)));
				return false;
			}

			return ValidateAddress(severity, AccSymbol, _accAddress.Value) && ValidateAddress(severity, PrenormSymbol, _prenormAddress.Value) && ValidateAddress(severity, ExitSymbol, _exitAddress.Value);
		}

		public bool ValidateCall(string mnemonic)
		{
			if (!ValidateAddresses(Severity.Error))
				return false;

			long? instructionValue = GetSymbolAddress(mnemonic);
			if (instructionValue == null)
			{
				AddLogLine(new LogLine(MyModuleName, Severity.Error, "Symbol unset", string.Format("Symbol for mnemonic {0} unset", mnemonic)));
				return false;
			}

			return ValidateAddress(Severity.Error, mnemonic, (int)instructionValue.Value);
		}

		public bool PreparePrenorm(IFullWord rAValue)
		{
			Registers.RA.LongValue = rAValue.LongValue;
			ProgramCounter = _prenormAddress.Value;

			if (IsBreakpointSet(ProgramCounter))
			{
				ReportBreakpointReached();
				return true;
			}

			return false;
		}

		private bool PrepareCall(string mnemonic)
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

			Memory[_accAddress.Value].LongValue = rAValue.LongValue;
			Registers.RA.LongValue = paramValue.LongValue;

			return PrepareCall(mnemonic);
		}

		public override RunMode Mode
		{
			get => RunMode.Normal;
			set
			{
				if (value != RunMode.Normal)
					AddLogLine(new LogLine(ModuleName, Severity.Warning, "Invalid mode", string.Format("Mode {0} not supported by this module, change request ignored", value)));
			}
		}

		public bool Tick()
		{
			bool overflowDetected = false;

			if (ProgramCounter == _exitAddress.Value)
			{
				Status = RunStatus.Idle;
				return false;
			}

			if (_unexpectedOverflowAddress != null && ProgramCounter == _unexpectedOverflowAddress.Value)
			{
				ReportRuntimeError(string.Format("Overflow set at instruction start"));
				return true;
			}

			if (_exponentOverflowAddress != null && ProgramCounter == _exponentOverflowAddress.Value)
			{
				AddLogLine(new LogLine(MyModuleName, Severity.Info, "Overflow", "Exponent overflow detected"));
				overflowDetected = true;
			}
			else if (_exponentUnderflowAddress != null && ProgramCounter == _exponentUnderflowAddress.Value)
			{
				AddLogLine(new LogLine(MyModuleName, Severity.Info, "Underflow", "Exponent underflow detected"));
				overflowDetected = true;
			}
			else if (_fixOverflowAddress != null && ProgramCounter == _fixOverflowAddress.Value)
			{
				AddLogLine(new LogLine(MyModuleName, Severity.Info, "Overflow", "Float to fix overflow detected"));
				overflowDetected = true;
			}
			else if (_divisionByZeroAddress != null && ProgramCounter == _divisionByZeroAddress.Value)
			{
				AddLogLine(new LogLine(MyModuleName, Severity.Info, "Overflow", "Division by zero detected"));
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
				programCounter++;

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

			return overflowDetected;
		}
	}
}
