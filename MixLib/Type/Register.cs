namespace MixLib.Type
{

	public abstract class Register : Word
	{
		readonly int mPaddingByteCount;

		protected Register(int byteCount, int paddingByteCount) : base(byteCount)
		{
			mPaddingByteCount = paddingByteCount;
		}

		public int ByteCountWithPadding => mPaddingByteCount + ByteCount;

		public MixByte GetByteWithPadding(int index)
		{
			return index < mPaddingByteCount ? 0 : base[index - mPaddingByteCount];
		}

		public FullWord FullWordValue
		{
			get
			{
				var word = new FullWord();

				for (int i = 0; (i < mPaddingByteCount) && (i < FullWord.ByteCount); i++)
				{
					word[i] = 0;
				}

				for (int i = 0; (i < ByteCount) && (i < FullWord.ByteCount); i++)
				{
					word[mPaddingByteCount + i] = base[i];
				}

				word.Sign = Sign;

				return word;
			}
		}
	}
}
