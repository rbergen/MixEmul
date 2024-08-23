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
		private const string MyShortName = "PRN";
		private const string FileNamePrefix = "prn";
		private const string InitializationDescription = "Initializing printer";
		private const string OpeningDescription = "Starting write to printer";
		private const string NextPageDescription = "Skipping to next page";
		private const int MyRecordWordCount = 24;
		public const int BytesPerRecord = MyRecordWordCount * FullWord.ByteCount;


		public PrinterDevice(int id)
			: base(id, FileNamePrefix) 
			=> UpdateSettings();

		public override int RecordWordCount 
			=> MyRecordWordCount;

		public override string ShortName 
			=> MyShortName;

		public override bool SupportsInput 
			=> false;

		public override bool SupportsOutput 
			=> true;

		public sealed override void UpdateSettings()
		{
			var tickCount = DeviceSettings.GetTickCount(DeviceSettings.PrinterInitialization);

			FirstInputDeviceStep = null;

			DeviceStep nextStep = new NoOpStep(tickCount, InitializationDescription);
			FirstOutputDeviceStep = nextStep;
			nextStep.NextStep = new ReadFromMemoryStep(false, MyRecordWordCount);
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new OpenStreamStep();
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new TextWriteStep(MyRecordWordCount);
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new CloseStreamStep
			{
				NextStep = null
			};

			nextStep = new NoOpStep(tickCount, InitializationDescription);
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

		private class OpenStreamStep : StreamStep
		{
			public override string StatusDescription 
				=> OpeningDescription;

			public override StreamStep.Instance CreateStreamInstance(StreamStatus streamStatus) 
				=> new Instance(streamStatus);

			private new class Instance(StreamStatus streamStatus) : StreamStep.Instance(streamStatus)
			{
				public override bool Tick()
				{
					try
					{
						var stream = new FileStream(StreamStatus.FileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);

						if (!StreamStatus.PositionSet)
							stream.Position = stream.Length;

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

		private class PageForwardStep : StreamStep
		{
			public override string StatusDescription => NextPageDescription;

			public override StreamStep.Instance CreateStreamInstance(StreamStatus streamStatus) => new Instance(streamStatus);

			private new class Instance(StreamStatus streamStatus) : StreamStep.Instance(streamStatus)
			{
				public override bool Tick()
				{
					if (StreamStatus.Stream == null)
						return true;

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
