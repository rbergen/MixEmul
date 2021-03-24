using System;
using MixLib.Type;

namespace MixLib
{
	public class Registers
	{
		public const int MaxOffset = 7;
		public const int RegisterCount = MaxOffset + 2;
		private const int addressByteIndex = 0;
		private const int flagsByteIndex = 2;
		private const int rJByteIndex = 3;
		private readonly Register[] mRegisters = new Register[RegisterCount - 1];
		private readonly Register mrJ;

		public CompValues CompareIndicator { get; set; }
		public bool OverflowIndicator { get; set; }

		public Registers()
		{
			mRegisters[(int)Offset.rA] = new FullWordRegister();
			mRegisters[(int)Offset.rX] = new FullWordRegister();

			mrJ = new AddressRegister();
			CompareIndicator = CompValues.Equal;
			OverflowIndicator = false;

			for (int i = (int)Offset.rI1; i <= (int)Offset.rI6; i++)
			{
				mRegisters[i] = new IndexRegister();
			}
		}

		public Register this[int offset] => mRegisters[offset];

		public Register this[Offset offset] => this[(int)offset];

		public Register RI1 => mRegisters[(int)Offset.rI1];

		public Register RI2 => mRegisters[(int)Offset.rI2];

		public Register RI3 => mRegisters[(int)Offset.rI3];

		public Register RI4 => mRegisters[(int)Offset.rI4];

		public Register RI5 => mRegisters[(int)Offset.rI5];

		public Register RI6 => mRegisters[(int)Offset.rI6];

		public Register RA => mRegisters[(int)Offset.rA];

		public Register RJ => mrJ;

		public Register RX => mRegisters[(int)Offset.rX];

		public int GetIndexedAddress(int mValue, int index)
		{
			if (index == 0)
			{
				return mValue;
			}

			if (index < (int)Offset.rI1 || index > (int)Offset.rI6)
			{
				throw new ArgumentException("index must be between 0 and " + (int)Offset.rI6, nameof(index));
			}

			return mValue + (int)mRegisters[index].LongValue;
		}

		public void SaveToMemory(IMemory memory, int firstAddress, int addressValue)
		{
			int i;

			for (i = 0; i < mRegisters.Length; i++)
			{
				memory[firstAddress + i].Magnitude = mRegisters[i].Magnitude;
				memory[firstAddress + i].Sign = mRegisters[i].Sign;
			}

			IFullWord composedWord = memory[firstAddress + i];
			if (addressValue < 0)
			{
				composedWord.Sign = Word.Signs.Negative;
				addressValue = -addressValue;
			}
			else
			{
				composedWord.Sign = Word.Signs.Positive;
			}

			composedWord[addressByteIndex] = (addressValue >> MixByte.BitCount) & MixByte.MaxValue;
			composedWord[addressByteIndex + 1] = addressValue & MixByte.MaxValue;

			FlagValues flags = FlagValues.None;
			if (OverflowIndicator)
			{
				flags |= FlagValues.Overflow;
			}
			if (CompareIndicator == CompValues.Greater)
			{
				flags |= FlagValues.Greater;
			}
			else if (CompareIndicator == CompValues.Less)
			{
				flags |= FlagValues.Less;
			}
			composedWord[flagsByteIndex] = (byte)flags;

			composedWord[rJByteIndex] = RJ[0];
			composedWord[rJByteIndex + 1] = RJ[1];
		}

		public int LoadFromMemory(IMemory memory, int firstAddress)
		{
			int i;

			for (i = 0; i < mRegisters.Length; i++)
			{
				mRegisters[i].Magnitude = memory[firstAddress + i].Magnitude;
				mRegisters[i].Sign = memory[firstAddress + i].Sign;
			}

			IFullWord composedWord = memory[firstAddress + i];
			int addressValue = ((byte)composedWord[addressByteIndex] << MixByte.BitCount) | (byte)composedWord[addressByteIndex + 1];
			if (composedWord.Sign.IsNegative())
			{
				addressValue = addressValue.GetMagnitude();
			}

			var flags = (FlagValues)(byte)composedWord[flagsByteIndex];
			OverflowIndicator = (flags & FlagValues.Overflow) == FlagValues.Overflow;
			if ((flags & FlagValues.Greater) == FlagValues.Greater)
			{
				CompareIndicator = CompValues.Greater;
			}
			else if ((flags & FlagValues.Less) == FlagValues.Less)
			{
				CompareIndicator = CompValues.Less;
			}
			else
			{
				CompareIndicator = CompValues.Equal;
			}

			RJ[0] = composedWord[rJByteIndex];
			RJ[1] = composedWord[rJByteIndex + 1];

			return addressValue;
		}

		public void Reset()
		{
			foreach (Register register in mRegisters)
			{
				register.MagnitudeLongValue = 0L;
				register.Sign = Word.Signs.Positive;
			}

			mrJ.MagnitudeLongValue = 0L;
			mrJ.Sign = Word.Signs.Positive;
			CompareIndicator = CompValues.Equal;
			OverflowIndicator = false;
		}

		public enum CompValues
		{
			Less,
			Equal,
			Greater
		}

		public enum Offset
		{
			rA = 0,
			rI1 = 1,
			rI2 = 2,
			rI3 = 3,
			rI4 = 4,
			rI5 = 5,
			rI6 = 6,
			rX = 7
		}

		[Flags]
		private enum FlagValues : byte
		{
			None = 0,
			Less = 1,
			Greater = 2,
			Overflow = 4
		}
	}
}
