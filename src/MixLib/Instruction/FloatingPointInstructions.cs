using System;
using MixLib.Modules;
using MixLib.Modules.Settings;
using MixLib.Type;

namespace MixLib.Instruction
{
	public static class FloatingPointInstructions
	{
		private const int FADD_Opcode = 1;
		private const int FSUB_Opcode = 2;
		private const int FMUL_Opcode = 3;
		private const int FDIV_Opcode = 4;
		private const int FCMP_Opcode = 56;
		public const string FCMP_Mnemonic = "FCMP";

		private static ExecutionStatus _executionStatus;
		private static readonly int[] _prenormOpcodes = { FADD_Opcode, FSUB_Opcode, FMUL_Opcode, FDIV_Opcode };

		public static bool DoFloatingPoint(ModuleBase module, MixInstruction.Instance instance)
		{
			if (module is not Mix mix)
			{
				module.ReportRuntimeError($"Floating point instruction {instance.Instruction.Mnemonic} is only available within Mix");
				return false;
			}

			FloatingPointModule floatingPointModule = mix.FloatingPointModule;
			if (floatingPointModule == null)
			{
				module.ReportRuntimeError($"Instruction {instance.Instruction.Mnemonic} requires floating point module to be enabled");
				return false;
			}

			if (_executionStatus != null && _executionStatus.ProgramCounter != module.ProgramCounter)
				_executionStatus = null;

			if (_executionStatus == null)
				_executionStatus = new ExecutionStatus(module.Mode, module.ProgramCounter, instance.Instruction.Mnemonic);

			if (_executionStatus.CurrentStep == ExecutionStatus.Step.Initialize && !InitializeInstruction(module, instance, floatingPointModule))
					return false;

			_executionStatus.OverflowDetected |= floatingPointModule.Tick();

			switch (floatingPointModule.Status)
			{
				case ModuleBase.RunStatus.Idle:

					var result = ProcessStep(module, floatingPointModule);

					if (result.HasValue)
						return result.Value;

					break;

				case ModuleBase.RunStatus.BreakpointReached:
					module.ReportBreakpointReached();

					break;

				case ModuleBase.RunStatus.InvalidInstruction:
				case ModuleBase.RunStatus.RuntimeError:
					module.ReportRuntimeError($"Floating point module failed to execute instruction {_executionStatus.Mnemonic}");
					module.Mode = _executionStatus.Mode;

					_executionStatus = null;

					break;
			}

			return false;
		}

		private static bool? ProcessStep(ModuleBase module, FloatingPointModule floatingPointModule)
		{
			switch (_executionStatus.CurrentStep)
			{
				case ExecutionStatus.Step.PrenormRA:
					_executionStatus.RAValue = floatingPointModule.Registers.RA.FullWordValue;
					_executionStatus.CurrentStep = ExecutionStatus.Step.PrenormParameter;

					if (floatingPointModule.PreparePrenorm(_executionStatus.ParameterValue))
					{
						module.ReportBreakpointReached();
						return false;
					}

					break;

				case ExecutionStatus.Step.PrenormParameter:
					_executionStatus.ParameterValue = floatingPointModule.Registers.RA.FullWordValue;
					_executionStatus.CurrentStep = ExecutionStatus.Step.ExecuteInstruction;

					if (floatingPointModule.PrepareCall(_executionStatus.Mnemonic, _executionStatus.RAValue, _executionStatus.ParameterValue))
					{
						module.ReportBreakpointReached();
						return false;
					}

					break;

				case ExecutionStatus.Step.ExecuteInstruction:
					_executionStatus.RAValue = floatingPointModule.Registers.RA.FullWordValue;
					Registers.CompValues comparatorValue = floatingPointModule.Registers.CompareIndicator;
					module.Registers.LoadFromMemory(floatingPointModule.FullMemory, ModuleSettings.FloatingPointMemoryWordCount);
					module.Registers.OverflowIndicator = _executionStatus.OverflowDetected;

					if (_executionStatus.Mnemonic == FCMP_Mnemonic)
						module.Registers.CompareIndicator = comparatorValue;

					else
					{
						module.Registers.RA.Magnitude = _executionStatus.RAValue.Magnitude;
						module.Registers.RA.Sign = _executionStatus.RAValue.Sign;
					}

					module.Mode = ModuleBase.RunMode.Normal;

					_executionStatus = null;

					return true;
			}

			return null;
		}

		private static bool InitializeInstruction(ModuleBase module, MixInstruction.Instance instance, FloatingPointModule floatingPointModule)
		{
			_executionStatus.RAValue = module.Registers.RA.FullWordValue;

			bool prenormInstruction = Array.IndexOf(_prenormOpcodes, instance.MixInstruction.Opcode) != -1;
			bool fcmpInstruction = !prenormInstruction && instance.MixInstruction.Opcode == FCMP_Opcode;

			if (prenormInstruction || fcmpInstruction)
			{
				var indexedAddress = InstructionHelpers.GetValidIndexedAddress(module, instance.AddressValue, instance.Index);
				if (indexedAddress == int.MinValue)
				{
					_executionStatus = null;
					return false;
				}

				_executionStatus.ParameterValue = module.Memory[indexedAddress];
			}

			module.Registers.SaveToMemory(floatingPointModule.FullMemory, ModuleSettings.FloatingPointMemoryWordCount, 0);

			if (!floatingPointModule.ValidateCall(_executionStatus.Mnemonic))
			{
				module.ReportRuntimeError("Unable to initialize floating point module");
				_executionStatus = null;

				return false;
			}

			module.Mode = ModuleBase.RunMode.Module;

			if (prenormInstruction)
			{
				_executionStatus.CurrentStep = ExecutionStatus.Step.PrenormRA;

				if (floatingPointModule.PreparePrenorm(_executionStatus.RAValue))
				{
					module.ReportBreakpointReached();
					return false;
				}
			}
			else
			{
				_executionStatus.CurrentStep = ExecutionStatus.Step.ExecuteInstruction;

				if (fcmpInstruction
					? floatingPointModule.PrepareCall(_executionStatus.Mnemonic, _executionStatus.RAValue, _executionStatus.ParameterValue)
					: floatingPointModule.PrepareCall(_executionStatus.Mnemonic, _executionStatus.RAValue))
				{
					module.ReportBreakpointReached();
					return false;
				}
			}

			return true;
		}

		private class ExecutionStatus
		{
			public ModuleBase.RunMode Mode { get; private set; }
			public string Mnemonic { get; private set; }
			public int ProgramCounter { get; private set; }
			public Step CurrentStep { get; set; }
			public IFullWord ParameterValue { get; set; }
			public IFullWord RAValue { get; set; }
			public bool OverflowDetected { get; set; }

			public ExecutionStatus(ModuleBase.RunMode mode, int programCounter, string mnemonic)
			{
				Mode = mode;
				ProgramCounter = programCounter;
				Mnemonic = mnemonic;
				CurrentStep = Step.Initialize;
				OverflowDetected = false;
			}

			public enum Step
			{
				Initialize,
				PrenormRA,
				PrenormParameter,
				ExecuteInstruction
			}
		}
	}
}
