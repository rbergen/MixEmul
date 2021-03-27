using System.IO;
using MixLib.Device.Settings;
using MixLib.Device.Step;
using MixLib.Events;
using MixLib.Interrupts;
using MixLib.Type;

namespace MixLib.Device
{
	public abstract class FileBasedDevice : MixDevice
	{
		public const string FileNameExtension = "mixdev";

		private readonly string _fileNamePrefix;
		private string _filePath;

		protected StreamStatus StreamStatus { get; set; }

		protected FileBasedDevice(int id, string fileNamePrefix) : base(id)
		{
			_fileNamePrefix = fileNamePrefix;
			_filePath = null;
			StreamStatus = new StreamStatus();
			StreamStatus.ReportingEvent += StreamStatus_Reporting;
		}

		public string DefaultFileName
			=> _fileNamePrefix + Id + "." + FileNameExtension;

		public static FileStream OpenStream(string fileName, FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
			=> new(fileName, fileMode, fileAccess, fileShare);

		public void CloseStream()
			=> StreamStatus.CloseStream();

		private void StreamStatus_Reporting(object sender, ReportingEventArgs args)
			=> OnReportingEvent(args);

		protected override DeviceStep.Instance GetCurrentStepInstance()
			=> CurrentStep is StreamStep step ? step.CreateStreamInstance(StreamStatus) : base.GetCurrentStepInstance();

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
				string str = _filePath ?? DefaultFileName;
				return Path.Combine(DeviceSettings.DeviceFilesDirectory, str);
			}
			set => _filePath = value;
		}
	}
}
