using MixLib.Modules;
using MixLib.Type;

namespace MixLib.Instruction
{
	/// <summary>
	/// Methods for performing MIX comparison instructions
	/// </summary>
	public static class ComparisonInstructions
	{
		private const int OpcodeBase = 56;

		/// <summary>
		/// Method for performing CMPx instructions
		/// </summary>
		public static bool Compare(ModuleBase module, MixInstruction.Instance instance)
		{
			var indexedAddress = InstructionHelpers.GetValidIndexedAddress(module, instance.AddressValue, instance.Index);

			if (indexedAddress == int.MinValue)
				return false;

			int registerIndex = instance.MixInstruction.Opcode - OpcodeBase;
			Register register = module.Registers[registerIndex];

			module.Registers.CompareIndicator = WordField.LoadFromFullWord(instance.FieldSpec, register.FullWordValue).CompareTo(module.Memory[indexedAddress]).ToCompValue();

			return true;
		}
	}
}
