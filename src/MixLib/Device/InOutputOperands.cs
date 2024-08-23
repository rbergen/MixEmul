using MixLib.Interrupts;
using MixLib.Type;

namespace MixLib.Device
{
	public class InOutputOperands(IMemory memory, int memAddress, int sector, InterruptQueueCallback callback)
  {
		public IMemory Memory => memory;
		public int MValue => memAddress;
		public int Sector => sector;
		public InterruptQueueCallback InterruptQueueCallback => callback;

		public InOutputOperands(IMemory memory, int memAddress, int sector) : this(memory, memAddress, sector, null) { }
  }
}
