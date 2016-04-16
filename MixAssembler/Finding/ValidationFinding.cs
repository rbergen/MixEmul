using MixAssembler;
using MixLib.Misc;

namespace MixAssembler.Finding
{
	public class ValidationFinding : AssemblyFinding
	{
		private ValidationError mError;

		public ValidationFinding(Severity severity, int lineNumber, LineSection lineSection, int causeStartIndex, int causeLength, ValidationError error)
			: base(severity, lineNumber, lineSection, causeStartIndex, causeLength)
		{
			mError = error;
		}

		public ValidationError Error
		{
			get
			{
				return mError;
			}
		}

		public override string Message
		{
			get
			{
				return mError.CompiledMessage;
			}
		}
	}
}
