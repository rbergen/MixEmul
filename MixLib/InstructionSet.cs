using MixLib.Instruction;
using MixLib.Type;
using MixLib.Utils;
using System.Collections.Generic;

namespace MixLib
{
	public class InstructionSet
	{
		private readonly SortedDictionary<string, MixInstruction> mMnemonicInstructionMap = new();
		private readonly SortedDictionary<byte, List<MixInstruction>> mOpcodeInstructionMap = new();
		private static InstructionSet mInstance;

		public InstructionSet()
		{
			var fullWordMetaSpec = new MetaFieldSpec(true, FullWordRegister.DefaultFieldSpec);
			var indexMetaSpec = new MetaFieldSpec(true, IndexRegister.DefaultFieldSpec);

			FieldSpec[] rangeSpecs = new FieldSpec[8];
			for (int i = 0; i < rangeSpecs.Length; i++)
			{
				rangeSpecs[i] = new FieldSpec(i);
			}

			MixInstruction.Executor executor = LoadInstructions.Load;
			MixInstruction.Validator validator = InstructionHelpers.ValidateIndexAndFieldSpec;
			AddInstruction("LDA", 8, fullWordMetaSpec, 2, executor, validator);
			AddInstruction("LD1", 9, indexMetaSpec, 2, executor, validator);
			AddInstruction("LD2", 10, indexMetaSpec, 2, executor, validator);
			AddInstruction("LD3", 11, indexMetaSpec, 2, executor, validator);
			AddInstruction("LD4", 12, indexMetaSpec, 2, executor, validator);
			AddInstruction("LD5", 13, indexMetaSpec, 2, executor, validator);
			AddInstruction("LD6", 14, indexMetaSpec, 2, executor, validator);
			AddInstruction("LDX", 15, fullWordMetaSpec, 2, executor, validator);

			executor = LoadInstructions.LoadNegative;
			AddInstruction("LDAN", 16, fullWordMetaSpec, 2, executor, validator);
			AddInstruction("LD1N", 17, indexMetaSpec, 2, executor, validator);
			AddInstruction("LD2N", 18, indexMetaSpec, 2, executor, validator);
			AddInstruction("LD3N", 19, indexMetaSpec, 2, executor, validator);
			AddInstruction("LD4N", 20, indexMetaSpec, 2, executor, validator);
			AddInstruction("LD5N", 21, indexMetaSpec, 2, executor, validator);
			AddInstruction("LD6N", 22, indexMetaSpec, 2, executor, validator);
			AddInstruction("LDXN", 23, fullWordMetaSpec, 2, executor, validator);

			executor = StoreInstructions.StoreRegister;
			AddInstruction("STA", 24, fullWordMetaSpec, 2, executor, validator);
			AddInstruction("ST1", 25, indexMetaSpec, 2, executor, validator);
			AddInstruction("ST2", 26, indexMetaSpec, 2, executor, validator);
			AddInstruction("ST3", 27, indexMetaSpec, 2, executor, validator);
			AddInstruction("ST4", 28, indexMetaSpec, 2, executor, validator);
			AddInstruction("ST5", 29, indexMetaSpec, 2, executor, validator);
			AddInstruction("ST6", 30, indexMetaSpec, 2, executor, validator);
			AddInstruction("STX", 31, fullWordMetaSpec, 2, executor, validator);
			AddInstruction("STJ", 32, new MetaFieldSpec(true, AddressRegister.DefaultFieldSpec), 2, executor, validator);

			executor = StoreInstructions.StoreZero;
			AddInstruction("STZ", 33, new MetaFieldSpec(true, Memory.DefaultFieldSpec), 2, executor, validator);

			executor = ArithmaticInstructions.AddSubstract;
			AddInstruction("ADD", 1, fullWordMetaSpec, 2, executor, validator);
			AddInstruction("SUB", 2, fullWordMetaSpec, 2, executor, validator);

			executor = ArithmaticInstructions.Multiply;
			AddInstruction("MUL", 3, fullWordMetaSpec, 10, executor, validator);

			executor = ArithmaticInstructions.Divide;
			AddInstruction("DIV", 4, fullWordMetaSpec, 12, executor, validator);

			executor = ComparisonInstructions.Compare;
			AddInstruction("CMPA", 56, fullWordMetaSpec, 2, executor, validator);
			AddInstruction("CMP1", 57, indexMetaSpec, 2, executor, validator);
			AddInstruction("CMP2", 58, indexMetaSpec, 2, executor, validator);
			AddInstruction("CMP3", 59, indexMetaSpec, 2, executor, validator);
			AddInstruction("CMP4", 60, indexMetaSpec, 2, executor, validator);
			AddInstruction("CMP5", 61, indexMetaSpec, 2, executor, validator);
			AddInstruction("CMP6", 62, indexMetaSpec, 2, executor, validator);
			AddInstruction("CMPX", 63, fullWordMetaSpec, 2, executor, validator);

			executor = AddressTransferInstructions.Increase;
			validator = InstructionHelpers.ValidateIndex;
			AddInstruction("INCA", 48, rangeSpecs[0], 1, executor, validator);
			AddInstruction("INC1", 49, rangeSpecs[0], 1, executor, validator);
			AddInstruction("INC2", 50, rangeSpecs[0], 1, executor, validator);
			AddInstruction("INC3", 51, rangeSpecs[0], 1, executor, validator);
			AddInstruction("INC4", 52, rangeSpecs[0], 1, executor, validator);
			AddInstruction("INC5", 53, rangeSpecs[0], 1, executor, validator);
			AddInstruction("INC6", 54, rangeSpecs[0], 1, executor, validator);
			AddInstruction("INCX", 55, rangeSpecs[0], 1, executor, validator);

			executor = AddressTransferInstructions.Decrease;
			AddInstruction("DECA", 48, rangeSpecs[1], 1, executor, validator);
			AddInstruction("DEC1", 49, rangeSpecs[1], 1, executor, validator);
			AddInstruction("DEC2", 50, rangeSpecs[1], 1, executor, validator);
			AddInstruction("DEC3", 51, rangeSpecs[1], 1, executor, validator);
			AddInstruction("DEC4", 52, rangeSpecs[1], 1, executor, validator);
			AddInstruction("DEC5", 53, rangeSpecs[1], 1, executor, validator);
			AddInstruction("DEC6", 54, rangeSpecs[1], 1, executor, validator);
			AddInstruction("DECX", 55, rangeSpecs[1], 1, executor, validator);

			executor = AddressTransferInstructions.Enter;
			AddInstruction("ENTA", 48, rangeSpecs[2], 1, executor, validator);
			AddInstruction("ENT1", 49, rangeSpecs[2], 1, executor, validator);
			AddInstruction("ENT2", 50, rangeSpecs[2], 1, executor, validator);
			AddInstruction("ENT3", 51, rangeSpecs[2], 1, executor, validator);
			AddInstruction("ENT4", 52, rangeSpecs[2], 1, executor, validator);
			AddInstruction("ENT5", 53, rangeSpecs[2], 1, executor, validator);
			AddInstruction("ENT6", 54, rangeSpecs[2], 1, executor, validator);
			AddInstruction("ENTX", 55, rangeSpecs[2], 1, executor, validator);

			executor = AddressTransferInstructions.EnterNegative;
			AddInstruction("ENNA", 48, rangeSpecs[3], 1, executor, validator);
			AddInstruction("ENN1", 49, rangeSpecs[3], 1, executor, validator);
			AddInstruction("ENN2", 50, rangeSpecs[3], 1, executor, validator);
			AddInstruction("ENN3", 51, rangeSpecs[3], 1, executor, validator);
			AddInstruction("ENN4", 52, rangeSpecs[3], 1, executor, validator);
			AddInstruction("ENN5", 53, rangeSpecs[3], 1, executor, validator);
			AddInstruction("ENN6", 54, rangeSpecs[3], 1, executor, validator);
			AddInstruction("ENNX", 55, rangeSpecs[3], 1, executor, validator);

			executor = JumpInstructions.NonRegJump;
			AddInstruction("JMP", 39, rangeSpecs[0], 1, executor, validator);
			AddInstruction("JSJ", 39, rangeSpecs[1], 1, executor, validator);
			AddInstruction("JOV", 39, rangeSpecs[2], 1, executor, validator);
			AddInstruction("JNOV", 39, rangeSpecs[3], 1, executor, validator);
			AddInstruction("JL", 39, rangeSpecs[4], 1, executor, validator);
			AddInstruction("JE", 39, rangeSpecs[5], 1, executor, validator);
			AddInstruction("JG", 39, rangeSpecs[6], 1, executor, validator);
			AddInstruction("JGE", 39, rangeSpecs[7], 1, executor, validator);
			AddInstruction("JNE", 39, new FieldSpec(8), 1, executor, validator);
			AddInstruction("JLE", 39, new FieldSpec(9), 1, executor, validator);

			executor = JumpInstructions.RegJump;
			AddInstruction("JAN", 40, rangeSpecs[0], 1, executor, validator);
			AddInstruction("JAZ", 40, rangeSpecs[1], 1, executor, validator);
			AddInstruction("JAP", 40, rangeSpecs[2], 1, executor, validator);
			AddInstruction("JANN", 40, rangeSpecs[3], 1, executor, validator);
			AddInstruction("JANZ", 40, rangeSpecs[4], 1, executor, validator);
			AddInstruction("JANP", 40, rangeSpecs[5], 1, executor, validator);
			AddInstruction("JAE", 40, rangeSpecs[6], 1, executor, validator);
			AddInstruction("JAO", 40, rangeSpecs[7], 1, executor, validator);

			AddInstruction("J1N", 41, rangeSpecs[0], 1, executor, validator);
			AddInstruction("J1Z", 41, rangeSpecs[1], 1, executor, validator);
			AddInstruction("J1P", 41, rangeSpecs[2], 1, executor, validator);
			AddInstruction("J1NN", 41, rangeSpecs[3], 1, executor, validator);
			AddInstruction("J1NZ", 41, rangeSpecs[4], 1, executor, validator);
			AddInstruction("J1NP", 41, rangeSpecs[5], 1, executor, validator);

			AddInstruction("J2N", 42, rangeSpecs[0], 1, executor, validator);
			AddInstruction("J2Z", 42, rangeSpecs[1], 1, executor, validator);
			AddInstruction("J2P", 42, rangeSpecs[2], 1, executor, validator);
			AddInstruction("J2NN", 42, rangeSpecs[3], 1, executor, validator);
			AddInstruction("J2NZ", 42, rangeSpecs[4], 1, executor, validator);
			AddInstruction("J2NP", 42, rangeSpecs[5], 1, executor, validator);

			AddInstruction("J3N", 43, rangeSpecs[0], 1, executor, validator);
			AddInstruction("J3Z", 43, rangeSpecs[1], 1, executor, validator);
			AddInstruction("J3P", 43, rangeSpecs[2], 1, executor, validator);
			AddInstruction("J3NN", 43, rangeSpecs[3], 1, executor, validator);
			AddInstruction("J3NZ", 43, rangeSpecs[4], 1, executor, validator);
			AddInstruction("J3NP", 43, rangeSpecs[5], 1, executor, validator);

			AddInstruction("J4N", 44, rangeSpecs[0], 1, executor, validator);
			AddInstruction("J4Z", 44, rangeSpecs[1], 1, executor, validator);
			AddInstruction("J4P", 44, rangeSpecs[2], 1, executor, validator);
			AddInstruction("J4NN", 44, rangeSpecs[3], 1, executor, validator);
			AddInstruction("J4NZ", 44, rangeSpecs[4], 1, executor, validator);
			AddInstruction("J4NP", 44, rangeSpecs[5], 1, executor, validator);

			AddInstruction("J5N", 45, rangeSpecs[0], 1, executor, validator);
			AddInstruction("J5Z", 45, rangeSpecs[1], 1, executor, validator);
			AddInstruction("J5P", 45, rangeSpecs[2], 1, executor, validator);
			AddInstruction("J5NN", 45, rangeSpecs[3], 1, executor, validator);
			AddInstruction("J5NZ", 45, rangeSpecs[4], 1, executor, validator);
			AddInstruction("J5NP", 45, rangeSpecs[5], 1, executor, validator);

			AddInstruction("J6N", 46, rangeSpecs[0], 1, executor, validator);
			AddInstruction("J6Z", 46, rangeSpecs[1], 1, executor, validator);
			AddInstruction("J6P", 46, rangeSpecs[2], 1, executor, validator);
			AddInstruction("J6NN", 46, rangeSpecs[3], 1, executor, validator);
			AddInstruction("J6NZ", 46, rangeSpecs[4], 1, executor, validator);
			AddInstruction("J6NP", 46, rangeSpecs[5], 1, executor, validator);

			AddInstruction("JXN", 47, rangeSpecs[0], 1, executor, validator);
			AddInstruction("JXZ", 47, rangeSpecs[1], 1, executor, validator);
			AddInstruction("JXP", 47, rangeSpecs[2], 1, executor, validator);
			AddInstruction("JXNN", 47, rangeSpecs[3], 1, executor, validator);
			AddInstruction("JXNZ", 47, rangeSpecs[4], 1, executor, validator);
			AddInstruction("JXNP", 47, rangeSpecs[5], 1, executor, validator);
			AddInstruction("JXE", 47, rangeSpecs[6], 1, executor, validator);
			AddInstruction("JXO", 47, rangeSpecs[7], 1, executor, validator);

			executor = FloatingPointInstructions.DoFloatingPoint;
			AddInstruction("FADD", 1, rangeSpecs[6], 4, executor, validator);
			AddInstruction("FSUB", 2, rangeSpecs[6], 4, executor, validator);
			AddInstruction("FMUL", 3, rangeSpecs[6], 9, executor, validator);
			AddInstruction("FDIV", 4, rangeSpecs[6], 11, executor, validator);
			AddInstruction("FLOT", 5, rangeSpecs[6], 3, executor, validator);
			AddInstruction("FIX", 5, rangeSpecs[7], 3, executor, validator);
			AddInstruction(FloatingPointInstructions.FcmpMnemonic, 56, rangeSpecs[6], 4, executor, validator);

			executor = MiscInstructions.Move;
			AddInstruction("MOVE", 7, new MetaFieldSpec(false, rangeSpecs[1]), 1, executor, validator);

			var ioMetaSpec = new MetaFieldSpec(MetaFieldSpec.Presences.Mandatory, false);
			executor = IOInstructions.JumpIfBusy;
			validator = IOInstructions.InstanceValid;
			AddInstruction("JBUS", 34, ioMetaSpec, 1, executor, validator);

			executor = IOInstructions.IOControl;
			AddInstruction("IOC", 35, ioMetaSpec, 1, executor, validator);

			executor = IOInstructions.Input;
			AddInstruction("IN", IOInstructions.INOpCode, ioMetaSpec, 1, executor, validator);

			executor = IOInstructions.Output;
			AddInstruction("OUT", 37, ioMetaSpec, 1, executor, validator);

			executor = IOInstructions.JumpIfReady;
			AddInstruction("JRED", 38, ioMetaSpec, 1, executor, validator);

			executor = ShiftInstructions.ShiftA;
			AddInstruction("SLA", 6, rangeSpecs[0], 2, executor, null);
			AddInstruction("SRA", 6, rangeSpecs[1], 2, executor, null);

			executor = ShiftInstructions.ShiftAX;
			AddInstruction("SLAX", 6, rangeSpecs[2], 2, executor, null);
			AddInstruction("SRAX", 6, rangeSpecs[3], 2, executor, null);
			AddInstruction("SLC", 6, rangeSpecs[4], 2, executor, null);
			AddInstruction("SRC", 6, rangeSpecs[5], 2, executor, null);

			executor = ShiftInstructions.ShiftAXBinary;
			AddInstruction("SLB", 6, rangeSpecs[6], 2, executor, null);
			AddInstruction("SRB", 6, rangeSpecs[7], 2, executor, null);

			executor = MiscInstructions.Noop;
			AddInstruction("NOP", 0, new MetaFieldSpec(false, rangeSpecs[0]), 1, executor, null);

			executor = MiscInstructions.ConvertToNumeric;
			AddInstruction("NUM", 5, rangeSpecs[0], 10, executor, null);

			executor = MiscInstructions.ConvertToChar;
			AddInstruction("CHAR", 5, rangeSpecs[1], 10, executor, null);

			executor = MiscInstructions.Halt;
			AddInstruction("HLT", 5, rangeSpecs[2], 10, executor, null);

			executor = MiscInstructions.ForceInterrupt;
			AddInstruction("INT", 5, new FieldSpec(9), 2, executor, null);
		}

		void AddInstruction(string mnemonic, byte opcode, MetaFieldSpec metaFieldSpec, int tickCount, MixInstruction.Executor executor, MixInstruction.Validator validator)
		{
			AddInstruction(mnemonic, new MixInstruction(opcode, mnemonic, metaFieldSpec, tickCount, executor, validator));
		}

		void AddInstruction(string mnemonic, byte opcode, FieldSpec fieldSpec, int tickCount, MixInstruction.Executor executor, MixInstruction.Validator validator)
		{
			AddInstruction(mnemonic, new MixInstruction(opcode, fieldSpec, mnemonic, tickCount, executor, validator));
		}

		void AddInstruction(string mnemonic, MixInstruction instruction)
		{
			mMnemonicInstructionMap.Add(mnemonic, instruction);
			var list = mOpcodeInstructionMap.GetOrCreate(instruction.Opcode);
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

			if (!mOpcodeInstructionMap.TryGetValue(opcode, out List<MixInstruction> instructions))
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

		public MixInstruction this[string mnemonic] =>
						mMnemonicInstructionMap.ContainsKey(mnemonic) ? mMnemonicInstructionMap[mnemonic] : null;

		public MixInstruction[] this[byte opcode]
		{
			get
			{
				var list = new List<MixInstruction>();

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
