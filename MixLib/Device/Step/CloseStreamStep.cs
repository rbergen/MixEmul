using MixLib.Device;

namespace MixLib.Device.Step
{
	public class CloseStreamStep : StreamStep
	{
		private const string statusDescription = "Ending data transfer";

		public override StreamStep.Instance CreateStreamInstance(StreamStatus streamStatus)
		{
			return new Instance(streamStatus);
		}

		public override string StatusDescription
		{
			get
			{
				return statusDescription;
			}
		}

		private new class Instance : StreamStep.Instance
		{
			public Instance(StreamStatus streamStatus)
				: base(streamStatus)
			{
			}

			public override bool Tick()
			{
				base.StreamStatus.CloseStream();
				return true;
			}
		}
	}
}
