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
        const string shortName = "CRD";
        const string fileNamePrefix = "crdin";

        const string initializationDescription = "Initializing card reader";
        const string openingDescription = "Starting read from card reader";

        const int recordWordCount = 16;
        public const int BytesPerRecord = recordWordCount * FullWord.ByteCount;

		public CardReaderDevice(int id) : base(id, fileNamePrefix)
		{
			UpdateSettings();
		}

        public override int RecordWordCount => recordWordCount;

        public override string ShortName => shortName;

        public override bool SupportsInput => true;

        public override bool SupportsOutput => false;

        public sealed override void UpdateSettings()
		{
			DeviceStep nextStep = new NoOpStep(DeviceSettings.GetTickCount(DeviceSettings.CardReaderInitialization), initializationDescription);
            FirstInputDeviceStep = nextStep;
			nextStep.NextStep = new openStreamStep();
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new TextReadStep(recordWordCount);
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new CloseStreamStep();
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new WriteToMemoryStep(false, recordWordCount);
			nextStep.NextStep.NextStep = null;

            FirstOutputDeviceStep = null;
            FirstIocDeviceStep = null;
		}

        class openStreamStep : StreamStep
        {
            public override string StatusDescription => openingDescription;

            public override StreamStep.Instance CreateStreamInstance(StreamStatus streamStatus) =>
                new Instance(streamStatus);

            new class Instance : StreamStep.Instance
            {
                public Instance(StreamStatus streamStatus) : base(streamStatus) { }

                public override bool Tick()
                {
                    try
                    {
                        var stream = new FileStream(StreamStatus.FileName, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);

                        if (!StreamStatus.PositionSet)
                        {
                            stream.Position = 0L;
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