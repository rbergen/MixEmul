using MixLib.Modules;
using MixLib.Type;

namespace MixLib.Instruction
{
	/// <summary>
	/// Methods for performing MIX arithmatic instructions
	/// </summary>
	public static class ArithmaticInstructions
	{
        const byte substractOpcode = 2;

        /// <summary>
        /// Method for performing ADD and SUB instructions
        /// </summary>
        public static bool AddSubstract(ModuleBase module, MixInstruction.Instance instance)
		{
			var indexedAddress = InstructionHelpers.GetValidIndexedAddress(module, instance.AddressValue, instance.Index);
			if (indexedAddress == int.MinValue) return false;

			Register rA = module.Registers.RA;
			long rALongValue = rA.LongValue;
			long memoryWordValue = WordField.LoadFromFullWord(instance.FieldSpec, module.Memory[indexedAddress]).LongValue;

			if (instance.MixInstruction.Opcode == substractOpcode)
			{
				memoryWordValue = -memoryWordValue;
			}

			long sumValue = rALongValue + memoryWordValue;

            if (sumValue != 0L)
			{
				rA.Sign = sumValue.GetSign();
				sumValue = sumValue.GetMagnitude();
			}

			if (sumValue > rA.MaxMagnitude)
			{
				module.ReportOverflow();
				sumValue &= rA.MaxMagnitude;
			}

			rA.MagnitudeLongValue = sumValue;

			return true;
		}

		/// <summary>
		/// Method for performing DIV instruction
		/// </summary>
		public static bool Divide(ModuleBase module, MixInstruction.Instance instance)
		{
			var indexedAddress = InstructionHelpers.GetValidIndexedAddress(module, instance.AddressValue, instance.Index);
			if (indexedAddress == int.MinValue) return false;

			long memoryWordValue = WordField.LoadFromFullWord(instance.FieldSpec, module.Memory[indexedAddress]).LongValue;
			if (memoryWordValue == 0L)
			{
				module.ReportOverflow();
				return true;
			}

			Register rA = module.Registers.RA;
			Register rX = module.Registers.RX;

			long rAValue = rA.LongValue;
			long rXValue = rX.LongValue;

			decimal rAXValue = (decimal)rAValue * ((long)1 << rX.BitCount) + rXValue;
			decimal divider = rAXValue / memoryWordValue;

			rX.Sign = rA.Sign;
		    rA.Sign = divider.GetSign();
		    divider = divider.GetMagnitude();

			if (divider > rA.MaxMagnitude)
			{
				module.ReportOverflow();
				return true;
			}

			decimal remainder = rAXValue % memoryWordValue;
			rA.MagnitudeLongValue = (long)divider;
			rX.MagnitudeLongValue = (long)remainder;

			return true;
		}

		/// <summary>
		/// Method for performing MUL instruction
		/// </summary>
		public static bool Multiply(ModuleBase module, MixInstruction.Instance instance)
		{
			var indexedAddress = InstructionHelpers.GetValidIndexedAddress(module, instance.AddressValue, instance.Index);
			if (indexedAddress == int.MinValue) return false;

			Register rA = module.Registers.RA;
			Register rX = module.Registers.RX;

			long rAValue = rA.LongValue;
			long memoryWordValue = WordField.LoadFromFullWord(instance.FieldSpec, module.Memory[indexedAddress]).LongValue;

			var result = decimal.Multiply(rAValue, memoryWordValue);
			rA.Sign = rX.Sign = result.GetSign();
			result = result.GetMagnitude();

			var rXResultValue = (long)(result % ((long)1 << rX.BitCount));
			var rAResultValue = (long)decimal.Truncate(result / ((long)1 << rX.BitCount));

			rA.MagnitudeLongValue = rAResultValue;
			rX.MagnitudeLongValue = rXResultValue;

			return true;
		}
	}
}
