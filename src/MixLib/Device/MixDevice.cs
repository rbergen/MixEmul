using System;
using MixLib.Device.Step;
using MixLib.Events;
using MixLib.Interrupts;
using MixLib.Type;

namespace MixLib.Device
{
	public abstract class MixDevice(int id)
  {
		private const string IdleDescription = "Idle";
		private const string ReadingDescription = "Reading from memory";
		private const string WritingDescription = "Writing to memory";

		private InOutputOperands currentOperands;
		private DeviceStep.Instance currentStepInstance;

	protected DeviceStep CurrentStep { get; private set; } = null;
	protected DeviceStep FirstInputDeviceStep { get; set; }
		protected DeviceStep FirstIocDeviceStep { get; set; }
		protected DeviceStep FirstOutputDeviceStep { get; set; }
		public int Id => id;
		public abstract int RecordWordCount { get; }
		public abstract string ShortName { get; }
		public abstract bool SupportsInput { get; }
		public abstract bool SupportsOutput { get; }


		public event ReportingEventHandler ReportingEvent;

	public bool Busy 
			=> CurrentStep != null;

		public string StatusDescription 
			=> Busy ? CurrentStep.StatusDescription : IdleDescription;

		protected virtual DeviceStep.Instance GetCurrentStepInstance() 
			=> CurrentStep.CreateInstance();

		public override string ToString() 
			=> ShortName + ": " + Id;

		protected virtual void OnReportingEvent(ReportingEventArgs args) 
			=> ReportingEvent?.Invoke(this, args);

		private void StepInstance_Reporting(object sender, ReportingEventArgs args) 
			=> OnReportingEvent(args);

		public virtual void Reset()
		{
			CurrentStep = null;
			this.currentStepInstance = null;
		}

		public virtual void StartInput(IMemory memory, int value, int sector, InterruptQueueCallback callback)
		{
			if (!Busy && SupportsInput)
			{
				this.currentOperands = new InOutputOperands(memory, value, sector, callback);
				CurrentStep = FirstInputDeviceStep;
			}
		}

		public virtual void StartIoc(int value, int sector, InterruptQueueCallback callback)
		{
			if (!Busy)
			{
				this.currentOperands = new InOutputOperands(null, value, sector, callback);
				CurrentStep = FirstIocDeviceStep;
			}
		}

		public virtual void StartOutput(IMemory memory, int value, int sector, InterruptQueueCallback callback)
		{
			if (!Busy && SupportsOutput)
			{
				this.currentOperands = new InOutputOperands(memory, value, sector, callback);
				CurrentStep = FirstOutputDeviceStep;
			}
		}

		public void Tick()
		{
			// If no I/O step is set, we're idle
			if (CurrentStep == null)
				return;

			// At I/O start, the step instance is unset
			if (this.currentStepInstance == null)
			{
				this.currentStepInstance = GetCurrentStepInstance();
				this.currentStepInstance.Operands = this.currentOperands;
				this.currentStepInstance.ReportingEvent += StepInstance_Reporting;
			}

			// Current step still busy? If so, we're done here
			if (!this.currentStepInstance.Tick())
				return;

			CurrentStep = CurrentStep.NextStep;

			// Have we reached the end of the I/O step chain? If so, trigger interrupt and quit
			if (CurrentStep == null)
			{
				this.currentStepInstance = null;
				this.currentOperands.InterruptQueueCallback?.Invoke(new Interrupt(Id));

				return;
			}

			// Move on to next I/O step
			var currentStepInstance = GetCurrentStepInstance();
			currentStepInstance.Operands = this.currentOperands;
			currentStepInstance.ReportingEvent += StepInstance_Reporting;
			currentStepInstance.InputFromPreviousStep = this.currentStepInstance.OutputForNextStep;
			this.currentStepInstance = currentStepInstance;
		}

		public abstract void UpdateSettings();

		protected class ReadFromMemoryStep(bool includeSign, int recordWordCount) : TickingStep(recordWordCount)
		{
			public override string StatusDescription 
				=> ReadingDescription;

			protected override TickingStep.Instance CreateTickingInstance() 
				=> new Instance(TickCount, includeSign);

			private new class Instance : TickingStep.Instance
			{
				private readonly MixByte[] mBytesForReading;
				private readonly bool mIncludeSign;
				private readonly int mWordByteCount;

				public Instance(int tickCount, bool includeSign) : base(tickCount)
				{
					mIncludeSign = includeSign;
					mWordByteCount = FullWord.ByteCount;

					if (includeSign)
						mWordByteCount++;

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
							mBytesForReading[currentByte + index] = Operands.Memory[currentAddress][index];
					}
				}
			}
		}

		protected class WriteToMemoryStep(bool includeSign, int recordWordCount) : TickingStep(recordWordCount)
		{
		  public override string StatusDescription => WritingDescription;

			protected override TickingStep.Instance CreateTickingInstance() => new Instance(TickCount, includeSign);

			private new class Instance : TickingStep.Instance
			{
				private readonly MixByte[] mBytesForWriting;
				private readonly bool mIncludeSign;
				private readonly int mWordByteCount;

				public Instance(int tickCount, bool includeSign) : base(tickCount)
				{
					mIncludeSign = includeSign;
					mWordByteCount = FullWord.ByteCount;

					if (includeSign)
						mWordByteCount++;

					mBytesForWriting = new MixByte[TickCount * mWordByteCount];
				}

				protected override void ProcessTick()
				{
					int currentAddress = Operands.MValue + CurrentTick;

					if (currentAddress >= Operands.Memory.MaxWordIndex)
						return;

					int currentByte = mWordByteCount * CurrentTick;

					if (mIncludeSign)
					{
						Operands.Memory[currentAddress].Sign = mBytesForWriting[currentByte].ToSign();
						currentByte++;
					}
					else
						Operands.Memory[currentAddress].Sign = Word.Signs.Positive;

					for (int index = 0; index < FullWord.ByteCount; index++)
						Operands.Memory[currentAddress][index] = mBytesForWriting[currentByte + index];
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
