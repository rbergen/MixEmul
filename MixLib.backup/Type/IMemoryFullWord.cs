
namespace MixLib.Type
{
	public interface IMemoryFullWord : IFullWord
	{
		int Index { get; }

		string SourceLine { get; set; }

		long ProfilingTickCount { get; }

		long ProfilingExecutionCount { get; }

		void IncreaseProfilingTickCount(int ticks);

		void IncreaseProfilingExecutionCount();
	}
}
