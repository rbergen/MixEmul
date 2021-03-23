using MixLib.Device.Settings;
using MixLib.Device.Step;
using MixLib.Events;
using MixLib.Misc;
using MixLib.Type;
using System;
using System.IO;

namespace MixLib.Device
{
	public class DiskDevice : FileBasedDevice
	{
		const string shortName = "DSK";
		const string fileNamePrefix = "disk";

		const string initializationDescription = "Initializing disk";
		const string openingDescription = "Starting data transfer with disk";
		const string seekingDescription = "Seeking sector";

		public const long SectorCount = 4096;
		public const int WordsPerSector = 100;

		public DiskDevice(int id) : base(id, fileNamePrefix)
		{
			UpdateSettings();
		}

		public override int RecordWordCount => WordsPerSector;

		public override string ShortName => shortName;

		public override bool SupportsInput => true;

		public override bool SupportsOutput => true;

		public static long CalculateBytePosition(long sector)
		{
			return sector * WordsPerSector * (FullWord.ByteCount + 1);
		}

		public sealed override void UpdateSettings()
		{
			var tickCount = DeviceSettings.GetTickCount(DeviceSettings.DiskInitialization);

			DeviceStep nextStep = new NoOpStep(tickCount, initializationDescription);
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

			nextStep = new NoOpStep(tickCount, initializationDescription);
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

			nextStep = new NoOpStep(tickCount, initializationDescription);
			FirstIocDeviceStep = nextStep;
			nextStep.NextStep = new SeekStep
			{
				NextStep = null
			};
		}

		public new static FileStream OpenStream(string fileName, FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
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
						var stream = DiskDevice.OpenStream(StreamStatus.FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

						if (!StreamStatus.PositionSet)
						{
							stream.Position = 0L;
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

		class SeekStep : StreamStep
		{
			public override string StatusDescription => seekingDescription;

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
					var desiredPosition = CalculateBytePosition(Operands.Sector);
					if (desiredPosition == StreamStatus.Position)
					{
						return true;
					}

					if (mTicksLeft == unset)
					{
						mTicksLeft = DeviceSettings.GetTickCount(DeviceSettings.DiskSectorSeek);
					}

					mTicksLeft -= 1L;

					if (mTicksLeft > 0L)
					{
						return false;
					}

					if (desiredPosition < 0L)
					{
						desiredPosition = 0L;
					}
					else if (Operands.Sector >= SectorCount)
					{
						desiredPosition = (SectorCount - 1) * WordsPerSector * (FullWord.ByteCount + 1);
					}

					StreamStatus.Position = desiredPosition;

					return true;
				}
			}
		}
	}
}
