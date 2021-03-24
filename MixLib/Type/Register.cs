namespace MixLib.Type
{

	public abstract class Register : Word
	{
		private readonly int _paddingByteCount;

		protected Register(int byteCount, int paddingByteCount) : base(byteCount) 
			=> _paddingByteCount = paddingByteCount;

		public int ByteCountWithPadding 
			=> _paddingByteCount + ByteCount;

		public MixByte GetByteWithPadding(int index) 
			=> index < _paddingByteCount ? 0 : base[index - _paddingByteCount];

		public FullWord FullWordValue
		{
			get
			{
				var word = new FullWord();

				for (int i = 0; (i < _paddingByteCount) && (i < FullWord.ByteCount); i++)
					word[i] = 0;

				for (int i = 0; (i < ByteCount) && (i < FullWord.ByteCount); i++)
					word[_paddingByteCount + i] = base[i];

				word.Sign = Sign;

				return word;
			}
		}
	}
}
