namespace MixLib.Device.Step
{
	public class CloseStreamStep : StreamStep
	{
		const string statusDescription = "Ending data transfer";

		public override string StatusDescription => statusDescription;

		public override StreamStep.Instance CreateStreamInstance(StreamStatus streamStatus) => new Instance(streamStatus);

		new class Instance : StreamStep.Instance
		{
			public Instance(StreamStatus streamStatus) : base(streamStatus) { }

			public override bool Tick()
			{
				StreamStatus.CloseStream();
				return true;
			}
		}
	}
}
