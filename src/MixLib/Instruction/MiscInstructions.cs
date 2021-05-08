using System.Collections.Generic;
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
		private static readonly SortedDictionary<string, MoveStatus> moveStatuses = new();

		private const byte ZeroCharValue = 30;


		/// <summary>
		/// Method for performing the NOP instruction
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
		public static bool Noop(ModuleBase module, MixInstruction.Instance instance)
			=> true;


		/// <summary>
		/// Method for performing the CHAR instruction
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
		public static bool ConvertToChar(ModuleBase module, MixInstruction.Instance instance)
		{
			Register rA = module.Registers.RA;
			long magnitudeLongValue = rA.MagnitudeLongValue;
			Register rX = module.Registers.RX;

			for (int i = rX.ByteCount - 1; i >= 0; i--)
			{
				rX[i] = ZeroCharValue + ((byte)(magnitudeLongValue % 10L));
				magnitudeLongValue /= 10L;
			}

			for (int i = rA.ByteCount - 1; i >= 0; i--)
			{
				rA[i] = ZeroCharValue + ((byte)(magnitudeLongValue % 10L));
				magnitudeLongValue /= 10L;
			}

			return true;
		}


		/// <summary>
		/// Method for performing the NUM instruction
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
		public static bool ConvertToNumeric(ModuleBase module, MixInstruction.Instance instance)
		{
			decimal num = 0M;
			Register rA = module.Registers.RA;

			for (int i = 0; i < rA.ByteCount; i++)
			{
				num *= 10M;
				num += rA[i] % 10;
			}

			Register rX = module.Registers.RX;

			for (int j = 0; j < rX.ByteCount; j++)
			{
				num *= 10M;
				num += rX[j] % 10;
			}

			if (num > rA.MaxMagnitude)
			{
				module.ReportOverflow();
				num %= rA.MaxMagnitude;
			}

			rA.MagnitudeLongValue = (long)num;

			return true;
		}

		public static bool ForceInterrupt(ModuleBase module, MixInstruction.Instance instance)
		{
			if (!(module is Mix))
			{
				module.ReportRuntimeError($"The {instance.Instruction.Mnemonic} instruction is only available in Mix");
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

			moveStatuses.TryGetValue(module.ModuleName, out MoveStatus status);

			// if we have a move status, check if it applies to this instruction
			if (status != null && status.ProgramCounter != module.ProgramCounter)
			{
				status = null;
				moveStatuses.Remove(module.ModuleName);
			}

			if (status == null)
			{
				var fromAddress = InstructionHelpers.GetValidIndexedAddress(module, instance.AddressValue, instance.Index);
				if (fromAddress == int.MinValue)
					return false;

				var toAddress = InstructionHelpers.GetValidIndexedAddress(module, 0, 1);
				if (toAddress == int.MinValue)
					return false;

				status = new MoveStatus(module.ProgramCounter, fromAddress, toAddress, instance.FieldSpec.MixByteValue.ByteValue);
				moveStatuses[module.ModuleName] = status;

				return false;
			}

			// the MOVE instruction takes two ticks per moved word...
			switch (status.CurrentWordState)
			{
				// ... during one of those, the actual move is performed...
				case MoveStatus.WordStates.BeforeMove:
				case MoveStatus.WordStates.RegisterIncreased:
					IMemory memory = module.Memory;
					int currentFromAddress = status.FromAddress + status.CurrentWord;
					int currentToAddress = status.ToAddress + status.CurrentWord;

					if (currentFromAddress > memory.MaxWordIndex || currentToAddress > memory.MaxWordIndex)
					{
						module.ReportRuntimeError("Source or target address overflow");
						moveStatuses.Remove(module.ModuleName);

						return false;
					}

					memory[currentToAddress] = memory[currentFromAddress];
					status.CurrentWordState = MoveStatus.WordStates.Moved;

					break;

				// ... during the other, the value of rI1 is increased
				case MoveStatus.WordStates.Moved:
					Register rI1 = module.Registers.RI1;
					rI1.LongValue++;
					status.NextWord();

					if (status.CurrentWord >= status.WordCount)
					{
						moveStatuses.Remove(module.ModuleName);
						return true;
					}

					status.CurrentWordState = MoveStatus.WordStates.RegisterIncreased;

					break;
			}

			// return false to prevent the PC from increasing
			return false;
		}

		/// <summary>
		/// This helper class tracks the status of a running MOVE instruction. It is necessary because a MIX MOVE instruction takes
		/// several ticks, the exact number of ticks depending on the number of words moved.
		/// </summary>
		private class MoveStatus
		{
			public int CurrentWord { get; private set; }
			public WordStates CurrentWordState { get; set; }
			public int FromAddress { get; private set; }
			public int ProgramCounter { get; private set; }
			public int ToAddress { get; private set; }
			public byte WordCount { get; private set; }

			public MoveStatus(int programCounter, int fromAddress, int toAddress, byte wordCount)
			{
				ProgramCounter = programCounter;
				FromAddress = fromAddress;
				ToAddress = toAddress;
				WordCount = wordCount;
				CurrentWord = 0;
				CurrentWordState = WordStates.BeforeMove;
			}

			public void NextWord() 
				=> CurrentWord++;

			public enum WordStates
			{
				BeforeMove,
				Moved,
				RegisterIncreased
			}
		}
	}
}
