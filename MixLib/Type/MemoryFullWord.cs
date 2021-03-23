using MixLib.Instruction;
using MixLib.Utils;

namespace MixLib.Type
{
	public class MemoryFullWord : FullWord, IMemoryFullWord
	{
		public int Index { get; private set; }
		public string SourceLine { get; set; }
		public long ProfilingTickCount { get; private set; }
		public long ProfilingExecutionCount { get; private set; }

		public MemoryFullWord(int index)
		{
			Index = index;
		}

		public MemoryFullWord(int index, long value) : base(value)
		{
			Index = index;
		}

		public override bool IsEmpty => base.IsEmpty && SourceLine == null;

		public void IncreaseProfilingTickCount(int ticks)
		{
			ProfilingTickCount += ticks;
		}

		public void IncreaseProfilingExecutionCount()
		{
			ProfilingExecutionCount++;
		}

		public SearchResult FindMatch(SearchParameters options)
		{
			int index;

			bool isStartIndex = Index == options.SearchFromWordIndex;

			if ((!isStartIndex || options.SearchFromField <= FieldTypes.Value) && (options.SearchFields & FieldTypes.Value) == FieldTypes.Value)
			{
				index = LongValue.ToString().FindMatch(options, isStartIndex && options.SearchFromField == FieldTypes.Value ? options.SearchFromFieldIndex : 0);

				if (index >= 0)
				{
					return new SearchResult { Field = FieldTypes.Value, FieldIndex = index };
				}
			}

			if ((!isStartIndex || options.SearchFromField <= FieldTypes.Chars) && (options.SearchFields & FieldTypes.Chars) == FieldTypes.Chars)
			{
				index = ToString(true).FindMatch(options, isStartIndex && options.SearchFromField == FieldTypes.Chars ? options.SearchFromFieldIndex : 0);

				if (index >= 0)
				{
					return new SearchResult { Field = FieldTypes.Chars, FieldIndex = index };
				}
			}

			if ((!isStartIndex || options.SearchFromField <= FieldTypes.Instruction) && (options.SearchFields & FieldTypes.Instruction) == FieldTypes.Instruction)
			{
				var instruction = InstructionSet.Instance.GetInstruction(this[MixInstruction.OpcodeByte], new FieldSpec(this[MixInstruction.FieldSpecByte]));
				if (instruction == null)
				{
					return null;
				}

				var instance = instruction.CreateInstance(this);
				if (instance == null)
				{
					return null;
				}

				index = new InstructionText(instance).InstanceText.FindMatch(options, isStartIndex && options.SearchFromField == FieldTypes.Instruction ? options.SearchFromFieldIndex : 0);

				if (index >= 0)
				{
					return new SearchResult { Field = FieldTypes.Instruction, FieldIndex = index };
				}
			}

			return null;
		}

		public void ResetProfilingCounts()
		{
			ProfilingTickCount = 0;
			ProfilingExecutionCount = 0;
		}
	}
}
