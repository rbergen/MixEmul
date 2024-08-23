namespace MixLib.Instruction
{

	/// <summary>
	/// This class serves as the base class for all instruction types recognized within MIXAL.
	/// It defines the attributes common to all types, which basically is the fact that they all have a mnemonic.
	/// </summary>
	public abstract class InstructionBase(string mnemonic)
  {
		public string Mnemonic => mnemonic;
  }
}
