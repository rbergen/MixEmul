namespace MixLib.Type
{

	public abstract class Register(int byteCount, int paddingByteCount) : Word(byteCount)
	{
		public int ByteCountWithPadding 
			=> paddingByteCount + ByteCount;

		public MixByte GetByteWithPadding(int index) 
			=> index < paddingByteCount ? 0 : base[index - paddingByteCount];

		public FullWord FullWordValue
		{
			get
			{
				var word = new FullWord();

				for (int i = 0; (i < paddingByteCount) && (i < FullWord.ByteCount); i++)
					word[i] = 0;

				for (int i = 0; (i < ByteCount) && (i < FullWord.ByteCount); i++)
					word[paddingByteCount + i] = base[i];

				word.Sign = Sign;

				return word;
			}
		}
	}
}
