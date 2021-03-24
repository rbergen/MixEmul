using System;
using MixLib.Modules;
using MixLib.Modules.Settings;
using MixLib.Type;

namespace MixLib.Instruction
{
	public static class FloatingPointInstructions
	{
		private const int FaddOpcode = 1;
		private const int FsubOpcode = 2;
		private const int FmulOpcode = 3;
		private const int FdivOpcode = 4;
		private const int FcmpOpcode = 56;
		public const string FcmpMnemonic = "FCMP";

		private static ExecutionStatus _executionStatus;
		private static readonly int[] _prenormOpcodes = { FaddOpcode, FsubOpcode, FmulOpcode, FdivOpcode };

		public static bool DoFloatingPoint(ModuleBase module, MixInstruction.Instance instance)
		{
			if (module is not Mix mix)
			{
				module.ReportRuntimeError(string.Format("Floating point instruction {0} is only available within Mix", instance.Instruction.Mnemonic));
				return false;
			}

			FloatingPointModule floatingPointModule = mix.FloatingPointModule;
			if (floatingPointModule == null)
			{
				module.ReportRuntimeError(string.Format("Instruction {0} requires floating point module to be enabled", instance.Instruction.Mnemonic));
				return false;
			}

			if (_executionStatus != null && _executionStatus.ProgramCounter != module.ProgramCounter)
				_executionStatus = null;

			if (_executionStatus == null)
				_executionStatus = new ExecutionStatus(module.Mode, module.ProgramCounter, instance.Instruction.Mnemonic);

			if (_executionStatus.CurrentStep == ExecutionStatus.Step.Initialize)
			{
				_executionStatus.RAValue = module.Registers.RA.FullWordValue;

				bool prenormInstruction = Array.IndexOf(_prenormOpcodes, instance.MixInstruction.Opcode) != -1;
				bool fcmpInstruction = !prenormInstruction && instance.MixInstruction.Opcode == FcmpOpcode;

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
			}

			_executionStatus.OverflowDetected |= floatingPointModule.Tick();

			switch (floatingPointModule.Status)
			{
				case ModuleBase.RunStatus.Idle:

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

							if (_executionStatus.Mnemonic == FcmpMnemonic)
							{
								module.Registers.CompareIndicator = comparatorValue;
							}
							else
							{
								module.Registers.RA.Magnitude = _executionStatus.RAValue.Magnitude;
								module.Registers.RA.Sign = _executionStatus.RAValue.Sign;
							}

							module.Mode = ModuleBase.RunMode.Normal;

							_executionStatus = null;

							return true;
					}

					break;

				case ModuleBase.RunStatus.BreakpointReached:
					module.ReportBreakpointReached();

					break;

				case ModuleBase.RunStatus.InvalidInstruction:
				case ModuleBase.RunStatus.RuntimeError:
					module.ReportRuntimeError(string.Format("Floating point module failed to execute instruction {0}", _executionStatus.Mnemonic));
					module.Mode = _executionStatus.Mode;

					_executionStatus = null;

					break;
			}

			return false;
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
