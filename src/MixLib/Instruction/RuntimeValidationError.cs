using MixLib.Misc;

namespace MixLib.Instruction
{
	public class RuntimeValidationError(int programCounter, MixInstruction.Instance instance, string message, int validLowerBound, int validUpperBound, int actualValue) : ValidationError(message, validLowerBound, validUpperBound)
	{
	public int ActualValue => actualValue;
	public MixInstruction.Instance Instance => instance;
	public int ProgramCounter => programCounter;

	public RuntimeValidationError(int programCounter, MixInstruction.Instance instance, string message)
			: this(programCounter, instance, message, int.MinValue, int.MinValue, int.MinValue) {	}

		public RuntimeValidationError(int programCounter, MixInstruction.Instance instance, int validLowerBound, int validUpperBound, int actualValue)
			: this(programCounter, instance, null, validLowerBound, validUpperBound, actualValue) { }
  }
}
