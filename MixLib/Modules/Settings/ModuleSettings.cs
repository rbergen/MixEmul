
namespace MixLib.Modules.Settings
{
	public static class ModuleSettings
	{
		public const int FloatingPointMemoryWordCountDefault = 200;
		public const bool FloatingPointEnabledDefault = true;
		public const string FloatingPointProgramFileDefault = "floatingpoint.mixal";
		public const string ControlProgramFileDefault = "control.mixal";

		static bool? mFloatingPointEnabled;
		static string mFloatingPointProgramFile;
		static string mControlProgramFile;
		static int? mFloatingPointMemoryWordCount;

		public static bool FloatingPointEnabled
		{
			get => mFloatingPointEnabled ?? FloatingPointEnabledDefault;
			set => mFloatingPointEnabled = value;
		}

		public static bool FloatingPointEnabledDefined
		{
			get => mFloatingPointEnabled != null;
			set
			{
				if (!value)
				{
					mFloatingPointEnabled = null;
				}
				else if (mFloatingPointEnabled == null)
				{
					mFloatingPointEnabled = FloatingPointEnabledDefault;
				}
			}
		}

		public static string FloatingPointProgramFile
		{
			get => mFloatingPointProgramFile ?? FloatingPointProgramFileDefault;
			set => mFloatingPointProgramFile = value;
		}

		public static string ControlProgramFile
		{
			get => mControlProgramFile ?? ControlProgramFileDefault;
			set => mControlProgramFile = value;
		}

		public static int FloatingPointMemoryWordCount
		{
			get => mFloatingPointMemoryWordCount ?? FloatingPointMemoryWordCountDefault;
			set => mFloatingPointMemoryWordCount = value;
		}

		public static bool FloatingPointMemoryWordCountDefined
		{
			get => mFloatingPointMemoryWordCount != null;
			set
			{
				if (!value)
				{
					mFloatingPointMemoryWordCount = null;
				}
				else if (mFloatingPointMemoryWordCount == null)
				{
					mFloatingPointMemoryWordCount = FloatingPointMemoryWordCountDefault;
				}
			}
		}
	}
}
