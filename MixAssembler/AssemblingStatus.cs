using MixAssembler.Finding;
using MixLib.Misc;

namespace MixAssembler
{
	public class AssemblingStatus
	{
		private AssemblyFindingCollection mFindings = new AssemblyFindingCollection();
		private int mLineNumber = 0;
		private int mLocationCounter = 0;

		public void ReportError(LineSection lineSection, int causeStartIndex, int causeLength, ValidationError error)
		{
			Findings.Add(new AssemblyError(LineNumber, lineSection, causeStartIndex, causeLength, error));
		}

		public void ReportParsingError(LineSection lineSection, int causeStartIndex, int causeLength, string message)
		{
			Findings.Add(new AssemblyError(LineNumber, lineSection, causeStartIndex, causeLength, new MixAssembler.Finding.ParsingError(message)));
		}

		public void ReportParsingError(LineSection lineSection, int causeStartIndex, int causeLength, string message, int lowerBound, int upperBound)
		{
			Findings.Add(new AssemblyError(LineNumber, lineSection, causeStartIndex, causeLength, new MixAssembler.Finding.ParsingError(message, lowerBound, upperBound)));
		}

		public void ReportParsingWarning(LineSection lineSection, int causeStartIndex, int causeLength, string message)
		{
			Findings.Add(new AssemblyWarning(LineNumber, lineSection, causeStartIndex, causeLength, new MixAssembler.Finding.ParsingError(message)));
		}

		public void ReportParsingWarning(LineSection lineSection, int causeStartIndex, int causeLength, string message, int lowerBound, int upperBound)
		{
			Findings.Add(new AssemblyWarning(LineNumber, lineSection, causeStartIndex, causeLength, new MixAssembler.Finding.ParsingError(message, lowerBound, upperBound)));
		}

		public void ReportWarning(LineSection lineSection, int causeStartIndex, int causeLength, ValidationError error)
		{
			Findings.Add(new AssemblyWarning(LineNumber, lineSection, causeStartIndex, causeLength, error));
		}

		public AssemblyFindingCollection Findings
		{
			get
			{
				return mFindings;
			}
		}

		public int LineNumber
		{
			get
			{
				return mLineNumber;
			}
			set
			{
				mLineNumber = value;
			}
		}

		public int LocationCounter
		{
			get
			{
				return mLocationCounter;
			}
			set
			{
				mLocationCounter = value;
			}
		}
	}
}
