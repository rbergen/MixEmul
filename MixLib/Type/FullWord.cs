using System;

namespace MixLib.Type
{
	public class FullWord : Word, IFullWord
	{
		public new const int ByteCount = 5;

		public FullWord()
			: base(ByteCount)
		{
		}

		public FullWord(long value)
			: this()
		{
			base.LongValue = value;
		}

		public FullWord(Word.Signs sign, long magnitude)
			: this()
		{
			base.MagnitudeLongValue = magnitude;
			base.Sign = sign;
		}

		public static implicit operator FullWord(long value) => new FullWord(value);

		public override Object Clone()
		{
			FullWord copy = new FullWord();
			CopyTo(copy);

			return copy;
		}
	}
}
