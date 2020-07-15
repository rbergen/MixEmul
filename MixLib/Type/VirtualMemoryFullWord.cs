using MixLib.Instruction;
using MixLib.Utils;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MixLib.Type
{
	public class VirtualMemoryFullWord : IMemoryFullWord
	{
		static readonly string mDefaultCharString;
		static readonly string mDefaultNonCharString;
		static readonly string mDefaultInstructionString;
		const long mDefaultLongValue = 0;
		static readonly FullWord mDefaultWord;

		MemoryFullWord mRealWord;
		IMemory mMemory;
		object mSyncRoot = new object();

		public int Index { get; private set; }

		static VirtualMemoryFullWord()
		{
			mDefaultWord = new FullWord(mDefaultLongValue);
			mDefaultCharString = mDefaultWord.ToString(true);
			mDefaultNonCharString = mDefaultWord.ToString(false);

			var instruction = InstructionSet.Instance.GetInstruction(mDefaultWord[MixInstruction.OpcodeByte], new FieldSpec(mDefaultWord[MixInstruction.FieldSpecByte]));
			if (instruction != null)
			{
				var instance = instruction.CreateInstance(mDefaultWord);
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

		FullWord ActiveWord => RealWordFetched ? mRealWord : mDefaultWord;

		public long MaxMagnitude => mDefaultWord.MaxMagnitude;

		public long ProfilingTickCount => RealWordFetched ? mRealWord.ProfilingTickCount : 0;

		public long ProfilingExecutionCount => RealWordFetched ? mRealWord.ProfilingExecutionCount : 0;

		public IEnumerator GetEnumerator() => ActiveWord.GetEnumerator();

		IEnumerator<MixByte> IEnumerable<MixByte>.GetEnumerator() => ((IEnumerable<MixByte>)ActiveWord).GetEnumerator();

		public int MaxByteCount => ActiveWord.MaxByteCount;

		public bool IsEmpty => !RealWordFetched || mRealWord.IsEmpty;

		public MixByte[] ToArray() => ActiveWord.ToArray();

		public object Clone() => ActiveWord.Clone();

		public string ToString(bool asChars) =>
				RealWordFetched ? mRealWord.ToString(asChars) : (asChars ? mDefaultCharString : mDefaultNonCharString);

		void FetchRealWordIfNotFetched()
		{
			lock (mSyncRoot)
			{
				if (mRealWord == null) FetchRealWord();
			}
		}

		bool RealWordFetched
		{
			get
			{
				lock (mSyncRoot)
				{
					return mRealWord != null;
				}
			}
		}

		void FetchRealWord()
		{
			mRealWord = mMemory.GetRealWord(Index);
		}

		bool FetchRealWordConditionally(Func<bool> condition)
		{
			lock (mSyncRoot)
			{
				if (mRealWord == null)
				{
					if (!condition()) return false;

					FetchRealWord();
				}
			}

			return true;
		}

		public string SourceLine
		{
			get
			{
				return RealWordFetched ? mRealWord.SourceLine : null;
			}
			set
			{
				if (FetchRealWordConditionally(() => value != null))
				{
					mRealWord.SourceLine = value;
				}
			}
		}

		public void Load(string text)
		{
			if (FetchRealWordConditionally(() => text != mDefaultCharString)) mRealWord.Load(text);
		}

		public void InvertSign()
		{
			FetchRealWordIfNotFetched();
			mRealWord.InvertSign();
		}

		public MixByte this[int index]
		{
			get
			{
				return ActiveWord[index];
			}
			set
			{
				if (FetchRealWordConditionally(() => mDefaultWord[index].ByteValue != value.ByteValue))
				{
					mRealWord[index] = value;
				}
			}
		}

		public long LongValue
		{
			get
			{
				return ActiveWord.LongValue;
			}
			set
			{
				if (FetchRealWordConditionally(() => value != mDefaultLongValue))
				{
					mRealWord.LongValue = value;
				}
			}
		}

		public MixByte[] Magnitude
		{
			get
			{
				return ActiveWord.Magnitude;
			}
			set
			{
				if (FetchRealWordConditionally(() =>
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
				return ActiveWord.MagnitudeLongValue;
			}
			set
			{
				if (FetchRealWordConditionally(() => value != mDefaultLongValue))
				{
					mRealWord.MagnitudeLongValue = value;
				}
			}
		}

		public Word.Signs Sign
		{
			get
			{
				return ActiveWord.Sign;
			}
			set
			{
				if (FetchRealWordConditionally(() => value != mDefaultWord.Sign))
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

				if (index >= 0) return new SearchResult { Field = FieldTypes.Value, FieldIndex = index };
			}

			if ((!isStartIndex || options.SearchFromField <= FieldTypes.Chars) && (options.SearchFields & FieldTypes.Chars) == FieldTypes.Chars)
			{
				index = mDefaultCharString.FindMatch(options, isStartIndex && options.SearchFromField == FieldTypes.Chars ? options.SearchFromFieldIndex : 0);

				if (index >= 0) return new SearchResult { Field = FieldTypes.Chars, FieldIndex = index };
			}

			if ((!isStartIndex || options.SearchFromField <= FieldTypes.Instruction) && (options.SearchFields & FieldTypes.Instruction) == FieldTypes.Instruction)
			{
				index = mDefaultInstructionString.FindMatch(options, isStartIndex && options.SearchFromField == FieldTypes.Instruction ? options.SearchFromFieldIndex : 0);

				if (index >= 0) return new SearchResult { Field = FieldTypes.Instruction, FieldIndex = index };
			}

			return null;
		}

		public override bool Equals(object obj)
		{
			var other = obj as VirtualMemoryFullWord;

			return other != null && other.Index == Index && other.mMemory == mMemory && other.mRealWord == mRealWord;
		}

		public override int GetHashCode()
		{
			var hashcode = Index.GetHashCode();

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
			FetchRealWordIfNotFetched();

			mRealWord.IncreaseProfilingTickCount(ticks);
		}

		public void IncreaseProfilingExecutionCount()
		{
			FetchRealWordIfNotFetched();

			mRealWord.IncreaseProfilingExecutionCount();
		}
	}
}
