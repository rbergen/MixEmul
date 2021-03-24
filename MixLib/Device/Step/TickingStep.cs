namespace MixLib.Device.Step
{
	public abstract class TickingStep : DeviceStep
	{
		protected int TickCount { get; private set; }

		protected TickingStep(int tickCount) 
			=> TickCount = tickCount;

		public sealed override DeviceStep.Instance CreateInstance() => CreateTickingInstance();

		protected abstract Instance CreateTickingInstance();

		protected new abstract class Instance : DeviceStep.Instance
		{
			protected int CurrentTick { get; private set; }
			protected int TickCount { get; private set; }

			protected Instance(int tickCount)
			{
				TickCount = tickCount;
				CurrentTick = 0;
			}

			protected abstract void ProcessTick();

			public sealed override bool Tick()
			{
				ProcessTick();
				CurrentTick++;

				return CurrentTick >= TickCount;
			}
		}
	}
}
