using MixLib.Interrupts;
using MixLib.Type;

namespace MixLib.Device
{
	public class InOutputOperands
	{
        public IMemory Memory { get; private set; }
        public int MValue { get; private set; }
        public int Sector { get; private set; }
        public InterruptQueueCallback InterruptQueueCallback { get; private set; }

        public InOutputOperands(IMemory memory, int memAddress, int sector) : this(memory, memAddress, sector, null) { }

		public InOutputOperands(IMemory memory, int memAddress, int sector, InterruptQueueCallback callback)
		{
			Memory = memory;
			MValue = memAddress;
			Sector = sector;
			InterruptQueueCallback = callback;
		}
    }
}
