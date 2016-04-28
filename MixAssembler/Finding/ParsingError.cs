using MixLib.Misc;

namespace MixAssembler.Finding
{
	public class ParsingError : ValidationError
	{
		public ParsingError(string message) : base(message) { }

		public ParsingError(string message, int lowerBound, int upperBound)
			: base(message, lowerBound, upperBound) { }
	}
}
