using System;
using System.IO;
using MixLib.Device.Settings;
using MixLib.Device.Step;
using MixLib.Events;
using MixLib.Misc;
using MixLib.Type;

namespace MixLib.Device
{
	public class DiskDevice : FileBasedDevice
	{
        const string shortName = "DSK";
        const string fileNamePrefix = "disk";

        const string initializationDescription = "Initializing disk";
        const string openingDescription = "Starting data transfer with disk";
        const string seekingDescription = "Seeking sector";

        public const long SectorCount = 4096;
		public const int WordsPerSector = 100;

		public DiskDevice(int id) : base(id, fileNamePrefix)
		{
			UpdateSettings();
		}

        public override int RecordWordCount => WordsPerSector;

        public override string ShortName => shortName;

        public override bool SupportsInput => true;

        public override bool SupportsOutput => true;

        public static long CalculateBytePosition(long sector) => sector * WordsPerSector * (FullWord.ByteCount + 1);

        public sealed override void UpdateSettings()
		{
			int tickCount = DeviceSettings.GetTickCount(DeviceSettings.DiskInitialization);

			DeviceStep nextStep = new NoOpStep(tickCount, initializationDescription);
            FirstInputDeviceStep = nextStep;
			nextStep.NextStep = new openStreamStep();
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new seekStep();
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new BinaryReadStep(WordsPerSector);
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new CloseStreamStep();
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new WriteToMemoryStep(true, WordsPerSector);
			nextStep.NextStep.NextStep = null;

			nextStep = new NoOpStep(tickCount, initializationDescription);
            FirstOutputDeviceStep = nextStep;
			nextStep.NextStep = new ReadFromMemoryStep(true, WordsPerSector);
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new openStreamStep();
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new seekStep();
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new BinaryWriteStep(WordsPerSector);
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new CloseStreamStep();
			nextStep.NextStep.NextStep = null;

			nextStep = new NoOpStep(tickCount, initializationDescription);
            FirstIocDeviceStep = nextStep;
			nextStep.NextStep = new seekStep();
			nextStep.NextStep.NextStep = null;
		}

		public new static FileStream OpenStream(string fileName, FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
		{
			FileStream stream = FileBasedDevice.OpenStream(fileName, fileMode, fileAccess, fileShare);
			long byteCount = SectorCount * WordsPerSector * (FullWord.ByteCount + 1);

			var bytesToWrite = (int)(byteCount - stream.Length);
			if (bytesToWrite > 0)
			{
				stream.Position = stream.Length;
				stream.Write(new byte[bytesToWrite], 0, bytesToWrite);
				stream.Flush();
			}

			return stream;
		}

        class openStreamStep : StreamStep
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
                        FileStream stream = DiskDevice.OpenStream(StreamStatus.FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

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

        class seekStep : StreamStep
        {
            public override string StatusDescription => seekingDescription;

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
                    long desiredPosition = CalculateBytePosition(Operands.Sector);
                    if (desiredPosition == StreamStatus.Position) return true;

                    if (mTicksLeft == unset)
                    {
                        mTicksLeft = DeviceSettings.GetTickCount(DeviceSettings.DiskSectorSeek);
                    }

                    mTicksLeft -= 1L;

                    if (mTicksLeft > 0L) return false;

                    if (desiredPosition < 0L)
                    {
                        desiredPosition = 0L;
                    }
                    else if (Operands.Sector >= SectorCount)
                    {
                        desiredPosition = (SectorCount - 1) * WordsPerSector * (FullWord.ByteCount + 1);
                    }

                    StreamStatus.Position = desiredPosition;

                    return true;
                }
            }
        }
    }
}
