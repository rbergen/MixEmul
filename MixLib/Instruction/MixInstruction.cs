using System;
using MixLib.Modules;
using MixLib.Type;

namespace MixLib.Instruction
{
	/// <summary>
	/// Instances of this class describe the characteristics of a specific MIX instruction. 
	/// That is, an instruction in the MIXAL language that encodes into an instruction word, and can be executed by MIX in runtime.
	/// 
	/// It adds the following information to that contained in InstructionBase:
	/// * The numeric opcode for the instruction (mandatory)
	/// * The fieldspec value for the instruction, for those instructions that have one (like JMP, JSJ and the other non-register jumps) (can only and must be specified if MetaFieldSpec.Presence is Forbidden)
	/// * The MetaFieldSpec for the instruction (mandatory)
	/// * The number of ticks that the instruction takes to execute. Note: certain instructions, like JBUS, take more ticks, depending on the state of the machine (mandatory)
	/// * The method (delegate) that performs the actual operation of the instruction (mandatory)
	/// * The method (delegate) that performs validations to determine if the instruction is in fact valid (optional)
	///
	/// After an object of this class is instantiated, the CreateInstruction method can be used to create an instruction instance (MixInstruction.Instance) to represent, validate and execute an actual, in-memory, instruction word.
	/// </summary>
	public class MixInstruction : InstructionBase
	{
		public const int AddressByteCount = 2;
		public const int IndexByte = AddressByteCount;
		public const int FieldSpecByte = IndexByte + 1;
		public const int OpcodeByte = FieldSpecByte + 1;
		private readonly Executor mExecutor;
		private readonly Validator mValidator;

		public FieldSpec FieldSpec { get; private set; }
		public MetaFieldSpec MetaFieldSpec { get; private set; }
		public byte Opcode { get; private set; }
		public int TickCount { get; private set; }

		/// <summary>
		/// Create a MIX instruction with the specified opcode, fieldspec, mnemonic, tickCount, executor and validator.
		/// The instruction's MetaFieldSpec will indicate that for this instruction, fieldspecs are forbidden in the MIXAL code.
		/// </summary>
		public MixInstruction(byte opcode, FieldSpec fieldSpec, string mnemonic, int tickCount, Executor executor, Validator validator)
			: this(opcode, fieldSpec, new MetaFieldSpec(), mnemonic, tickCount, executor, validator) { }

		/// <summary>
		/// Create a MIX instruction with the specified opcode, MetaFieldSpec, mnemonic, tickCount, executor and validator.
		/// </summary>
		public MixInstruction(byte opcode, string mnemonic, MetaFieldSpec metaFieldSpec, int tickCount, Executor executor, Validator validator)
			: this(opcode, null, metaFieldSpec, mnemonic, tickCount, executor, validator) { }

		private MixInstruction(byte opcode, FieldSpec fieldSpec, MetaFieldSpec metaFieldSpec, string mnemonic, int tickCount, Executor executor, Validator validator)
				: base(mnemonic)
		{

			// if the MetaFieldSpec Presence is Forbidden then an instruction fieldspec must be specified. Likewise, if an instruction fieldspec is provided, the MetaFieldSpec Presence must be Forbidden.
			if ((fieldSpec == null && metaFieldSpec.Presence == Instruction.MetaFieldSpec.Presences.Forbidden) || (metaFieldSpec.Presence != Instruction.MetaFieldSpec.Presences.Forbidden && fieldSpec != null))
				throw new ArgumentException("forbidden fieldspec presence makes instruction fieldspec mandatory and vice versa");

			Opcode = opcode;
			FieldSpec = fieldSpec;
			MetaFieldSpec = metaFieldSpec;
			TickCount = tickCount;
			mExecutor = executor ?? throw new ArgumentNullException(nameof(executor));
			mValidator = validator;
		}

		public Instance CreateInstance(IFullWord instructionWord)
			=> new(this, instructionWord);

		public class Instance : InstructionInstanceBase
		{
			public MixInstruction MixInstruction { get; private set; }
			public IFullWord InstructionWord { get; private set; }

			public Instance(MixInstruction instruction, IFullWord instructionWord)
			{
				// check if this instruction word is actually encoding the instruction it is provided with...
				if (instructionWord[OpcodeByte] != instruction.Opcode)
					throw new ArgumentException("opcode in word doesn't match instruction opcode", nameof(instructionWord));

				// ... and include the instruction fieldspec if there is one
				if (instruction.FieldSpec != null && instructionWord[FieldSpecByte] != instruction.FieldSpec.MixByteValue)
					throw new ArgumentException("fieldspec in word doesn't match instruction fieldspec", nameof(instructionWord));

				MixInstruction = instruction;
				InstructionWord = instructionWord;
			}

			public int AddressValue
				=> AddressValueFromWord(InstructionWord);

			public int AddressMagnitude
				=> AddressMagnitudeFromWord(InstructionWord);

			public Word.Signs AddressSign
				=> InstructionWord.Sign;

			public FieldSpec FieldSpec
				=> new(InstructionWord[FieldSpecByte]);

			public byte Index
				=> InstructionWord[IndexByte];

			public override InstructionBase Instruction
				=> MixInstruction;

			public Word.Signs Sign
				=> InstructionWord.Sign;

			public InstanceValidationError[] Validate()
				=> MixInstruction.mValidator == null ? null : MixInstruction.mValidator(this);

			public override string ToString()
				=> new InstructionText(this).InstanceText;

			private static int AddressMagnitudeFromWord(IWord word)
				=> (int)Word.BytesToLong(Word.Signs.Positive, new MixByte[] { word[0], word[1] });

			private static int AddressValueFromWord(IWord word)
				=> (int)Word.BytesToLong(word.Sign, new MixByte[] { word[0], word[1] });

			public bool Execute(ModuleBase module)
			{
				if (Validate() != null)
					throw new ArgumentException("instance not valid for instruction");

				return MixInstruction.mExecutor(module, this);
			}
		}

		public delegate bool Executor(ModuleBase module, MixInstruction.Instance instance);
		public delegate InstanceValidationError[] Validator(MixInstruction.Instance instance);
	}
}
