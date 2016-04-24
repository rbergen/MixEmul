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
		private const string shortName = "CWR";
		private const string fileNamePrefix = "crdout";

		private const string initializationDescription = "Initializing card punch";
		private const string openingDescription = "Starting write to card punch";

		private const int recordWordCount = 16;
		public const int BytesPerRecord = recordWordCount * FullWord.ByteCount;

		public CardWriterDevice(int id)
			: base(id, fileNamePrefix)
		{
			UpdateSettings();
		}

        public override int RecordWordCount => recordWordCount;

        public override string ShortName => shortName;

        public override bool SupportsInput => false;

        public override bool SupportsOutput => true;

        public override void UpdateSettings()
		{
            FirstInputDeviceStep = null;

			DeviceStep nextStep = new NoOpStep(DeviceSettings.GetTickCount(DeviceSettings.CardWriterInitialization), initializationDescription);
            FirstOutputDeviceStep = nextStep;
			nextStep.NextStep = new ReadFromMemoryStep(false, recordWordCount);
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new openStreamStep();
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new TextWriteStep(recordWordCount);
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new CloseStreamStep();
			nextStep.NextStep.NextStep = null;

            FirstIocDeviceStep = null;
		}

		private class openStreamStep : StreamStep
		{
			public override StreamStep.Instance CreateStreamInstance(StreamStatus streamStatus) => new Instance(streamStatus);
	
			public override string StatusDescription => openingDescription;

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
						FileStream stream = new FileStream(StreamStatus.FileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
						if (!StreamStatus.PositionSet)
						{
							stream.Position = stream.Length;
						}

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
