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

		private Queue mInputBuffer;
		private Queue mOutputBuffer;

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
			=> mInputBuffer.Enqueue(line);

		public string GetOutputLine()
			=> mOutputBuffer.Count > 0 ? (string)mOutputBuffer.Dequeue() : null;

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

			mInputBuffer = Queue.Synchronized(new Queue());
			mOutputBuffer = Queue.Synchronized(new Queue());

			DeviceStep nextStep = new NoOpStep(tickCount, InitializationDescription);
			FirstInputDeviceStep = nextStep;
			nextStep.NextStep = new ReadLineStep(mInputBuffer);
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new WriteToMemoryStep(false, MyRecordWordCount)
			{
				NextStep = null
			};

			nextStep = new NoOpStep(tickCount, InitializationDescription);
			FirstOutputDeviceStep = nextStep;
			nextStep.NextStep = new ReadFromMemoryStep(false, MyRecordWordCount);
			nextStep = nextStep.NextStep;
			nextStep.NextStep = new WriteLineStep(mOutputBuffer)
			{
				NextStep = null
			};

			FirstIocDeviceStep = null;
		}

		private class ReadLineStep : DeviceStep
		{
			private readonly Queue _inputBuffer;

			public ReadLineStep(Queue inputBuffer) 
				=> _inputBuffer = inputBuffer;

			public override string StatusDescription 
				=> ReadingDescription;

			public override DeviceStep.Instance CreateInstance() 
				=> new Instance(_inputBuffer);

			public new class Instance : DeviceStep.Instance
			{
				private readonly Queue _inputBuffer;
				private MixByte[] _readBytes;

				public event EventHandler InputRequired;

				public Instance(Queue inputBuffer) => _inputBuffer = inputBuffer;

				public override object OutputForNextStep => _readBytes;

				public override bool Tick()
				{
					if (_inputBuffer.Count == 0)
					{
						InputRequired(this, new EventArgs());
						return false;
					}

					string stringToRead = (_inputBuffer.Count == 0) ? "" : ((string)_inputBuffer.Dequeue());
					_readBytes = new MixByte[FullWord.ByteCount * MyRecordWordCount];

					var bytesToReadCount = Math.Min(stringToRead.Length, _readBytes.Length);
					int index = 0;

					while (index < bytesToReadCount)
					{
						_readBytes[index] = stringToRead[index];
						index++;
					}

					while (index < _readBytes.Length)
					{
						_readBytes[index] = 0;
						index++;
					}

					return true;
				}
			}
		}

		private class WriteLineStep : DeviceStep
		{
			private readonly Queue _outputBuffer;

			public WriteLineStep(Queue outputBuffer) 
				=> _outputBuffer = outputBuffer;

			public override string StatusDescription 
				=> WritingDescription;

			public override DeviceStep.Instance CreateInstance() 
				=> new Instance(_outputBuffer);

			public new class Instance : DeviceStep.Instance
			{
				private readonly Queue _outputBuffer;
				private MixByte[] _writeBytes;

				public event EventHandler OutputAdded;

				public Instance(Queue outputBuffer) 
					=> _outputBuffer = outputBuffer;

				public override bool Tick()
				{
					if (_writeBytes == null)
						return true;

					char[] charsToWrite = new char[MyRecordWordCount * FullWord.ByteCount];
					var bytesToWriteCount = Math.Min(charsToWrite.Length, _writeBytes.Length);
					int index = 0;

					while (index < bytesToWriteCount)
					{
						charsToWrite[index] = _writeBytes[index];
						index++;
					}

					while (index < charsToWrite.Length)
					{
						charsToWrite[index] = ' ';
						index++;
					}

					_outputBuffer.Enqueue(new string(charsToWrite).TrimEnd(Array.Empty<char>()));

					OutputAdded(this, new EventArgs());

					return true;
				}

				public override object InputFromPreviousStep
				{
					set => _writeBytes = (MixByte[])value;
				}
			}
		}
	}
}
