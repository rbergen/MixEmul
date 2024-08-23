namespace MixLib.Device.Step
{
	public abstract class StreamStep : DeviceStep
	{
		protected StreamStep() { }

		public override DeviceStep.Instance CreateInstance() 
			=> CreateStreamInstance(null);

		public abstract Instance CreateStreamInstance(StreamStatus streamStatus);

		public new abstract class Instance(StreamStatus streamStatus) : DeviceStep.Instance
		{
			protected StreamStatus StreamStatus { get; private set; } = streamStatus;
		}
	}
}
