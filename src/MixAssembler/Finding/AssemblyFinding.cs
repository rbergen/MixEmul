using MixLib.Misc;

namespace MixAssembler.Finding
{

	public abstract class AssemblyFinding(Severity severity, int lineNumber, LineSection section, int startCharIndex, int length)
  {
		public int Length { get; private set; } = length;
		public int LineNumber { get; private set; } = lineNumber;
		public LineSection LineSection { get; private set; } = section;
		public Severity Severity { get; private set; } = severity;
		public int StartCharIndex { get; private set; } = startCharIndex;

		public const int NoNumber = int.MinValue;

		public abstract string Message { get; }
	}
}
