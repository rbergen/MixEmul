using MixLib.Type;

namespace MixAssembler.Value
{
	/// <summary>
	/// This class represents the location counter placeholder
	/// </summary>
	public class LocationCounterValue
	{
		public static IValue ParseValue(string text, int sectionCharIndex, ParsingStatus status) => 
            text != "*" ? null : new NumberValue(status.LocationCounter);
	}
}
