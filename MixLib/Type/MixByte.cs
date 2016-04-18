using System;

namespace MixLib.Type
{
	public class MixByte
	{
		public const int BitCount = 6;
		public const byte MaxValue = (1 << BitCount) - 1;
		public const byte MinValue = 0;
		public const string MixChars = " ABCDEFGHIΔJKLMNOPQRΣΠSTUVWXYZ0123456789.,()+-*/=$<>@;:'█";

        public byte ByteValue { get; private set; }

        public MixByte()
			: this(0)
		{
		}

		public MixByte(byte value)
		{
			if (value > MaxValue)
			{
				throw new ArgumentException("value too large for MixByte", "value");
			}

			ByteValue = value;
		}

		public MixByte(char value)
		{
			int index = MixChars.IndexOf(value);
			ByteValue = (index >= 0 && index <= MixChars.Length - 1) ? (byte)index : MaxValue;
		}

		public override bool Equals(object o)
		{
			return o is MixByte && ByteValue == ((MixByte)o).ByteValue;
		}

		public override int GetHashCode()
		{
			return ByteValue.GetHashCode();
		}

		public static int operator +(MixByte mixbyte, byte delta)
		{
			return mixbyte.ByteValue + delta;
		}

		public static int operator /(MixByte mixbyte, byte divisor)
		{
			return mixbyte.ByteValue / divisor;
		}

		public static bool operator ==(MixByte mixbyte, byte value)
		{
			return mixbyte != null && mixbyte.ByteValue == value;
		}

		public static bool operator >(MixByte mixbyte, byte value)
		{
			return mixbyte.ByteValue > value;
		}

		public static bool operator >=(MixByte mixbyte, byte value)
		{
			return mixbyte.ByteValue >= value;
		}

		public static implicit operator byte(MixByte mixbyte)
		{
			return mixbyte.ByteValue;
		}

		public static implicit operator char(MixByte mixbyte)
		{
			return mixbyte.CharValue;
		}

		public static implicit operator int(MixByte mixbyte)
		{
			return mixbyte.ByteValue;
		}

		public static implicit operator MixByte(byte value)
		{
			return new MixByte(value);
		}

		public static implicit operator MixByte(char value)
		{
			return new MixByte(value);
		}

		public static implicit operator MixByte(int value)
		{
			return new MixByte((byte)value);
		}

		public static bool operator !=(MixByte mixbyte, byte value)
		{
			return mixbyte != null && mixbyte.ByteValue != value;
		}

		public static bool operator <(MixByte mixbyte, byte value)
		{
			return mixbyte.ByteValue < value;
		}

		public static bool operator <=(MixByte mixbyte, byte value)
		{
			return mixbyte.ByteValue <= value;
		}

		public static int operator %(MixByte mixbyte, byte divisor)
		{
			return mixbyte.ByteValue % divisor;
		}

		public static int operator -(MixByte mixbyte, byte delta)
		{
			return mixbyte.ByteValue - delta;
		}

		public override string ToString()
		{
			return ByteValue.ToString("D2");
		}

		public char CharValue
		{
			get
			{
				return ByteValue >= MixChars.Length ? MixChars[MixChars.Length - 1] : MixChars[ByteValue];
			}
		}
	}
}
