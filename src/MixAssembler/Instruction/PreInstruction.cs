using MixLib.Instruction;

namespace MixAssembler.Instruction
{
	/// <summary>
	/// This class represents an instruction that has been parsed (first assembly pass), but not yet assembled.
	/// Specifically, it contains an instruction "template" and the parameters required to create an actual instance of that instruction.
	/// </summary>
	public class PreInstruction(InstructionBase instruction, IInstructionParameters parameters)
  {
		public InstructionBase Instruction => instruction;
		public IInstructionParameters Parameters => parameters;

		/// <summary>
		/// This property indicates if the PreInstruction can actually generate an instruction instance. For this to be the case, both an instruction template and 
		/// the instruction parameters must have been provided during construction of this PreInstruction.
		/// </summary>
		public bool IsDefined
			=> Instruction != null && Parameters != null;

		public InstructionInstanceBase CreateInstance(AssemblingStatus status)
			=> Parameters.CreateInstance(Instruction, status);
	}
}
