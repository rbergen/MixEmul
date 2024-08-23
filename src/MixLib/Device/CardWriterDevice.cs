using System;
using System.IO;
using MixLib.Device.Settings;
using MixLib.Device.Step;
using MixLib.Events;
using MixLib.Misc;
using MixLib.Type;

namespace MixLib.Device
{
	public class CardWriterDevice : FileBasedDevice
	{
		private const string MyShortName = "CWR";
		private const string FileNamePrefix = "crdout";
		private const string InitializationDescription = "Initializing card punch";
		private const string OpeningDescription = "Starting write to card punch";
		private const int MyRecordWordCount = 16;
		public const int BytesPerRecord = MyRecordWordCount * FullWord.ByteCount;

		public CardWriterDevice(int id) : base(id, FileNamePrefix) 
			=> UpdateSettings();

		public override int RecordWordCount 
			=> MyRecordWordCount;

		public override string ShortName 
			=> MyShortName;

		public override bool SupportsInput 
			=> false;

		public override bool SupportsOutput 
			=> true;

		public sealed override void UpdateSettings()
		{
			FirstInputDeviceStep = null;

			DeviceStep nextStep = new NoOpStep(DeviceSettings.GetTickCount(DeviceSettings.CardWriterInitialization), InitializationDescription);
			FirstOutputDeviceStep = nextStep;
			nextStep.NextStep = new ReadFromMemoryStep(false, MyRecordWordCount);
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new OpenStreamStep();
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new TextWriteStep(MyRecordWordCount);
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new CloseStreamStep
			{
				NextStep = null
			};

			FirstIocDeviceStep = null;
		}

		private class OpenStreamStep : StreamStep
		{
			public override StreamStep.Instance CreateStreamInstance(StreamStatus streamStatus) 
				=> new Instance(streamStatus);

			public override string StatusDescription 
				=> OpeningDescription;

			private new class Instance(StreamStatus streamStatus) : StreamStep.Instance(streamStatus)
			{
				public override bool Tick()
				{
					try
					{
						var stream = new FileStream(StreamStatus.FileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);

						if (!StreamStatus.PositionSet)
							stream.Position = stream.Length;

						StreamStatus.Stream = stream;
					}
					catch (Exception exception)
					{
						OnReportingEvent(new ReportingEventArgs(Severity.Error, "exception while opening file " + StreamStatus.FileName + ": " + exception.Message));
					}

					return true;
				}
			}
		}
	}
}
