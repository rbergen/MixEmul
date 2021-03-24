using MixLib.Device;
using MixLib.Interrupts;
using MixLib.Modules;
using MixLib.Type;

namespace MixLib.Instruction
{
	/// <summary>
	/// Methods for performing MIX I/O instructions
	/// </summary>
	public static class IOInstructions
	{
		public const byte INOpCode = 36;

		/// <summary>
		/// Method for performing the IN instruction
		/// </summary>
		public static bool Input(ModuleBase module, MixInstruction.Instance instance)
		{
			if (module.Devices == null)
			{
				module.ReportRuntimeError("Module does not provide devices");
				return false;
			}

			var mValue = InstructionHelpers.GetValidIndexedAddress(module, instance.AddressValue, instance.Index);
			if (mValue == int.MinValue)
			{
				return false;
			}

			int byteValue = instance.FieldSpec.MixByteValue.ByteValue;
			MixDevice device = module.Devices[byteValue];
			if (!device.SupportsInput)
			{
				module.ReportRuntimeError("Device " + byteValue + " doesn't support input");
				return false;
			}

			if (device.Busy)
				return false;

			var field = WordField.LoadFromRegister(new FieldSpec(4, 5), module.Registers.RX);
			device.StartInput(module.Memory, mValue, (int)field.LongValue, module is Mix mix && mix.Mode == ModuleBase.RunMode.Control ? new InterruptQueueCallback(mix.QueueInterrupt) : null);

			return true;
		}

		public static InstanceValidationError[] InstanceValid(MixInstruction.Instance instance)
		{
			int index = 0;
			InstanceValidationError[] errorArray = new InstanceValidationError[2];

			if (instance.Index > 6)
			{
				errorArray[index] = new InstanceValidationError(InstanceValidationError.Sources.Index, 0, 6);
				index++;
			}

			if (instance.FieldSpec.MixByteValue.ByteValue >= Devices.DeviceCount)
			{
				errorArray[index] = new InstanceValidationError(InstanceValidationError.Sources.FieldSpec, 0, 20);
				index++;
			}

			if (index == 1)
				return new InstanceValidationError[] { errorArray[0] };

			if (index == 2)
				return errorArray;

			return null;
		}

		/// <summary>
		/// Method for performing the IOC instruction
		/// </summary>
		public static bool IOControl(ModuleBase module, MixInstruction.Instance instance)
		{
			if (module.Devices == null)
			{
				module.ReportRuntimeError("Module does not provide devices");
				return false;
			}

			var indexedAddress = module.Registers.GetIndexedAddress(instance.AddressValue, instance.Index);
			int deviceIndex = instance.FieldSpec.MixByteValue.ByteValue;
			MixDevice device = module.Devices[deviceIndex];

			if (device.Busy)
				return false;

			var field = WordField.LoadFromRegister(new FieldSpec(4, 5), module.Registers.RX);
			device.StartIoc(indexedAddress, (int)field.LongValue, module is Mix mix && mix.Mode == ModuleBase.RunMode.Control ? new InterruptQueueCallback(mix.QueueInterrupt) : null);

			return true;
		}

		/// <summary>
		/// Method for performing the JBUS instruction
		/// </summary>
		public static bool JumpIfBusy(ModuleBase module, MixInstruction.Instance instance)
		{
			if (module.Devices == null)
			{
				module.ReportRuntimeError("Module does not provide devices");
				return false;
			}

			var indexedAddress = InstructionHelpers.GetValidIndexedAddress(module, instance.AddressValue, instance.Index);
			if (indexedAddress == int.MinValue)
				return false;

			int deviceIndex = instance.FieldSpec.MixByteValue.ByteValue;
			MixDevice device = module.Devices[deviceIndex];
			if (device.Busy)
			{
				JumpInstructions.Jump(module, indexedAddress);
				return false;
			}

			return true;
		}

		/// <summary>
		/// Method for performing the JRED instruction
		/// </summary>
		public static bool JumpIfReady(ModuleBase module, MixInstruction.Instance instance)
		{
			if (module.Devices == null)
			{
				module.ReportRuntimeError("Module does not provide devices");
				return false;
			}

			var indexedAddress = InstructionHelpers.GetValidIndexedAddress(module, instance.AddressValue, instance.Index);
			if (indexedAddress == int.MinValue)
				return false;

			int deviceIndex = instance.FieldSpec.MixByteValue.ByteValue;
			MixDevice device = module.Devices[deviceIndex];
			if (!device.Busy)
			{
				JumpInstructions.Jump(module, indexedAddress);
				return false;
			}

			return true;
		}

		/// <summary>
		/// Method for performing the OUT instruction
		/// </summary>
		public static bool Output(ModuleBase module, MixInstruction.Instance instance)
		{
			if (module.Devices == null)
			{
				module.ReportRuntimeError("Module does not provide devices");
				return false;
			}

			var mValue = InstructionHelpers.GetValidIndexedAddress(module, instance.AddressValue, instance.Index);

			if (mValue == int.MinValue)
				return false;

			int deviceIndex = instance.FieldSpec.MixByteValue.ByteValue;
			MixDevice device = module.Devices[deviceIndex];

			if (!device.SupportsOutput)
			{
				module.ReportRuntimeError("Device " + deviceIndex + " doesn't support output");
				return false;
			}

			if (device.Busy)
				return false;

			var field = WordField.LoadFromRegister(new FieldSpec(4, 5), module.Registers.RX);
			device.StartOutput(module.Memory, mValue, (int)field.LongValue, module is Mix mix && mix.Mode == ModuleBase.RunMode.Control ? new InterruptQueueCallback(mix.QueueInterrupt) : null);

			return true;
		}
	}
}
