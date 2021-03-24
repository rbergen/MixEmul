using MixLib.Misc;

namespace MixAssembler.Finding
{
	public class ValidationFinding : AssemblyFinding
	{
		public ValidationError Error { get; private set; }

		public ValidationFinding(Severity severity, int lineNumber, LineSection lineSection, int causeStartIndex, int causeLength, ValidationError error)
			: base(severity, lineNumber, lineSection, causeStartIndex, causeLength) 
			=> Error = error;

		public override string Message 
			=> Error.CompiledMessage;

	}
}
