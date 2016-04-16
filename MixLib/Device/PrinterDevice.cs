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

		public override void UpdateSettings()
		{
			int tickCount = DeviceSettings.GetTickCount(DeviceSettings.PrinterInitialization);

			base.FirstInputDeviceStep = null;

			DeviceStep nextStep = new NoOpStep(tickCount, initializationDescription);
			base.FirstOutputDeviceStep = nextStep;
			nextStep.NextStep = new MixDevice.ReadFromMemoryStep(false, recordWordCount);
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new openStreamStep();
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new TextWriteStep(recordWordCount);
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new CloseStreamStep();
			nextStep.NextStep.NextStep = null;

			nextStep = new NoOpStep(tickCount, initializationDescription);
			base.FirstIocDeviceStep = nextStep;
			nextStep.NextStep = new openStreamStep();
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new pageForwardStep();
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new CloseStreamStep();
			nextStep.NextStep.NextStep = null;
		}

		public override int RecordWordCount
		{
			get
			{
				return recordWordCount;
			}
		}

		public override string ShortName
		{
			get
			{
				return shortName;
			}
		}

		public override bool SupportsInput
		{
			get
			{
				return false;
			}
		}

		public override bool SupportsOutput
		{
			get
			{
				return true;
			}
		}

		private class openStreamStep : StreamStep
		{
			public override StreamStep.Instance CreateStreamInstance(StreamStatus streamStatus)
			{
				return new Instance(streamStatus);
			}

			public override string StatusDescription
			{
				get
				{
					return openingDescription;
				}
			}

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
						FileStream stream = new FileStream(base.StreamStatus.FileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);

						if (!base.StreamStatus.PositionSet)
						{
							stream.Position = stream.Length;
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

		private class pageForwardStep : StreamStep
		{
			public override StreamStep.Instance CreateStreamInstance(StreamStatus streamStatus)
			{
				return new Instance(streamStatus);
			}

			public override string StatusDescription
			{
				get
				{
					return nextPageDescription;
				}
			}

			private new class Instance : StreamStep.Instance
			{
				public Instance(StreamStatus streamStatus)
					: base(streamStatus)
				{
				}

				public override bool Tick()
				{
					if (base.StreamStatus.Stream != null)
					{
						try
						{
							StreamWriter writer = new StreamWriter(base.StreamStatus.Stream, Encoding.ASCII);
							writer.WriteLine();
							writer.WriteLine("===================== PAGE BREAK ===================== PAGE BREAK ===================== PAGE BREAK =====================");
							writer.WriteLine();
							writer.Flush();
							base.StreamStatus.UpdatePosition();
						}
						catch (Exception exception)
						{
							OnReportingEvent(new ReportingEventArgs(Severity.Error, "exception while writing file " + base.StreamStatus.FileName + ": " + exception.Message));
						}
					}

					return true;
				}
			}
		}
	}
}
