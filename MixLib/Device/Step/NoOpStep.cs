namespace MixLib.Device.Step
{
	public class NoOpStep : TickingStep
	{
		private string mStatusDescription;

		public NoOpStep(int tickCount, string statusDescription)
			: base(tickCount)
		{
			mStatusDescription = statusDescription;
		}

		protected override TickingStep.Instance CreateTickingInstance()
		{
			return new Instance(base.TickCount);
		}

		public override string StatusDescription
		{
			get
			{
				return mStatusDescription;
			}
		}

		protected new class Instance : TickingStep.Instance
		{
			public Instance(int tickCount)
				: base(tickCount)
			{
			}

			protected override void ProcessTick()
			{
			}
		}
	}
}
