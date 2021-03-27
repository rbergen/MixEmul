using MixLib.Modules;

namespace MixLib.Instruction
{
	/// <summary>
	/// Methods for performing MIX jump instructions
	/// </summary>
	public static class JumpInstructions
	{
		private const byte JmpField = 0;
		private const byte JsjField = 1;
		private const byte JovField = 2;
		private const byte JnovField = 3;
		private const byte JlField = 4;
		private const byte JeField = 5;
		private const byte JgField = 6;
		private const byte JgeField = 7;
		private const byte JneField = 8;
		private const byte JleField = 9;
		private const byte RegJumpOpcodeBase = 40;
		private const byte RegJumpNegField = 0;
		private const byte RegJumpZeroField = 1;
		private const byte RegJumpPosField = 2;
		private const byte RegJumpNonNegField = 3;
		private const byte RegJumpNonZeroField = 4;
		private const byte RegJumpNonPosField = 5;
		private const byte RegJumpEvenField = 6;
		private const byte RegJumpOddField = 7;

		private static void DoJump(ModuleBase module, int indexedAddress, bool saveJ)
		{
			if (!saveJ)
				module.Registers.RJ.LongValue = module.ProgramCounter + 1;

			module.ProgramCounter = indexedAddress;
		}

		/// <summary>
		/// Public jump method for use by other classes
		/// </summary>
		public static void Jump(ModuleBase module, int indexedAddress) 
			=> DoJump(module, indexedAddress, false);

		/// <summary>
		/// Method for performing JMP, JSJ, JOV, JNOV, JL, JE, JG, JGE, JNE and JLE instructions
		/// </summary>
		public static bool NonRegJump(ModuleBase module, MixInstruction.Instance instance)
		{
			var indexedAddress = InstructionHelpers.GetValidIndexedAddress(module, instance.AddressValue, instance.Index);
			if (indexedAddress == int.MinValue)
				return false;

			switch (instance.MixInstruction.FieldSpec.MixByteValue.ByteValue)
			{
				case JmpField:
					Jump(module, indexedAddress);
					return false;

				case JsjField:
					DoJump(module, indexedAddress, true);
					return false;

				case JovField:
					if (!module.Registers.OverflowIndicator)
					{
						module.Registers.OverflowIndicator = false;
						break;
					}

					Jump(module, indexedAddress);
					return false;

				case JnovField:
					if (module.Registers.OverflowIndicator)
					{
						module.Registers.OverflowIndicator = false;
						break;
					}

					Jump(module, indexedAddress);
					return false;

				case JlField:
					if (module.Registers.CompareIndicator != Registers.CompValues.Less)
						break;

					Jump(module, indexedAddress);
					return false;

				case JeField:
					if (module.Registers.CompareIndicator != Registers.CompValues.Equal)
						break;

					Jump(module, indexedAddress);
					return false;

				case JgField:
					if (module.Registers.CompareIndicator != Registers.CompValues.Greater)
						break;

					Jump(module, indexedAddress);
					return false;

				case JgeField:
					if (module.Registers.CompareIndicator != Registers.CompValues.Greater && module.Registers.CompareIndicator != Registers.CompValues.Equal)
						break;

					Jump(module, indexedAddress);
					return false;

				case JneField:
					if (module.Registers.CompareIndicator == Registers.CompValues.Equal)
						break;

					Jump(module, indexedAddress);

					return false;

				case JleField:
					if (module.Registers.CompareIndicator != Registers.CompValues.Less && module.Registers.CompareIndicator != Registers.CompValues.Equal)
						break;

					Jump(module, indexedAddress);
					return false;
			}

			return true;
		}

		/// <summary>
		/// Method for performing JxN, JxZ, JxP, JxNN, JxNZ, JxNP, JxE and JxO instructions
		/// </summary>
		public static bool RegJump(ModuleBase module, MixInstruction.Instance instance)
		{
			var indexedAddress = InstructionHelpers.GetValidIndexedAddress(module, instance.AddressValue, instance.Index);

			if (indexedAddress == int.MinValue)
				return false;

			int registerIndex = instance.MixInstruction.Opcode - RegJumpOpcodeBase;
			long longValue = module.Registers[registerIndex].LongValue;
			switch (instance.MixInstruction.FieldSpec.MixByteValue.ByteValue)
			{
				case RegJumpNegField:
					if (longValue >= 0L)
						break;

					Jump(module, indexedAddress);
					return false;

				case RegJumpZeroField:
					if (longValue != 0L)
						break;

					Jump(module, indexedAddress);
					return false;

				case RegJumpPosField:
					if (longValue <= 0L)
						break;

					Jump(module, indexedAddress);
					return false;

				case RegJumpNonNegField:
					if (longValue < 0L)
						break;

					Jump(module, indexedAddress);
					return false;

				case RegJumpNonZeroField:
					if (longValue == 0L)
						break;

					Jump(module, indexedAddress);

					return false;

				case RegJumpNonPosField:
					if (longValue > 0L)
						break;

					Jump(module, indexedAddress);
					return false;

				case RegJumpEvenField:
					if (longValue % 2 != 0)
						break;

					Jump(module, indexedAddress);
					return false;

				case RegJumpOddField:
					if (longValue % 2 == 0)
						break;

					Jump(module, indexedAddress);
					return false;
			}

			return true;
		}
	}
}
