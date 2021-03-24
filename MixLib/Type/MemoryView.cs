using System;

namespace MixLib.Type
{
	public class MemoryView : IMemory
	{
		private int _minIndex;
		private int _maxIndex;
		private int _indexOffset;

		public IMemory SourceMemory { get; private set; }

		public MemoryView(IMemory sourceMemory)
			: this(sourceMemory, sourceMemory.MinWordIndex, sourceMemory.MaxWordIndex, 0) { }

		public MemoryView(IMemory sourceMemory, int indexOffset)
			: this(sourceMemory, sourceMemory.MinWordIndex, sourceMemory.MaxWordIndex, indexOffset) { }

		public MemoryView(IMemory sourceMemory, int minIndex, int maxIndex, int indexOffset)
		{
			SourceMemory = sourceMemory;

			ValidateParameters(minIndex, maxIndex, indexOffset);

			_minIndex = minIndex;
			_maxIndex = maxIndex;
			_indexOffset = indexOffset;
		}

		public int WordCount 
			=> _maxIndex - _minIndex + 1;

		public long MaxProfilingTickCount 
			=> SourceMemory.MaxProfilingTickCount;

		public long MaxProfilingExecutionCount 
			=> SourceMemory.MaxProfilingExecutionCount;

		public MemoryFullWord GetRealWord(int index) 
			=> SourceMemory.GetRealWord(index + _indexOffset);

		public void ResetRealWord(int index) 
			=> SourceMemory.ResetRealWord(index + _indexOffset);

		public void ClearRealWordSourceLine(int index) 
			=> SourceMemory.ClearRealWordSourceLine(index + _indexOffset);

		private void ValidateParameters(int minIndex, int maxIndex, int offset)
		{
			if (minIndex + offset < SourceMemory.MinWordIndex)
				throw new IndexOutOfRangeException("MinWordIndex + IndexOffset must be greater than or equal to the MinWordIndex of SourceMemory");

			if (maxIndex + offset > SourceMemory.MaxWordIndex)
				throw new IndexOutOfRangeException("MaxWordIndex + IndexOffset must be less than or equal to the MaxWordIndex of SourceMemory");
		}

		public void ClearSourceLines()
		{
			for (int index = _minIndex; index <= _maxIndex; index++)
				ClearRealWordSourceLine(index);
		}

		public void Reset()
		{
			for (int index = _minIndex; index <= _maxIndex; index++)
				ResetRealWord(index);
		}

		public SearchResult FindMatch(SearchParameters options)
		{
			if (options.SearchFromWordIndex < _minIndex || options.SearchFromWordIndex > _maxIndex)
				return null;

			var filteredOptions = new SearchParameters
			{
				SearchText = options.SearchText,
				SearchFields = options.SearchFields,
				MatchWholeWord = options.MatchWholeWord,
				WrapSearch = options.WrapSearch,
				IncludeOriginalSource = options.IncludeOriginalSource,
				SearchFromWordIndex = options.SearchFromWordIndex + _indexOffset,
				SearchFromField = options.SearchFromField,
				SearchFromFieldIndex = options.SearchFromFieldIndex
			};

			var result = SourceMemory.FindMatch(filteredOptions);

			if (result == null)
				return null;

			result.WordIndex -= _indexOffset;

			if (result.WordIndex >= _minIndex && result.WordIndex <= _maxIndex)
				return result;

			if (!options.WrapSearch || options.SearchFromWordIndex == _minIndex)
				return null;

			filteredOptions.SearchFromWordIndex = _minIndex + _indexOffset;
			filteredOptions.WrapSearch = false;

			result = SourceMemory.FindMatch(filteredOptions);

			return result != null && (result.WordIndex -= _indexOffset) < options.SearchFromWordIndex ? result : null;
		}

		public IMemoryFullWord this[int index]
		{
			get => SourceMemory[index + _indexOffset];
			set => SourceMemory[index + _indexOffset] = value;
		}

		public int MinWordIndex
		{
			get => _minIndex;
			set
			{
				ValidateParameters(value, _maxIndex, _indexOffset);

				_minIndex = value;
			}
		}

		public int MaxWordIndex
		{
			get => _maxIndex;
			set
			{
				ValidateParameters(_minIndex, value, _indexOffset);

				_maxIndex = value;
			}
		}

		public int IndexOffset
		{
			get => _indexOffset;
			set
			{
				ValidateParameters(_minIndex, _maxIndex, value);

				_indexOffset = value;
			}
		}
	}
}
