using System.Collections.Generic;

namespace MixLib.Device.Settings
{
	public static class DeviceSettings
	{
		public const string CardReaderInitialization = "CardReaderInitialization";
		public const string CardWriterInitialization = "CardWriterInitialization";
		public const string DiskInitialization = "DiskInitialization";
		public const string DiskSectorSeek = "DiskSectorSeek";
		public const string PaperTapeInitialization = "PaperTapeInitialization";
		public const string PaperTapeRecordWind = "PaperTapeRecordWind";
		public const string PrinterInitialization = "PrinterInitialization";
		public const string TapeInitialization = "TapeInitialization";
		public const string TapeRecordWind = "TapeRecordWind";
		public const string TeletypeInitialization = "TeletypeInitialization";
		public const int DefaultDeviceReloadInterval = 500;
		public const int UnsetDeviceReloadInterval = int.MinValue;
		public const int DefaultDefaultTickCount = 50;

		private static string defaultDeviceFilesDirectory;
		private static string deviceFilesDirectory;
		private static int deviceReloadInterval;
		private static readonly Dictionary<string, int> defaultTickCounts;

		public static Dictionary<string, int> TickCounts { private get; set; }

		static DeviceSettings()
		{
			defaultTickCounts = new Dictionary<string, int>
			{
				{ CardReaderInitialization, 100 },
				{ CardWriterInitialization, 100 },
				{ DiskInitialization, 50 },
				{ DiskSectorSeek, 20 },
				{ PaperTapeInitialization, 100 },
				{ PaperTapeRecordWind, 5 },
				{ PrinterInitialization, 50 },
				{ TapeInitialization, 100 },
				{ TapeRecordWind, 10 },
				{ TeletypeInitialization, 25 }
			};

			TickCounts = [];

			deviceReloadInterval = UnsetDeviceReloadInterval;
		}

		public static bool IsKnownTickCount(string name) 
			=> defaultTickCounts.ContainsKey(name);

		public static int GetDefaultTickCount(string name) 
			=> IsKnownTickCount(name) ? defaultTickCounts[name] : DefaultDefaultTickCount;

		public static int GetTickCount(string name) 
			=> TickCounts.TryGetValue(name, out var value) ? value : GetDefaultTickCount(name);

		public static string DefaultDeviceFilesDirectory
		{
			get => defaultDeviceFilesDirectory ?? string.Empty;
			set => defaultDeviceFilesDirectory = value;
		}

		public static string DeviceFilesDirectory
		{
			get => deviceFilesDirectory ?? DefaultDeviceFilesDirectory;
			set => deviceFilesDirectory = value;
		}

		public static int DeviceReloadInterval
		{
			get => deviceReloadInterval == UnsetDeviceReloadInterval ? DefaultDeviceReloadInterval : deviceReloadInterval;
			set => deviceReloadInterval = value;
		}

		public static string[] KnownTickCountNames
		{
			get
			{
				string[] array = new string[defaultTickCounts.Count];
				defaultTickCounts.Keys.CopyTo(array, 0);
				return array;
			}
		}
	}
}
