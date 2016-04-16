namespace MixLib.Type
{

	public class IndexRegister : Register
	{
		public const int RegisterByteCount = 2;
		public const int PaddingByteCount = FullWord.ByteCount - RegisterByteCount;

		private static FieldSpec mDefaultFieldSpec = new FieldSpec(0, FullWord.ByteCount);

		public IndexRegister()
			: base(RegisterByteCount, PaddingByteCount)
		{
		}

		public static FieldSpec DefaultFieldSpec
		{
			get
			{
				return mDefaultFieldSpec;
			}
		}
	}
}
