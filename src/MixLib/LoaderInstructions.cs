using System.Collections.Generic;
using MixLib.Instruction;

namespace MixLib
{
	public class LoaderInstructions
	{
		private readonly SortedDictionary<string, LoaderInstruction> _instructions = new();

		public LoaderInstructions()
		{
			AddInstruction("ORIG", LoaderInstruction.Operations.SetLocationCounter, false);
			AddInstruction("CON", LoaderInstruction.Operations.SetMemoryWord, false);
			AddInstruction("ALF", LoaderInstruction.Operations.SetMemoryWord, true);
			AddInstruction("END", LoaderInstruction.Operations.SetProgramCounter, false);
		}

		public LoaderInstruction this[string mnemonic] 
			=> _instructions.ContainsKey(mnemonic) ? _instructions[mnemonic] : null;

		private void AddInstruction(string mnemonic, LoaderInstruction.Operations operation, bool alphanumeric) 
			=> _instructions.Add(mnemonic, new LoaderInstruction(mnemonic, operation, alphanumeric));
	}
}
