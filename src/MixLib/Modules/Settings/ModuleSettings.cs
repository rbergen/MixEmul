
namespace MixLib.Modules.Settings
{
	public static class ModuleSettings
	{
		public const int FloatingPointMemoryWordCountDefault = 200;
		public const bool FloatingPointEnabledDefault = true;
		public const string FloatingPointProgramFileDefault = "floatingpoint.mixal";
		public const string ControlProgramFileDefault = "control.mixal";

		private static bool? floatingPointEnabled;
		private static string floatingPointProgramFile;
		private static string controlProgramFile;
		private static int? floatingPointMemoryWordCount;

		public static bool FloatingPointEnabled
		{
			get => floatingPointEnabled ?? FloatingPointEnabledDefault;
			set => floatingPointEnabled = value;
		}

		public static bool FloatingPointEnabledDefined
		{
			get => floatingPointEnabled != null;
			set
			{
				if (!value)
					floatingPointEnabled = null;

				else if (floatingPointEnabled == null)
					floatingPointEnabled = FloatingPointEnabledDefault;
			}
		}

		public static string FloatingPointProgramFile
		{
			get => floatingPointProgramFile ?? FloatingPointProgramFileDefault;
			set => floatingPointProgramFile = value;
		}

		public static string ControlProgramFile
		{
			get => controlProgramFile ?? ControlProgramFileDefault;
			set => controlProgramFile = value;
		}

		public static int FloatingPointMemoryWordCount
		{
			get => floatingPointMemoryWordCount ?? FloatingPointMemoryWordCountDefault;
			set => floatingPointMemoryWordCount = value;
		}

		public static bool FloatingPointMemoryWordCountDefined
		{
			get => floatingPointMemoryWordCount != null;
			set
			{
				if (!value)
					floatingPointMemoryWordCount = null;

				else if (floatingPointMemoryWordCount == null)
					floatingPointMemoryWordCount = FloatingPointMemoryWordCountDefault;
			}
		}
	}
}
