using System;

namespace MixLib.Type
{
	public class MemoryView : IMemory
	{
		private int minIndex;
		private int maxIndex;
		private int indexOffset;

		public IMemory SourceMemory { get; private set; }

		public MemoryView(IMemory sourceMemory)
			: this(sourceMemory, sourceMemory.MinWordIndex, sourceMemory.MaxWordIndex, 0) { }

		public MemoryView(IMemory sourceMemory, int indexOffset)
			: this(sourceMemory, sourceMemory.MinWordIndex, sourceMemory.MaxWordIndex, indexOffset) { }

		public MemoryView(IMemory sourceMemory, int minIndex, int maxIndex, int indexOffset)
		{
			SourceMemory = sourceMemory;

			ValidateParameters(minIndex, maxIndex, indexOffset);

			this.minIndex = minIndex;
			this.maxIndex = maxIndex;
			this.indexOffset = indexOffset;
		}

		public int WordCount 
			=> this.maxIndex - this.minIndex + 1;

		public long MaxProfilingTickCount 
			=> SourceMemory.MaxProfilingTickCount;

		public long MaxProfilingExecutionCount 
			=> SourceMemory.MaxProfilingExecutionCount;

		public MemoryFullWord GetRealWord(int index) 
			=> SourceMemory.GetRealWord(index + this.indexOffset);

		public void ResetRealWord(int index) 
			=> SourceMemory.ResetRealWord(index + this.indexOffset);

		public void ClearRealWordSourceLine(int index) 
			=> SourceMemory.ClearRealWordSourceLine(index + this.indexOffset);

		private void ValidateParameters(int minIndex, int maxIndex, int offset)
		{
			if (minIndex + offset < SourceMemory.MinWordIndex)
				throw new IndexOutOfRangeException("MinWordIndex + IndexOffset must be greater than or equal to the MinWordIndex of SourceMemory");

			if (maxIndex + offset > SourceMemory.MaxWordIndex)
				throw new IndexOutOfRangeException("MaxWordIndex + IndexOffset must be less than or equal to the MaxWordIndex of SourceMemory");
		}

		public void ClearSourceLines()
		{
			for (int index = this.minIndex; index <= this.maxIndex; index++)
				ClearRealWordSourceLine(index);
		}

		public void Reset()
		{
			for (int index = this.minIndex; index <= this.maxIndex; index++)
				ResetRealWord(index);
		}

		public SearchResult FindMatch(SearchParameters options)
		{
			if (options.SearchFromWordIndex < this.minIndex || options.SearchFromWordIndex > this.maxIndex)
				return null;

			var filteredOptions = new SearchParameters
			{
				SearchText = options.SearchText,
				SearchFields = options.SearchFields,
				MatchWholeWord = options.MatchWholeWord,
				WrapSearch = options.WrapSearch,
				IncludeOriginalSource = options.IncludeOriginalSource,
				SearchFromWordIndex = options.SearchFromWordIndex + this.indexOffset,
				SearchFromField = options.SearchFromField,
				SearchFromFieldIndex = options.SearchFromFieldIndex
			};

			var result = SourceMemory.FindMatch(filteredOptions);

			if (result == null)
				return null;

			result.WordIndex -= this.indexOffset;

			if (result.WordIndex >= this.minIndex && result.WordIndex <= this.maxIndex)
				return result;

			if (!options.WrapSearch || options.SearchFromWordIndex == this.minIndex)
				return null;

			filteredOptions.SearchFromWordIndex = this.minIndex + this.indexOffset;
			filteredOptions.WrapSearch = false;

			result = SourceMemory.FindMatch(filteredOptions);

			return result != null && (result.WordIndex -= this.indexOffset) < options.SearchFromWordIndex ? result : null;
		}

		public IMemoryFullWord this[int index]
		{
			get => SourceMemory[index + this.indexOffset];
			set => SourceMemory[index + this.indexOffset] = value;
		}

		public int MinWordIndex
		{
			get => this.minIndex;
			set
			{
				ValidateParameters(value, this.maxIndex, this.indexOffset);

				this.minIndex = value;
			}
		}

		public int MaxWordIndex
		{
			get => this.maxIndex;
			set
			{
				ValidateParameters(this.minIndex, value, this.indexOffset);

				this.maxIndex = value;
			}
		}

		public int IndexOffset
		{
			get => this.indexOffset;
			set
			{
				ValidateParameters(this.minIndex, this.maxIndex, value);

				this.indexOffset = value;
			}
		}
	}
}
