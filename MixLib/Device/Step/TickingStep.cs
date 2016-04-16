namespace MixLib.Device.Step
{
	public abstract class TickingStep : DeviceStep
	{
		protected int TickCount { get; private set; }

		public TickingStep(int tickCount)
		{
			TickCount = tickCount;
		}

		public sealed override DeviceStep.Instance CreateInstance()
		{
			return CreateTickingInstance();
		}

		protected abstract Instance CreateTickingInstance();

		protected abstract new class Instance : DeviceStep.Instance
		{
			private int mCurrentTick;
			private int mTickCount;

			public Instance(int tickCount)
			{
				mTickCount = tickCount;
				mCurrentTick = 0;
			}

			protected abstract void ProcessTick();
			public sealed override bool Tick()
			{
				ProcessTick();
				mCurrentTick = CurrentTick + 1;

				return (CurrentTick >= mTickCount);
			}

			protected int CurrentTick
			{
				get
				{
					return mCurrentTick;
				}
			}

			protected int TickCount
			{
				get
				{
					return mTickCount;
				}
			}
		}
	}
}
