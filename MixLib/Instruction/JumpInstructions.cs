using MixLib;
using MixLib.Modules;

namespace MixLib.Instruction
{
	/// <summary>
	/// Methods for performing MIX jump instructions
	/// </summary>
	public static class JumpInstructions
	{
        const byte jmpField = 0;
        const byte jsjField = 1;
        const byte jovField = 2;
        const byte jnovField = 3;
        const byte jlField = 4;
        const byte jeField = 5;
        const byte jgField = 6;
        const byte jgeField = 7;
        const byte jneField = 8;
        const byte jleField = 9;

        const byte regJumpOpcodeBase = 40;

        const byte regJumpNegField = 0;
        const byte regJumpZeroField = 1;
        const byte regJumpPosField = 2;
        const byte regJumpNonNegField = 3;
        const byte regJumpNonZeroField = 4;
        const byte regJumpNonPosField = 5;
        const byte regJumpEvenField = 6;
        const byte regJumpOddField = 7;

        static void doJump(ModuleBase module, int indexedAddress, bool saveJ)
        {
            if (!saveJ)
            {
                module.Registers.rJ.LongValue = module.ProgramCounter + 1;
            }

            module.ProgramCounter = indexedAddress;
        }

        /// <summary>
        /// Public jump method for use by other classes
        /// </summary>
        public static void Jump(ModuleBase module, int indexedAddress)
		{
			doJump(module, indexedAddress, false);
		}

		/// <summary>
		/// Method for performing JMP, JSJ, JOV, JNOV, JL, JE, JG, JGE, JNE and JLE instructions
		/// </summary>
		public static bool NonRegJump(ModuleBase module, MixInstruction.Instance instance)
		{
			int indexedAddress = InstructionHelpers.GetValidIndexedAddress(module, instance.AddressValue, instance.Index);
			if (indexedAddress == int.MinValue)
			{
				return false;
			}

			switch (instance.MixInstruction.FieldSpec.MixByteValue.ByteValue)
			{
				case jmpField:
					Jump(module, indexedAddress);

					return false;

				case jsjField:
					doJump(module, indexedAddress, true);

					return false;

				case jovField:
					if (!module.Registers.OverflowIndicator)
					{
						module.Registers.OverflowIndicator = false;
						break;
					}

					Jump(module, indexedAddress);

					return false;

				case jnovField:
					if (module.Registers.OverflowIndicator)
					{
						module.Registers.OverflowIndicator = false;
						break;
					}

					Jump(module, indexedAddress);

					return false;

				case jlField:
					if (module.Registers.CompareIndicator != Registers.CompValues.Less)
					{
						break;
					}

					Jump(module, indexedAddress);

					return false;

				case jeField:
					if (module.Registers.CompareIndicator != Registers.CompValues.Equal)
					{
						break;
					}

					Jump(module, indexedAddress);

					return false;

				case jgField:
					if (module.Registers.CompareIndicator != Registers.CompValues.Greater)
					{
						break;
					}

					Jump(module, indexedAddress);

					return false;

				case jgeField:
					if (module.Registers.CompareIndicator != Registers.CompValues.Greater && module.Registers.CompareIndicator != Registers.CompValues.Equal)
					{
						break;
					}

					Jump(module, indexedAddress);

					return false;

				case jneField:
					if (module.Registers.CompareIndicator == Registers.CompValues.Equal)
					{
						break;
					}

					Jump(module, indexedAddress);

					return false;

				case jleField:
					if (module.Registers.CompareIndicator != Registers.CompValues.Less && module.Registers.CompareIndicator != Registers.CompValues.Equal)
					{
						break;
					}

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
			int indexedAddress = InstructionHelpers.GetValidIndexedAddress(module, instance.AddressValue, instance.Index);
			if (indexedAddress == int.MinValue)
			{
				return false;
			}

			int registerIndex = instance.MixInstruction.Opcode - regJumpOpcodeBase;
			long longValue = module.Registers[registerIndex].LongValue;
			switch (instance.MixInstruction.FieldSpec.MixByteValue.ByteValue)
			{
				case regJumpNegField:
					if (longValue >= 0L)
					{
						break;
					}

					Jump(module, indexedAddress);

					return false;

				case regJumpZeroField:
					if (longValue != 0L)
					{
						break;
					}

					Jump(module, indexedAddress);

					return false;

				case regJumpPosField:
					if (longValue <= 0L)
					{
						break;
					}

					Jump(module, indexedAddress);

					return false;

				case regJumpNonNegField:
					if (longValue < 0L)
					{
						break;
					}

					Jump(module, indexedAddress);

					return false;

				case regJumpNonZeroField:
					if (longValue == 0L)
					{
						break;
					}

					Jump(module, indexedAddress);

					return false;

				case regJumpNonPosField:
					if (longValue > 0L)
					{
						break;
					}

					Jump(module, indexedAddress);

					return false;

				case regJumpEvenField:
					if (longValue % 2 != 0)
					{
						break;
					}

					Jump(module, indexedAddress);

					return false;

				case regJumpOddField:
					if (longValue % 2 == 0)
					{
						break;
					}

					Jump(module, indexedAddress);

					return false;
			}

			return true;
		}
	}
}
