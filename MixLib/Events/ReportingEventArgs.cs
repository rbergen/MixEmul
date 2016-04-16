using System;

namespace MixLib.Events
{
	public class ReportingEventArgs : EventArgs
	{
        public string Message { get; private set; }
        public MixLib.Misc.Severity Severity { get; private set; }

        public ReportingEventArgs(MixLib.Misc.Severity severity, string message)
		{
			Severity = severity;
			Message = message;
		}
	}
}
