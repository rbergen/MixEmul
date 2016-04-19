using System.Collections.Generic;
using System.Drawing;

namespace MixGui.Settings
{
	public static class GuiSettings
	{
		public const string AddressFieldText = "AddressFieldText";
		public const string AddressText = "AddressText";
		public const string CommentFieldText = "CommentFieldText";
		public const string DebugText = "DebugText";
		public const string DeviceBusy = "DeviceBusy";
		public const string DeviceIdle = "DeviceIdle";
		public const string EditingText = "EditingText";
		public const string EditorBackground = "EditorBackground";
		public const string ErrorText = "ErrorText";
		public const string FixedWidth = "FixedWidthFont";
		public const string ImmutableText = "ImmutableText";
		public const string InfoText = "InfoText";
		public const string LoadedInstructionBackground = "LoadedInstructionBackground";
		public const string LoadedInstructionText = "LoadedInstructionText";
		public const string LineNumberSeparator = "LineNumberSeparator";
		public const string LineNumberText = "LineNumberText";
		public const string LocationFieldText = "LocationFieldText";
		public const string OpFieldText = "OpFieldText";
		public const string ProgramCounterAddressBackground = "ProgramCounterAddressBackground";
		public const string RenderedText = "RenderedText";
		public const string TeletypeInputBackground = "TeletypeInputBackground";
		public const string TeletypeInputText = "TeletypeInputText";
		public const string TeletypeOutputBackground = "TeletypeOutputBackground";
		public const string TeletypeOutputText = "TeletypeOutputText";
		public const string WarningText = "WarningText";

		private static Dictionary<string, Color> mDefaultColors;
		private static Dictionary<string, Font> mDefaultFonts;

        public static Dictionary<string, Color> Colors { private get; set; }
        public static ProfilingInfoType ShowProfilingInfo { get; set; }
        public static bool ColorProfilingCounts { get; set; }

        static GuiSettings()
		{
			mDefaultColors = new Dictionary<string, Color>();
			mDefaultColors.Add(ErrorText, Color.Red);
			mDefaultColors.Add(WarningText, Color.DarkOrange);
			mDefaultColors.Add(InfoText, Color.DarkGreen);
			mDefaultColors.Add(DebugText, Color.DarkBlue);
			mDefaultColors.Add(RenderedText, Color.Black);
			mDefaultColors.Add(EditingText, Color.Blue);
			mDefaultColors.Add(ImmutableText, Color.DarkGray);
			mDefaultColors.Add(EditorBackground, Color.White);
			mDefaultColors.Add(LoadedInstructionText, Color.Black);
			mDefaultColors.Add(LoadedInstructionBackground, Color.FromArgb(255, 255, 255, 204));
			mDefaultColors.Add(AddressText, Color.Black);
			mDefaultColors.Add(ProgramCounterAddressBackground, Color.Yellow);
			mDefaultColors.Add(DeviceIdle, Color.LawnGreen);
			mDefaultColors.Add(DeviceBusy, Color.Yellow);
			mDefaultColors.Add(LocationFieldText, Color.Black);
			mDefaultColors.Add(OpFieldText, Color.Blue);
			mDefaultColors.Add(AddressFieldText, Color.Black);
			mDefaultColors.Add(CommentFieldText, Color.DarkGreen);
			mDefaultColors.Add(LineNumberText, Color.DarkSeaGreen);
			mDefaultColors.Add(LineNumberSeparator, Color.DarkGray);
			mDefaultColors.Add(TeletypeInputText, Color.Black);
			mDefaultColors.Add(TeletypeInputBackground, Color.White);
			mDefaultColors.Add(TeletypeOutputText, Color.White);
			mDefaultColors.Add(TeletypeOutputBackground, Color.Black);
			Colors = new Dictionary<string, Color>();
			mDefaultFonts = new Dictionary<string, Font>();
			mDefaultFonts.Add(FixedWidth, new Font("Courier New", 9f, FontStyle.Regular, GraphicsUnit.Point, 0));
			ColorProfilingCounts = true;
		}

        public static bool IsKnownColor(string name) => mDefaultColors.ContainsKey(name);

        public static Color GetColor(string name)
		{
			if (Colors.ContainsKey(name))
			{
				return Colors[name];
			}

			return GetDefaultColor(name);
		}

		public static Color GetDefaultColor(string name)
		{
			if (mDefaultColors.ContainsKey(name))
			{
				return mDefaultColors[name];
			}

			return SystemColors.WindowText;
		}

		public static Font GetFont(string name)
		{
			if (mDefaultFonts.ContainsKey(name))
			{
				return mDefaultFonts[name];
			}
			IEnumerator<Font> enumerator = ((IEnumerable<Font>)mDefaultFonts).GetEnumerator();
			enumerator.MoveNext();
			return enumerator.Current;
		}

		public static string[] KnownColorNames
		{
			get
			{
				string[] array = new string[mDefaultColors.Count];
				mDefaultColors.Keys.CopyTo(array, 0);
				return array;
			}
		}

		public enum ProfilingInfoType
		{
			Execution = 0,
			Tick
		}
	}
}
