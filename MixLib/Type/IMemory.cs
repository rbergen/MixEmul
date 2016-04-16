
namespace MixLib.Type
{
	public interface IMemory
	{
		void ClearSourceLines();

		void Reset();

		SearchResult FindMatch(SearchParameters options);

		IMemoryFullWord this[int index]
		{
			get;
			set;
		}

		MemoryFullWord GetRealWord(int index);

		void ResetRealWord(int index);

		void ClearRealWordSourceLine(int index);

		int WordCount
		{
			get;
		}

		int MinWordIndex
		{
			get;
			set;
		}

		int MaxWordIndex
		{
			get;
			set;
		}

		long MaxProfilingTickCount
		{
			get;
		}

		long MaxProfilingExecutionCount
		{
			get;
		}
	}
}
