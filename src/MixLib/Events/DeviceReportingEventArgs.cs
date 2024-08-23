using MixLib.Device;
using MixLib.Misc;

namespace MixLib.Events
{
	public class DeviceReportingEventArgs(MixDevice reportingDevice, Severity severity, string message) : ReportingEventArgs(severity, message)
	{
		public MixDevice ReportingDevice => reportingDevice;
  }
}
