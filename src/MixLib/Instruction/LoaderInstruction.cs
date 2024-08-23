﻿using MixLib.Type;

namespace MixLib.Instruction
{
	/// <summary>
	/// Instances of this class describe the characteristics of a specific loader instruction. 
	/// That is, an instruction in the MIXAL language that controls program assembly, but does not encode into a MIX instruction.
	/// 
	/// It adds the following information to that contained in InstructionBase:
	/// * The operation the loader instruction stands for
	/// * An indication if the instruction's parameter value ("address section") is alfanumeric
	/// </summary>
	public class LoaderInstruction(string mnemonic, LoaderInstruction.Operations operation, bool alphanumeric) : InstructionBase(mnemonic)
	{
		public bool Alphanumeric => alphanumeric;
		public Operations Operation => operation;

		/// <summary>
		/// This class represents a specific instance of a loader instruction.
		/// </summary>
		public class Instance(LoaderInstruction instruction, Word.Signs sign, long magnitude) : InstructionInstanceBase
		{
			public FullWord Value { get; private set; } = new FullWord(sign, magnitude);

			public override InstructionBase Instruction => instruction;
		}

		public enum Operations
		{
			SetLocationCounter = 1,
			SetMemoryWord = 2,
			SetProgramCounter = 3
		}
	}
}
