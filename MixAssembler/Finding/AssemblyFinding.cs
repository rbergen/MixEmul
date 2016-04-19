using MixLib.Misc;

namespace MixAssembler.Finding
{

    public abstract class AssemblyFinding
    {
        public int Length { get; private set; }
        public int LineNumber { get; private set; }
        public LineSection LineSection { get; private set; }
        public Severity Severity { get; private set; }
        public int StartCharIndex { get; private set; }

        public const int NoNumber = int.MinValue;

        public AssemblyFinding(Severity severity, int lineNumber, LineSection section, int startCharIndex, int length)
        {
            LineSection = section;
            StartCharIndex = startCharIndex;
            Length = length;
            Severity = severity;
            LineNumber = lineNumber;
        }

        public abstract string Message { get; }
	}
}
