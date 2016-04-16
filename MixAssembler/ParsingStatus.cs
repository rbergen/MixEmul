using MixLib.Type;

namespace MixAssembler
{
	public class ParsingStatus : AssemblingStatus
	{
		private MixAssembler.LineSection mLineSection = MixAssembler.LineSection.LocationField;
		private SymbolCollection mSymbols;

		public ParsingStatus()
			: this(null)
		{ }

		public ParsingStatus(SymbolCollection symbols)
		{
			if (symbols != null)
			{
				mSymbols = symbols;
			}
			else
			{
				mSymbols = new SymbolCollection();
			}
		}

		public void ReportParsingError(int causeStartIndex, int causeLength, string message)
		{
			base.ReportParsingError(LineSection, causeStartIndex, causeLength, message);
		}

		public void ReportParsingWarning(int causeStartIndex, int causeLength, string message)
		{
			base.ReportParsingWarning(LineSection, causeStartIndex, causeLength, message);
		}

		public MixAssembler.LineSection LineSection
		{
			get
			{
				return mLineSection;
			}
			set
			{
				mLineSection = value;
			}
		}

		public SymbolCollection Symbols
		{
			get
			{
				return mSymbols;
			}
		}
	}
}
