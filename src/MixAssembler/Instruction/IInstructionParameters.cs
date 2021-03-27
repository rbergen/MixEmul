using MixLib.Instruction;

namespace MixAssembler.Instruction
{
	public interface IInstructionParameters
	{
		InstructionInstanceBase CreateInstance(InstructionBase instruction, AssemblingStatus status);
	}
}
