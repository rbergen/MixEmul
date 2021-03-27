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

		private static readonly Dictionary<string, Color> _defaultColors;
		private static readonly Dictionary<string, Font> _defaultFonts;

		public static Dictionary<string, Color> Colors { private get; set; }
		public static ProfilingInfoType ShowProfilingInfo { get; set; }
		public static bool ColorProfilingCounts { get; set; }

		static GuiSettings()
		{
			_defaultColors = new Dictionary<string, Color>
			{
				{ ErrorText, Color.Red },
				{ WarningText, Color.DarkOrange },
				{ InfoText, Color.DarkGreen },
				{ DebugText, Color.DarkBlue },
				{ RenderedText, Color.Black },
				{ EditingText, Color.Blue },
				{ ImmutableText, Color.DarkGray },
				{ EditorBackground, Color.White },
				{ LoadedInstructionText, Color.Black },
				{ LoadedInstructionBackground, Color.FromArgb(255, 255, 255, 204) },
				{ AddressText, Color.Black },
				{ ProgramCounterAddressBackground, Color.Yellow },
				{ DeviceIdle, Color.LawnGreen },
				{ DeviceBusy, Color.Yellow },
				{ LocationFieldText, Color.Black },
				{ OpFieldText, Color.Blue },
				{ AddressFieldText, Color.Black },
				{ CommentFieldText, Color.DarkGreen },
				{ LineNumberText, Color.DarkSeaGreen },
				{ LineNumberSeparator, Color.DarkGray },
				{ TeletypeInputText, Color.Black },
				{ TeletypeInputBackground, Color.White },
				{ TeletypeOutputText, Color.White },
				{ TeletypeOutputBackground, Color.Black }
			};

			Colors = new Dictionary<string, Color>();
			_defaultFonts = new Dictionary<string, Font>
			{
				{ FixedWidth, new Font("Courier New", 9f, FontStyle.Regular, GraphicsUnit.Point, 0) }
			};

			ColorProfilingCounts = true;
		}

		public static bool IsKnownColor(string name) => _defaultColors.ContainsKey(name);

		public static Color GetColor(string name)
		{
			if (Colors.ContainsKey(name))
				return Colors[name];

			return GetDefaultColor(name);
		}

		public static Color GetDefaultColor(string name)
		{
			if (_defaultColors.ContainsKey(name))
				return _defaultColors[name];

			return SystemColors.WindowText;
		}

		public static Font GetFont(string name)
		{
			if (_defaultFonts.ContainsKey(name))
				return _defaultFonts[name];

			var enumerator = ((IEnumerable<Font>)_defaultFonts).GetEnumerator();
			enumerator.MoveNext();
			return enumerator.Current;
		}

		public static string[] KnownColorNames
		{
			get
			{
				string[] array = new string[_defaultColors.Count];
				_defaultColors.Keys.CopyTo(array, 0);
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
