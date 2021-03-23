using MixLib.Device.Settings;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace MixGui.Settings
{
	[Serializable]
	public class Configuration : ISerializable, IDeserializationCallback
	{
		const string fileName = "config.bin";
		readonly SerializationInfo mSerializationInfo;
		string mDeviceFilesDirectory;

		public Dictionary<string, Color> Colors { get; private set; }
		public Dictionary<int, string> DeviceFilePaths { get; private set; }
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
			FillEmptyMembers();
		}

		protected Configuration(SerializationInfo info, StreamingContext context)
		{
			mSerializationInfo = info;
		}

		void FillEmptyMembers()
		{
			if (Colors == null)
			{
				Colors = new Dictionary<string, Color>();
			}

			if (TickCounts == null)
			{
				TickCounts = new Dictionary<string, int>();
			}

			if (DeviceFilePaths == null)
			{
				DeviceFilePaths = new Dictionary<int, string>();
			}
		}

		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("mColors", Colors);
			info.AddValue("mTickCounts", TickCounts);
			info.AddValue("mDeviceFiles", DeviceFilePaths);
			info.AddValue("mDeviceDirectory", DeviceFilesDirectory);
			info.AddValue("mDeviceEditorWindowLocation", DeviceEditorWindowLocation);
			info.AddValue("mDeviceEditorWindowSize", DeviceEditorWindowSize);
			info.AddValue("mDeviceEditorVisible", DeviceEditorVisible);
			info.AddValue("mMainWindowLocation", MainWindowLocation);
			info.AddValue("mMainWindowSize", MainWindowSize);
			info.AddValue("mMainWindowState", MainWindowState);
			info.AddValue("mTeletypeOnTop", TeletypeOnTop);
			info.AddValue("mTeletypeWindowLocation", TeletypeWindowLocation);
			info.AddValue("mTeletypeWindowSize", TeletypeWindowSize);
			info.AddValue("mTeletypeVisible", TeletypeVisible);
			info.AddValue("mTeletypeEchoInput", TeletypeEchoInput);
			info.AddValue("mDeviceReloadInterval", DeviceReloadInterval);
			info.AddValue("mLoaderCards", LoaderCards);
			info.AddValue("mFloatingPointMemoryWordCount", FloatingPointMemoryWordCount);
			info.AddValue("mProfilingEnabled", ProfilingEnabled);
			info.AddValue("mShowProfilingInfo", ShowProfilingInfo);
			info.AddValue("mColorProfilingCounts", ColorProfilingCounts);
		}

		public static Configuration Load(string directory)
		{
			try
			{
				var path = Path.Combine(directory, fileName);

				if (File.Exists(path))
				{
					var serializationStream = new FileStream(path, FileMode.Open);
					var formatter = new BinaryFormatter();
					var configuration = (Configuration)formatter.Deserialize(serializationStream);
					serializationStream.Close();

					return configuration;
				}
			}
			catch (Exception)
			{
			}

			return new Configuration();
		}

		public void Save(string directory)
		{
			var serializationStream = new FileStream(Path.Combine(directory, fileName), FileMode.Create);
			new BinaryFormatter().Serialize(serializationStream, this);
			serializationStream.Close();
		}

		public string DeviceFilesDirectory
		{
			get
			{
				return mDeviceFilesDirectory;
			}
			set
			{
				if (value == null || Directory.Exists(value))
				{
					mDeviceFilesDirectory = value;
				}
			}
		}

		public void OnDeserialization(object sender)
		{
			DeviceEditorWindowSize = Size.Empty;
			DeviceEditorWindowLocation = Point.Empty;
			TeletypeWindowSize = Size.Empty;
			TeletypeWindowLocation = Point.Empty;
			TeletypeEchoInput = true;
			MainWindowSize = Size.Empty;
			MainWindowLocation = Point.Empty;
			DeviceReloadInterval = DeviceSettings.UnsetDeviceReloadInterval;
			FloatingPointMemoryWordCount = null;
			ColorProfilingCounts = true;

			try
			{
				Colors = (Dictionary<string, Color>)mSerializationInfo.GetValue("mColors", typeof(Dictionary<string, Color>));
			}
			catch (SystemException) { }

			try
			{
				TickCounts = (Dictionary<string, int>)mSerializationInfo.GetValue("mTickCounts", typeof(Dictionary<string, int>));
			}
			catch (SystemException) { }

			try
			{
				DeviceFilePaths = (Dictionary<int, string>)mSerializationInfo.GetValue("mDeviceFiles", typeof(Dictionary<int, string>));
			}
			catch (SystemException) { }

			try
			{
				DeviceFilesDirectory = mSerializationInfo.GetString("mDeviceDirectory");
			}
			catch (SystemException) { }

			try
			{
				DeviceEditorWindowLocation = (Point)mSerializationInfo.GetValue("mDeviceEditorWindowLocation", typeof(Point));
			}
			catch (SystemException) { }

			try
			{
				DeviceEditorWindowSize = (Size)mSerializationInfo.GetValue("mDeviceEditorWindowSize", typeof(Size));
			}
			catch (SystemException) { }

			try
			{
				DeviceEditorVisible = mSerializationInfo.GetBoolean("mDeviceEditorVisible");
			}
			catch (SystemException) { }

			try
			{
				MainWindowLocation = (Point)mSerializationInfo.GetValue("mMainWindowLocation", typeof(Point));
			}
			catch (SystemException) { }

			try
			{
				MainWindowSize = (Size)mSerializationInfo.GetValue("mMainWindowSize", typeof(Size));
			}
			catch (SystemException) { }

			try
			{
				MainWindowState = (FormWindowState)mSerializationInfo.GetValue("mMainWindowState", typeof(FormWindowState));
			}
			catch (SystemException) { }

			try
			{
				TeletypeOnTop = mSerializationInfo.GetBoolean("mTeletypeOnTop");
			}
			catch (SystemException) { }

			try
			{
				TeletypeWindowLocation = (Point)mSerializationInfo.GetValue("mTeletypeWindowLocation", typeof(Point));
			}
			catch (SystemException) { }

			try
			{
				TeletypeWindowSize = (Size)mSerializationInfo.GetValue("mTeletypeWindowSize", typeof(Size));
			}
			catch (SystemException) { }

			try
			{
				TeletypeVisible = mSerializationInfo.GetBoolean("mTeletypeVisible");
			}
			catch (SystemException) { }

			try
			{
				TeletypeEchoInput = mSerializationInfo.GetBoolean("mTeletypeEchoInput");
			}
			catch (SystemException) { }

			try
			{
				DeviceReloadInterval = mSerializationInfo.GetInt32("mDeviceReloadInterval");
			}
			catch (SystemException) { }

			try
			{
				LoaderCards = (string[])mSerializationInfo.GetValue("mLoaderCards", typeof(string[]));
			}
			catch (SystemException) { }

			try
			{
				FloatingPointMemoryWordCount = (int?)mSerializationInfo.GetValue("mFloatingPointMemoryWordCount", typeof(int?));
			}
			catch (SystemException) { }

			try
			{
				ProfilingEnabled = mSerializationInfo.GetBoolean("mProfilingEnabled");
			}
			catch (SystemException) { }

			try
			{
				ShowProfilingInfo = (GuiSettings.ProfilingInfoType)mSerializationInfo.GetValue("mShowProfilingInfo", typeof(GuiSettings.ProfilingInfoType));
			}
			catch (SystemException) { }

			try
			{
				ColorProfilingCounts = mSerializationInfo.GetBoolean("mColorProfilingCounts");
			}
			catch (SystemException) { }

			FillEmptyMembers();
		}
	}
}
