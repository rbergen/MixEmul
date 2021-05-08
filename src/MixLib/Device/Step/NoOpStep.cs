namespace MixLib.Device.Step
{
	public class NoOpStep : TickingStep
	{
		private readonly string statusDescription;

		public NoOpStep(int tickCount, string statusDescription) : base(tickCount) 
			=> this.statusDescription = statusDescription;

		public override string StatusDescription 
			=> this.statusDescription;

		protected override TickingStep.Instance CreateTickingInstance() 
			=> new Instance(TickCount);

		protected new class Instance : TickingStep.Instance
		{
			public Instance(int tickCount) : base(tickCount) { }

			protected override void ProcessTick() { }
		}
	}
}
