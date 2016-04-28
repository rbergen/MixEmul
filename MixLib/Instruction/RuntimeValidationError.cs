using MixLib.Misc;

namespace MixLib.Instruction
{
	public class RuntimeValidationError : ValidationError
	{
        public int ActualValue { get; private set; }
        public MixInstruction.Instance Instance { get; private set; }
        public int ProgramCounter { get; private set; }

        public RuntimeValidationError(int programCounter, MixInstruction.Instance instance, string message)
			: this(programCounter, instance, message, int.MinValue, int.MinValue, int.MinValue)
		{
		}

		public RuntimeValidationError(int programCounter, MixInstruction.Instance instance, int validLowerBound, int validUpperBound, int actualValue)
			: this(programCounter, instance, null, validLowerBound, validUpperBound, actualValue) { }

		public RuntimeValidationError(int programCounter, MixInstruction.Instance instance, string message, int validLowerBound, int validUpperBound, int actualValue)
			: base(message, validLowerBound, validUpperBound)
		{
			Instance = instance;
			ProgramCounter = programCounter;
			ActualValue = actualValue;
		}
    }
}
