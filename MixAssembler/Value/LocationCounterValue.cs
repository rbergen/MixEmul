using MixLib.Type;

namespace MixAssembler.Value
{
	/// <summary>
	/// This class represents the location counter placeholder
	/// </summary>
	public class LocationCounterValue
	{
		public static IValue ParseValue(string text, int sectionCharIndex, ParsingStatus status)
		{
			if (text != "*")
			{
				return null;
			}

			return new NumberValue(status.LocationCounter);
		}
	}
}
