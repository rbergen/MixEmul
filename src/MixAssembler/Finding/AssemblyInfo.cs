namespace MixAssembler.Finding
{
	public class AssemblyInfo : AssemblyFinding
	{
		private readonly string message;

		public AssemblyInfo(string message, int lineNumber, LineSection lineSection, int startCharIndex, int length)
			: base(MixLib.Misc.Severity.Info, lineNumber, lineSection, startCharIndex, length)
			=> this.message = message;

		public override string Message
			=> this.message;
	}
}
