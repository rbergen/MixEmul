using MixLib.Modules;
using MixLib.Type;

namespace MixLib.Instruction
{
	/// <summary>
	/// Methods for performing MIX shift instructions
	/// </summary>
	public static class ShiftInstructions
	{
        const byte slaField = 0;
        const byte sraField = 1;
        const byte slaxField = 2;
        const byte sraxField = 3;
        const byte slcField = 4;
        const byte srcField = 5;
        const byte slbField = 6;
        const byte srbField = 7;

        /// <summary>
        /// Method for performing SLA and SRA instructions
        /// </summary>
        public static bool ShiftA(ModuleBase module, MixInstruction.Instance instance)
		{
			int indexedAddress = module.Registers.GetIndexedAddress(instance.AddressValue, instance.Index);
			if (indexedAddress < 0)
			{
				module.ReportRuntimeError("Indexed value must be nonnegative: " + indexedAddress);
				return false;
			}

			Register rA = module.Registers.rA;
			int shiftCount = indexedAddress % rA.ByteCount;
			if (shiftCount != 0)
			{
				switch (instance.MixInstruction.FieldSpec.MixByteValue.ByteValue)
				{
					case slaField:
						shiftWordLeft(rA, shiftCount);

						for (int i = rA.ByteCount - shiftCount; i < rA.ByteCount; i++)
						{
							rA[i] = 0;
						}

						break;

					case sraField:
						shiftWordRight(rA, shiftCount);

						for (int j = shiftCount - 1; j >= 0; j--)
						{
							rA[j] = 0;
						}

						break;
				}
			}
			return true;
		}

		/// <summary>
		/// Method for performing SLAX, SRAX, SLC and SRC instructions
		/// </summary>
		public static bool ShiftAX(ModuleBase module, MixInstruction.Instance instance)
		{
			int indexedAddress = module.Registers.GetIndexedAddress(instance.AddressValue, instance.Index);
			if (indexedAddress < 0)
			{
				module.ReportRuntimeError("Indexed value must be nonnegative: " + indexedAddress);
				return false;
			}

			Register rA = module.Registers.rA;
			Register rX = module.Registers.rX;

			int shiftCount = indexedAddress % (FullWordRegister.RegisterByteCount * 2);
			if (shiftCount == 0)
			{
				return true;
			}

			switch (instance.MixInstruction.FieldSpec.MixByteValue.ByteValue)
			{
				case slaxField:
					if (shiftCount < FullWordRegister.RegisterByteCount)
					{
						shiftRegistersLeft((FullWordRegister)rA, (FullWordRegister)rX, shiftCount);
					}
					else
					{

						shiftCount -= FullWordRegister.RegisterByteCount;

						for (int i = shiftCount; i < FullWordRegister.RegisterByteCount; i++)
						{
							rA[i - shiftCount] = rX[i];
						}

						for (int i = FullWordRegister.RegisterByteCount - shiftCount; i < FullWordRegister.RegisterByteCount; i++)
						{
							rA[i] = 0;
						}

						shiftCount = FullWordRegister.RegisterByteCount;
					}

					for (int i = FullWordRegister.RegisterByteCount - shiftCount; i < FullWordRegister.RegisterByteCount; i++)
					{
						rX[i] = 0;
					}

					break;

				case sraxField:
					if (shiftCount < FullWordRegister.RegisterByteCount)
					{
						shiftRegistersRight((FullWordRegister)rA, (FullWordRegister)rX, shiftCount);
					}
					else
					{
						shiftCount -= FullWordRegister.RegisterByteCount;
						for (int i = (FullWordRegister.RegisterByteCount - shiftCount) - 1; i >= 0; i--)
						{
							rX[i + shiftCount] = rA[i];
						}

						for (int i = shiftCount - 1; i >= 0; i--)
						{
							rX[i] = 0;
						}

						shiftCount = FullWordRegister.RegisterByteCount;
					}

					for (int i = shiftCount - 1; i >= 0; i--)
					{
						rA[i] = 0;
					}

					break;

				case slcField:
					if (shiftCount < FullWordRegister.RegisterByteCount)
					{
						MixByte[] shiftedBytes = new MixByte[shiftCount];
						for (int i = 0; i < shiftCount; i++)
						{
							shiftedBytes[i] = rA[i];
						}

						shiftRegistersLeft((FullWordRegister)rA, (FullWordRegister)rX, shiftCount);

						for (int i = 0; i < shiftCount; i++)
						{
							rX[(rX.ByteCount - shiftCount) + i] = shiftedBytes[i];
						}
					}
					else
					{
						shiftCount -= FullWordRegister.RegisterByteCount;
						MixByte[] numArray = new MixByte[FullWordRegister.RegisterByteCount - shiftCount];

						for (int i = shiftCount; i < FullWordRegister.RegisterByteCount; i++)
						{
							numArray[i - shiftCount] = rA[i];
						}

						for (int i = 1; i <= shiftCount; i++)
						{
							rA[FullWordRegister.RegisterByteCount - i] = rA[shiftCount - i];
						}

						for (int i = shiftCount; i < FullWordRegister.RegisterByteCount; i++)
						{
							rA[i - shiftCount] = rX[i];
						}

						for (int i = 1; i <= shiftCount; i++)
						{
							rX[FullWordRegister.RegisterByteCount - i] = rX[shiftCount - i];
						}

						for (int i = 0; i < numArray.Length; i++)
						{
							rX[i] = numArray[i];
						}
					}

					return true;

				case srcField:
					if (shiftCount < FullWordRegister.RegisterByteCount)
					{
						MixByte[] shiftBytes = new MixByte[shiftCount];

						for (int i = 0; i < shiftCount; i++)
						{
							shiftBytes[i] = rX[(rX.ByteCount - shiftCount) + i];
						}

						shiftRegistersRight((FullWordRegister)rA, (FullWordRegister)rX, shiftCount);

						for (int i = 0; i < shiftCount; i++)
						{
							rA[i] = shiftBytes[i];
						}
					}
					else
					{
						shiftCount -= FullWordRegister.RegisterByteCount;
						MixByte[] shiftBytes = new MixByte[FullWordRegister.RegisterByteCount - shiftCount];

						for (int i = 0; i < shiftBytes.Length; i++)
						{
							shiftBytes[i] = rX[i];
						}

						for (int i = 0; i < shiftCount; i++)
						{
							rX[i] = rX[(FullWordRegister.RegisterByteCount - shiftCount) + i];
						}

						for (int i = shiftCount; i < FullWordRegister.RegisterByteCount; i++)
						{
							rX[i] = rA[i - shiftCount];
						}

						for (int i = 0; i < shiftCount; i++)
						{
							rA[i] = rA[(FullWordRegister.RegisterByteCount - shiftCount) + i];
						}

						for (int i = shiftCount; i < FullWordRegister.RegisterByteCount; i++)
						{
							rA[i] = shiftBytes[i - shiftCount];
						}
					}
					break;
			}

			return true;
		}

		/// <summary>
		/// Method for performing SLB and SRB instructions
		/// </summary>
		public static bool ShiftAXBinary(ModuleBase module, MixInstruction.Instance instance)
		{
			int indexedAddress = module.Registers.GetIndexedAddress(instance.AddressValue, instance.Index);
			if (indexedAddress < 0)
			{
				module.ReportRuntimeError("Indexed value must be nonnegative: " + indexedAddress);
				return false;
			}

			Register rA = module.Registers.rA;
			Register rX = module.Registers.rX;

			int registerBitCount = FullWordRegister.RegisterByteCount * MixByte.BitCount;
			int shiftCount = indexedAddress % (registerBitCount * 2);
			if (shiftCount == 0)
			{
				return true;
			}

			long rAlongValue = rA.MagnitudeLongValue;
			long rXlongValue = rX.MagnitudeLongValue;

			switch (instance.MixInstruction.FieldSpec.MixByteValue.ByteValue)
			{
				case slbField:
					rA.MagnitudeLongValue = ((rAlongValue << shiftCount) & rA.MaxMagnitude) | (rXlongValue >> (rX.BitCount - shiftCount));
					rX.MagnitudeLongValue = (rXlongValue << shiftCount) & rX.MaxMagnitude;

					break;

				case srbField:

					rA.MagnitudeLongValue = rAlongValue >> shiftCount;
					rX.MagnitudeLongValue = (rXlongValue >> shiftCount) | (rAlongValue << (rX.BitCount - shiftCount));

					break;
			}

			return true;
		}

        static void shiftRegistersLeft(FullWordRegister left, FullWordRegister right, int shiftCount)
        {
            shiftWordLeft(left, shiftCount);

            for (int i = 0; i < shiftCount; i++)
            {
                left[(FullWordRegister.RegisterByteCount - shiftCount) + i] = right[i];
            }

            shiftWordLeft(right, shiftCount);
        }

        static void shiftRegistersRight(FullWordRegister left, FullWordRegister right, int shiftCount)
        {
            shiftWordRight(right, shiftCount);

            for (int i = shiftCount - 1; i >= 0; i--)
            {
                right[i] = left[(FullWordRegister.RegisterByteCount - shiftCount) + i];
            }

            shiftWordRight(left, shiftCount);
        }

        static void shiftWordLeft(Word word, int shiftCount)
        {
            for (int i = shiftCount; i < word.ByteCount; i++)
            {
                word[i - shiftCount] = word[i];
            }
        }

        static void shiftWordRight(Word word, int shiftCount)
        {
            for (int i = (word.ByteCount - shiftCount) - 1; i >= 0; i--)
            {
                word[i + shiftCount] = word[i];
            }
        }
    }
}
