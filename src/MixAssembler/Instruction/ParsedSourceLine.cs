using MixLib.Instruction;

namespace MixAssembler.Instruction
{
	/// <summary>
	/// Instances of this class represent a parsed line of MIXAL source code. Note that the fact that a source code line is parsed 
	/// does not mean that the instruction is valid.
	///
	/// The primary constructor creates an instance of this class that represents a (potentially) executable instruction. Note that the
	/// instruction does not necessarily have to be executable by MIX; it can also concern a loader instruction, for instance. In 
	/// addition, the instruction may be invalid.
	/// </summary>
	public class ParsedSourceLine(int lineNumber, string locationField, string opField, string addressField, string comment, InstructionBase instruction, IInstructionParameters parameters) 
		: PreInstruction(instruction, parameters)
	{
		public string AddressField => addressField;
		public string Comment => comment;
		public int LineNumber => lineNumber;
		public string LocationField => locationField;
		public string OpField => opField;

		/// <summary>
		/// Creates a instance of this class that represents a comment line.
		/// </summary>
		public ParsedSourceLine(int lineNumber, string comment) : this(lineNumber, null, null, null, comment, null, null)
		{
		}

		public bool IsCommentLine
			=> LocationField == null;
	}
}
