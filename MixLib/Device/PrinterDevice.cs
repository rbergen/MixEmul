using MixLib.Device.Settings;
using MixLib.Device.Step;
using MixLib.Events;
using MixLib.Misc;
using MixLib.Type;
using System;
using System.IO;
using System.Text;

namespace MixLib.Device
{
	public class PrinterDevice : FileBasedDevice
	{
		const string shortName = "PRN";
		const string fileNamePrefix = "prn";

		const string initializationDescription = "Initializing printer";
		const string openingDescription = "Starting write to printer";
		const string nextPageDescription = "Skipping to next page";

		const int recordWordCount = 24;
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

		public sealed override void UpdateSettings()
		{
			var tickCount = DeviceSettings.GetTickCount(DeviceSettings.PrinterInitialization);

			FirstInputDeviceStep = null;

			DeviceStep nextStep = new NoOpStep(tickCount, initializationDescription);
			FirstOutputDeviceStep = nextStep;
			nextStep.NextStep = new ReadFromMemoryStep(false, recordWordCount);
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new OpenStreamStep();
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new TextWriteStep(recordWordCount);
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new CloseStreamStep
			{
				NextStep = null
			};

			nextStep = new NoOpStep(tickCount, initializationDescription);
			FirstIocDeviceStep = nextStep;
			nextStep.NextStep = new OpenStreamStep();
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new PageForwardStep();
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new CloseStreamStep
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
						var stream = new FileStream(StreamStatus.FileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);

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

		class PageForwardStep : StreamStep
		{
			public override string StatusDescription => nextPageDescription;

			public override StreamStep.Instance CreateStreamInstance(StreamStatus streamStatus)
			{
				return new Instance(streamStatus);
			}

			new class Instance : StreamStep.Instance
			{
				public Instance(StreamStatus streamStatus) : base(streamStatus) { }

				public override bool Tick()
				{
					if (StreamStatus.Stream == null)
					{
						return true;
					}

					try
					{
						var writer = new StreamWriter(StreamStatus.Stream, Encoding.ASCII);
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

					return true;
				}
			}
		}
	}
}
