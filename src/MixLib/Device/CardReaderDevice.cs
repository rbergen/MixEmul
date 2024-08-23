using System;
using System.IO;
using MixLib.Device.Settings;
using MixLib.Device.Step;
using MixLib.Events;
using MixLib.Misc;
using MixLib.Type;

namespace MixLib.Device
{
	public class CardReaderDevice : FileBasedDevice
	{
		private const string MyShortName = "CRD";
		private const string FileNamePrefix = "crdin";
		private const string InitializationDescription = "Initializing card reader";
		private const string OpeningDescription = "Starting read from card reader";
		private const int MyRecordWordCount = 16;
		public const int BytesPerRecord = MyRecordWordCount * FullWord.ByteCount;

		public CardReaderDevice(int id) : base(id, FileNamePrefix) 
			=> UpdateSettings();

		public override int RecordWordCount 
			=> MyRecordWordCount;

		public override string ShortName 
			=> MyShortName;

		public override bool SupportsInput 
			=> true;

		public override bool SupportsOutput 
			=> false;

		public sealed override void UpdateSettings()
		{
			DeviceStep nextStep = new NoOpStep(DeviceSettings.GetTickCount(DeviceSettings.CardReaderInitialization), InitializationDescription);
			FirstInputDeviceStep = nextStep;
			nextStep.NextStep = new OpenStreamStep();
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new TextReadStep(MyRecordWordCount);
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new CloseStreamStep();
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new WriteToMemoryStep(false, MyRecordWordCount)
			{
				NextStep = null
			};

			FirstOutputDeviceStep = null;
			FirstIocDeviceStep = null;
		}

		private class OpenStreamStep : StreamStep
		{
			public override string StatusDescription 
				=> OpeningDescription;

			public override StreamStep.Instance CreateStreamInstance(StreamStatus streamStatus) 
				=> new Instance(streamStatus);

			private new class Instance(StreamStatus streamStatus) : StreamStep.Instance(streamStatus)
			{
				public override bool Tick()
				{
					try
					{
						var stream = new FileStream(StreamStatus.FileName, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);

						if (!StreamStatus.PositionSet)
							stream.Position = 0L;

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
