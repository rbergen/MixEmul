using MixLib.Device;

namespace MixLib.Device.Step
{
	public class CloseStreamStep : StreamStep
	{
		private const string statusDescription = "Ending data transfer";

        public override string StatusDescription => statusDescription;

        public override StreamStep.Instance CreateStreamInstance(StreamStatus streamStatus) => new Instance(streamStatus);

		private new class Instance : StreamStep.Instance
		{
			public Instance(StreamStatus streamStatus)
				: base(streamStatus)
			{
			}

			public override bool Tick()
			{
                StreamStatus.CloseStream();
				return true;
			}
		}
	}
}
