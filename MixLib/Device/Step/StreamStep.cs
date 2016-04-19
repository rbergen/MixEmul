using MixLib.Device;

namespace MixLib.Device.Step
{
	public abstract class StreamStep : DeviceStep
	{
		protected StreamStep()
		{
		}

		public override DeviceStep.Instance CreateInstance() => CreateStreamInstance(null);

		public abstract Instance CreateStreamInstance(StreamStatus streamStatus);

        public abstract new class Instance : DeviceStep.Instance
        {
            protected StreamStatus StreamStatus { get; set; }

			public Instance(StreamStatus streamStatus)
			{
				StreamStatus = streamStatus;
			}
		}
	}
}
