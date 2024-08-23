
namespace MixLib.Misc
{
	public class LogLine(string moduleName, Severity severity, int address, string title, string message)
  {
		public const int NoAddress = -1;
		public int Address => address;
		public string Message => message;
		public Severity Severity => severity;
		public string Title => title;
		public string ModuleName => moduleName;

		public LogLine(string moduleName, Severity severity, string title, string message)
			: this(moduleName, severity, NoAddress, title, message) { }
  }
}
