using MixLib.Instruction;

namespace MixAssembler.Instruction
{
	/// <summary>
	/// This class represents an instruction that has been parsed (first assembly pass), but not yet assembled.
	/// Specifically, it contains an instruction "template" and the the parameters required to create an actual instance of that instruction.
	/// </summary>
	public class PreInstruction
	{
		public InstructionBase Instruction { get; private set; }
		public IInstructionParameters Parameters { get; private set; }

		public PreInstruction(InstructionBase instruction, IInstructionParameters parameters)
		{
			Instruction = instruction;
			Parameters = parameters;
		}

		public InstructionInstanceBase CreateInstance(AssemblingStatus status)
		{
			return Parameters.CreateInstance(Instruction, status);
		}

		/// <summary>
		/// This property indicates if the PreInstruction can actually generate an instruction instance. For this to be the case, both an instruction template and 
		/// the instruction parameters must have been provided during construction of this PreInstruction.
		/// </summary>
		public bool IsDefined
		{
			get
			{
				return Instruction != null && Parameters != null;
			}
		}
	}
}
