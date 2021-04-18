using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;
using MixGui.Utils;
using MixLib.Device.Settings;

namespace MixGui.Settings
{
	public class Configuration
	{
		private const string FileName = "config.json";

		private static JsonSerializerOptions _jsonSerializerOptions;

		private string _deviceFilesDirectory;

		public Dictionary<string, Color> Colors { get; set; }
		public Dictionary<int, string> DeviceFilePaths { get; set; }
		public string[] LoaderCards { get; set; }
		public bool DeviceEditorVisible { get; set; }
		public Point DeviceEditorWindowLocation { get; set; }
		public Size DeviceEditorWindowSize { get; set; }
		public Point MainWindowLocation { get; set; }
		public Size MainWindowSize { get; set; }
		public FormWindowState MainWindowState { get; set; }
		public bool TeletypeEchoInput { get; set; }
		public bool TeletypeOnTop { get; set; }
		public bool TeletypeVisible { get; set; }
		public Point TeletypeWindowLocation { get; set; }
		public Size TeletypeWindowSize { get; set; }
		public int DeviceReloadInterval { get; set; }
		public int? FloatingPointMemoryWordCount { get; set; }
		public bool ProfilingEnabled { get; set; }
		public Dictionary<string, int> TickCounts { get; set; }
		public GuiSettings.ProfilingInfoType ShowProfilingInfo { get; set; }
		public bool ColorProfilingCounts { get; set; }
		public bool ShowSourceInline { get; set; }

		public Configuration()
		{
			TeletypeWindowSize = Size.Empty;
			TeletypeWindowLocation = Point.Empty;
			TeletypeEchoInput = true;
			DeviceEditorWindowSize = Size.Empty;
			DeviceEditorWindowLocation = Point.Empty;
			MainWindowSize = Size.Empty;
			MainWindowLocation = Point.Empty;
			DeviceReloadInterval = DeviceSettings.UnsetDeviceReloadInterval;
			FloatingPointMemoryWordCount = null;
			ColorProfilingCounts = true;
			ShowSourceInline = true;
			FillEmptyMembers();
		}

		private void FillEmptyMembers()
		{
			if (Colors == null)
				Colors = new Dictionary<string, Color>();

			if (TickCounts == null)
				TickCounts = new Dictionary<string, int>();

			if (DeviceFilePaths == null)
				DeviceFilePaths = new Dictionary<int, string>();
		}

		public static JsonSerializerOptions JsonSerializerOptions
		{
			get
			{
				if (_jsonSerializerOptions == null)
				{
					_jsonSerializerOptions = new JsonSerializerOptions();
					_jsonSerializerOptions.Converters.Add(new ColorJsonConverter());
				}

				return _jsonSerializerOptions;
			}
		}


		public static Configuration Load(string directory)
		{
			Configuration configuration = null;

			try
			{
				var path = Path.Combine(directory, FileName);

				if (File.Exists(path))
					configuration = JsonSerializer.Deserialize<Configuration>(File.ReadAllBytes(path), JsonSerializerOptions);
			}
			catch (Exception) 
			{}

			if (configuration == null)
				configuration = new();

			configuration.FillEmptyMembers();

			return configuration;
		}

		public void Save(string directory)
		{
			using var serializationStream = new FileStream(Path.Combine(directory, FileName), FileMode.Create);
			JsonSerializer.Serialize(new Utf8JsonWriter(serializationStream), this, JsonSerializerOptions);
			serializationStream.Close();
		}

		public string DeviceFilesDirectory
		{
			get => _deviceFilesDirectory;
			set
			{
				if (value == null || Directory.Exists(value))
					_deviceFilesDirectory = value;
			}
		}
	}
}
