namespace MixLib.Instruction
{
	public abstract class InstructionInstanceBase
	{
		public string SourceLine { get; set; }
		public abstract InstructionBase Instruction { get; }
	}
}
