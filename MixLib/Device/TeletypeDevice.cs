using MixLib.Device.Settings;
using MixLib.Device.Step;
using MixLib.Type;
using System;
using System.Collections;

namespace MixLib.Device
{
	public class TeletypeDevice : MixDevice
	{
		const string shortName = "TTY";

		const string initializationDescription = "Initializing teletype";
		const string readingDescription = "Reading line from teletype";
		const string writingDescription = "Writing line to teletype";

		const int recordWordCount = 14;

		Queue mInputBuffer;
		Queue mOutputBuffer;

		public event EventHandler InputRequired;

		public event EventHandler OutputAdded;

		public TeletypeDevice(int id) : base(id)
		{
			UpdateSettings();
		}

		public override int RecordWordCount => recordWordCount;

		public override string ShortName => shortName;

		public override bool SupportsInput => true;

		public override bool SupportsOutput => true;

		public void AddInputLine(string line) => mInputBuffer.Enqueue(line);

		public string GetOutputLine() => mOutputBuffer.Count <= 0 ? null : (string)mOutputBuffer.Dequeue();

		void This_InputRequired(object sender, EventArgs e) => InputRequired?.Invoke(this, e);

		void This_OutputAdded(object sender, EventArgs e) => OutputAdded?.Invoke(this, e);

		protected override DeviceStep.Instance GetCurrentStepInstance()
		{
			if (CurrentStep is ReadLineStep)
			{
				var instance = (ReadLineStep.Instance)((ReadLineStep)CurrentStep).CreateInstance();
				instance.InputRequired += This_InputRequired;

				return instance;
			}

			if (CurrentStep is WriteLineStep)
			{
				var instance = (WriteLineStep.Instance)((WriteLineStep)CurrentStep).CreateInstance();
				instance.OutputAdded += This_OutputAdded;

				return instance;
			}

			return base.GetCurrentStepInstance();
		}

		public sealed override void UpdateSettings()
		{
			var tickCount = DeviceSettings.GetTickCount(DeviceSettings.TeletypeInitialization);

			mInputBuffer = Queue.Synchronized(new Queue());
			mOutputBuffer = Queue.Synchronized(new Queue());

			DeviceStep nextStep = new NoOpStep(tickCount, initializationDescription);
			FirstInputDeviceStep = nextStep;
			nextStep.NextStep = new ReadLineStep(mInputBuffer);
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new WriteToMemoryStep(false, recordWordCount)
			{
				NextStep = null
			};

			nextStep = new NoOpStep(tickCount, initializationDescription);
			FirstOutputDeviceStep = nextStep;
			nextStep.NextStep = new ReadFromMemoryStep(false, recordWordCount);
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new WriteLineStep(mOutputBuffer)
			{
				NextStep = null
			};

			FirstIocDeviceStep = null;
		}

		class ReadLineStep : DeviceStep
		{
			Queue mInputBuffer;

			public ReadLineStep(Queue inputBuffer)
			{
				mInputBuffer = inputBuffer;
			}

			public override string StatusDescription => readingDescription;

			public override DeviceStep.Instance CreateInstance() => new Instance(mInputBuffer);

			public new class Instance : DeviceStep.Instance
			{
				readonly Queue mInputBuffer;
				MixByte[] mReadBytes;

				public event EventHandler InputRequired;

				public Instance(Queue inputBuffer)
				{
					mInputBuffer = inputBuffer;
				}

				public override object OutputForNextStep => mReadBytes;

				public override bool Tick()
				{
					if (mInputBuffer.Count == 0)
					{
						InputRequired(this, new EventArgs());
						return false;
					}

					string stringToRead = (mInputBuffer.Count == 0) ? "" : ((string)mInputBuffer.Dequeue());
					mReadBytes = new MixByte[FullWord.ByteCount * recordWordCount];

					var bytesToReadCount = Math.Min(stringToRead.Length, mReadBytes.Length);
					int index = 0;

					while (index < bytesToReadCount)
					{
						mReadBytes[index] = stringToRead[index];
						index++;
					}

					while (index < mReadBytes.Length)
					{
						mReadBytes[index] = 0;
						index++;
					}

					return true;
				}
			}
		}

		class WriteLineStep : DeviceStep
		{
			Queue mOutputBuffer;

			public WriteLineStep(Queue outputBuffer)
			{
				mOutputBuffer = outputBuffer;
			}

			public override string StatusDescription => writingDescription;

			public override DeviceStep.Instance CreateInstance() => new Instance(mOutputBuffer);

			public new class Instance : DeviceStep.Instance
			{
				Queue mOutputBuffer;
				MixByte[] mWriteBytes;

				public event EventHandler OutputAdded;

				public Instance(Queue outputBuffer)
				{
					mOutputBuffer = outputBuffer;
				}

				public override bool Tick()
				{
					if (mWriteBytes == null) return true;

					char[] charsToWrite = new char[recordWordCount * FullWord.ByteCount];
					var bytesToWriteCount = Math.Min(charsToWrite.Length, mWriteBytes.Length);
					int index = 0;

					while (index < bytesToWriteCount)
					{
						charsToWrite[index] = mWriteBytes[index];
						index++;
					}

					while (index < charsToWrite.Length)
					{
						charsToWrite[index] = ' ';
						index++;
					}

					mOutputBuffer.Enqueue(new string(charsToWrite).TrimEnd(new char[0]));

					OutputAdded(this, new EventArgs());

					return true;
				}

				public override object InputFromPreviousStep
				{
					set
					{
						mWriteBytes = (MixByte[])value;
					}
				}
			}
		}
	}
}
