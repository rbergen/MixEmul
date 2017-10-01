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

        static string mDefaultDeviceFilesDirectory;
        static string mDeviceFilesDirectory;
        static int mDeviceReloadInterval;
        static readonly Dictionary<string, int> mDefaultTickCounts;

        public static Dictionary<string, int> TickCounts { private get; set; }

        static DeviceSettings()
		{
            mDefaultTickCounts = new Dictionary<string, int>
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

            TickCounts = new Dictionary<string, int>();

			mDeviceReloadInterval = UnsetDeviceReloadInterval;
		}

        public static bool IsKnownTickCount(string name) => mDefaultTickCounts.ContainsKey(name);

        public static int GetDefaultTickCount(string name) => 
            IsKnownTickCount(name) ? mDefaultTickCounts[name] : DefaultDefaultTickCount;

		public static int GetTickCount(string name) => 
            TickCounts.ContainsKey(name) ? TickCounts[name] : GetDefaultTickCount(name);

		public static string DefaultDeviceFilesDirectory
		{
			get
			{
				return mDefaultDeviceFilesDirectory ?? "";
			}
			set
			{
				mDefaultDeviceFilesDirectory = value;
			}
		}

		public static string DeviceFilesDirectory
		{
			get
			{
				return mDeviceFilesDirectory ?? DefaultDeviceFilesDirectory;
			}
			set
			{
				mDeviceFilesDirectory = value;
			}
		}

		public static int DeviceReloadInterval
		{
			get
			{
				return mDeviceReloadInterval == UnsetDeviceReloadInterval ? DefaultDeviceReloadInterval : mDeviceReloadInterval;
			}
			set
			{
				mDeviceReloadInterval = value;
			}
		}

		public static string[] KnownTickCountNames
		{
			get
			{
				string[] array = new string[mDefaultTickCounts.Count];
				mDefaultTickCounts.Keys.CopyTo(array, 0);
				return array;
			}
		}
	}
}
