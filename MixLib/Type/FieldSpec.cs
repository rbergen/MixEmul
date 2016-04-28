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

        public int HighBoundByteIndex => (HighBound >= FullWord.ByteCount + 1) ? FullWord.ByteCount - 1 : HighBound - 1;

        public bool IncludesSign => LowBound == 0;

        public bool IsValid => IsValidFieldSpec(MixByteValue);

        public int LowBoundByteIndex => LowBound <= 0 ? 0 : (LowBound - 1);

        public int ByteCount => HighBoundByteIndex - LowBoundByteIndex + 1;

        public bool FloatingPoint => LowBound == 0 && HighBound == FullWord.ByteCount + 1;

        public static bool operator ==(FieldSpec one, FieldSpec two) =>
            (((object)one) == null && ((object)two) == null) || one.Equals(two);

        public static bool operator !=(FieldSpec one, FieldSpec two) => !(one == two);

        public override string ToString() => IsValid ? string.Concat("(", LowBound, ':', HighBound, ')') : "(" + MixByteValue + ')';

        public override bool Equals(object obj) => obj != null && obj is FieldSpec && ((FieldSpec)obj).MixByteValue == MixByteValue;

		public override int GetHashCode() => MixByteValue.GetHashCode();

		public static bool IsValidFieldSpec(MixByte value) => IsValidFieldSpec(value / 8, value % 8);

		public static bool IsValidFieldSpec(int lowBound, int highBound)
		{
			if (lowBound >= 0 && highBound <= FullWord.ByteCount && lowBound <= highBound) return true;

			if (lowBound == 0) return highBound == FullWord.ByteCount + 1;

			return false;
		}

		public int HighBound
		{
			get
			{
				if (!IsValid)
				{
					throw new InvalidOperationException("this MixFieldSpec is not valid");
				}

				return MixByteValue % 8;
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

				return MixByteValue / 8;
			}
		}
	}
}
