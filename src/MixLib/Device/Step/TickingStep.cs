namespace MixLib.Device.Step
{
	public abstract class TickingStep(int tickCount) : DeviceStep
	{
	protected int TickCount => tickCount;

	public sealed override DeviceStep.Instance CreateInstance() => CreateTickingInstance();

		protected abstract Instance CreateTickingInstance();

		protected new abstract class Instance(int tickCount) : DeviceStep.Instance
		{
		  protected int CurrentTick { get; private set; } = 0;
		  protected int TickCount => tickCount;

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
