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

        public MixByte() : this(0) { }

		public MixByte(byte value)
		{
			if (value > MaxValue)
			{
				throw new ArgumentException("value too large for MixByte", nameof(value));
			}

			ByteValue = value;
		}

		public MixByte(char value)
		{
			var index = MixChars.IndexOf(value);
			ByteValue = (index >= 0 && index <= MixChars.Length - 1) ? (byte)index : MaxValue;
		}

        public char CharValue => ByteValue >= MixChars.Length ? MixChars[MixChars.Length - 1] : MixChars[ByteValue];

        public static int operator +(MixByte mixbyte, byte delta) => mixbyte.ByteValue + delta;

		public static int operator /(MixByte mixbyte, byte divisor) => mixbyte.ByteValue / divisor;

		public static bool operator ==(MixByte mixbyte, byte value) => mixbyte != null && mixbyte.ByteValue == value;

		public static bool operator >(MixByte mixbyte, byte value) => mixbyte.ByteValue > value;

		public static bool operator >=(MixByte mixbyte, byte value) => mixbyte.ByteValue >= value;

		public static implicit operator byte(MixByte mixbyte) => mixbyte.ByteValue;

		public static implicit operator char(MixByte mixbyte) => mixbyte.CharValue;

		public static implicit operator int(MixByte mixbyte) => mixbyte.ByteValue;

		public static implicit operator MixByte(byte value) => new MixByte(value);

		public static implicit operator MixByte(char value) => new MixByte(value);

		public static implicit operator MixByte(int value) => new MixByte((byte)value);

		public static bool operator !=(MixByte mixbyte, byte value) => mixbyte != null && mixbyte.ByteValue != value;

		public static bool operator <(MixByte mixbyte, byte value) => mixbyte.ByteValue < value;

		public static bool operator <=(MixByte mixbyte, byte value) => mixbyte.ByteValue <= value;

		public static int operator %(MixByte mixbyte, byte divisor) => mixbyte.ByteValue % divisor;

		public static int operator -(MixByte mixbyte, byte delta) => mixbyte.ByteValue - delta;

		public override string ToString() => ByteValue.ToString("D2");

        public override bool Equals(object obj) => obj is MixByte && ByteValue == ((MixByte)obj).ByteValue;

        public override int GetHashCode() => ByteValue.GetHashCode();
	}
}
