using System;
using MixLib.Type;

namespace MixLib
{
	public class Registers
	{
		public const int MaxOffset = 7;
		public const int RegisterCount = MaxOffset + 2;

		private const int AddressByteIndex = 0;
		private const int FlagsByteIndex = 2;
		private const int RJByteIndex = 3;

		private readonly Register[] _registers = new Register[RegisterCount - 1];
		private readonly Register _rJ;

		public CompValues CompareIndicator { get; set; }
		public bool OverflowIndicator { get; set; }

		public Registers()
		{
			_registers[(int)Offset.rA] = new FullWordRegister();
			_registers[(int)Offset.rX] = new FullWordRegister();

			_rJ = new AddressRegister();
			CompareIndicator = CompValues.Equal;
			OverflowIndicator = false;

			for (int i = (int)Offset.rI1; i <= (int)Offset.rI6; i++)
			{
				_registers[i] = new IndexRegister();
			}
		}

		public Register this[int offset] 
			=> _registers[offset];

		public Register this[Offset offset] 
			=> this[(int)offset];

		public Register RI1 
			=> _registers[(int)Offset.rI1];

		public Register RI2 
			=> _registers[(int)Offset.rI2];

		public Register RI3 
			=> _registers[(int)Offset.rI3];

		public Register RI4 
			=> _registers[(int)Offset.rI4];

		public Register RI5 
			=> _registers[(int)Offset.rI5];

		public Register RI6 
			=> _registers[(int)Offset.rI6];

		public Register RA => 
			_registers[(int)Offset.rA];

		public Register RJ 
			=> _rJ;

		public Register RX 
			=> _registers[(int)Offset.rX];

		public int GetIndexedAddress(int mValue, int index)
		{
			if (index == 0)
				return mValue;

			if (index < (int)Offset.rI1 || index > (int)Offset.rI6)
				throw new ArgumentException("index must be between 0 and " + (int)Offset.rI6, nameof(index));

			return mValue + (int)_registers[index].LongValue;
		}

		public void SaveToMemory(IMemory memory, int firstAddress, int addressValue)
		{
			int i;

			for (i = 0; i < _registers.Length; i++)
			{
				memory[firstAddress + i].Magnitude = _registers[i].Magnitude;
				memory[firstAddress + i].Sign = _registers[i].Sign;
			}

			IFullWord composedWord = memory[firstAddress + i];
			if (addressValue < 0)
			{
				composedWord.Sign = Word.Signs.Negative;
				addressValue = -addressValue;
			}
			else
				composedWord.Sign = Word.Signs.Positive;

			composedWord[AddressByteIndex] = (addressValue >> MixByte.BitCount) & MixByte.MaxValue;
			composedWord[AddressByteIndex + 1] = addressValue & MixByte.MaxValue;

			FlagValues flags = FlagValues.None;

			if (OverflowIndicator)
				flags |= FlagValues.Overflow;

			if (CompareIndicator == CompValues.Greater)
				flags |= FlagValues.Greater;

			else if (CompareIndicator == CompValues.Less)
				flags |= FlagValues.Less;

			composedWord[FlagsByteIndex] = (byte)flags;

			composedWord[RJByteIndex] = RJ[0];
			composedWord[RJByteIndex + 1] = RJ[1];
		}

		public int LoadFromMemory(IMemory memory, int firstAddress)
		{
			int i;

			for (i = 0; i < _registers.Length; i++)
			{
				_registers[i].Magnitude = memory[firstAddress + i].Magnitude;
				_registers[i].Sign = memory[firstAddress + i].Sign;
			}

			IFullWord composedWord = memory[firstAddress + i];
			int addressValue = ((byte)composedWord[AddressByteIndex] << MixByte.BitCount) | (byte)composedWord[AddressByteIndex + 1];

			if (composedWord.Sign.IsNegative())
				addressValue = addressValue.GetMagnitude();

			var flags = (FlagValues)(byte)composedWord[FlagsByteIndex];
			OverflowIndicator = (flags & FlagValues.Overflow) == FlagValues.Overflow;

			if ((flags & FlagValues.Greater) == FlagValues.Greater)
				CompareIndicator = CompValues.Greater;

			else if ((flags & FlagValues.Less) == FlagValues.Less)
				CompareIndicator = CompValues.Less;

			else
				CompareIndicator = CompValues.Equal;

			RJ[0] = composedWord[RJByteIndex];
			RJ[1] = composedWord[RJByteIndex + 1];

			return addressValue;
		}

		public void Reset()
		{
			foreach (Register register in _registers)
			{
				register.MagnitudeLongValue = 0L;
				register.Sign = Word.Signs.Positive;
			}

			_rJ.MagnitudeLongValue = 0L;
			_rJ.Sign = Word.Signs.Positive;
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
