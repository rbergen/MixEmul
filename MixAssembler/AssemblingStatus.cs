using MixAssembler.Finding;
using MixLib.Misc;

namespace MixAssembler
{
	public class AssemblingStatus
	{
		private readonly AssemblyFindingCollection _findings = new();
		private int _lineNumber;
		private int _locationCounter;

		public AssemblyFindingCollection Findings => _findings;

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

		public int LineNumber
		{
			get => _lineNumber;
			set => _lineNumber = value;
		}

		public int LocationCounter
		{
			get => _locationCounter;
			set => _locationCounter = value;
		}
	}
}
