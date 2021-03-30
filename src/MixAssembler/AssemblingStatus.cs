using MixAssembler.Finding;
using MixLib.Misc;

namespace MixAssembler
{
	public class AssemblingStatus
	{
		public AssemblyFindingCollection Findings { get; } = new();

		public void ReportError(LineSection lineSection, int causeStartIndex, int causeLength, ValidationError error)
			=> Findings.Add(new AssemblyError(LineNumber, lineSection, causeStartIndex, causeLength, error));

		public void ReportParsingError(LineSection lineSection, int causeStartIndex, int causeLength, string message)
			=> Findings.Add(new AssemblyError(LineNumber, lineSection, causeStartIndex, causeLength, new ParsingError(message)));

		public void ReportParsingError(LineSection lineSection, int causeStartIndex, int causeLength, string message, int lowerBound, int upperBound)
			=> Findings.Add(new AssemblyError(LineNumber, lineSection, causeStartIndex, causeLength, new ParsingError(message, lowerBound, upperBound)));

		public void ReportParsingWarning(LineSection lineSection, int causeStartIndex, int causeLength, string message)
			=> Findings.Add(new AssemblyWarning(LineNumber, lineSection, causeStartIndex, causeLength, new ParsingError(message)));

		public void ReportParsingWarning(LineSection lineSection, int causeStartIndex, int causeLength, string message, int lowerBound, int upperBound)
			=> Findings.Add(new AssemblyWarning(LineNumber, lineSection, causeStartIndex, causeLength, new ParsingError(message, lowerBound, upperBound)));

		public void ReportWarning(LineSection lineSection, int causeStartIndex, int causeLength, ValidationError error)
			=> Findings.Add(new AssemblyWarning(LineNumber, lineSection, causeStartIndex, causeLength, error));

		public int LineNumber { get; set; }

		public int LocationCounter { get; set; }
	}
}
