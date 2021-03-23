using MixLib.Device;
using MixLib.Misc;

namespace MixLib.Events
{
	public class DeviceReportingEventArgs : ReportingEventArgs
	{
		public MixDevice ReportingDevice { get; private set; }

		public DeviceReportingEventArgs(MixDevice reportingDevice, Severity severity, string message)
			: base(severity, message)
		{
			ReportingDevice = reportingDevice;
		}
	}
}
