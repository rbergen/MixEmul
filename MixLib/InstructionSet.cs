using System.Collections.Generic;
using MixLib.Instruction;
using MixLib.Type;
using MixLib.Utils;

namespace MixLib
{
	public class InstructionSet
	{
		private SortedDictionary<string, MixInstruction> mMnemonicInstructionMap = new SortedDictionary<string, MixInstruction>();
		private SortedDictionary<byte, List<MixInstruction>> mOpcodeInstructionMap = new SortedDictionary<byte, List<MixInstruction>>();
		private static InstructionSet mInstance;

		public InstructionSet()
		{
			MetaFieldSpec fullWordMetaSpec = new MetaFieldSpec(true, FullWordRegister.DefaultFieldSpec);
			MetaFieldSpec indexMetaSpec = new MetaFieldSpec(true, IndexRegister.DefaultFieldSpec);

			FieldSpec[] rangeSpecs = new FieldSpec[8];
			for (int i = 0; i < rangeSpecs.Length; i++)
			{
				rangeSpecs[i] = new FieldSpec(i);
			}

			MixInstruction.Executor executor = LoadInstructions.Load;
			MixInstruction.Validator validator = InstructionHelpers.ValidateIndexAndFieldSpec;
			addInstruction("LDA", 8, fullWordMetaSpec, 2, executor, validator);
			addInstruction("LD1", 9, indexMetaSpec, 2, executor, validator);
			addInstruction("LD2", 10, indexMetaSpec, 2, executor, validator);
			addInstruction("LD3", 11, indexMetaSpec, 2, executor, validator);
			addInstruction("LD4", 12, indexMetaSpec, 2, executor, validator);
			addInstruction("LD5", 13, indexMetaSpec, 2, executor, validator);
			addInstruction("LD6", 14, indexMetaSpec, 2, executor, validator);
			addInstruction("LDX", 15, fullWordMetaSpec, 2, executor, validator);

			executor = LoadInstructions.LoadNegative;
			addInstruction("LDAN", 16, fullWordMetaSpec, 2, executor, validator);
			addInstruction("LD1N", 17, indexMetaSpec, 2, executor, validator);
			addInstruction("LD2N", 18, indexMetaSpec, 2, executor, validator);
			addInstruction("LD3N", 19, indexMetaSpec, 2, executor, validator);
			addInstruction("LD4N", 20, indexMetaSpec, 2, executor, validator);
			addInstruction("LD5N", 21, indexMetaSpec, 2, executor, validator);
			addInstruction("LD6N", 22, indexMetaSpec, 2, executor, validator);
			addInstruction("LDXN", 23, fullWordMetaSpec, 2, executor, validator);

			executor = StoreInstructions.StoreRegister;
			addInstruction("STA", 24, fullWordMetaSpec, 2, executor, validator);
			addInstruction("ST1", 25, indexMetaSpec, 2, executor, validator);
			addInstruction("ST2", 26, indexMetaSpec, 2, executor, validator);
			addInstruction("ST3", 27, indexMetaSpec, 2, executor, validator);
			addInstruction("ST4", 28, indexMetaSpec, 2, executor, validator);
			addInstruction("ST5", 29, indexMetaSpec, 2, executor, validator);
			addInstruction("ST6", 30, indexMetaSpec, 2, executor, validator);
			addInstruction("STX", 31, fullWordMetaSpec, 2, executor, validator);
			addInstruction("STJ", 32, new MetaFieldSpec(true, AddressRegister.DefaultFieldSpec), 2, executor, validator);

			executor = StoreInstructions.StoreZero;
			addInstruction("STZ", 33, new MetaFieldSpec(true, Memory.DefaultFieldSpec), 2, executor, validator);

			executor = ArithmaticInstructions.AddSubstract;
			addInstruction("ADD", 1, fullWordMetaSpec, 2, executor, validator);
			addInstruction("SUB", 2, fullWordMetaSpec, 2, executor, validator);

			executor = ArithmaticInstructions.Multiply;
			addInstruction("MUL", 3, fullWordMetaSpec, 10, executor, validator);

			executor = ArithmaticInstructions.Divide;
			addInstruction("DIV", 4, fullWordMetaSpec, 12, executor, validator);

			executor = ComparisonInstructions.Compare;
			addInstruction("CMPA", 56, fullWordMetaSpec, 2, executor, validator);
			addInstruction("CMP1", 57, indexMetaSpec, 2, executor, validator);
			addInstruction("CMP2", 58, indexMetaSpec, 2, executor, validator);
			addInstruction("CMP3", 59, indexMetaSpec, 2, executor, validator);
			addInstruction("CMP4", 60, indexMetaSpec, 2, executor, validator);
			addInstruction("CMP5", 61, indexMetaSpec, 2, executor, validator);
			addInstruction("CMP6", 62, indexMetaSpec, 2, executor, validator);
			addInstruction("CMPX", 63, fullWordMetaSpec, 2, executor, validator);

			executor = AddressTransferInstructions.Increase;
			validator = InstructionHelpers.ValidateIndex;
			addInstruction("INCA", 48, rangeSpecs[0], 1, executor, validator);
			addInstruction("INC1", 49, rangeSpecs[0], 1, executor, validator);
			addInstruction("INC2", 50, rangeSpecs[0], 1, executor, validator);
			addInstruction("INC3", 51, rangeSpecs[0], 1, executor, validator);
			addInstruction("INC4", 52, rangeSpecs[0], 1, executor, validator);
			addInstruction("INC5", 53, rangeSpecs[0], 1, executor, validator);
			addInstruction("INC6", 54, rangeSpecs[0], 1, executor, validator);
			addInstruction("INCX", 55, rangeSpecs[0], 1, executor, validator);

			executor = AddressTransferInstructions.Decrease;
			addInstruction("DECA", 48, rangeSpecs[1], 1, executor, validator);
			addInstruction("DEC1", 49, rangeSpecs[1], 1, executor, validator);
			addInstruction("DEC2", 50, rangeSpecs[1], 1, executor, validator);
			addInstruction("DEC3", 51, rangeSpecs[1], 1, executor, validator);
			addInstruction("DEC4", 52, rangeSpecs[1], 1, executor, validator);
			addInstruction("DEC5", 53, rangeSpecs[1], 1, executor, validator);
			addInstruction("DEC6", 54, rangeSpecs[1], 1, executor, validator);
			addInstruction("DECX", 55, rangeSpecs[1], 1, executor, validator);

			executor = AddressTransferInstructions.Enter;
			addInstruction("ENTA", 48, rangeSpecs[2], 1, executor, validator);
			addInstruction("ENT1", 49, rangeSpecs[2], 1, executor, validator);
			addInstruction("ENT2", 50, rangeSpecs[2], 1, executor, validator);
			addInstruction("ENT3", 51, rangeSpecs[2], 1, executor, validator);
			addInstruction("ENT4", 52, rangeSpecs[2], 1, executor, validator);
			addInstruction("ENT5", 53, rangeSpecs[2], 1, executor, validator);
			addInstruction("ENT6", 54, rangeSpecs[2], 1, executor, validator);
			addInstruction("ENTX", 55, rangeSpecs[2], 1, executor, validator);

			executor = AddressTransferInstructions.EnterNegative;
			addInstruction("ENNA", 48, rangeSpecs[3], 1, executor, validator);
			addInstruction("ENN1", 49, rangeSpecs[3], 1, executor, validator);
			addInstruction("ENN2", 50, rangeSpecs[3], 1, executor, validator);
			addInstruction("ENN3", 51, rangeSpecs[3], 1, executor, validator);
			addInstruction("ENN4", 52, rangeSpecs[3], 1, executor, validator);
			addInstruction("ENN5", 53, rangeSpecs[3], 1, executor, validator);
			addInstruction("ENN6", 54, rangeSpecs[3], 1, executor, validator);
			addInstruction("ENNX", 55, rangeSpecs[3], 1, executor, validator);

			executor = JumpInstructions.NonRegJump;
			addInstruction("JMP", 39, rangeSpecs[0], 1, executor, validator);
			addInstruction("JSJ", 39, rangeSpecs[1], 1, executor, validator);
			addInstruction("JOV", 39, rangeSpecs[2], 1, executor, validator);
			addInstruction("JNOV", 39, rangeSpecs[3], 1, executor, validator);
			addInstruction("JL", 39, rangeSpecs[4], 1, executor, validator);
			addInstruction("JE", 39, rangeSpecs[5], 1, executor, validator);
			addInstruction("JG", 39, rangeSpecs[6], 1, executor, validator);
			addInstruction("JGE", 39, rangeSpecs[7], 1, executor, validator);
			addInstruction("JNE", 39, new FieldSpec(8), 1, executor, validator);
			addInstruction("JLE", 39, new FieldSpec(9), 1, executor, validator);

			executor = JumpInstructions.RegJump;
			addInstruction("JAN", 40, rangeSpecs[0], 1, executor, validator);
			addInstruction("JAZ", 40, rangeSpecs[1], 1, executor, validator);
			addInstruction("JAP", 40, rangeSpecs[2], 1, executor, validator);
			addInstruction("JANN", 40, rangeSpecs[3], 1, executor, validator);
			addInstruction("JANZ", 40, rangeSpecs[4], 1, executor, validator);
			addInstruction("JANP", 40, rangeSpecs[5], 1, executor, validator);
			addInstruction("JAE", 40, rangeSpecs[6], 1, executor, validator);
			addInstruction("JAO", 40, rangeSpecs[7], 1, executor, validator);

			addInstruction("J1N", 41, rangeSpecs[0], 1, executor, validator);
			addInstruction("J1Z", 41, rangeSpecs[1], 1, executor, validator);
			addInstruction("J1P", 41, rangeSpecs[2], 1, executor, validator);
			addInstruction("J1NN", 41, rangeSpecs[3], 1, executor, validator);
			addInstruction("J1NZ", 41, rangeSpecs[4], 1, executor, validator);
			addInstruction("J1NP", 41, rangeSpecs[5], 1, executor, validator);

			addInstruction("J2N", 42, rangeSpecs[0], 1, executor, validator);
			addInstruction("J2Z", 42, rangeSpecs[1], 1, executor, validator);
			addInstruction("J2P", 42, rangeSpecs[2], 1, executor, validator);
			addInstruction("J2NN", 42, rangeSpecs[3], 1, executor, validator);
			addInstruction("J2NZ", 42, rangeSpecs[4], 1, executor, validator);
			addInstruction("J2NP", 42, rangeSpecs[5], 1, executor, validator);

			addInstruction("J3N", 43, rangeSpecs[0], 1, executor, validator);
			addInstruction("J3Z", 43, rangeSpecs[1], 1, executor, validator);
			addInstruction("J3P", 43, rangeSpecs[2], 1, executor, validator);
			addInstruction("J3NN", 43, rangeSpecs[3], 1, executor, validator);
			addInstruction("J3NZ", 43, rangeSpecs[4], 1, executor, validator);
			addInstruction("J3NP", 43, rangeSpecs[5], 1, executor, validator);

			addInstruction("J4N", 44, rangeSpecs[0], 1, executor, validator);
			addInstruction("J4Z", 44, rangeSpecs[1], 1, executor, validator);
			addInstruction("J4P", 44, rangeSpecs[2], 1, executor, validator);
			addInstruction("J4NN", 44, rangeSpecs[3], 1, executor, validator);
			addInstruction("J4NZ", 44, rangeSpecs[4], 1, executor, validator);
			addInstruction("J4NP", 44, rangeSpecs[5], 1, executor, validator);

			addInstruction("J5N", 45, rangeSpecs[0], 1, executor, validator);
			addInstruction("J5Z", 45, rangeSpecs[1], 1, executor, validator);
			addInstruction("J5P", 45, rangeSpecs[2], 1, executor, validator);
			addInstruction("J5NN", 45, rangeSpecs[3], 1, executor, validator);
			addInstruction("J5NZ", 45, rangeSpecs[4], 1, executor, validator);
			addInstruction("J5NP", 45, rangeSpecs[5], 1, executor, validator);

			addInstruction("J6N", 46, rangeSpecs[0], 1, executor, validator);
			addInstruction("J6Z", 46, rangeSpecs[1], 1, executor, validator);
			addInstruction("J6P", 46, rangeSpecs[2], 1, executor, validator);
			addInstruction("J6NN", 46, rangeSpecs[3], 1, executor, validator);
			addInstruction("J6NZ", 46, rangeSpecs[4], 1, executor, validator);
			addInstruction("J6NP", 46, rangeSpecs[5], 1, executor, validator);

			addInstruction("JXN", 47, rangeSpecs[0], 1, executor, validator);
			addInstruction("JXZ", 47, rangeSpecs[1], 1, executor, validator);
			addInstruction("JXP", 47, rangeSpecs[2], 1, executor, validator);
			addInstruction("JXNN", 47, rangeSpecs[3], 1, executor, validator);
			addInstruction("JXNZ", 47, rangeSpecs[4], 1, executor, validator);
			addInstruction("JXNP", 47, rangeSpecs[5], 1, executor, validator);
			addInstruction("JXE", 47, rangeSpecs[6], 1, executor, validator);
			addInstruction("JXO", 47, rangeSpecs[7], 1, executor, validator);

			executor = FloatingPointInstructions.DoFloatingPoint;
			addInstruction("FADD", 1, rangeSpecs[6], 4, executor, validator);
			addInstruction("FSUB", 2, rangeSpecs[6], 4, executor, validator);
			addInstruction("FMUL", 3, rangeSpecs[6], 9, executor, validator);
			addInstruction("FDIV", 4, rangeSpecs[6], 11, executor, validator);
			addInstruction("FLOT", 5, rangeSpecs[6], 3, executor, validator);
			addInstruction("FIX", 5, rangeSpecs[7], 3, executor, validator);
			addInstruction(FloatingPointInstructions.FcmpMnemonic, 56, rangeSpecs[6], 4, executor, validator);

			executor = MiscInstructions.Move;
			addInstruction("MOVE", 7, new MetaFieldSpec(false, rangeSpecs[1]), 1, executor, validator);

			MetaFieldSpec ioMetaSpec = new MetaFieldSpec(MetaFieldSpec.Presences.Mandatory, false);
			executor = IOInstructions.JumpIfBusy;
			validator = IOInstructions.InstanceValid;
			addInstruction("JBUS", 34, ioMetaSpec, 1, executor, validator);

			executor = IOInstructions.IOControl;
			addInstruction("IOC", 35, ioMetaSpec, 1, executor, validator);

			executor = IOInstructions.Input;
			addInstruction("IN", IOInstructions.INOpCode, ioMetaSpec, 1, executor, validator);

			executor = IOInstructions.Output;
			addInstruction("OUT", 37, ioMetaSpec, 1, executor, validator);

			executor = IOInstructions.JumpIfReady;
			addInstruction("JRED", 38, ioMetaSpec, 1, executor, validator);

			executor = ShiftInstructions.ShiftA;
			addInstruction("SLA", 6, rangeSpecs[0], 2, executor, null);
			addInstruction("SRA", 6, rangeSpecs[1], 2, executor, null);

			executor = ShiftInstructions.ShiftAX;
			addInstruction("SLAX", 6, rangeSpecs[2], 2, executor, null);
			addInstruction("SRAX", 6, rangeSpecs[3], 2, executor, null);
			addInstruction("SLC", 6, rangeSpecs[4], 2, executor, null);
			addInstruction("SRC", 6, rangeSpecs[5], 2, executor, null);

			executor = ShiftInstructions.ShiftAXBinary;
			addInstruction("SLB", 6, rangeSpecs[6], 2, executor, null);
			addInstruction("SRB", 6, rangeSpecs[7], 2, executor, null);

			executor = MiscInstructions.Noop;
			addInstruction("NOP", 0, new MetaFieldSpec(false, rangeSpecs[0]), 1, executor, null);

			executor = MiscInstructions.ConvertToNumeric;
			addInstruction("NUM", 5, rangeSpecs[0], 10, executor, null);

			executor = MiscInstructions.ConvertToChar;
			addInstruction("CHAR", 5, rangeSpecs[1], 10, executor, null);

			executor = MiscInstructions.Halt;
			addInstruction("HLT", 5, rangeSpecs[2], 10, executor, null);

			executor = MiscInstructions.ForceInterrupt;
			addInstruction("INT", 5, new FieldSpec(9), 2, executor, null);
		}

        private void addInstruction(string mnemonic, byte opcode, MetaFieldSpec metaFieldSpec, int tickCount, MixInstruction.Executor executor, MixInstruction.Validator validator) =>
            addInstruction(mnemonic, new MixInstruction(opcode, mnemonic, metaFieldSpec, tickCount, executor, validator));

        private void addInstruction(string mnemonic, byte opcode, FieldSpec fieldSpec, int tickCount, MixInstruction.Executor executor, MixInstruction.Validator validator) =>
            addInstruction(mnemonic, new MixInstruction(opcode, fieldSpec, mnemonic, tickCount, executor, validator));

        private void addInstruction(string mnemonic, MixInstruction instruction)
		{
			mMnemonicInstructionMap.Add(mnemonic, instruction);
			List<MixInstruction> list = mOpcodeInstructionMap.GetOrCreate(instruction.Opcode);
			if (instruction.FieldSpec == null)
			{
				list.Insert(0, instruction);
			}
			else
			{
				list.Add(instruction);
			}
		}

		public static InstructionSet Instance
		{
			get
			{
				if (mInstance == null)
				{
					mInstance = new InstructionSet();
				}

				return mInstance;
			}
		}

		public MixInstruction GetInstruction(byte opcode, FieldSpec fieldSpec)
		{
			List<MixInstruction> instructions;

			if (!mOpcodeInstructionMap.TryGetValue(opcode, out instructions))
			{
				return null;
			}

			int startIndex = 0;
			MixInstruction defaultInstruction = null;

			if (instructions[0].FieldSpec == null)
			{
				defaultInstruction = instructions[0];
				startIndex = 1;
			}

			for (int index = startIndex; index < instructions.Count; index++)
			{
				if (instructions[index].FieldSpec == fieldSpec)
				{
					return instructions[index];
				}
			}

			return defaultInstruction;
		}

		public MixInstruction this[string mnemonic]
		{
			get
			{
				if (!mMnemonicInstructionMap.ContainsKey(mnemonic))
				{
					return null;
				}
				return mMnemonicInstructionMap[mnemonic];
			}
		}

		public MixInstruction[] this[byte opcode]
		{
			get
			{
				List<MixInstruction> list = new List<MixInstruction>();

				foreach (MixInstruction instruction in mMnemonicInstructionMap.Values)
				{
					if (instruction.Opcode == opcode)
					{
						list.Add(instruction);

						if (instruction.FieldSpec == null)
						{
							break;
						}
					}
				}

				return list.ToArray();
			}
		}
	}
}
