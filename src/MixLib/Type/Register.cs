namespace MixLib.Type
{

	public abstract class Register : Word
	{
		private readonly int paddingByteCount;

		protected Register(int byteCount, int paddingByteCount) : base(byteCount) 
			=> this.paddingByteCount = paddingByteCount;

		public int ByteCountWithPadding 
			=> this.paddingByteCount + ByteCount;

		public MixByte GetByteWithPadding(int index) 
			=> index < this.paddingByteCount ? 0 : base[index - this.paddingByteCount];

		public FullWord FullWordValue
		{
			get
			{
				var word = new FullWord();

				for (int i = 0; (i < this.paddingByteCount) && (i < FullWord.ByteCount); i++)
					word[i] = 0;

				for (int i = 0; (i < ByteCount) && (i < FullWord.ByteCount); i++)
					word[this.paddingByteCount + i] = base[i];

				word.Sign = Sign;

				return word;
			}
		}
	}
}
