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
		private const string MyShortName = "TAP";
		private const string FileNamePrefix = "tape";
		private const string InitializationDescription = "Initializing tape device";
		private const string OpeningDescription = "Starting data transfer with tape";
		private const string WindingDescription = "Winding tape";

		public const int WordsPerRecord = 100;
		private const int BytesPerRecord = WordsPerRecord * (FullWord.ByteCount + 1);

		public TapeDevice(int id) : base(id, FileNamePrefix) 
			=> UpdateSettings();

		public override int RecordWordCount 
			=> WordsPerRecord;

		public override string ShortName 
			=> MyShortName;

		public override bool SupportsInput 
			=> true;

		public override bool SupportsOutput 
			=> true;

		public static long CalculateBytePosition(long record) 
			=> record * BytesPerRecord;

		public static long CalculateRecordCount(FileStream stream) 
			=> stream.Length / BytesPerRecord;

		public sealed override void UpdateSettings()
		{
			var tickCount = DeviceSettings.GetTickCount(DeviceSettings.TapeInitialization);

			DeviceStep nextStep = new NoOpStep(tickCount, InitializationDescription);
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

			nextStep = new NoOpStep(tickCount, InitializationDescription);
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

			nextStep = new NoOpStep(tickCount, InitializationDescription);
			FirstIocDeviceStep = nextStep;
			nextStep.NextStep = new SeekStep
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
						var stream = OpenStream(StreamStatus.FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

						if (!StreamStatus.PositionSet)
							stream.Position = stream.Length;

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

		private class SeekStep : StreamStep
		{
			public override string StatusDescription 
				=> WindingDescription;

			public override StreamStep.Instance CreateStreamInstance(StreamStatus streamStatus) 
				=> new Instance(streamStatus);

			private new class Instance(StreamStatus streamStatus) : StreamStep.Instance(streamStatus)
			{
				private long ticksLeft = Unset;
				private const long Unset = long.MinValue;

				public override bool Tick()
				{
					if (this.ticksLeft == Unset)
					{
						if (!StreamStatus.PositionSet)
							InitiateStreamPosition();

						this.ticksLeft = (Operands.MValue == 0 
							? (StreamStatus.Position / ((FullWord.ByteCount + 1) * WordsPerRecord)) 
							: Math.Abs(Operands.MValue)) * DeviceSettings.GetTickCount(DeviceSettings.TapeRecordWind);
					}

					this.ticksLeft -= 1L;

					if (this.ticksLeft > 0L)
						return false;

					if (Operands.MValue == 0)
					{
						StreamStatus.Position = 0L;
						return true;
					}

					long currentPosition = StreamStatus.Position + Operands.MValue * WordsPerRecord * (FullWord.ByteCount + 1);

					if (currentPosition < 0L)
						currentPosition = 0L;

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
