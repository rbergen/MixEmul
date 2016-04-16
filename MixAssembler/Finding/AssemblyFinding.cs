namespace MixAssembler.Finding
{

	public abstract class AssemblyFinding
	{
		private int mLength;
		private int mLineNumber;
		private MixAssembler.LineSection mSection;
		private MixLib.Misc.Severity mSeverity;
		private int mStartCharIndex;
		public const int NoNumber = int.MinValue;

		public AssemblyFinding(MixLib.Misc.Severity severity, int lineNumber, MixAssembler.LineSection section, int startCharIndex, int length)
		{
			mSection = section;
			mStartCharIndex = startCharIndex;
			mLength = length;
			mSeverity = severity;
			mLineNumber = lineNumber;
		}

		public int Length
		{
			get
			{
				return mLength;
			}
		}

		public int LineNumber
		{
			get
			{
				return mLineNumber;
			}
		}

		public MixAssembler.LineSection LineSection
		{
			get
			{
				return mSection;
			}
		}

		public abstract string Message
		{
			get;
		}

		public MixLib.Misc.Severity Severity
		{
			get
			{
				return mSeverity;
			}
		}

		public int StartCharIndex
		{
			get
			{
				return mStartCharIndex;
			}
		}
	}
}
