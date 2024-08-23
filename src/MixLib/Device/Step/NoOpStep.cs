namespace MixLib.Device.Step
{
	public class NoOpStep(int tickCount, string statusDescription) : TickingStep(tickCount)
	{
		public override string StatusDescription 
			=> statusDescription;

		protected override TickingStep.Instance CreateTickingInstance() 
			=> new Instance(TickCount);

		protected new class Instance(int tickCount) : TickingStep.Instance(tickCount)
		{
			protected override void ProcessTick() { }
		}
	}
}
