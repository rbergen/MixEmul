
namespace MixLib.Misc
{
	public class LogLine
	{
		public const int NoAddress = -1;

		public int Address { get; private set; }
		public string Message { get; private set; }
		public Misc.Severity Severity { get; private set; }
		public string Title { get; private set; }
		public string ModuleName { get; private set; }

		public LogLine(string moduleName, Misc.Severity severity, string title, string message)
	: this(moduleName, severity, NoAddress, title, message) { }

		public LogLine(string moduleName, Misc.Severity severity, int address, string title, string message)
		{
			Severity = severity;
			Address = address;
			Title = title;
			Message = message;
			ModuleName = moduleName;
		}
	}
}
