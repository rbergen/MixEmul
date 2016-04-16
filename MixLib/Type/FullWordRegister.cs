namespace MixLib.Type
{

	public class FullWordRegister : Register
	{
		public const int PaddingByteCount = 0;
		public const int RegisterByteCount = FullWord.ByteCount;

		public static FieldSpec DefaultFieldSpec { get; private set; } = new FieldSpec(0, RegisterByteCount);

		public FullWordRegister()
			: base(RegisterByteCount, PaddingByteCount)
		{
		}
	}
}
