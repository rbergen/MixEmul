using System.Collections.Generic;
using MixLib.Instruction;

namespace MixLib
{
	public class LoaderInstructions
	{
		private SortedDictionary<string, LoaderInstruction> mInstructions = new SortedDictionary<string, LoaderInstruction>();

		public LoaderInstructions()
		{
			addInstruction("ORIG", LoaderInstruction.Operations.SetLocationCounter, false);
			addInstruction("CON", LoaderInstruction.Operations.SetMemoryWord, false);
			addInstruction("ALF", LoaderInstruction.Operations.SetMemoryWord, true);
			addInstruction("END", LoaderInstruction.Operations.SetProgramCounter, false);
		}

        public LoaderInstruction this[string mnemonic] => mInstructions.ContainsKey(mnemonic) ? mInstructions[mnemonic] : null;

		private void addInstruction(string mnemonic, LoaderInstruction.Operations operation, bool alphanumeric) => 
            mInstructions.Add(mnemonic, new LoaderInstruction(mnemonic, operation, alphanumeric));
	}
}
