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

		private readonly Mix mix;
		private readonly Memory fullMemory;
		private readonly MemoryView memory;
		private readonly Registers registers;
		private RunStatus status;
		private int? accAddress;
		private int? prenormAddress;
		private int? exitAddress;
		private int? unexpectedOverflowAddress;
		private int? exponentOverflowAddress;
		private int? exponentUnderflowAddress;
		private int? fixOverflowAddress;
		private int? divisionByZeroAddress;

		public SymbolCollection Symbols { get; private set; }

		public FloatingPointModule(Mix mix)
		{
			this.mix = mix;
			this.fullMemory = new Memory(0, ModuleSettings.FloatingPointMemoryWordCount + 8);
			this.memory = new MemoryView(this.fullMemory, 0, ModuleSettings.FloatingPointMemoryWordCount - 1, 0);
			this.registers = new Registers();
			ProgramCounter = 0;
			this.status = RunStatus.Idle;
		}

		public override string ModuleName 
			=> MyModuleName;

		public override IMemory Memory 
			=> this.memory;

		public override IMemory FullMemory 
			=> this.fullMemory;

		public override Registers Registers 
			=> this.mix.Registers;

		public override void AddLogLine(LogLine line) 
			=> this.mix.AddLogLine(line);

		public override void ResetProfilingCounts() 
			=> this.fullMemory.ResetProfilingCounts();

		public override RunStatus Status
		{
			get => this.status;
			protected set => this.status = value;
		}

		public override void Halt(int code)
		{
			Status = RunStatus.Idle;
			AddLogLine(new LogLine(ModuleName, Severity.Info, ProgramCounter, "Halt requested", "Halting Mix"));
			this.mix.Halt(code);
		}

		public override void Reset()
		{
			this.registers.Reset();

			ProgramCounter = 0;
			AddLogLine(new LogLine(ModuleName, Severity.Info, "Reset", "Module reset"));
			Status = RunStatus.Idle;
		}

		private int? GetSymbolAddress(string name)
			=> Symbols[name] is not IValueSymbol symbol ? null : (int?)symbol.Value;

		public override bool LoadInstructionInstances(InstructionInstanceBase[] instances, SymbolCollection symbols)
		{
			this.memory.Reset();
			Symbols = null;
			this.accAddress = null;
			this.prenormAddress = null;
			this.exitAddress = null;
			this.unexpectedOverflowAddress = null;
			this.exponentOverflowAddress = null;
			this.exponentUnderflowAddress = null;
			this.fixOverflowAddress = null;
			this.divisionByZeroAddress = null;

			if (!base.LoadInstructionInstances(instances, symbols))
				return false;

			Symbols = symbols;
			this.accAddress = GetSymbolAddress(AccSymbol);
			this.prenormAddress = GetSymbolAddress(PrenormSymbol);
			this.exitAddress = GetSymbolAddress(ExitSymbol);
			this.unexpectedOverflowAddress = GetSymbolAddress(UnexpectedOverflowSymbol);
			this.exponentOverflowAddress = GetSymbolAddress(ExponentOverflowSymbol);
			this.exponentUnderflowAddress = GetSymbolAddress(ExponentUnderflowSymbol);
			this.fixOverflowAddress = GetSymbolAddress(FixOverflowSymbol);
			this.divisionByZeroAddress = GetSymbolAddress(DivisionByZeroSymbol);

			ValidateAddresses(Severity.Warning);

			return true;
		}

		private bool ValidateAddress(Severity severity, string name, int address)
		{
			if (address < 0 || address >= ModuleSettings.FloatingPointMemoryWordCount)
			{
				AddLogLine(new LogLine(MyModuleName, severity, "Address invalid", $"Symbol {name} contains invalid address value {address}"));
				return false;
			}

			return true;
		}

		private bool ValidateAddresses(Severity severity)
		{
			if (this.accAddress == null || this.prenormAddress == null || this.exitAddress == null)
			{
				AddLogLine(new LogLine(MyModuleName, severity, "Symbols unset", $"Symbols {AccSymbol}, {PrenormSymbol} and/or {ExitSymbol} are unset"));
				return false;
			}

			return ValidateAddress(severity, AccSymbol, this.accAddress.Value) && ValidateAddress(severity, PrenormSymbol, this.prenormAddress.Value) && ValidateAddress(severity, ExitSymbol, this.exitAddress.Value);
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
			ProgramCounter = this.prenormAddress.Value;

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

			Memory[this.accAddress.Value].LongValue = rAValue.LongValue;
			Registers.RA.LongValue = paramValue.LongValue;

			return PrepareCall(mnemonic);
		}

		public override RunMode Mode
		{
			get => RunMode.Normal;
			set
			{
				if (value != RunMode.Normal)
					AddLogLine(new LogLine(ModuleName, Severity.Warning, "Invalid mode", $"Mode {value} not supported by this module, change request ignored"));
			}
		}

		public bool Tick()
		{
			bool overflowDetected = false;

			if (ProgramCounter == this.exitAddress.Value)
			{
				Status = RunStatus.Idle;
				return false;
			}

			if (this.unexpectedOverflowAddress != null && ProgramCounter == this.unexpectedOverflowAddress.Value)
			{
				ReportRuntimeError("Overflow set at instruction start");
				return true;
			}

			if (this.exponentOverflowAddress != null && ProgramCounter == this.exponentOverflowAddress.Value)
			{
				AddLogLine(new LogLine(MyModuleName, Severity.Info, "Overflow", "Exponent overflow detected"));
				overflowDetected = true;
			}
			else if (this.exponentUnderflowAddress != null && ProgramCounter == this.exponentUnderflowAddress.Value)
			{
				AddLogLine(new LogLine(MyModuleName, Severity.Info, "Underflow", "Exponent underflow detected"));
				overflowDetected = true;
			}
			else if (this.fixOverflowAddress != null && ProgramCounter == this.fixOverflowAddress.Value)
			{
				AddLogLine(new LogLine(MyModuleName, Severity.Info, "Overflow", "Float to fix overflow detected"));
				overflowDetected = true;
			}
			else if (this.divisionByZeroAddress != null && ProgramCounter == this.divisionByZeroAddress.Value)
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

			return overflowDetected;
		}
	}
}
