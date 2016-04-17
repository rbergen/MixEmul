using System;
using MixLib.Device.Step;
using MixLib.Events;
using MixLib.Interrupts;
using MixLib.Type;

namespace MixLib.Device
{
	public abstract class MixDevice
	{
		private const string idleDescription = "Idle";
		private const string readingDescription = "Reading from memory";
		private const string writingDescription = "Writing to memory";

		private InOutputOperands mCurrentOperands;
		private DeviceStep.Instance mCurrentStepInstance;

        protected DeviceStep CurrentStep { get; private set; }
        protected DeviceStep FirstInputDeviceStep { get; set; }
        protected DeviceStep FirstIocDeviceStep { get; set; }
        protected DeviceStep FirstOutputDeviceStep { get; set; }
        public int Id { get; private set; }
        public abstract int RecordWordCount { get; }
        public abstract string ShortName { get; }
        public abstract bool SupportsInput { get; }
        public abstract bool SupportsOutput { get; }


        public event ReportingEventHandler ReportingEvent;

		protected MixDevice(int id)
		{
			Id = id;
			CurrentStep = null;
		}

		protected virtual DeviceStep.Instance GetCurrentStepInstance()
		{
			return CurrentStep.CreateInstance();
		}

		protected virtual void OnReportingEvent(ReportingEventArgs args)
		{
			if (ReportingEvent != null)
			{
				ReportingEvent(this, args);
			}
		}

		public virtual void Reset()
		{
			CurrentStep = null;
			mCurrentStepInstance = null;
		}

		public virtual void StartInput(IMemory memory, int mValue, int sector, InterruptQueueCallback callback)
		{
			if (!Busy && SupportsInput)
			{
				mCurrentOperands = new InOutputOperands(memory, mValue, sector, callback);
				CurrentStep = FirstInputDeviceStep;
			}
		}

		public virtual void StartIoc(int mValue, int sector, InterruptQueueCallback callback)
		{
			if (!Busy)
			{
				mCurrentOperands = new InOutputOperands(null, mValue, sector, callback);
				CurrentStep = FirstIocDeviceStep;
			}
		}

		public virtual void StartOutput(IMemory memory, int mValue, int sector, InterruptQueueCallback callback)
		{
			if (!Busy && SupportsOutput)
			{
				mCurrentOperands = new InOutputOperands(memory, mValue, sector, callback);
				CurrentStep = FirstOutputDeviceStep;
			}
		}

		private void stepInstance_Reporting(object sender, ReportingEventArgs args)
		{
			OnReportingEvent(args);
		}

		public void Tick()
		{
			if (CurrentStep != null)
			{
				if (mCurrentStepInstance == null)
				{
					mCurrentStepInstance = GetCurrentStepInstance();
					mCurrentStepInstance.Operands = mCurrentOperands;
					mCurrentStepInstance.ReportingEvent += new ReportingEventHandler(stepInstance_Reporting);
				}
				if (mCurrentStepInstance.Tick())
				{
					CurrentStep = CurrentStep.NextStep;
					if (CurrentStep == null)
					{
						mCurrentStepInstance = null;

						if (mCurrentOperands.InterruptQueueCallback != null)
						{
							mCurrentOperands.InterruptQueueCallback(new Interrupt(Id));
						}
					}
					else
					{
						DeviceStep.Instance currentStepInstance = GetCurrentStepInstance();
						currentStepInstance.Operands = mCurrentOperands;
						currentStepInstance.ReportingEvent += new ReportingEventHandler(stepInstance_Reporting);
						currentStepInstance.InputFromPreviousStep = mCurrentStepInstance.OutputForNextStep;
						mCurrentStepInstance = currentStepInstance;
					}
				}
			}
		}

		public abstract void UpdateSettings();

		public bool Busy
		{
			get
			{
				return (CurrentStep != null);
			}
		}

        public string StatusDescription
		{
			get
			{
				if (!Busy)
				{
					return idleDescription;
				}
				return CurrentStep.StatusDescription;
			}
		}

        public override string ToString()
		{
			return ShortName + ": " + Id.ToString();
		}

		protected class ReadFromMemoryStep : TickingStep
		{
			private bool mIncludeSign;

			public ReadFromMemoryStep(bool includeSign, int recordWordCount)
				: base(recordWordCount)
			{
				mIncludeSign = includeSign;
			}

			protected override TickingStep.Instance CreateTickingInstance()
			{
				return new Instance(base.TickCount, mIncludeSign);
			}

			public override string StatusDescription
			{
				get
				{
					return readingDescription;
				}
			}

			private new class Instance : TickingStep.Instance
			{
				private MixByte[] mBytesForReading;
				private bool mIncludeSign;
				private int mWordByteCount;

				public Instance(int tickCount, bool includeSign)
					: base(tickCount)
				{
					mIncludeSign = includeSign;
					mWordByteCount = FullWord.ByteCount;

					if (includeSign)
					{
						mWordByteCount++;
					}

					mBytesForReading = new MixByte[base.TickCount * mWordByteCount];
				}

				protected override void ProcessTick()
				{
					int currentAddress = base.Operands.MValue + base.CurrentTick;

					if (currentAddress <= base.Operands.Memory.MaxWordIndex)
					{
						int currentByte = mWordByteCount * base.CurrentTick;

						if (mIncludeSign)
						{
							mBytesForReading[currentByte] = base.Operands.Memory[currentAddress].Sign.ToChar();
							currentByte++;
						}

						for (int index = 0; index < FullWord.ByteCount; index++)
						{
							mBytesForReading[currentByte + index] = base.Operands.Memory[currentAddress][index];
						}
					}
				}

				public override object OutputForNextStep
				{
					get
					{
						return mBytesForReading;
					}
				}
			}
		}

		protected class WriteToMemoryStep : TickingStep
		{
			private bool mIncludeSign;

			public WriteToMemoryStep(bool includeSign, int recordWordCount)
				: base(recordWordCount)
			{
				mIncludeSign = includeSign;
			}

			protected override TickingStep.Instance CreateTickingInstance()
			{
				return new Instance(base.TickCount, mIncludeSign);
			}

			public override string StatusDescription
			{
				get
				{
					return writingDescription;
				}
			}

			private new class Instance : TickingStep.Instance
			{
				private MixByte[] mBytesForWriting;
				private bool mIncludeSign;
				private int mWordByteCount;

				public Instance(int tickCount, bool includeSign)
					: base(tickCount)
				{
					mIncludeSign = includeSign;
					mWordByteCount = FullWord.ByteCount;

					if (includeSign)
					{
						mWordByteCount++;
					}

					mBytesForWriting = new MixByte[base.TickCount * mWordByteCount];
				}

				protected override void ProcessTick()
				{
					int currentAddress = base.Operands.MValue + base.CurrentTick;

					if (currentAddress < base.Operands.Memory.MaxWordIndex)
					{
						int currentByte = mWordByteCount * base.CurrentTick;

						if (mIncludeSign)
						{
							base.Operands.Memory[currentAddress].Sign = mBytesForWriting[currentByte].ToSign();
							currentByte++;
						}
						else
						{
							base.Operands.Memory[currentAddress].Sign = Word.Signs.Positive;
						}

						for (int index = 0; index < FullWord.ByteCount; index++)
						{
							base.Operands.Memory[currentAddress][index] = mBytesForWriting[currentByte + index];
						}
					}
				}

				public override object InputFromPreviousStep
				{
					set
					{
						MixByte[] sourceArray = (MixByte[])value;
						Array.Copy(sourceArray, mBytesForWriting, (sourceArray.Length > mBytesForWriting.Length) ? mBytesForWriting.Length : sourceArray.Length);
					}
				}
			}
		}
	}
}