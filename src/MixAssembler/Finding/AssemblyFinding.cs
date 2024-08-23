using MixLib.Misc;

namespace MixAssembler.Finding
{

	public abstract class AssemblyFinding(Severity severity, int lineNumber, LineSection section, int startCharIndex, int length)
  {
		public int Length => length;
		public int LineNumber => lineNumber;
		public LineSection LineSection => section;
		public Severity Severity => severity;
		public int StartCharIndex => startCharIndex;

		public const int NoNumber = int.MinValue;

		public abstract string Message { get; }
	}
}
