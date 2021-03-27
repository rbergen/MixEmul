namespace MixLib.Type
{

	public interface IValue
	{
		long GetValue(int currentAddress);
		bool IsValueDefined(int currentAddress);
		long GetMagnitude(int currentAddress);
		Word.Signs GetSign(int currentAddress);
	}
}
