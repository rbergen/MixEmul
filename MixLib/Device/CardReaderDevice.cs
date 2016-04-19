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
		private const string shortName = "CRD";
		private const string fileNamePrefix = "crdin";

		private const string initializationDescription = "Initializing card reader";
		private const string openingDescription = "Starting read from card reader";

		private const int recordWordCount = 16;
		public const int BytesPerRecord = recordWordCount * FullWord.ByteCount;

		public CardReaderDevice(int id)
			: base(id, fileNamePrefix)
		{
			UpdateSettings();
		}

        public override int RecordWordCount => 16;

        public override string ShortName => shortName;

        public override bool SupportsInput => true;

        public override bool SupportsOutput => false;

        public override void UpdateSettings()
		{
			DeviceStep nextStep = new NoOpStep(DeviceSettings.GetTickCount(DeviceSettings.CardReaderInitialization), initializationDescription);
			base.FirstInputDeviceStep = nextStep;
			nextStep.NextStep = new openStreamStep();
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new TextReadStep(recordWordCount);
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new CloseStreamStep();
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new MixDevice.WriteToMemoryStep(false, recordWordCount);
			nextStep.NextStep.NextStep = null;

			base.FirstOutputDeviceStep = null;

			base.FirstIocDeviceStep = null;
		}

		private class openStreamStep : StreamStep
		{
            public override string StatusDescription => openingDescription;

            public override StreamStep.Instance CreateStreamInstance(StreamStatus streamStatus) => new Instance(streamStatus);
	
			private new class Instance : StreamStep.Instance
			{
				public Instance(StreamStatus streamStatus)
					: base(streamStatus)
				{
				}

				public override bool Tick()
				{
					try
					{
						FileStream stream = new FileStream(base.StreamStatus.FileName, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);

						if (!base.StreamStatus.PositionSet)
						{
							stream.Position = 0L;
						}

						base.StreamStatus.Stream = stream;
					}
					catch (Exception exception)
					{
						OnReportingEvent(new ReportingEventArgs(Severity.Error, "exception while opening file " + base.StreamStatus.FileName + ": " + exception.Message));
					}

					return true;
				}
			}
		}
	}
}