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
		private const string shortName = "TAP";
		private const string fileNamePrefix = "tape";

		private const string initializationDescription = "Initializing tape device";
		private const string openingDescription = "Starting data transfer with tape";
		private const string windingDescription = "Winding tape";

		public const int WordsPerRecord = 100;

		private const int bytesPerRecord = WordsPerRecord * (FullWord.ByteCount + 1);

		public TapeDevice(int id)
			: base(id, fileNamePrefix)
		{
			UpdateSettings();
		}

        public override int RecordWordCount => WordsPerRecord;

        public override string ShortName => shortName;

        public override bool SupportsInput => true;

        public override bool SupportsOutput => true;

        public static long CalculateBytePosition(long record) => record * bytesPerRecord;

        public static long CalculateRecordCount(FileStream stream) => stream.Length / bytesPerRecord;

        public override void UpdateSettings()
		{
			int tickCount = DeviceSettings.GetTickCount(DeviceSettings.TapeInitialization);

			DeviceStep nextStep = new NoOpStep(tickCount, initializationDescription);
			base.FirstInputDeviceStep = nextStep;
			nextStep.NextStep = new openStreamStep();
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new BinaryReadStep(WordsPerRecord);
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new CloseStreamStep();
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new MixDevice.WriteToMemoryStep(true, WordsPerRecord);
			nextStep.NextStep.NextStep = null;

			nextStep = new NoOpStep(tickCount, initializationDescription);
			base.FirstOutputDeviceStep = nextStep;
			nextStep.NextStep = new MixDevice.ReadFromMemoryStep(true, WordsPerRecord);
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new openStreamStep();
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new BinaryWriteStep(WordsPerRecord);
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new CloseStreamStep();
			nextStep.NextStep.NextStep = null;

			nextStep = new NoOpStep(tickCount, initializationDescription);
			base.FirstIocDeviceStep = nextStep;
			nextStep.NextStep = new seekStep();
			nextStep.NextStep.NextStep = null;
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
						FileStream stream = OpenStream(base.StreamStatus.FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
						if (!base.StreamStatus.PositionSet)
						{
							stream.Position = stream.Length;
						}
						base.StreamStatus.Stream = stream;
					}
					catch (Exception exception)
					{
						OnReportingEvent(new ReportingEventArgs(Severity.Error, "Exception while opening file " + base.StreamStatus.FileName + ": " + exception.Message));
					}
					return true;
				}
			}
		}

		private class seekStep : StreamStep
		{
            public override string StatusDescription => windingDescription;

            public override StreamStep.Instance CreateStreamInstance(StreamStatus streamStatus) => new Instance(streamStatus);

			private new class Instance : StreamStep.Instance
			{
				private long mTicksLeft;
				private const long unset = long.MinValue;

				public Instance(StreamStatus streamStatus)
					: base(streamStatus)
				{
					mTicksLeft = unset;
				}

				public override bool Tick()
				{
					if (mTicksLeft == unset)
					{
						if (!base.StreamStatus.PositionSet)
						{
							try
							{
								FileStream stream = File.OpenRead(base.StreamStatus.FileName);
								base.StreamStatus.Position = stream.Length;
								stream.Close();
							}
							catch (Exception exception)
							{
								OnReportingEvent(new ReportingEventArgs(Severity.Error, "Exception while determining length of file " + base.StreamStatus.FileName + ": " + exception.Message));
							}
						}

						mTicksLeft = (base.Operands.MValue == 0 ? (base.StreamStatus.Position / ((FullWord.ByteCount + 1) * WordsPerRecord)) : Math.Abs(base.Operands.MValue)) * DeviceSettings.GetTickCount(DeviceSettings.TapeRecordWind);
					}

					mTicksLeft -= 1L;
					if (mTicksLeft > 0L)
					{
						return false;
					}

					if (base.Operands.MValue == 0)
					{
						base.StreamStatus.Position = 0L;
						return true;
					}

					long currentPosition = base.StreamStatus.Position + base.Operands.MValue * WordsPerRecord * (FullWord.ByteCount + 1);
					if (currentPosition < 0L)
					{
						currentPosition = 0L;
					}

					base.StreamStatus.Position = currentPosition;

					return true;
				}
			}
		}
	}
}
