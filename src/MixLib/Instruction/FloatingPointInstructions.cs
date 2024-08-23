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

		private static ExecutionStatus executionStatus;
		private static readonly int[] prenormOpcodes = [FADD_Opcode, FSUB_Opcode, FMUL_Opcode, FDIV_Opcode];

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

			if (executionStatus != null && executionStatus.ProgramCounter != module.ProgramCounter)
				executionStatus = null;

			executionStatus ??= new ExecutionStatus(module.Mode, module.ProgramCounter, instance.Instruction.Mnemonic);

			if (executionStatus.CurrentStep == ExecutionStatus.Step.Initialize && !InitializeInstruction(module, instance, floatingPointModule))
					return false;

			executionStatus.OverflowDetected |= floatingPointModule.Tick();

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
					module.ReportRuntimeError($"Floating point module failed to execute instruction { executionStatus.Mnemonic}");
					module.Mode = executionStatus.Mode;

					executionStatus = null;

					break;
			}

			return false;
		}

		private static bool? ProcessStep(ModuleBase module, FloatingPointModule floatingPointModule)
		{
			switch (executionStatus.CurrentStep)
			{
				case ExecutionStatus.Step.PrenormRA:
					executionStatus.RAValue = floatingPointModule.Registers.RA.FullWordValue;
					executionStatus.CurrentStep = ExecutionStatus.Step.PrenormParameter;

					if (floatingPointModule.PreparePrenorm(executionStatus.ParameterValue))
					{
						module.ReportBreakpointReached();
						return false;
					}

					break;

				case ExecutionStatus.Step.PrenormParameter:
					executionStatus.ParameterValue = floatingPointModule.Registers.RA.FullWordValue;
					executionStatus.CurrentStep = ExecutionStatus.Step.ExecuteInstruction;

					if (floatingPointModule.PrepareCall(executionStatus.Mnemonic, executionStatus.RAValue, executionStatus.ParameterValue))
					{
						module.ReportBreakpointReached();
						return false;
					}

					break;

				case ExecutionStatus.Step.ExecuteInstruction:
					executionStatus.RAValue = floatingPointModule.Registers.RA.FullWordValue;
					Registers.CompValues comparatorValue = floatingPointModule.Registers.CompareIndicator;
					module.Registers.LoadFromMemory(floatingPointModule.FullMemory, ModuleSettings.FloatingPointMemoryWordCount);
					module.Registers.OverflowIndicator = executionStatus.OverflowDetected;

					if (executionStatus.Mnemonic == FCMP_Mnemonic)
						module.Registers.CompareIndicator = comparatorValue;

					else
					{
						module.Registers.RA.Magnitude = executionStatus.RAValue.Magnitude;
						module.Registers.RA.Sign = executionStatus.RAValue.Sign;
					}

					module.Mode = ModuleBase.RunMode.Normal;

					executionStatus = null;

					return true;
			}

			return null;
		}

		private static bool InitializeInstruction(ModuleBase module, MixInstruction.Instance instance, FloatingPointModule floatingPointModule)
		{
			executionStatus.RAValue = module.Registers.RA.FullWordValue;

			bool prenormInstruction = Array.IndexOf(prenormOpcodes, instance.MixInstruction.Opcode) != -1;
			bool fcmpInstruction = !prenormInstruction && instance.MixInstruction.Opcode == FCMP_Opcode;

			if (prenormInstruction || fcmpInstruction)
			{
				var indexedAddress = InstructionHelpers.GetValidIndexedAddress(module, instance.AddressValue, instance.Index);
				if (indexedAddress == int.MinValue)
				{
					executionStatus = null;
					return false;
				}

				executionStatus.ParameterValue = module.Memory[indexedAddress];
			}

			module.Registers.SaveToMemory(floatingPointModule.FullMemory, ModuleSettings.FloatingPointMemoryWordCount, 0);

			if (!floatingPointModule.ValidateCall(executionStatus.Mnemonic))
			{
				module.ReportRuntimeError("Unable to initialize floating point module");
				executionStatus = null;

				return false;
			}

			module.Mode = ModuleBase.RunMode.Module;

			if (prenormInstruction)
			{
				executionStatus.CurrentStep = ExecutionStatus.Step.PrenormRA;

				if (floatingPointModule.PreparePrenorm(executionStatus.RAValue))
				{
					module.ReportBreakpointReached();
					return false;
				}
			}
			else
			{
				executionStatus.CurrentStep = ExecutionStatus.Step.ExecuteInstruction;

				if (fcmpInstruction
					? floatingPointModule.PrepareCall(executionStatus.Mnemonic, executionStatus.RAValue, executionStatus.ParameterValue)
					: floatingPointModule.PrepareCall(executionStatus.Mnemonic, executionStatus.RAValue))
				{
					module.ReportBreakpointReached();
					return false;
				}
			}

			return true;
		}

		private class ExecutionStatus(ModuleBase.RunMode mode, int programCounter, string mnemonic)
		{
			public ModuleBase.RunMode Mode => mode;
			public string Mnemonic => mnemonic;
			public int ProgramCounter => programCounter;
			public Step CurrentStep { get; set; } = Step.Initialize;
			public IFullWord ParameterValue { get; set; }
			public IFullWord RAValue { get; set; }
			public bool OverflowDetected { get; set; } = false;

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
