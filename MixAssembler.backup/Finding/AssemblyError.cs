using MixLib.Misc;

namespace MixAssembler.Finding
{
	public class AssemblyError : ValidationFinding
	{
		public AssemblyError(int lineNumber, LineSection lineSection, int causeStartIndex, int causeLength, ValidationError error)
			: base(Severity.Error, lineNumber, lineSection, causeStartIndex, causeLength, error) { }
	}
}
