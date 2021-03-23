namespace MixLib.Device.Step
{
	public abstract class StreamStep : DeviceStep
	{
		protected StreamStep() { }

		public override DeviceStep.Instance CreateInstance()
		{
			return CreateStreamInstance(null);
		}

		public abstract Instance CreateStreamInstance(StreamStatus streamStatus);

		public abstract new class Instance : DeviceStep.Instance
		{
			protected StreamStatus StreamStatus { get; private set; }

			protected Instance(StreamStatus streamStatus)
			{
				StreamStatus = streamStatus;
			}
		}
	}
}
