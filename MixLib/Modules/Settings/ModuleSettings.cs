
namespace MixLib.Modules.Settings
{
	public static class ModuleSettings
	{
		public const int FloatingPointMemoryWordCountDefault = 200;
		public const bool FloatingPointEnabledDefault = true;
		public const string FloatingPointProgramFileDefault = "floatingpoint.mixal";
		public const string ControlProgramFileDefault = "control.mixal";

		private static bool? _floatingPointEnabled;
		private static string _floatingPointProgramFile;
		private static string _controlProgramFile;
		private static int? _floatingPointMemoryWordCount;

		public static bool FloatingPointEnabled
		{
			get => _floatingPointEnabled ?? FloatingPointEnabledDefault;
			set => _floatingPointEnabled = value;
		}

		public static bool FloatingPointEnabledDefined
		{
			get => _floatingPointEnabled != null;
			set
			{
				if (!value)
					_floatingPointEnabled = null;

				else if (_floatingPointEnabled == null)
					_floatingPointEnabled = FloatingPointEnabledDefault;
			}
		}

		public static string FloatingPointProgramFile
		{
			get => _floatingPointProgramFile ?? FloatingPointProgramFileDefault;
			set => _floatingPointProgramFile = value;
		}

		public static string ControlProgramFile
		{
			get => _controlProgramFile ?? ControlProgramFileDefault;
			set => _controlProgramFile = value;
		}

		public static int FloatingPointMemoryWordCount
		{
			get => _floatingPointMemoryWordCount ?? FloatingPointMemoryWordCountDefault;
			set => _floatingPointMemoryWordCount = value;
		}

		public static bool FloatingPointMemoryWordCountDefined
		{
			get => _floatingPointMemoryWordCount != null;
			set
			{
				if (!value)
					_floatingPointMemoryWordCount = null;

				else if (_floatingPointMemoryWordCount == null)
					_floatingPointMemoryWordCount = FloatingPointMemoryWordCountDefault;
			}
		}
	}
}
