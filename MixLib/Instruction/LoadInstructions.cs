using MixLib.Modules;
using MixLib.Type;

namespace MixLib.Instruction
{
	/// <summary>
	/// Methods for performing MIX load instructions
	/// </summary>
	public static class LoadInstructions
	{
        const byte loadOpcodeBase = 8;
        const byte loadNegOpcodeBase = 16;

        static bool doLoad(ModuleBase module, MixInstruction.Instance instance, int registerIndex, bool negateSign)
        {
            int indexedAddress = InstructionHelpers.GetValidIndexedAddress(module, instance.AddressValue, instance.Index);
            if (indexedAddress == int.MinValue) return false;

            WordField memoryField = WordField.LoadFromFullWord(instance.FieldSpec, module.Memory[indexedAddress]);

            if (negateSign) memoryField.InvertSign();

            memoryField.ApplyToRegister(module.Registers[registerIndex]);

            return true;
        }

        /// <summary>
        /// Method for performing LDx instructions
        /// </summary>
        public static bool Load(ModuleBase module, MixInstruction.Instance instance)
		{
			int registerIndex = instance.MixInstruction.Opcode - loadOpcodeBase;

			return doLoad(module, instance, registerIndex, false);
		}

		/// <summary>
		/// Method for performing LDxN instructions
		/// </summary>
		public static bool LoadNegative(ModuleBase module, MixInstruction.Instance instance)
		{
			int registerIndex = instance.MixInstruction.Opcode - loadNegOpcodeBase;

			return doLoad(module, instance, registerIndex, true);
		}
	}
}
