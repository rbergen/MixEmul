using System;
using System.IO;
using System.Text;
using MixLib.Device.Settings;
using MixLib.Device.Step;
using MixLib.Events;
using MixLib.Misc;
using MixLib.Type;

namespace MixLib.Device
{
	public class PrinterDevice : FileBasedDevice
	{
		private const string shortName = "PRN";
		private const string fileNamePrefix = "prn";

		private const string initializationDescription = "Initializing printer";
		private const string openingDescription = "Starting write to printer";
		private const string nextPageDescription = "Skipping to next page";

		private const int recordWordCount = 24;
		public const int BytesPerRecord = recordWordCount * FullWord.ByteCount;


		public PrinterDevice(int id)
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
			int tickCount = DeviceSettings.GetTickCount(DeviceSettings.PrinterInitialization);

            FirstInputDeviceStep = null;

			DeviceStep nextStep = new NoOpStep(tickCount, initializationDescription);
            FirstOutputDeviceStep = nextStep;
			nextStep.NextStep = new ReadFromMemoryStep(false, recordWordCount);
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new openStreamStep();
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new TextWriteStep(recordWordCount);
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new CloseStreamStep();
			nextStep.NextStep.NextStep = null;

			nextStep = new NoOpStep(tickCount, initializationDescription);
            FirstIocDeviceStep = nextStep;
			nextStep.NextStep = new openStreamStep();
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new pageForwardStep();
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new CloseStreamStep();
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

		private class pageForwardStep : StreamStep
		{
            public override string StatusDescription => nextPageDescription;

            public override StreamStep.Instance CreateStreamInstance(StreamStatus streamStatus) => new Instance(streamStatus);

			private new class Instance : StreamStep.Instance
			{
				public Instance(StreamStatus streamStatus)
					: base(streamStatus)
				{
				}

				public override bool Tick()
				{
					if (StreamStatus.Stream != null)
					{
						try
						{
							StreamWriter writer = new StreamWriter(StreamStatus.Stream, Encoding.ASCII);
							writer.WriteLine();
							writer.WriteLine("===================== PAGE BREAK ===================== PAGE BREAK ===================== PAGE BREAK =====================");
							writer.WriteLine();
							writer.Flush();
                            StreamStatus.UpdatePosition();
						}
						catch (Exception exception)
						{
							OnReportingEvent(new ReportingEventArgs(Severity.Error, "exception while writing file " + StreamStatus.FileName + ": " + exception.Message));
						}
					}

					return true;
				}
			}
		}
	}
}
