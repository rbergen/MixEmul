using System;
using System.IO;
using MixLib.Device.Settings;
using MixLib.Device.Step;
using MixLib.Events;
using MixLib.Misc;
using MixLib.Type;

namespace MixLib.Device
{
	public class TapeDevice : FileBasedDevice
	{
        const string shortName = "TAP";
        const string fileNamePrefix = "tape";

        const string initializationDescription = "Initializing tape device";
        const string openingDescription = "Starting data transfer with tape";
        const string windingDescription = "Winding tape";

        public const int WordsPerRecord = 100;

        const int bytesPerRecord = WordsPerRecord * (FullWord.ByteCount + 1);

        public TapeDevice(int id) : base(id, fileNamePrefix)
		{
			UpdateSettings();
		}

        public override int RecordWordCount => WordsPerRecord;

        public override string ShortName => shortName;

        public override bool SupportsInput => true;

        public override bool SupportsOutput => true;

        public static long CalculateBytePosition(long record) => record * bytesPerRecord;

        public static long CalculateRecordCount(FileStream stream) => stream.Length / bytesPerRecord;

        public sealed override void UpdateSettings()
		{
			var tickCount = DeviceSettings.GetTickCount(DeviceSettings.TapeInitialization);

			DeviceStep nextStep = new NoOpStep(tickCount, initializationDescription);
            FirstInputDeviceStep = nextStep;
			nextStep.NextStep = new OpenStreamStep();
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new BinaryReadStep(WordsPerRecord);
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new CloseStreamStep();
			nextStep = nextStep.NextStep;
            nextStep.NextStep = new WriteToMemoryStep(true, WordsPerRecord)
            {
                NextStep = null
            };

            nextStep = new NoOpStep(tickCount, initializationDescription);
            FirstOutputDeviceStep = nextStep;
			nextStep.NextStep = new ReadFromMemoryStep(true, WordsPerRecord);
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new OpenStreamStep();
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new BinaryWriteStep(WordsPerRecord);
			nextStep = nextStep.NextStep;
            nextStep.NextStep = new CloseStreamStep
            {
                NextStep = null
            };

            nextStep = new NoOpStep(tickCount, initializationDescription);
            FirstIocDeviceStep = nextStep;
            nextStep.NextStep = new SeekStep
            {
                NextStep = null
            };
        }

        class OpenStreamStep : StreamStep
        {
            public override string StatusDescription => openingDescription;

            public override StreamStep.Instance CreateStreamInstance(StreamStatus streamStatus) => new Instance(streamStatus);

            new class Instance : StreamStep.Instance
            {
                public Instance(StreamStatus streamStatus) : base(streamStatus) { }

                public override bool Tick()
                {
                    try
                    {
                        var stream = OpenStream(StreamStatus.FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                        if (!StreamStatus.PositionSet)
                        {
                            stream.Position = stream.Length;
                        }
                        StreamStatus.Stream = stream;
                    }
                    catch (Exception exception)
                    {
                        OnReportingEvent(new ReportingEventArgs(Severity.Error, "Exception while opening file " + StreamStatus.FileName + ": " + exception.Message));
                    }
                    return true;
                }
            }
        }

        class SeekStep : StreamStep
        {
            public override string StatusDescription => windingDescription;

            public override StreamStep.Instance CreateStreamInstance(StreamStatus streamStatus) => new Instance(streamStatus);

            new class Instance : StreamStep.Instance
            {
                long mTicksLeft;
                const long unset = long.MinValue;

                public Instance(StreamStatus streamStatus) : base(streamStatus)
                {
                    mTicksLeft = unset;
                }

                public override bool Tick()
                {
                    if (mTicksLeft == unset)
                    {
                        if (!StreamStatus.PositionSet) InitiateStreamPosition();

                        mTicksLeft = (Operands.MValue == 0 ? (StreamStatus.Position / ((FullWord.ByteCount + 1) * WordsPerRecord)) : Math.Abs(Operands.MValue)) * DeviceSettings.GetTickCount(DeviceSettings.TapeRecordWind);
                    }

                    mTicksLeft -= 1L;

                    if (mTicksLeft > 0L) return false;

                    if (Operands.MValue == 0)
                    {
                        StreamStatus.Position = 0L;
                        return true;
                    }

                    long currentPosition = StreamStatus.Position + Operands.MValue * WordsPerRecord * (FullWord.ByteCount + 1);
                    if (currentPosition < 0L)
                    {
                        currentPosition = 0L;
                    }

                    StreamStatus.Position = currentPosition;

                    return true;
                }

                private void InitiateStreamPosition()
                {
                    try
                    {
                        var stream = File.OpenRead(StreamStatus.FileName);
                        StreamStatus.Position = stream.Length;
                        stream.Close();
                    }
                    catch (Exception exception)
                    {
                        OnReportingEvent(new ReportingEventArgs(Severity.Error, "Exception while determining length of file " + StreamStatus.FileName + ": " + exception.Message));
                    }
                }
            }
        }
    }
}
