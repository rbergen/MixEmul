using System;

namespace MixLib.Events
{
	public class ReportingEventArgs(Misc.Severity severity, string message) : EventArgs
	{
		public string Message => message;
		public Misc.Severity Severity => severity;
  }
}
