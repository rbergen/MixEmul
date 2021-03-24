using MixLib.Events;

namespace MixLib.Device.Step
{
	public abstract class DeviceStep
	{
		public DeviceStep NextStep { get; set; }

		protected DeviceStep() { }

		public abstract Instance CreateInstance();

		public abstract string StatusDescription { get; }

		public abstract class Instance
		{
			private object _passthrough;

			public InOutputOperands Operands { get; set; }

			public event ReportingEventHandler ReportingEvent;

			public virtual object OutputForNextStep 
				=> _passthrough;

			protected virtual void OnReportingEvent(ReportingEventArgs args) 
				=> ReportingEvent?.Invoke(this, args);

			protected Instance() { }

			public abstract bool Tick();

			public virtual object InputFromPreviousStep
			{
				set => _passthrough = value;
			}
		}
	}
}
