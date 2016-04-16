using MixLib.Misc;

namespace MixAssembler.Finding
{
	public class AssemblyWarning : ValidationFinding
	{
		public AssemblyWarning(int lineNumber, LineSection lineSection, int causeStartIndex, int causeLength, ValidationError error)
			: base(Severity.Warning, lineNumber, lineSection, causeStartIndex, causeLength, error)
		{
		}
	}
}
