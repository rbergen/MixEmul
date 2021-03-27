namespace MixLib.Type
{

	public class AddressRegister : Register
	{
		public static FieldSpec DefaultFieldSpec { get; private set; } = new FieldSpec(0, 2);

		public AddressRegister() : base(2, 0) { }
	}
}
