namespace MixAssembler.Finding
{
    public class AssemblyInfo : AssemblyFinding
    {
        private string mMessage;

        public AssemblyInfo(string message, int lineNumber, LineSection lineSection, int startCharIndex, int length)
          : base(MixLib.Misc.Severity.Info, lineNumber, lineSection, startCharIndex, length)
        {
            mMessage = message;
        }

        public override string Message => mMessage;
    }
}
