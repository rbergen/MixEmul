using MixLib.Type;

namespace MixAssembler
{
	public class ParsingStatus : AssemblingStatus
	{
		public LineSection LineSection { get; set; } = LineSection.LocationField;
		public SymbolCollection Symbols { get; private set; }

		public ParsingStatus()
			: this(null)
		{ }

		public ParsingStatus(SymbolCollection symbols)
		{
			Symbols = symbols ?? new SymbolCollection();
		}

		public void ReportParsingError(int causeStartIndex, int causeLength, string message) =>
				ReportParsingError(LineSection, causeStartIndex, causeLength, message);

		public void ReportParsingWarning(int causeStartIndex, int causeLength, string message) =>
						ReportParsingWarning(LineSection, causeStartIndex, causeLength, message);
	}
}
