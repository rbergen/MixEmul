using System.Collections.Generic;
using MixLib.Instruction;

namespace MixLib
{
	public class LoaderInstructions
	{
		private readonly SortedDictionary<string, LoaderInstruction> instructions = [];

		public LoaderInstructions()
		{
			AddInstruction("ORIG", LoaderInstruction.Operations.SetLocationCounter, false);
			AddInstruction("CON", LoaderInstruction.Operations.SetMemoryWord, false);
			AddInstruction("ALF", LoaderInstruction.Operations.SetMemoryWord, true);
			AddInstruction("END", LoaderInstruction.Operations.SetProgramCounter, false);
		}

		public LoaderInstruction this[string mnemonic] 
			=> this.instructions.TryGetValue(mnemonic, out var value) ? value : null;

		private void AddInstruction(string mnemonic, LoaderInstruction.Operations operation, bool alphanumeric) 
			=> this.instructions.Add(mnemonic, new LoaderInstruction(mnemonic, operation, alphanumeric));
	}
}
