using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using MixLib.Device.Settings;

namespace MixGui.Settings
{
	[Serializable]
	public class Configuration : ISerializable, IDeserializationCallback
	{
		private const string fileName = "config.bin";

		private SerializationInfo mSerializationInfo;
		private string mDeviceFilesDirectory;

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
			fillEmptyMembers();
		}

		protected Configuration(SerializationInfo info, StreamingContext context)
		{
			mSerializationInfo = info;
		}

		private void fillEmptyMembers()
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
				string path = Path.Combine(directory, fileName);

				if (File.Exists(path))
				{
					FileStream serializationStream = new FileStream(path, FileMode.Open);
					BinaryFormatter formatter = new BinaryFormatter();
					Configuration configuration = (Configuration)formatter.Deserialize(serializationStream);
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
			FileStream serializationStream = new FileStream(Path.Combine(directory, fileName), FileMode.Create);
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
			catch (Exception) { }

            try
            {
                TickCounts = (Dictionary<string, int>)mSerializationInfo.GetValue("mTickCounts", typeof(Dictionary<string, int>));
            }
            catch (Exception) { }

            try
            {
                DeviceFilePaths = (Dictionary<int, string>)mSerializationInfo.GetValue("mDeviceFiles", typeof(Dictionary<int, string>));
            }
            catch (Exception) { }

			try
			{
				DeviceFilesDirectory = mSerializationInfo.GetString("mDeviceDirectory");
			}
			catch (Exception) { }

            try
            {
                DeviceEditorWindowLocation = (Point)mSerializationInfo.GetValue("mDeviceEditorWindowLocation", typeof(Point));
            }
            catch (Exception) { }

            try
            {
                DeviceEditorWindowSize = (Size)mSerializationInfo.GetValue("mDeviceEditorWindowSize", typeof(Size));
            }
            catch (Exception) { }

            try
            {
                DeviceEditorVisible = mSerializationInfo.GetBoolean("mDeviceEditorVisible");
            }
            catch (Exception) { }

            try
            {
                MainWindowLocation = (Point)mSerializationInfo.GetValue("mMainWindowLocation", typeof(Point));
            }
            catch (Exception) { }

            try
            {
                MainWindowSize = (Size)mSerializationInfo.GetValue("mMainWindowSize", typeof(Size));
            }
            catch (Exception) { }

            try
            {
                MainWindowState = (FormWindowState)mSerializationInfo.GetValue("mMainWindowState", typeof(FormWindowState));
            }
            catch (Exception) { }

            try
            {
                TeletypeOnTop = mSerializationInfo.GetBoolean("mTeletypeOnTop");
            }
            catch (Exception) { }

            try
            {
                TeletypeWindowLocation = (Point)mSerializationInfo.GetValue("mTeletypeWindowLocation", typeof(Point));
            }
            catch (Exception) { }

            try
            {
                TeletypeWindowSize = (Size)mSerializationInfo.GetValue("mTeletypeWindowSize", typeof(Size));
            }
            catch (Exception) { }

            try
            {
                TeletypeVisible = mSerializationInfo.GetBoolean("mTeletypeVisible");
            }
            catch (Exception) { }

            try
            {
                TeletypeEchoInput = mSerializationInfo.GetBoolean("mTeletypeEchoInput");
            }
            catch (Exception) { }

            try
            {
                DeviceReloadInterval = mSerializationInfo.GetInt32("mDeviceReloadInterval");
            }
            catch (Exception) { }

            try
            {
                LoaderCards = (string[])mSerializationInfo.GetValue("mLoaderCards", typeof(string[]));
            }
            catch (Exception) { }

            try
            {
                FloatingPointMemoryWordCount = (int?)mSerializationInfo.GetValue("mFloatingPointMemoryWordCount", typeof(int?));
            }
            catch (Exception) { }

            try
            {
                ProfilingEnabled = mSerializationInfo.GetBoolean("mProfilingEnabled");
            }
            catch (Exception) { }

            try
            {
                ShowProfilingInfo = (GuiSettings.ProfilingInfoType)mSerializationInfo.GetValue("mShowProfilingInfo", typeof(GuiSettings.ProfilingInfoType));
            }
            catch (Exception) { }

            try
            {
                ColorProfilingCounts = mSerializationInfo.GetBoolean("mColorProfilingCounts");
            }
            catch (Exception) { }

			fillEmptyMembers();
		}
	}
}
