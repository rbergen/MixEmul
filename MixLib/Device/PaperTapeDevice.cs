using MixLib.Device.Settings;
using MixLib.Device.Step;
using MixLib.Events;
using MixLib.Misc;
using MixLib.Type;
using System;
using System.IO;

namespace MixLib.Device
{
	public class PaperTapeDevice : FileBasedDevice
	{
		const string shortName = "PTP";
		const string fileNamePrefix = "ppr";

		const string initializationDescription = "Initializing tape reader";
		const string openingDescription = "Starting read from tape";
		const string rewindingDescription = "Rewinding tape";

		const int recordWordCount = 14;
		public const int BytesPerRecord = recordWordCount * FullWord.ByteCount;

		public PaperTapeDevice(int id) : base(id, fileNamePrefix)
		{
			UpdateSettings();
		}

		public override int RecordWordCount => recordWordCount;

		public override string ShortName => shortName;

		public override bool SupportsInput => true;

		public override bool SupportsOutput => false;

		public sealed override void UpdateSettings()
		{
			var tickCount = DeviceSettings.GetTickCount(DeviceSettings.PaperTapeInitialization);

			DeviceStep nextStep = new NoOpStep(tickCount, initializationDescription);
			FirstInputDeviceStep = nextStep;
			nextStep.NextStep = new OpenStreamStep();
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new TextReadStep(recordWordCount);
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new CloseStreamStep();
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new WriteToMemoryStep(false, recordWordCount)
			{
				NextStep = null
			};

			FirstOutputDeviceStep = null;

			nextStep = new NoOpStep(tickCount, initializationDescription);
			FirstIocDeviceStep = nextStep;
			nextStep.NextStep = new RewindStep
			{
				NextStep = null
			};
		}

		class OpenStreamStep : StreamStep
		{
			public override string StatusDescription => openingDescription;

			public override StreamStep.Instance CreateStreamInstance(StreamStatus streamStatus)
			{
				return new Instance(streamStatus);
			}

			new class Instance : StreamStep.Instance
			{
				public Instance(StreamStatus streamStatus) : base(streamStatus) { }

				public override bool Tick()
				{
					try
					{
						StreamStatus.Stream = new FileStream(StreamStatus.FileName, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
					}
					catch (Exception exception)
					{
						OnReportingEvent(new ReportingEventArgs(Severity.Error, "exception while opening file " + StreamStatus.FileName + ": " + exception.Message));
					}

					return true;
				}
			}
		}

		class RewindStep : StreamStep
		{
			public override string StatusDescription => rewindingDescription;

			public override StreamStep.Instance CreateStreamInstance(StreamStatus streamStatus)
			{
				return new Instance(streamStatus);
			}

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
						mTicksLeft = ((StreamStatus.Position / (recordWordCount * FullWord.ByteCount + Environment.NewLine.Length)) + 1L) * DeviceSettings.GetTickCount("PaperTapeRecordWind");
					}

					mTicksLeft -= 1L;
					if (mTicksLeft > 0L)
					{
						return false;
					}

					StreamStatus.Position = 0L;

					return true;
				}
			}
		}
	}
}
