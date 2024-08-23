using System.Collections.Generic;
using System.Drawing;

namespace MixGui.Settings
{
	public static class GuiSettings
	{
		public const string AddressFieldText = nameof(AddressFieldText);
		public const string AddressText = nameof(AddressText);
		public const string CommentFieldText = nameof(CommentFieldText);
		public const string DebugText = nameof(DebugText);
		public const string DeviceBusy = nameof(DeviceBusy);
		public const string DeviceIdle = nameof(DeviceIdle);
		public const string EditingText = nameof(EditingText);
		public const string EditorBackground = nameof(EditorBackground);
		public const string ErrorText = nameof(ErrorText);
		public const string FixedWidth = "FixedWidthFont";
		public const string ImmutableText = nameof(ImmutableText);
		public const string InfoText = nameof(InfoText);
		public const string LoadedInstructionBackground = nameof(LoadedInstructionBackground);
		public const string LoadedInstructionText = nameof(LoadedInstructionText);
		public const string LineNumberSeparator = nameof(LineNumberSeparator);
		public const string LineNumberText = nameof(LineNumberText);
		public const string LocationFieldText = nameof(LocationFieldText);
		public const string OpFieldText = nameof(OpFieldText);
		public const string ProgramCounterAddressBackground = nameof(ProgramCounterAddressBackground);
		public const string RenderedText = nameof(RenderedText);
		public const string TeletypeInputBackground = nameof(TeletypeInputBackground);
		public const string TeletypeInputText = nameof(TeletypeInputText);
		public const string TeletypeOutputBackground = nameof(TeletypeOutputBackground);
		public const string TeletypeOutputText = nameof(TeletypeOutputText);
		public const string WarningText = nameof(WarningText);

		private static readonly Dictionary<string, Color> defaultColors;
		private static readonly Dictionary<string, Font> defaultFonts;

		public static Dictionary<string, Color> Colors { private get; set; }
		public static ProfilingInfoType ShowProfilingInfo { get; set; }
		public static bool ColorProfilingCounts { get; set; }
		public static bool ShowSourceInline { get; set; }

		static GuiSettings()
		{
			defaultColors = new Dictionary<string, Color>
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

			Colors = [];
			defaultFonts = new Dictionary<string, Font>
			{
				{ FixedWidth, new Font("Courier New", 9f, FontStyle.Regular, GraphicsUnit.Point, 0) }
			};

			ColorProfilingCounts = true;
		}

		public static bool IsKnownColor(string name) => defaultColors.ContainsKey(name);

		public static Color GetColor(string name)
		{
			if (Colors.TryGetValue(name, out var value))
				return value;

			return GetDefaultColor(name);
		}

		public static Color GetDefaultColor(string name)
		{
			if (defaultColors.TryGetValue(name, out var value))
				return value;

			return SystemColors.WindowText;
		}

		public static Font GetFont(string name)
		{
			if (defaultFonts.TryGetValue(name, out var value))
				return value;

			var enumerator = ((IEnumerable<Font>)defaultFonts).GetEnumerator();
			enumerator.MoveNext();
			return enumerator.Current;
		}

		public static string[] KnownColorNames
		{
			get
			{
				string[] array = new string[defaultColors.Count];
				defaultColors.Keys.CopyTo(array, 0);
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
