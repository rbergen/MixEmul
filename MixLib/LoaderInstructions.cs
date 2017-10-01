using System.Collections.Generic;
using MixLib.Instruction;

namespace MixLib
{
	public class LoaderInstructions
	{
        readonly SortedDictionary<string, LoaderInstruction> mInstructions = new SortedDictionary<string, LoaderInstruction>();

        public LoaderInstructions()
		{
			AddInstruction("ORIG", LoaderInstruction.Operations.SetLocationCounter, false);
			AddInstruction("CON", LoaderInstruction.Operations.SetMemoryWord, false);
			AddInstruction("ALF", LoaderInstruction.Operations.SetMemoryWord, true);
			AddInstruction("END", LoaderInstruction.Operations.SetProgramCounter, false);
		}

        public LoaderInstruction this[string mnemonic] => mInstructions.ContainsKey(mnemonic) ? mInstructions[mnemonic] : null;

        void AddInstruction(string mnemonic, LoaderInstruction.Operations operation, bool alphanumeric) =>
            mInstructions.Add(mnemonic, new LoaderInstruction(mnemonic, operation, alphanumeric));
    }
}
