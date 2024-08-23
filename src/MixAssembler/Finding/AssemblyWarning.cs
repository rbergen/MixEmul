using MixLib.Misc;

namespace MixAssembler.Finding
{
	public class AssemblyWarning(int lineNumber, LineSection lineSection, int causeStartIndex, int causeLength, ValidationError error) 
		: ValidationFinding(Severity.Warning, lineNumber, lineSection, causeStartIndex, causeLength, error)
	{
  }
}
