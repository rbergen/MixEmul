using MixLib.Modules;
using MixLib.Type;

namespace MixLib.Instruction
{
	/// <summary>
	/// Methods for performing MIX store instructions
	/// </summary>
	public static class StoreInstructions
	{
		private const byte storeOpcodeBase = 24;
		private const byte storeJOpcode = 32;

		/// <summary>
		/// Method for performing STx instructions
		/// </summary>
		public static bool StoreRegister(ModuleBase module, MixInstruction.Instance instance)
		{
			var indexedAddress = InstructionHelpers.GetValidIndexedAddress(module, instance.AddressValue, instance.Index);
			if (indexedAddress != int.MinValue)
			{
				Register sourceRegister;

				if (instance.MixInstruction.Opcode == storeJOpcode)
				{
					sourceRegister = module.Registers.RJ;
				}
				else
				{
					sourceRegister = module.Registers[instance.MixInstruction.Opcode - storeOpcodeBase];
				}

				WordField.LoadFromRegister(instance.FieldSpec, sourceRegister).ApplyToFullWord(module.Memory[indexedAddress]);
			}

			return true;
		}

		/// <summary>
		/// Method for performing the STZ instruction
		/// </summary>
		public static bool StoreZero(ModuleBase module, MixInstruction.Instance instance)
		{
			var indexedAddress = InstructionHelpers.GetValidIndexedAddress(module, instance.AddressValue, instance.Index);

			if (indexedAddress != int.MinValue)
			{
				var word = new FullWord();
				WordField.LoadFromFullWord(instance.FieldSpec, word).ApplyToFullWord(module.Memory[indexedAddress]);
			}

			return true;
		}
	}
}
