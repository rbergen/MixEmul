using System;
using System.Collections;
using MixLib.Device.Settings;
using MixLib.Device.Step;
using MixLib.Type;

namespace MixLib.Device
{
	public class TeletypeDevice : MixDevice
	{
		private const string shortName = "TTY";

		private const string initializationDescription = "Initializing teletype";
		private const string readingDescription = "Reading line from teletype";
		private const string writingDescription = "Writing line to teletype";

		private const int recordWordCount = 14;

		private Queue mInputBuffer;
		private Queue mOutputBuffer;

		public event EventHandler InputRequired;

		public event EventHandler OutputAdded;

		public TeletypeDevice(int id)
			: base(id)
		{
			UpdateSettings();
		}

        public override int RecordWordCount => recordWordCount;

        public override string ShortName => shortName;

        public override bool SupportsInput => true;

        public override bool SupportsOutput => true;

        public void AddInputLine(string line) => mInputBuffer.Enqueue(line);

        public string GetOutputLine() => mOutputBuffer.Count <= 0 ? null : (string)mOutputBuffer.Dequeue();

        private void inputRequired(object sender, EventArgs e) => InputRequired?.Invoke(this, e);

        private void outputAdded(object sender, EventArgs e) => OutputAdded?.Invoke(this, e);

		protected override DeviceStep.Instance GetCurrentStepInstance()
		{
			if (base.CurrentStep is readLineStep)
			{
				readLineStep.Instance instance = (readLineStep.Instance)((readLineStep)base.CurrentStep).CreateInstance();
				instance.InputRequired += new EventHandler(inputRequired);

				return instance;
			}

			if (base.CurrentStep is writeLineStep)
			{
				writeLineStep.Instance instance2 = (writeLineStep.Instance)((writeLineStep)base.CurrentStep).CreateInstance();
				instance2.OutputAdded += new EventHandler(outputAdded);

				return instance2;
			}

			return base.GetCurrentStepInstance();
		}

		public override void UpdateSettings()
		{
			int tickCount = DeviceSettings.GetTickCount(DeviceSettings.TeletypeInitialization);

			mInputBuffer = Queue.Synchronized(new Queue());
			mOutputBuffer = Queue.Synchronized(new Queue());

			DeviceStep nextStep = new NoOpStep(tickCount, initializationDescription);
            FirstInputDeviceStep = nextStep;
			nextStep.NextStep = new readLineStep(mInputBuffer);
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new WriteToMemoryStep(false, recordWordCount);
			nextStep.NextStep.NextStep = null;

			nextStep = new NoOpStep(tickCount, initializationDescription);
            FirstOutputDeviceStep = nextStep;
			nextStep.NextStep = new ReadFromMemoryStep(false, recordWordCount);
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new writeLineStep(mOutputBuffer);
			nextStep.NextStep.NextStep = null;

            FirstIocDeviceStep = null;
		}

		private class readLineStep : DeviceStep
		{
			private Queue mInputBuffer;

			public readLineStep(Queue inputBuffer)
			{
				mInputBuffer = inputBuffer;
			}

            public override string StatusDescription => readingDescription;

            public override DeviceStep.Instance CreateInstance() => new Instance(mInputBuffer);

			public new class Instance : DeviceStep.Instance
			{
				private Queue mInputBuffer;
				private MixByte[] mReadBytes;

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

					int bytesToReadCount = Math.Min(stringToRead.Length, mReadBytes.Length);
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

		private class writeLineStep : DeviceStep
		{
			private Queue mOutputBuffer;

			public writeLineStep(Queue outputBuffer)
			{
				mOutputBuffer = outputBuffer;
			}

            public override string StatusDescription => writingDescription;

            public override DeviceStep.Instance CreateInstance() => new Instance(mOutputBuffer);

			public new class Instance : DeviceStep.Instance
			{
				private Queue mOutputBuffer;
				private MixByte[] mWriteBytes;

				public event EventHandler OutputAdded;

				public Instance(Queue outputBuffer)
				{
					mOutputBuffer = outputBuffer;
				}

				public override bool Tick()
				{
					if (mWriteBytes != null)
					{
						char[] charsToWrite = new char[recordWordCount * FullWord.ByteCount];
						int bytesToWriteCount = Math.Min(charsToWrite.Length, mWriteBytes.Length);
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
					}
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
