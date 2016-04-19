using System;
using System.Collections;
using System.Collections.Generic;
using MixLib.Instruction;
using MixLib.Utils;

namespace MixLib.Type
{
	public class VirtualMemoryFullWord : IMemoryFullWord
	{
		private static readonly string mDefaultCharString;
		private static readonly string mDefaultNonCharString;
		private static readonly string mDefaultInstructionString = null;
		private const long mDefaultLongValue = 0;
		private static readonly FullWord mDefaultWord;

		private MemoryFullWord mRealWord = null;
		private IMemory mMemory;
		private object mSyncRoot = new object();

        public int Index { get; private set; }

        static VirtualMemoryFullWord()
		{
			mDefaultWord = new FullWord(mDefaultLongValue);
			mDefaultCharString = mDefaultWord.ToString(true);
			mDefaultNonCharString = mDefaultWord.ToString(false);

			MixInstruction instruction = InstructionSet.Instance.GetInstruction(mDefaultWord[MixInstruction.OpcodeByte], new FieldSpec(mDefaultWord[MixInstruction.FieldSpecByte]));
			if (instruction != null)
			{
				MixInstruction.Instance instance = instruction.CreateInstance(mDefaultWord);
				if (instance != null)
				{
					mDefaultInstructionString = new InstructionText(instance).InstanceText;
				}
			}
		}

		public VirtualMemoryFullWord(IMemory memory, int index)
		{
			mMemory = memory;
			Index = index;
		}

        public int BitCount => mDefaultWord.BitCount;

        public int ByteCount => FullWord.ByteCount;

        private FullWord activeWord => realWordFetched ? mRealWord : mDefaultWord;

        public long MaxMagnitude => mDefaultWord.MaxMagnitude;

        public long ProfilingTickCount => realWordFetched ? mRealWord.ProfilingTickCount : 0;

        public long ProfilingExecutionCount => realWordFetched ? mRealWord.ProfilingExecutionCount : 0;

        public IEnumerator GetEnumerator() => activeWord.GetEnumerator();

        IEnumerator<MixByte> IEnumerable<MixByte>.GetEnumerator() => ((IEnumerable<MixByte>)activeWord).GetEnumerator();

        public int MaxByteCount => activeWord.MaxByteCount;

        public bool IsEmpty => realWordFetched ? mRealWord.IsEmpty : true;

        public MixByte[] ToArray() => activeWord.ToArray();

        public object Clone() => activeWord.Clone();

        public string ToString(bool asChars) =>
            realWordFetched ? mRealWord.ToString(asChars) : (asChars ? mDefaultCharString : mDefaultNonCharString);

        private void fetchRealWordIfNotFetched()
		{
			lock (mSyncRoot)
			{
				if (mRealWord == null)
				{
					fetchRealWord();
				}
			}
		}

		private bool realWordFetched
		{
			get
			{
				lock (mSyncRoot)
				{
					return mRealWord != null;
				}
			}
		}

		private void fetchRealWord()
		{
			mRealWord = mMemory.GetRealWord(Index);
		}

		private bool fetchRealWordConditionally(Func<bool> condition)
		{
			lock (mSyncRoot)
			{
				if (mRealWord == null)
				{
					if (!condition())
					{
						return false;
					}

					fetchRealWord();
				}
			}

			return true;
		}

		public string SourceLine
		{
			get
			{
				return realWordFetched ? mRealWord.SourceLine : null;
			}
			set
			{
				if (fetchRealWordConditionally(() => value != null))
				{
					mRealWord.SourceLine = value;
				}
			}
		}

		public void Load(string text)
		{
			if (fetchRealWordConditionally(() => text != mDefaultCharString))
			{
				mRealWord.Load(text);
			}
		}

		public void InvertSign()
		{
			fetchRealWordIfNotFetched();
			mRealWord.InvertSign();
		}

		public MixByte this[int index]
		{
			get
			{
				return activeWord[index];
			}
			set
			{
				if (fetchRealWordConditionally(() => mDefaultWord[index].ByteValue != value.ByteValue))
				{
					mRealWord[index] = value;
				}
			}
		}

		public long LongValue
		{
			get
			{
				return activeWord.LongValue;
			}
			set
			{
				if (fetchRealWordConditionally(() => value != mDefaultLongValue))
				{
					mRealWord.LongValue = value;
				}
			}
		}

		public MixByte[] Magnitude
		{
			get
			{
				return activeWord.Magnitude;
			}
			set
			{
				if (fetchRealWordConditionally(() =>
				{

					bool valueDifferent = false;
					int thisStartIndex = 0;
					int valueStartIndex = 0;
					if (value.Length < FullWord.ByteCount)
					{
						thisStartIndex = FullWord.ByteCount - value.Length;
						for (int i = 0; i < thisStartIndex; i++)
						{
							if (mDefaultWord[i].ByteValue != 0)
							{
								valueDifferent = true;
								break;
							}
						}
					}
					else if (value.Length > FullWord.ByteCount)
					{
						valueStartIndex = value.Length - FullWord.ByteCount;
					}

					if (!valueDifferent)
					{
						while (thisStartIndex < FullWord.ByteCount)
						{
							if (mDefaultWord[thisStartIndex].ByteValue != value[valueStartIndex].ByteValue)
							{
								valueDifferent = true;
								break;
							}

							thisStartIndex++;
							valueStartIndex++;
						}
					}

					return valueDifferent;

				}))
				{
					mRealWord.Magnitude = value;
				}
			}
		}

		public long MagnitudeLongValue
		{
			get
			{
				return activeWord.MagnitudeLongValue;
			}
			set
			{
				if (fetchRealWordConditionally(() => value != mDefaultLongValue))
				{
					mRealWord.MagnitudeLongValue = value;
				}
			}
		}

        public Word.Signs Sign
		{
			get
			{
				return activeWord.Sign;
			}
			set
			{
				if (fetchRealWordConditionally(() => value != mDefaultWord.Sign))
				{
					mRealWord.Sign = value;
				}
			}
		}

		public static SearchResult FindDefaultMatch(SearchParameters options, bool isStartIndex)
		{
			int index;

			if ((!isStartIndex || options.SearchFromField <= FieldTypes.Value) && (options.SearchFields & FieldTypes.Value) == FieldTypes.Value)
			{
				index = mDefaultLongValue.ToString().FindMatch(options, isStartIndex && options.SearchFromField == FieldTypes.Value ? options.SearchFromFieldIndex : 0);

				if (index >= 0)
				{
					return new SearchResult() { Field = FieldTypes.Value, FieldIndex = index };
				}
			}

			if ((!isStartIndex || options.SearchFromField <= FieldTypes.Chars) && (options.SearchFields & FieldTypes.Chars) == FieldTypes.Chars)
			{
				index = mDefaultCharString.FindMatch(options, isStartIndex && options.SearchFromField == FieldTypes.Chars ? options.SearchFromFieldIndex : 0);

				if (index >= 0)
				{
					return new SearchResult() { Field = FieldTypes.Chars, FieldIndex = index };
				}
			}

			if ((!isStartIndex || options.SearchFromField <= FieldTypes.Instruction) && (options.SearchFields & FieldTypes.Instruction) == FieldTypes.Instruction)
			{
				index = mDefaultInstructionString.FindMatch(options, isStartIndex && options.SearchFromField == FieldTypes.Instruction ? options.SearchFromFieldIndex : 0);

				if (index >= 0)
				{
					return new SearchResult() { Field = FieldTypes.Instruction, FieldIndex = index };
				}
			}

			return null;
		}

		public override bool Equals(object obj)
		{
			VirtualMemoryFullWord other = obj as VirtualMemoryFullWord;

			return other == null ? false : other.Index == Index && other.mMemory == mMemory && other.mRealWord == mRealWord;
		}

        public override int GetHashCode()
        {
            int hashcode = Index.GetHashCode();

            if (mMemory != null)
            {
                hashcode ^= mMemory.GetHashCode();
            }

            if (mRealWord != null)
            {
                hashcode ^= mRealWord.GetHashCode();
            }

            return hashcode;
        }

		public void IncreaseProfilingTickCount(int ticks)
		{
			fetchRealWordIfNotFetched();

			mRealWord.IncreaseProfilingTickCount(ticks);
		}

		public void IncreaseProfilingExecutionCount()
		{
			fetchRealWordIfNotFetched();

			mRealWord.IncreaseProfilingExecutionCount();
		}
	}
}
