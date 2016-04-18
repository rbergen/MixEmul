using MixLib.Device;
using MixLib.Events;

namespace MixLib.Device.Step
{
	public abstract class DeviceStep
	{
		public DeviceStep NextStep { get; set; }

		protected DeviceStep()
		{
		}

		public abstract Instance CreateInstance();

		public abstract string StatusDescription { get; }

		public abstract class Instance
		{
			private object mPassthrough;

            public InOutputOperands Operands { get; set; }

            public event ReportingEventHandler ReportingEvent;

			protected Instance()
			{
			}

			protected virtual void OnReportingEvent(ReportingEventArgs args)
			{
                ReportingEvent?.Invoke(this, args);
            }

			public abstract bool Tick();

			public virtual object InputFromPreviousStep
			{
				set
				{
					mPassthrough = value;
				}
			}

			public virtual object OutputForNextStep
			{
				get
				{
					return mPassthrough;
				}
			}
		}
	}
}
