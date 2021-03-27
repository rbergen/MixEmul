using System;
using System.Collections;
using System.Collections.Generic;
using MixLib.Instruction;
using MixLib.Utils;

namespace MixLib.Type
{
	public class VirtualMemoryFullWord : IMemoryFullWord
	{
		private static readonly string _defaultCharString;
		private static readonly string _defaultNonCharString;
		private static readonly string _defaultInstructionString;
		private const long DefaultLongValue = 0;
		private static readonly FullWord _defaultWord;

		private MemoryFullWord _realWord;
		private readonly IMemory _memory;
		private readonly object _syncRoot = new();

		public int Index { get; private set; }

		static VirtualMemoryFullWord()
		{
			_defaultWord = new FullWord(DefaultLongValue);
			_defaultCharString = _defaultWord.ToString(true);
			_defaultNonCharString = _defaultWord.ToString(false);

			var instruction = InstructionSet.Instance.GetInstruction(_defaultWord[MixInstruction.OpcodeByte], new FieldSpec(_defaultWord[MixInstruction.FieldSpecByte]));
			if (instruction != null)
			{
				var instance = instruction.CreateInstance(_defaultWord);

				if (instance != null)
					_defaultInstructionString = new InstructionText(instance).InstanceText;
			}
		}

		public VirtualMemoryFullWord(IMemory memory, int index)
		{
			_memory = memory;
			Index = index;
		}

		public int BitCount
			=> _defaultWord.BitCount;

		public int ByteCount
			=> FullWord.ByteCount;

		private FullWord ActiveWord
			=> RealWordFetched ? _realWord : _defaultWord;

		public long MaxMagnitude
			=> _defaultWord.MaxMagnitude;

		public long ProfilingTickCount
			=> RealWordFetched ? _realWord.ProfilingTickCount : 0;

		public long ProfilingExecutionCount
			=> RealWordFetched ? _realWord.ProfilingExecutionCount : 0;

		public IEnumerator GetEnumerator()
			=> ActiveWord.GetEnumerator();

		IEnumerator<MixByte> IEnumerable<MixByte>.GetEnumerator()
			=> ((IEnumerable<MixByte>)ActiveWord).GetEnumerator();

		public int MaxByteCount
			=> ActiveWord.MaxByteCount;

		public bool IsEmpty
			=> !RealWordFetched || _realWord.IsEmpty;

		public MixByte[] ToArray()
			=> ActiveWord.ToArray();

		public object Clone()
			=> ActiveWord.Clone();

		public string ToString(bool asChars)
			=> RealWordFetched ? _realWord.ToString(asChars) : (asChars ? _defaultCharString : _defaultNonCharString);

		private void FetchRealWordIfNotFetched()
		{
			lock (_syncRoot)
			{
				if (_realWord == null)
					FetchRealWord();
			}
		}

		private bool RealWordFetched
		{
			get
			{
				lock (_syncRoot)
				{
					return _realWord != null;
				}
			}
		}

		private void FetchRealWord()
			=> _realWord = _memory.GetRealWord(Index);

		private bool FetchRealWordConditionally(Func<bool> condition)
		{
			lock (_syncRoot)
			{
				if (_realWord == null)
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
			get => RealWordFetched ? _realWord.SourceLine : null;
			set
			{
				if (FetchRealWordConditionally(() => value != null))
					_realWord.SourceLine = value;
			}
		}

		public void Load(string text)
		{
			if (FetchRealWordConditionally(() => text != _defaultCharString))
				_realWord.Load(text);
		}

		public void InvertSign()
		{
			FetchRealWordIfNotFetched();
			_realWord.InvertSign();
		}

		public MixByte this[int index]
		{
			get => ActiveWord[index];
			set
			{
				if (FetchRealWordConditionally(() => _defaultWord[index].ByteValue != value.ByteValue))
					_realWord[index] = value;
			}
		}

		public long LongValue
		{
			get => ActiveWord.LongValue;
			set
			{
				if (FetchRealWordConditionally(() => value != DefaultLongValue))
					_realWord.LongValue = value;
			}
		}

		public MixByte[] Magnitude
		{
			get => ActiveWord.Magnitude;
			set
			{
				if (FetchRealWordConditionally(VerifyDifferentMixBytes))
					_realWord.Magnitude = value;

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
							if (_defaultWord[i].ByteValue != 0)
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
							if (_defaultWord[thisStartIndex].ByteValue != value[valueStartIndex].ByteValue)
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
					_realWord.MagnitudeLongValue = value;
			}
		}

		public Word.Signs Sign
		{
			get => ActiveWord.Sign;
			set
			{
				if (FetchRealWordConditionally(() => value != _defaultWord.Sign))
					_realWord.Sign = value;
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
				index = _defaultCharString.FindMatch(options, isStartIndex && options.SearchFromField == FieldTypes.Chars ? options.SearchFromFieldIndex : 0);

				if (index >= 0)
					return new SearchResult { Field = FieldTypes.Chars, FieldIndex = index };
			}

			if ((!isStartIndex || options.SearchFromField <= FieldTypes.Instruction) && (options.SearchFields & FieldTypes.Instruction) == FieldTypes.Instruction)
			{
				index = _defaultInstructionString.FindMatch(options, isStartIndex && options.SearchFromField == FieldTypes.Instruction ? options.SearchFromFieldIndex : 0);

				if (index >= 0)
					return new SearchResult { Field = FieldTypes.Instruction, FieldIndex = index };
			}

			return null;
		}

		public override bool Equals(object obj)
			=> obj is VirtualMemoryFullWord other && other.Index == Index && other._memory == _memory && other._realWord == _realWord;

		public override int GetHashCode()
		{
			var hashcode = Index.GetHashCode();

			if (_memory != null)
				hashcode ^= _memory.GetHashCode();

			if (_realWord != null)
				hashcode ^= _realWord.GetHashCode();

			return hashcode;
		}

		public void IncreaseProfilingTickCount(int ticks)
		{
			FetchRealWordIfNotFetched();

			_realWord.IncreaseProfilingTickCount(ticks);
		}

		public void IncreaseProfilingExecutionCount()
		{
			FetchRealWordIfNotFetched();

			_realWord.IncreaseProfilingExecutionCount();
		}
	}
}
