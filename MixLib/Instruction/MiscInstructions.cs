using System.Collections.Generic;
using MixLib;
using MixLib.Interrupts;
using MixLib.Modules;
using MixLib.Type;

namespace MixLib.Instruction
{
	/// <summary>
	/// Methods for performing miscellaneous MIX instructions
	/// </summary>
	public static class MiscInstructions
	{
        static SortedDictionary<string, moveStatus> mMoveStatuses = new SortedDictionary<string, moveStatus>();
        const byte zeroCharValue = 30;

        /// <summary>
        /// Method for performing the NOP instruction
        /// </summary>
        public static bool Noop(ModuleBase module, MixInstruction.Instance instance) => true;

        /// <summary>
        /// Method for performing the CHAR instruction
        /// </summary>
        public static bool ConvertToChar(ModuleBase module, MixInstruction.Instance instance)
		{
			Register rA = module.Registers.rA;
			long magnitudeLongValue = rA.MagnitudeLongValue;
			Register rX = module.Registers.rX;

			for (int i = rX.ByteCount - 1; i >= 0; i--)
			{
				rX[i] = zeroCharValue + ((byte)(magnitudeLongValue % 10L));
				magnitudeLongValue /= 10L;
			}

			for (int i = rA.ByteCount - 1; i >= 0; i--)
			{
				rA[i] = zeroCharValue + ((byte)(magnitudeLongValue % 10L));
				magnitudeLongValue /= 10L;
			}

			return true;
		}

		/// <summary>
		/// Method for performing the NUM instruction
		/// </summary>
		public static bool ConvertToNumeric(ModuleBase module, MixInstruction.Instance instance)
		{
			decimal num = 0M;
			Register rA = module.Registers.rA;

			for (int i = 0; i < rA.ByteCount; i++)
			{
				num *= 10M;
				num += rA[i] % 10;
			}

			Register rX = module.Registers.rX;

			for (int j = 0; j < rX.ByteCount; j++)
			{
				num *= 10M;
				num += rX[j] % 10;
			}

			if (num > rA.MaxMagnitude)
			{
				module.ReportOverflow();
				num = num % rA.MaxMagnitude;
			}

			rA.MagnitudeLongValue = (long)num;

			return true;
		}

		public static bool ForceInterrupt(ModuleBase module, MixInstruction.Instance instance)
		{
			if (!(module is Mix))
			{
				module.ReportRuntimeError(string.Format("The {0} instruction is only available in Mix", instance.Instruction.Mnemonic));

				return false;
			}

			InterruptHandler.HandleInterrupt((Mix)module, new Interrupt(Interrupt.Types.Forced));

			return false;
		}

		/// <summary>
		/// Method for performing the HLT instruction
		/// </summary>
		public static bool Halt(ModuleBase module, MixInstruction.Instance instance)
		{
			module.Halt(instance.AddressValue);

			return false;
		}

		/// <summary>
		/// Method for performing the MOVE instruction
		/// </summary>
		public static bool Move(ModuleBase module, MixInstruction.Instance instance)
		{
			moveStatus status;

			mMoveStatuses.TryGetValue(module.ModuleName, out status);

			// if we have a move status, check if it applies to this instruction
			if (status != null && status.ProgramCounter != module.ProgramCounter)
			{
				status = null;
				mMoveStatuses.Remove(module.ModuleName);
			}

			if (status == null)
			{
				int fromAddress = InstructionHelpers.GetValidIndexedAddress(module, instance.AddressValue, instance.Index);

				if (fromAddress == int.MinValue)
				{
					return false;
				}

				int toAddress = InstructionHelpers.GetValidIndexedAddress(module, 0, 1);

				if (toAddress == int.MinValue)
				{
					return false;
				}

				status = new moveStatus(module.ProgramCounter, fromAddress, toAddress, instance.FieldSpec.MixByteValue.ByteValue);
				mMoveStatuses[module.ModuleName] = status;

				return false;
			}

			// the MOVE instruction takes two ticks per moved word...
			switch (status.CurrentWordState)
			{
				// ... during one of those, the actual move is performed...
				case moveStatus.WordStates.BeforeMove:
				case moveStatus.WordStates.RegisterIncreased:
					{
						IMemory memory = module.Memory;
						int currentFromAddress = status.FromAddress + status.CurrentWord;
						int currentToAddress = status.ToAddress + status.CurrentWord;

						if (currentFromAddress > memory.MaxWordIndex || currentToAddress > memory.MaxWordIndex)
						{
							module.ReportRuntimeError("Source or target address overflow");
							status = null;
							mMoveStatuses.Remove(module.ModuleName);

							return false;
						}

						memory[currentToAddress] = memory[currentFromAddress];
						status.CurrentWordState = moveStatus.WordStates.Moved;

						break;
					}

				// ... during the other, the value of rI1 is increased
				case moveStatus.WordStates.Moved:
					{
						Register rI1 = module.Registers.rI1;
						rI1.LongValue++;
						status.NextWord();

						if (status.CurrentWord >= status.WordCount)
						{
							status = null;
							mMoveStatuses.Remove(module.ModuleName);
							return true;
						}

						status.CurrentWordState = moveStatus.WordStates.RegisterIncreased;

						break;
					}
			}

			// return false to prevent the PC from increasing
			return false;
		}

        /// <summary>
        /// This helper class tracks the status of a running MOVE instruction. It is necessary because a MIX MOVE instruction takes
        /// several ticks, the exact number of ticks depending on the number of words moved.
        /// </summary>
        class moveStatus
        {
            public int CurrentWord { get; private set; }
            public WordStates CurrentWordState { get; set; }
            public int FromAddress { get; private set; }
            public int ProgramCounter { get; private set; }
            public int ToAddress { get; private set; }
            public byte WordCount { get; private set; }

            public moveStatus(int programCounter, int fromAddress, int toAddress, byte wordCount)
            {
                ProgramCounter = programCounter;
                FromAddress = fromAddress;
                ToAddress = toAddress;
                WordCount = wordCount;
                CurrentWord = 0;
                CurrentWordState = WordStates.BeforeMove;
            }

            public void NextWord() => CurrentWord++;

            public enum WordStates
            {
                BeforeMove,
                Moved,
                RegisterIncreased
            }
        }
    }
}
