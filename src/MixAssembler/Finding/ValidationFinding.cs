using MixLib.Misc;

namespace MixAssembler.Finding
{
	public class ValidationFinding(Severity severity, int lineNumber, LineSection lineSection, int causeStartIndex, int causeLength, ValidationError error) 
		: AssemblyFinding(severity, lineNumber, lineSection, causeStartIndex, causeLength)
	{
		public ValidationError Error => error;

		public override string Message
				=> Error.CompiledMessage;

	}
}
