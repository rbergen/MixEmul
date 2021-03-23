using MixLib.Device.Settings;
using MixLib.Device.Step;
using MixLib.Events;
using MixLib.Interrupts;
using MixLib.Type;
using System.IO;

namespace MixLib.Device
{
	public abstract class FileBasedDevice : MixDevice
	{
		public const string FileNameExtension = "mixdev";

		readonly string mFileNamePrefix;
		string mFilePath;

		protected StreamStatus StreamStatus { get; set; }

		protected FileBasedDevice(int id, string fileNamePrefix) : base(id)
		{
			mFileNamePrefix = fileNamePrefix;
			mFilePath = null;
			StreamStatus = new StreamStatus();
			StreamStatus.ReportingEvent += StreamStatus_Reporting;
		}

		public string DefaultFileName => mFileNamePrefix + Id + "." + FileNameExtension;

		public static FileStream OpenStream(string fileName, FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
		{
			return new FileStream(fileName, fileMode, fileAccess, fileShare);
		}

		public void CloseStream()
		{
			StreamStatus.CloseStream();
		}

		void StreamStatus_Reporting(object sender, ReportingEventArgs args)
		{
			OnReportingEvent(args);
		}

		protected override DeviceStep.Instance GetCurrentStepInstance()
		{
			return CurrentStep is StreamStep ? ((StreamStep)CurrentStep).CreateStreamInstance(StreamStatus)
: base.GetCurrentStepInstance();
		}

		public override void Reset()
		{
			StreamStatus.Reset();
			base.Reset();
		}

		public override void StartInput(IMemory memory, int mValue, int sector, InterruptQueueCallback callback)
		{
			StreamStatus.FileName = FilePath;
			base.StartInput(memory, mValue, sector, callback);
		}

		public override void StartIoc(int mValue, int sector, InterruptQueueCallback callback)
		{
			StreamStatus.FileName = FilePath;
			base.StartIoc(mValue, sector, callback);
		}

		public override void StartOutput(IMemory memory, int mValue, int sector, InterruptQueueCallback callback)
		{
			StreamStatus.FileName = FilePath;
			base.StartOutput(memory, mValue, sector, callback);
		}

		public string FilePath
		{
			get
			{
				string str = mFilePath ?? DefaultFileName;
				return Path.Combine(DeviceSettings.DeviceFilesDirectory, str);
			}
			set => mFilePath = value;
		}
	}
}
