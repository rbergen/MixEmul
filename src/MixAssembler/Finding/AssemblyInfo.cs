namespace MixAssembler.Finding
{
	public class AssemblyInfo(string message, int lineNumber, LineSection lineSection, int startCharIndex, int length) 
		: AssemblyFinding(MixLib.Misc.Severity.Info, lineNumber, lineSection, startCharIndex, length)
	{
		public override string Message
			=> message;
	}
}
