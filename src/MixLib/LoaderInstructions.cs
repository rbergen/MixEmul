using System.Collections.Generic;
using MixLib.Instruction;

namespace MixLib
{
	public class LoaderInstructions
	{
		private readonly SortedDictionary<string, LoaderInstruction> instructions = new();

		public LoaderInstructions()
		{
			AddInstruction("ORIG", LoaderInstruction.Operations.SetLocationCounter, false);
			AddInstruction("CON", LoaderInstruction.Operations.SetMemoryWord, false);
			AddInstruction("ALF", LoaderInstruction.Operations.SetMemoryWord, true);
			AddInstruction("END", LoaderInstruction.Operations.SetProgramCounter, false);
		}

		public LoaderInstruction this[string mnemonic] 
			=> this.instructions.ContainsKey(mnemonic) ? this.instructions[mnemonic] : null;

		private void AddInstruction(string mnemonic, LoaderInstruction.Operations operation, bool alphanumeric) 
			=> this.instructions.Add(mnemonic, new LoaderInstruction(mnemonic, operation, alphanumeric));
	}
}
