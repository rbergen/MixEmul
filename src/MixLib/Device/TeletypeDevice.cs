using System;
using System.Collections;
using MixLib.Device.Settings;
using MixLib.Device.Step;
using MixLib.Type;

namespace MixLib.Device
{
	public class TeletypeDevice : MixDevice
	{
		private const string MyShortName = "TTY";

		private const string InitializationDescription = "Initializing teletype";
		private const string ReadingDescription = "Reading line from teletype";
		private const string WritingDescription = "Writing line to teletype";

		private const int MyRecordWordCount = 14;

		private Queue inputBuffer;
		private Queue outputBuffer;

		public event EventHandler InputRequired;
		public event EventHandler OutputAdded;

		public TeletypeDevice(int id) : base(id)
			=> UpdateSettings();

		public override int RecordWordCount
			=> MyRecordWordCount;

		public override string ShortName
			=> MyShortName;

		public override bool SupportsInput
			=> true;

		public override bool SupportsOutput
			=> true;

		public void AddInputLine(string line)
			=> this.inputBuffer.Enqueue(line);

		public string GetOutputLine()
			=> this.outputBuffer.Count > 0 ? (string)this.outputBuffer.Dequeue() : null;

		private void This_InputRequired(object sender, EventArgs e)
			=> InputRequired?.Invoke(this, e);

		private void This_OutputAdded(object sender, EventArgs e)
			=> OutputAdded?.Invoke(this, e);

		protected override DeviceStep.Instance GetCurrentStepInstance()
		{
			if (CurrentStep is ReadLineStep readLineStep)
			{
				var instance = (ReadLineStep.Instance)readLineStep.CreateInstance();
				instance.InputRequired += This_InputRequired;

				return instance;
			}

			if (CurrentStep is WriteLineStep writeLineStep)
			{
				var instance = (WriteLineStep.Instance)writeLineStep.CreateInstance();
				instance.OutputAdded += This_OutputAdded;

				return instance;
			}

			return base.GetCurrentStepInstance();
		}

		public sealed override void UpdateSettings()
		{
			var tickCount = DeviceSettings.GetTickCount(DeviceSettings.TeletypeInitialization);

			this.inputBuffer = Queue.Synchronized(new Queue());
			this.outputBuffer = Queue.Synchronized(new Queue());

			DeviceStep nextStep = new NoOpStep(tickCount, InitializationDescription);
			FirstInputDeviceStep = nextStep;
			nextStep.NextStep = new ReadLineStep(this.inputBuffer);
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new WriteToMemoryStep(false, MyRecordWordCount)
			{
				NextStep = null
			};

			nextStep = new NoOpStep(tickCount, InitializationDescription);
			FirstOutputDeviceStep = nextStep;
			nextStep.NextStep = new ReadFromMemoryStep(false, MyRecordWordCount);
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new WriteLineStep(this.outputBuffer)
			{
				NextStep = null
			};

			FirstIocDeviceStep = null;
		}

		private class ReadLineStep : DeviceStep
		{
			private readonly Queue inputBuffer;

			public ReadLineStep(Queue inputBuffer) 
				=> this.inputBuffer = inputBuffer;

			public override string StatusDescription 
				=> ReadingDescription;

			public override DeviceStep.Instance CreateInstance() 
				=> new Instance(this.inputBuffer);

			public new class Instance : DeviceStep.Instance
			{
				private readonly Queue inputBuffer;
				private MixByte[] readBytes;

				public event EventHandler InputRequired;

				public Instance(Queue inputBuffer) => this.inputBuffer = inputBuffer;

				public override object OutputForNextStep => this.readBytes;

				public override bool Tick()
				{
					if (this.inputBuffer.Count == 0)
					{
						InputRequired(this, new EventArgs());
						return false;
					}

					string stringToRead = (this.inputBuffer.Count == 0) ? "" : ((string)this.inputBuffer.Dequeue());
					this.readBytes = new MixByte[FullWord.ByteCount * MyRecordWordCount];

					var bytesToReadCount = Math.Min(stringToRead.Length, this.readBytes.Length);
					int index = 0;

					while (index < bytesToReadCount)
					{
						this.readBytes[index] = stringToRead[index];
						index++;
					}

					while (index < this.readBytes.Length)
					{
						this.readBytes[index] = 0;
						index++;
					}

					return true;
				}
			}
		}

		private class WriteLineStep : DeviceStep
		{
			private readonly Queue outputBuffer;

			public WriteLineStep(Queue outputBuffer) 
				=> this.outputBuffer = outputBuffer;

			public override string StatusDescription 
				=> WritingDescription;

			public override DeviceStep.Instance CreateInstance() 
				=> new Instance(this.outputBuffer);

			public new class Instance : DeviceStep.Instance
			{
				private readonly Queue outputBuffer;
				private MixByte[] writeBytes;

				public event EventHandler OutputAdded;

				public Instance(Queue outputBuffer) 
					=> this.outputBuffer = outputBuffer;

				public override bool Tick()
				{
					if (this.writeBytes == null)
						return true;

					char[] charsToWrite = new char[MyRecordWordCount * FullWord.ByteCount];
					var bytesToWriteCount = Math.Min(charsToWrite.Length, this.writeBytes.Length);
					int index = 0;

					while (index < bytesToWriteCount)
					{
						charsToWrite[index] = this.writeBytes[index];
						index++;
					}

					while (index < charsToWrite.Length)
					{
						charsToWrite[index] = ' ';
						index++;
					}

					this.outputBuffer.Enqueue(new string(charsToWrite).TrimEnd(Array.Empty<char>()));

					OutputAdded(this, new EventArgs());

					return true;
				}

				public override object InputFromPreviousStep
				{
					set => this.writeBytes = (MixByte[])value;
				}
			}
		}
	}
}
