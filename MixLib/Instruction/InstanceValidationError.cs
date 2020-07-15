using MixLib.Misc;

namespace MixLib.Instruction
{
	public class InstanceValidationError : ValidationError
	{
		public Sources Source { get; private set; }

		public InstanceValidationError(Sources source, string message) : base(message)
		{
			Source = source;
		}

		public InstanceValidationError(Sources source, int validLowerBound, int validUpperBound)
			: base(validLowerBound, validUpperBound)
		{
			Source = source;
		}

		public InstanceValidationError(Sources source, string message, int validLowerBound, int validUpperBound)
			: base(message, validLowerBound, validUpperBound)
		{
			Source = source;
		}

		public override string CompiledMessage
		{
			get
			{
				string baseMessage = Source.ToString().ToLower() + " invalid";
				string compiledMessage = base.CompiledMessage;
				return compiledMessage.Length != 0 ? baseMessage + ", " + compiledMessage : baseMessage;
			}
		}

		public enum Sources
		{
			Address,
			Index,
			FieldSpec
		}
	}
}
