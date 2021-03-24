namespace MixLib.Type
{
	public class FullWord : Word, IFullWord
	{
		public new const int ByteCount = 5;

		public FullWord() : base(ByteCount) { }

		public FullWord(long value) : this() 
			=> LongValue = value;

		public FullWord(Word.Signs sign, long magnitude) : this()
		{
			MagnitudeLongValue = magnitude;
			Sign = sign;
		}

		public static implicit operator FullWord(long value) 
			=> new(value);

		public override object Clone()
		{
			var copy = new FullWord();
			CopyTo(copy);

			return copy;
		}
	}
}
