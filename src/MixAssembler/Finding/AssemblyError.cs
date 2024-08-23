using MixLib.Misc;

namespace MixAssembler.Finding
{
	public class AssemblyError(int lineNumber, LineSection lineSection, int causeStartIndex, int causeLength, ValidationError error) 
		: ValidationFinding(Severity.Error, lineNumber, lineSection, causeStartIndex, causeLength, error)
	{
  }
}
