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

        static string mDefaultDeviceFilesDirectory;
        static string mDeviceFilesDirectory;
        static int mDeviceReloadInterval;
        static readonly Dictionary<string, int> mDefaultTickCounts;

        public static Dictionary<string, int> TickCounts { private get; set; }

        static DeviceSettings()
		{
			mDefaultTickCounts = new Dictionary<string, int>();
			mDefaultTickCounts.Add(CardReaderInitialization, 100);
			mDefaultTickCounts.Add(CardWriterInitialization, 100);
			mDefaultTickCounts.Add(DiskInitialization, 50);
			mDefaultTickCounts.Add(DiskSectorSeek, 20);
			mDefaultTickCounts.Add(PaperTapeInitialization, 100);
			mDefaultTickCounts.Add(PaperTapeRecordWind, 5);
			mDefaultTickCounts.Add(PrinterInitialization, 50);
			mDefaultTickCounts.Add(TapeInitialization, 100);
			mDefaultTickCounts.Add(TapeRecordWind, 10);
			mDefaultTickCounts.Add(TeletypeInitialization, 25);

			TickCounts = new Dictionary<string, int>();

			mDeviceReloadInterval = UnsetDeviceReloadInterval;
		}

        public static bool IsKnownTickCount(string name) => mDefaultTickCounts.ContainsKey(name);

        public static int GetDefaultTickCount(string name)
		{
			if (mDefaultTickCounts.ContainsKey(name))
			{
				return mDefaultTickCounts[name];
			}

			return 50;
		}

		public static int GetTickCount(string name)
		{
			if (TickCounts.ContainsKey(name))
			{
				return TickCounts[name];
			}

			return GetDefaultTickCount(name);
		}

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
