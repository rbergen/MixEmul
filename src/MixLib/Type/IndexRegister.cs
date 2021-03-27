namespace MixLib.Type
{

	public class IndexRegister : Register
	{
		public const int RegisterByteCount = 2;
		public const int PaddingByteCount = FullWord.ByteCount - RegisterByteCount;

		private static readonly FieldSpec mDefaultFieldSpec = new(0, FullWord.ByteCount);

		public IndexRegister() : base(RegisterByteCount, PaddingByteCount) { }

		public static FieldSpec DefaultFieldSpec => mDefaultFieldSpec;
	}
}
