﻿using MixLib.Modules;
using MixLib.Type;

namespace MixLib.Instruction
{
	/// <summary>
	/// Methods for performing MIX load instructions
	/// </summary>
	public static class LoadInstructions
	{
		private const byte LoadOpcodeBase = 8;
		private const byte LoadNegOpcodeBase = 16;

		private static bool DoLoad(ModuleBase module, MixInstruction.Instance instance, int registerIndex, bool negateSign)
		{
			var indexedAddress = InstructionHelpers.GetValidIndexedAddress(module, instance.AddressValue, instance.Index);

			if (indexedAddress == int.MinValue)
				return false;

			var memoryField = WordField.LoadFromFullWord(instance.FieldSpec, module.Memory[indexedAddress]);

			if (negateSign)
				memoryField.InvertSign();

			memoryField.ApplyToRegister(module.Registers[registerIndex]);

			return true;
		}

		/// <summary>
		/// Method for performing LDx instructions
		/// </summary>
		public static bool Load(ModuleBase module, MixInstruction.Instance instance)
		{
			int registerIndex = instance.MixInstruction.Opcode - LoadOpcodeBase;

			return DoLoad(module, instance, registerIndex, false);
		}

		/// <summary>
		/// Method for performing LDxN instructions
		/// </summary>
		public static bool LoadNegative(ModuleBase module, MixInstruction.Instance instance)
		{
			int registerIndex = instance.MixInstruction.Opcode - LoadNegOpcodeBase;

			return DoLoad(module, instance, registerIndex, true);
		}
	}
}
