using MixLib.Type;

namespace MixAssembler
{
	public class ParsingStatus(SymbolCollection symbols) : AssemblingStatus
	{
		public LineSection LineSection { get; set; } = LineSection.LocationField;
		public SymbolCollection Symbols { get; private set; } = symbols ?? [];

		public ParsingStatus() : this(null) { }

		public void ReportParsingError(int causeStartIndex, int causeLength, string message)
			=> ReportParsingError(LineSection, causeStartIndex, causeLength, message);

		public void ReportParsingWarning(int causeStartIndex, int causeLength, string message)
			=> ReportParsingWarning(LineSection, causeStartIndex, causeLength, message);
	}
}
