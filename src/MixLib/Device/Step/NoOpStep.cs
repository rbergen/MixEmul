namespace MixLib.Device.Step
{
	public class NoOpStep : TickingStep
	{
		private readonly string _statusDescription;

		public NoOpStep(int tickCount, string statusDescription) : base(tickCount) 
			=> _statusDescription = statusDescription;

		public override string StatusDescription 
			=> _statusDescription;

		protected override TickingStep.Instance CreateTickingInstance() 
			=> new Instance(TickCount);

		protected new class Instance : TickingStep.Instance
		{
			public Instance(int tickCount) : base(tickCount) { }

			protected override void ProcessTick() { }
		}
	}
}
