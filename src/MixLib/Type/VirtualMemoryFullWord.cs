using System;
using System.Collections;
using System.Collections.Generic;
using MixLib.Instruction;
using MixLib.Utils;

namespace MixLib.Type
{
	public class VirtualMemoryFullWord(IMemory memory, int index) : IMemoryFullWord
	{
		private static readonly string defaultCharString;
		private static readonly string defaultNonCharString;
		private static readonly string defaultInstructionString;
		private const long DefaultLongValue = 0;
		private static readonly FullWord defaultWord;

		private MemoryFullWord realWord;
		private readonly IMemory memory = memory;
		private readonly object syncRoot = new();

		public int Index => index;

		static VirtualMemoryFullWord()
		{
			defaultWord = new FullWord(DefaultLongValue);
			defaultCharString = defaultWord.ToString(true);
			defaultNonCharString = defaultWord.ToString(false);

			var instruction = InstructionSet.Instance.GetInstruction(defaultWord[MixInstruction.OpcodeByte], new FieldSpec(defaultWord[MixInstruction.FieldSpecByte]));
			if (instruction != null)
			{
				var instance = instruction.CreateInstance(defaultWord);

				if (instance != null)
					defaultInstructionString = new InstructionText(instance).InstanceText;
			}
		}

		public int BitCount
			=> defaultWord.BitCount;

		public int ByteCount
			=> FullWord.ByteCount;

		public int Count
			=> ByteCount;

		private FullWord ActiveWord
			=> RealWordFetched ? this.realWord : defaultWord;

		public long MaxMagnitude
			=> defaultWord.MaxMagnitude;

		public long ProfilingTickCount
			=> RealWordFetched ? this.realWord.ProfilingTickCount : 0;

		public long ProfilingExecutionCount
			=> RealWordFetched ? this.realWord.ProfilingExecutionCount : 0;

		IEnumerator IEnumerable.GetEnumerator()
			=> ((IEnumerable)ActiveWord).GetEnumerator();

		IEnumerator<MixByte> IEnumerable<MixByte>.GetEnumerator()
			=> ((IEnumerable<MixByte>)ActiveWord).GetEnumerator();

		public int MaxByteCount
			=> ActiveWord.MaxByteCount;

		public bool IsEmpty
			=> !RealWordFetched || this.realWord.IsEmpty;

		public MixByte[] ToArray()
			=> [..ActiveWord];

		public MixByte[] Slice(int startIndex, int count)
			=> ActiveWord.Slice(startIndex, count);

		public object Clone()
			=> ActiveWord.Clone();

		public string ToString(bool asChars)
			=> RealWordFetched ? this.realWord.ToString(asChars) : (asChars ? defaultCharString : defaultNonCharString);

		private void FetchRealWordIfNotFetched()
		{
			lock (this.syncRoot)
			{
				if (this.realWord == null)
					FetchRealWord();
			}
		}

		private bool RealWordFetched
		{
			get
			{
				lock (this.syncRoot)
				{
					return this.realWord != null;
				}
			}
		}

		private void FetchRealWord()
			=> this.realWord = this.memory.GetRealWord(Index);

		private bool FetchRealWordConditionally(Func<bool> condition)
		{
			lock (this.syncRoot)
			{
				if (this.realWord == null)
				{
					if (!condition())
						return false;

					FetchRealWord();
				}
			}

			return true;
		}

		public string SourceLine
		{
			get => RealWordFetched ? this.realWord.SourceLine : null;
			set
			{
				if (FetchRealWordConditionally(() => value != null))
					this.realWord.SourceLine = value;
			}
		}

		public void Load(string text)
		{
			if (FetchRealWordConditionally(() => text != defaultCharString))
				this.realWord.Load(text);
		}

		public void InvertSign()
		{
			FetchRealWordIfNotFetched();
			this.realWord.InvertSign();
		}

		public MixByte this[int index]
		{
			get => ActiveWord[index];
			set
			{
				if (FetchRealWordConditionally(() => defaultWord[index].ByteValue != value.ByteValue))
					this.realWord[index] = value;
			}
		}

		public long LongValue
		{
			get => ActiveWord.LongValue;
			set
			{
				if (FetchRealWordConditionally(() => value != DefaultLongValue))
					this.realWord.LongValue = value;
			}
		}

		public MixByte[] Magnitude
		{
			get => ActiveWord.Magnitude;
			set
			{
				if (FetchRealWordConditionally(VerifyDifferentMixBytes))
					this.realWord.Magnitude = value;

				bool VerifyDifferentMixBytes()
				{
					bool valueDifferent = false;
					int thisStartIndex = 0;
					int valueStartIndex = 0;

					if (value.Length < FullWord.ByteCount)
					{
						thisStartIndex = FullWord.ByteCount - value.Length;
						for (int i = 0; i < thisStartIndex; i++)
						{
							if (defaultWord[i].ByteValue != 0)
							{
								valueDifferent = true;
								break;
							}
						}
					}
					else if (value.Length > FullWord.ByteCount)
						valueStartIndex = value.Length - FullWord.ByteCount;

					if (!valueDifferent)
					{
						while (thisStartIndex < FullWord.ByteCount)
						{
							if (defaultWord[thisStartIndex].ByteValue != value[valueStartIndex].ByteValue)
							{
								valueDifferent = true;
								break;
							}

							thisStartIndex++;
							valueStartIndex++;
						}
					}

					return valueDifferent;
				}
			}
		}

		public long MagnitudeLongValue
		{
			get => ActiveWord.MagnitudeLongValue;
			set
			{
				if (FetchRealWordConditionally(() => value != DefaultLongValue))
					this.realWord.MagnitudeLongValue = value;
			}
		}

		public Word.Signs Sign
		{
			get => ActiveWord.Sign;
			set
			{
				if (FetchRealWordConditionally(() => value != defaultWord.Sign))
					this.realWord.Sign = value;
			}
		}

		public static SearchResult FindDefaultMatch(SearchParameters options, bool isStartIndex)
		{
			int index;

			if ((!isStartIndex || options.SearchFromField <= FieldTypes.Value) && (options.SearchFields & FieldTypes.Value) == FieldTypes.Value)
			{
				index = DefaultLongValue.ToString().FindMatch(options, isStartIndex && options.SearchFromField == FieldTypes.Value ? options.SearchFromFieldIndex : 0);

				if (index >= 0)
					return new SearchResult { Field = FieldTypes.Value, FieldIndex = index };
			}

			if ((!isStartIndex || options.SearchFromField <= FieldTypes.Chars) && (options.SearchFields & FieldTypes.Chars) == FieldTypes.Chars)
			{
				index = defaultCharString.FindMatch(options, isStartIndex && options.SearchFromField == FieldTypes.Chars ? options.SearchFromFieldIndex : 0);

				if (index >= 0)
					return new SearchResult { Field = FieldTypes.Chars, FieldIndex = index };
			}

			if ((!isStartIndex || options.SearchFromField <= FieldTypes.Instruction) && (options.SearchFields & FieldTypes.Instruction) == FieldTypes.Instruction)
			{
				index = defaultInstructionString.FindMatch(options, isStartIndex && options.SearchFromField == FieldTypes.Instruction ? options.SearchFromFieldIndex : 0);

				if (index >= 0)
					return new SearchResult { Field = FieldTypes.Instruction, FieldIndex = index };
			}

			return null;
		}

		public override bool Equals(object obj)
			=> obj is VirtualMemoryFullWord other && other.Index == Index && other.memory == this.memory && other.realWord == this.realWord;

		public override int GetHashCode()
		{
			var hashcode = Index.GetHashCode();

			if (this.memory != null)
				hashcode ^= this.memory.GetHashCode();

			if (this.realWord != null)
				hashcode ^= this.realWord.GetHashCode();

			return hashcode;
		}

		public void IncreaseProfilingTickCount(int ticks)
		{
			FetchRealWordIfNotFetched();

			this.realWord.IncreaseProfilingTickCount(ticks);
		}

		public void IncreaseProfilingExecutionCount()
		{
			FetchRealWordIfNotFetched();

			this.realWord.IncreaseProfilingExecutionCount();
		}
	}
}
