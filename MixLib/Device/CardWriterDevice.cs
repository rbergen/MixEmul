using System;
using System.IO;
using MixLib.Device.Settings;
using MixLib.Device.Step;
using MixLib.Events;
using MixLib.Misc;
using MixLib.Type;

namespace MixLib.Device
{
	public class CardWriterDevice : FileBasedDevice
	{
		private const string shortName = "CWR";
		private const string fileNamePrefix = "crdout";

		private const string initializationDescription = "Initializing card punch";
		private const string openingDescription = "Starting write to card punch";

		private const int recordWordCount = 16;
		public const int BytesPerRecord = recordWordCount * FullWord.ByteCount;

		public CardWriterDevice(int id)
			: base(id, fileNamePrefix)
		{
			UpdateSettings();
		}

		public override void UpdateSettings()
		{
			base.FirstInputDeviceStep = null;

			DeviceStep nextStep = new NoOpStep(DeviceSettings.GetTickCount(DeviceSettings.CardWriterInitialization), initializationDescription);
			base.FirstOutputDeviceStep = nextStep;
			nextStep.NextStep = new MixDevice.ReadFromMemoryStep(false, recordWordCount);
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new openStreamStep();
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new TextWriteStep(recordWordCount);
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new CloseStreamStep();
			nextStep.NextStep.NextStep = null;

			base.FirstIocDeviceStep = null;
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
	}
}
