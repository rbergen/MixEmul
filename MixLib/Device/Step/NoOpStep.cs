namespace MixLib.Device.Step
{
	public class NoOpStep : TickingStep
	{
		readonly string mStatusDescription;

		public NoOpStep(int tickCount, string statusDescription) : base(tickCount)
		{
			mStatusDescription = statusDescription;
		}

		public override string StatusDescription => mStatusDescription;

		protected override TickingStep.Instance CreateTickingInstance()
		{
			return new Instance(TickCount);
		}

		protected new class Instance : TickingStep.Instance
		{
			public Instance(int tickCount) : base(tickCount) { }

			protected override void ProcessTick() { }
		}
	}
}
