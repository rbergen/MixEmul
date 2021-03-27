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
		private const string MyShortName = "DSK";
		private const string FileNamePrefix = "disk";
		private const string InitializationDescription = "Initializing disk";
		private const string OpeningDescription = "Starting data transfer with disk";
		private const string SeekingDescription = "Seeking sector";

		public const long SectorCount = 4096;
		public const int WordsPerSector = 100;

		public DiskDevice(int id) : base(id, FileNamePrefix) 
			=> UpdateSettings();

		public override int RecordWordCount 
			=> WordsPerSector;

		public override string ShortName 
			=> MyShortName;

		public override bool SupportsInput 
			=> true;

		public override bool SupportsOutput 
			=> true;

		public static long CalculateBytePosition(long sector) 
			=> sector * WordsPerSector * (FullWord.ByteCount + 1);

		public sealed override void UpdateSettings()
		{
			var tickCount = DeviceSettings.GetTickCount(DeviceSettings.DiskInitialization);

			DeviceStep nextStep = new NoOpStep(tickCount, InitializationDescription);
			FirstInputDeviceStep = nextStep;
			nextStep.NextStep = new OpenStreamStep();
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new SeekStep();
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new BinaryReadStep(WordsPerSector);
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new CloseStreamStep();
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new WriteToMemoryStep(true, WordsPerSector)
			{
				NextStep = null
			};

			nextStep = new NoOpStep(tickCount, InitializationDescription);
			FirstOutputDeviceStep = nextStep;
			nextStep.NextStep = new ReadFromMemoryStep(true, WordsPerSector);
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new OpenStreamStep();
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new SeekStep();
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new BinaryWriteStep(WordsPerSector);
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

		public static new FileStream OpenStream(string fileName, FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
		{
			var stream = FileBasedDevice.OpenStream(fileName, fileMode, fileAccess, fileShare);
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

		private class OpenStreamStep : StreamStep
		{
			public override string StatusDescription 
				=> OpeningDescription;

			public override StreamStep.Instance CreateStreamInstance(StreamStatus streamStatus) 
				=> new Instance(streamStatus);

			private new class Instance : StreamStep.Instance
			{
				public Instance(StreamStatus streamStatus) : base(streamStatus) { }

				public override bool Tick()
				{
					try
					{
						var stream = OpenStream(StreamStatus.FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

						if (!StreamStatus.PositionSet)
							stream.Position = 0L;

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

		private class SeekStep : StreamStep
		{
			public override string StatusDescription => SeekingDescription;

			public override StreamStep.Instance CreateStreamInstance(StreamStatus streamStatus) => new Instance(streamStatus);

			private new class Instance : StreamStep.Instance
			{
				private long _ticksLeft;
				private const long Unset = long.MinValue;

				public Instance(StreamStatus streamStatus) : base(streamStatus) => _ticksLeft = Unset;

				public override bool Tick()
				{
					var desiredPosition = CalculateBytePosition(Operands.Sector);

					if (desiredPosition == StreamStatus.Position)
						return true;

					if (_ticksLeft == Unset)
						_ticksLeft = DeviceSettings.GetTickCount(DeviceSettings.DiskSectorSeek);

					_ticksLeft -= 1L;

					if (_ticksLeft > 0L)
						return false;

					if (desiredPosition < 0L)
						desiredPosition = 0L;

					else if (Operands.Sector >= SectorCount)
						desiredPosition = (SectorCount - 1) * WordsPerSector * (FullWord.ByteCount + 1);

					StreamStatus.Position = desiredPosition;

					return true;
				}
			}
		}
	}
}
