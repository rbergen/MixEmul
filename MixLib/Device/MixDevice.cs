using System;
using MixLib.Device.Step;
using MixLib.Events;
using MixLib.Interrupts;
using MixLib.Type;

namespace MixLib.Device
{
	public abstract class MixDevice
	{
        const string idleDescription = "Idle";
        const string readingDescription = "Reading from memory";
        const string writingDescription = "Writing to memory";

        InOutputOperands mCurrentOperands;
        DeviceStep.Instance mCurrentStepInstance;

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

        public bool Busy => CurrentStep != null;

        public string StatusDescription => Busy ? CurrentStep.StatusDescription : idleDescription;

        protected virtual DeviceStep.Instance GetCurrentStepInstance() => CurrentStep.CreateInstance();

        public override string ToString() => ShortName + ": " + Id;

        protected virtual void OnReportingEvent(ReportingEventArgs args) => ReportingEvent?.Invoke(this, args);

        void stepInstance_Reporting(object sender, ReportingEventArgs args) => OnReportingEvent(args);

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

		public void Tick()
		{
            // If no I/O step is set, we're idle
            if (CurrentStep == null) return;

            // At I/O start, the step instance is unset
            if (mCurrentStepInstance == null)
			{
				mCurrentStepInstance = GetCurrentStepInstance();
				mCurrentStepInstance.Operands = mCurrentOperands;
				mCurrentStepInstance.ReportingEvent += stepInstance_Reporting;
			}

            // Current step still busy? If so, we're done here
            if (!mCurrentStepInstance.Tick()) return;

			CurrentStep = CurrentStep.NextStep;

            // Have we reached the end of the I/O step chain? If so, trigger interrupt and quit
            if (CurrentStep == null)
			{
				mCurrentStepInstance = null;
                mCurrentOperands.InterruptQueueCallback?.Invoke(new Interrupt(Id));

                return;
            }

            // Move on to next I/O step
            var currentStepInstance = GetCurrentStepInstance();
			currentStepInstance.Operands = mCurrentOperands;
			currentStepInstance.ReportingEvent += stepInstance_Reporting;
			currentStepInstance.InputFromPreviousStep = mCurrentStepInstance.OutputForNextStep;
			mCurrentStepInstance = currentStepInstance;
		}

		public abstract void UpdateSettings();

		protected class ReadFromMemoryStep : TickingStep
		{
            bool mIncludeSign;

            public ReadFromMemoryStep(bool includeSign, int recordWordCount) : base(recordWordCount)
			{
				mIncludeSign = includeSign;
			}

            public override string StatusDescription => readingDescription;

            protected override TickingStep.Instance CreateTickingInstance() => new Instance(TickCount, mIncludeSign);

            new class Instance : TickingStep.Instance
            {
                readonly MixByte[] mBytesForReading;
                readonly bool mIncludeSign;
                readonly int mWordByteCount;

                public Instance(int tickCount, bool includeSign) : base(tickCount)
                {
                    mIncludeSign = includeSign;
                    mWordByteCount = FullWord.ByteCount;

                    if (includeSign)
                    {
                        mWordByteCount++;
                    }

                    mBytesForReading = new MixByte[TickCount * mWordByteCount];
                }

                public override object OutputForNextStep => mBytesForReading;

                protected override void ProcessTick()
                {
                    int currentAddress = Operands.MValue + CurrentTick;

                    if (currentAddress <= Operands.Memory.MaxWordIndex)
                    {
                        int currentByte = mWordByteCount * CurrentTick;

                        if (mIncludeSign)
                        {
                            mBytesForReading[currentByte] = Operands.Memory[currentAddress].Sign.ToChar();
                            currentByte++;
                        }

                        for (int index = 0; index < FullWord.ByteCount; index++)
                        {
                            mBytesForReading[currentByte + index] = Operands.Memory[currentAddress][index];
                        }
                    }
                }
            }
        }

		protected class WriteToMemoryStep : TickingStep
		{
            bool mIncludeSign;

            public WriteToMemoryStep(bool includeSign, int recordWordCount) : base(recordWordCount)
			{
				mIncludeSign = includeSign;
			}

            public override string StatusDescription => writingDescription;

            protected override TickingStep.Instance CreateTickingInstance() => new Instance(TickCount, mIncludeSign);

            new class Instance : TickingStep.Instance
            {
                readonly MixByte[] mBytesForWriting;
                bool mIncludeSign;
                int mWordByteCount;

                public Instance(int tickCount, bool includeSign) : base(tickCount)
                {
                    mIncludeSign = includeSign;
                    mWordByteCount = FullWord.ByteCount;

                    if (includeSign)
                    {
                        mWordByteCount++;
                    }

                    mBytesForWriting = new MixByte[TickCount * mWordByteCount];
                }

                protected override void ProcessTick()
                {
                    int currentAddress = Operands.MValue + CurrentTick;

                    if (currentAddress >= Operands.Memory.MaxWordIndex) return;

                    int currentByte = mWordByteCount * CurrentTick;

                    if (mIncludeSign)
                    {
                        Operands.Memory[currentAddress].Sign = mBytesForWriting[currentByte].ToSign();
                        currentByte++;
                    }
                    else
                    {
                        Operands.Memory[currentAddress].Sign = Word.Signs.Positive;
                    }

                    for (int index = 0; index < FullWord.ByteCount; index++)
                    {
                        Operands.Memory[currentAddress][index] = mBytesForWriting[currentByte + index];
                    }
                }

                public override object InputFromPreviousStep
                {
                    set
                    {
                        var sourceArray = (MixByte[])value;
                        Array.Copy(sourceArray, mBytesForWriting, (sourceArray.Length > mBytesForWriting.Length) ? mBytesForWriting.Length : sourceArray.Length);
                    }
                }
            }
        }
	}
}