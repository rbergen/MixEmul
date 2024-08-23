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
		private const string MyShortName = "PTP";
		private const string FileNamePrefix = "ppr";
		private const string InitializationDescription = "Initializing tape reader";
		private const string OpeningDescription = "Starting read from tape";
		private const string RewindingDescription = "Rewinding tape";
		private const int MyRecordWordCount = 14;
		public const int BytesPerRecord = MyRecordWordCount * FullWord.ByteCount;

		public PaperTapeDevice(int id) : base(id, FileNamePrefix) 
			=> UpdateSettings();

		public override int RecordWordCount 
			=> MyRecordWordCount;

		public override string ShortName 
			=> MyShortName;

		public override bool SupportsInput 
			=> true;

		public override bool SupportsOutput 
			=> false;

		public sealed override void UpdateSettings()
		{
			var tickCount = DeviceSettings.GetTickCount(DeviceSettings.PaperTapeInitialization);

			DeviceStep nextStep = new NoOpStep(tickCount, InitializationDescription);
			FirstInputDeviceStep = nextStep;
			nextStep.NextStep = new OpenStreamStep();
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new TextReadStep(MyRecordWordCount);
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new CloseStreamStep();
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new WriteToMemoryStep(false, MyRecordWordCount)
			{
				NextStep = null
			};

			FirstOutputDeviceStep = null;

			nextStep = new NoOpStep(tickCount, InitializationDescription);
			FirstIocDeviceStep = nextStep;
			nextStep.NextStep = new RewindStep
			{
				NextStep = null
			};
		}

		private class OpenStreamStep : StreamStep
		{
			public override string StatusDescription => OpeningDescription;

			public override StreamStep.Instance CreateStreamInstance(StreamStatus streamStatus) => new Instance(streamStatus);

			private new class Instance(StreamStatus streamStatus) : StreamStep.Instance(streamStatus)
			{
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

		private class RewindStep : StreamStep
		{
			public override string StatusDescription 
				=> RewindingDescription;

			public override StreamStep.Instance CreateStreamInstance(StreamStatus streamStatus) 
				=> new Instance(streamStatus);

			private new class Instance(StreamStatus streamStatus) : StreamStep.Instance(streamStatus)
			{
				private long ticksLeft = Unset;
				private const long Unset = long.MinValue;

				public override bool Tick()
				{
					if (this.ticksLeft == Unset)
						this.ticksLeft = ((StreamStatus.Position / (MyRecordWordCount * FullWord.ByteCount + Environment.NewLine.Length)) + 1L) * DeviceSettings.GetTickCount("PaperTapeRecordWind");

					this.ticksLeft -= 1L;
					if (this.ticksLeft > 0L)
						return false;

					StreamStatus.Position = 0L;

					return true;
				}
			}
		}
	}
}
