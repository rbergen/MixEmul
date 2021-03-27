using System.Collections;

namespace MixLib.Misc
{
	public interface IBreakpointManager
	{
		bool IsBreakpointSet(int address);

		ICollection Breakpoints { get; }
	}
}
