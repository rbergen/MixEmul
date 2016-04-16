using System;

namespace MixLib.Type
{
	public class FieldSpec
	{
        public MixByte MixByteValue { get; private set; }

        public FieldSpec(MixByte value)
		{
			MixByteValue = value;
		}

		public FieldSpec(int lowBound, int highBound)
		{
			if (!IsValidFieldSpec(lowBound, highBound))
			{
				throw new ArgumentException("low and/or high bounds are invalid");
			}

            MixByteValue = (8 * lowBound) + highBound;
		}

		public override bool Equals(object o)
		{
			return o != null && o is FieldSpec && ((FieldSpec)o).MixByteValue == MixByteValue;
		}

		public override int GetHashCode()
		{
			return MixByteValue.GetHashCode();
		}

		public static bool IsValidFieldSpec(MixByte value)
		{
			return IsValidFieldSpec((int)(value / 8), (int)(value % 8));
		}

		public static bool IsValidFieldSpec(int lowBound, int highBound)
		{
			if (lowBound >= 0 && highBound <= FullWord.ByteCount && lowBound <= highBound)
			{
				return true;
			}

			if (lowBound == 0)
			{
				return highBound == FullWord.ByteCount + 1;
			}

			return false;
		}

		public static bool operator ==(FieldSpec one, FieldSpec two)
		{
			return (((object)one) == null && ((object)two) == null) || one.Equals(two);
		}

		public static bool operator !=(FieldSpec one, FieldSpec two)
		{
			return !(one == two);
		}

		public override string ToString()
		{
			if (IsValid)
			{
				return string.Concat("(", LowBound, ':', HighBound, ')');
			}

			return ("(" + MixByteValue + ')');
		}

		public int ByteCount
		{
			get
			{
				return HighBoundByteIndex - LowBoundByteIndex + 1;
			}
		}

		public bool FloatingPoint
		{
			get
			{
				return LowBound == 0 && HighBound == FullWord.ByteCount + 1;
			}
		}

		public int HighBound
		{
			get
			{
				if (!IsValid)
				{
					throw new InvalidOperationException("this MixFieldSpec is not valid");
				}

				return (int)(MixByteValue % 8);
			}
		}

		public int HighBoundByteIndex
		{
			get
			{
				return (HighBound >= FullWord.ByteCount + 1) ? FullWord.ByteCount - 1 : HighBound - 1;
			}
		}

		public bool IncludesSign
		{
			get
			{
				return LowBound == 0;
			}
		}

		public bool IsValid
		{
			get
			{
				return IsValidFieldSpec(MixByteValue);
			}
		}

		public int LowBound
		{
			get
			{
				if (!IsValid)
				{
					throw new InvalidOperationException("this MixFieldSpec is not valid");
				}

				return (int)(MixByteValue / 8);
			}
		}

		public int LowBoundByteIndex
		{
			get
			{
				return LowBound <= 0 ? 0 : (LowBound - 1);
			}
		}
	}
}
