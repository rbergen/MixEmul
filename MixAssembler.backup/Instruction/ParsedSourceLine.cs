using MixLib.Instruction;

namespace MixAssembler.Instruction
{
	/// <summary>
	/// Instances of this class represent a parsed line of MIXAL source code. Note that the fact that a source code line is parsed 
	/// does not mean that the instruction is valid.
	/// </summary>
	public class ParsedSourceLine : PreInstruction
	{
		public string AddressField { get; private set; }
		public string Comment { get; private set; }
		public int LineNumber { get; private set; }
		public string LocationField { get; private set; }
		public string OpField { get; private set; }

		/// <summary>
		/// Creates a instance of this class that represents a comment line.
		/// </summary>
		public ParsedSourceLine(int lineNumber, string comment) : base(null, null)
		{
			LineNumber = lineNumber;
			LocationField = null;
			OpField = null;
			AddressField = null;
			Comment = comment;
		}

		/// <summary>
		/// Creates an instance of this class that represents a (potentially) executable instruction. Note that the instruction does not
		/// necessarily have to be executable by MIX; it can also concern a loader instruction, for instance. In addition, the instruction
		/// may be invalid.
		/// </summary>
		public ParsedSourceLine(int lineNumber, string locationField, string opField, string addressField, string comment, InstructionBase instruction, IInstructionParameters parameters)
			: base(instruction, parameters)
		{
			LineNumber = lineNumber;
			LocationField = locationField;
			OpField = opField;
			AddressField = addressField;
			Comment = comment;
		}

		public bool IsCommentLine => LocationField == null;
	}
}
