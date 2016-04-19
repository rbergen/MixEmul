using System;
using System.IO;
using MixLib.Device.Settings;
using MixLib.Device.Step;
using MixLib.Events;
using MixLib.Misc;
using MixLib.Type;

namespace MixLib.Device
{
	public class PaperTapeDevice : FileBasedDevice
	{
		private const string shortName = "PTP";
		private const string fileNamePrefix = "ppr";

		private const string initializationDescription = "Initializing tape reader";
		private const string openingDescription = "Starting read from tape";
		private const string rewindingDescription = "Rewinding tape";

		private const int recordWordCount = 14;
		public const int BytesPerRecord = recordWordCount * FullWord.ByteCount;

		public PaperTapeDevice(int id)
			: base(id, fileNamePrefix)
		{
			UpdateSettings();
		}

        public override int RecordWordCount => recordWordCount;

        public override string ShortName => shortName;

        public override bool SupportsInput => true;

        public override bool SupportsOutput => false;

        public override void UpdateSettings()
		{
			int tickCount = DeviceSettings.GetTickCount(DeviceSettings.PaperTapeInitialization);

			DeviceStep nextStep = new NoOpStep(tickCount, initializationDescription);
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

			nextStep = new NoOpStep(tickCount, initializationDescription);
			base.FirstIocDeviceStep = nextStep;
			nextStep.NextStep = new rewindStep();
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
						base.StreamStatus.Stream = new FileStream(base.StreamStatus.FileName, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
					}
					catch (Exception exception)
					{
						OnReportingEvent(new ReportingEventArgs(Severity.Error, "exception while opening file " + base.StreamStatus.FileName + ": " + exception.Message));
					}

					return true;
				}
			}
		}

		private class rewindStep : StreamStep
		{
            public override string StatusDescription => rewindingDescription;

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
						mTicksLeft = ((base.StreamStatus.Position / (recordWordCount * FullWord.ByteCount + Environment.NewLine.Length)) + 1L) * DeviceSettings.GetTickCount("PaperTapeRecordWind");
					}

					mTicksLeft -= 1L;
					if (mTicksLeft > 0L)
					{
						return false;
					}

					base.StreamStatus.Position = 0L;

					return true;
				}
			}
		}
	}
}
