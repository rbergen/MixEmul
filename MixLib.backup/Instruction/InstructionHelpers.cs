using MixLib.Modules;

namespace MixLib.Instruction
{
	public static class InstructionHelpers
	{
		public const int InvalidAddress = int.MinValue;

		public static int GetValidIndexedAddress(ModuleBase module, int addressBase, int registerIndex) =>
						GetValidIndexedAddress(module, addressBase, registerIndex, true);

		public static InstanceValidationError[] ValidateIndex(MixInstruction.Instance instance) =>
				instance.Index > 6 ? new InstanceValidationError[] { new InstanceValidationError(InstanceValidationError.Sources.Index, 0, 6) } : null;

		public static int GetValidIndexedAddress(ModuleBase module, int addressBase, int registerIndex, bool reportErrors)
		{
			if (registerIndex < 0 || registerIndex > (int)Registers.Offset.rI6)
			{
				if (reportErrors) module.ReportRuntimeError("Register index invalid: " + registerIndex);

				return int.MinValue;
			}

			var indexedAddress = module.Registers.GetIndexedAddress(addressBase, registerIndex);
			if (indexedAddress >= module.Memory.MinWordIndex && indexedAddress <= module.Memory.MaxWordIndex)
			{
				return indexedAddress;
			}

			if (reportErrors) module.ReportRuntimeError("Indexed memory address invalid: " + indexedAddress);

			return int.MinValue;
		}

		public static InstanceValidationError[] ValidateIndexAndFieldSpec(MixInstruction.Instance instance)
		{
			int index = 0;
			InstanceValidationError[] errorArray = new InstanceValidationError[2];

			if (instance.Index > 6)
			{
				errorArray[index] = new InstanceValidationError(InstanceValidationError.Sources.Index, 0, 6);
				index++;
			}

			if (!instance.FieldSpec.IsValid)
			{
				errorArray[index] = new InstanceValidationError(InstanceValidationError.Sources.FieldSpec, "valid fieldspec required");
				index++;
			}

			if (index == 1) return new InstanceValidationError[] { errorArray[0] };
			if (index == 2) return errorArray;

			return null;
		}
	}
}
