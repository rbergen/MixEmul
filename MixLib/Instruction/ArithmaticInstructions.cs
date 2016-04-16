using MixLib.Modules;
using MixLib.Type;

namespace MixLib.Instruction
{
	/// <summary>
	/// Methods for performing MIX arithmatic instructions
	/// </summary>
	public class ArithmaticInstructions
	{
		private const byte substractOpcode = 2;

		/// <summary>
		/// Method for performing ADD and SUB instructions
		/// </summary>
		public static bool AddSubstract(ModuleBase module, MixInstruction.Instance instance)
		{
			int indexedAddress = InstructionHelpers.GetValidIndexedAddress(module, instance.AddressValue, instance.Index);
			if (indexedAddress == int.MinValue)
			{
				return false;
			}

			Register rA = module.Registers.rA;
			long rALongValue = rA.LongValue;
			long memoryWordValue = WordField.LoadFromFullWord(instance.FieldSpec, module.Memory[indexedAddress]).LongValue;

			if (instance.MixInstruction.Opcode == substractOpcode)
			{
				memoryWordValue = -memoryWordValue;
			}

			long sumValue = rALongValue + memoryWordValue;
			if (sumValue < 0L)
			{
				rA.Sign = Word.Signs.Negative;
				sumValue = -sumValue;
			}
			else if (sumValue > 0L)
			{
				rA.Sign = Word.Signs.Positive;
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
			int indexedAddress = InstructionHelpers.GetValidIndexedAddress(module, instance.AddressValue, instance.Index);
			if (indexedAddress == int.MinValue)
			{
				return false;
			}

			long memoryWordValue = WordField.LoadFromFullWord(instance.FieldSpec, module.Memory[indexedAddress]).LongValue;
			if (memoryWordValue == 0L)
			{
				module.ReportOverflow();
				return true;
			}

			Register rA = module.Registers.rA;
			Register rX = module.Registers.rX;

			long rAValue = rA.LongValue;
			long rXValue = rX.LongValue;

			decimal rAXValue = (decimal)rAValue * ((long)1 << rX.BitCount) + rXValue;
			decimal divider = (decimal)rAXValue / memoryWordValue;

			rX.Sign = rA.Sign;
			if (divider < 0M)
			{
				rA.Sign = Word.Signs.Negative;
				divider = -divider;
			}
			else
			{
				rA.Sign = Word.Signs.Positive;
			}

			if (divider > rA.MaxMagnitude)
			{
				module.ReportOverflow();
				return true;
			}

			decimal remainder = (decimal)rAXValue % memoryWordValue;
			rA.MagnitudeLongValue = (long)divider;
			rX.MagnitudeLongValue = (long)remainder;

			return true;
		}

		/// <summary>
		/// Method for performing MUL instruction
		/// </summary>
		public static bool Multiply(ModuleBase module, MixInstruction.Instance instance)
		{
			int indexedAddress = InstructionHelpers.GetValidIndexedAddress(module, instance.AddressValue, instance.Index);
			if (indexedAddress == int.MinValue)
			{
				return false;
			}

			Register rA = module.Registers.rA;
			Register rX = module.Registers.rX;

			long rAValue = rA.LongValue;
			long memoryWordValue = WordField.LoadFromFullWord(instance.FieldSpec, module.Memory[indexedAddress]).LongValue;

			decimal result = decimal.Multiply(rAValue, memoryWordValue);
			if (result < 0M)
			{
				rA.Sign = rX.Sign = Word.Signs.Negative;
				result = -result;
			}
			else
			{
				rA.Sign = rX.Sign = Word.Signs.Positive;
			}

			long rXResultValue = (long)(result % ((long)1 << rX.BitCount));
			long rAResultValue = (long)decimal.Truncate(result / ((long)1 << rX.BitCount));

			rA.MagnitudeLongValue = rAResultValue;
			rX.MagnitudeLongValue = rXResultValue;

			return true;
		}
	}
}
