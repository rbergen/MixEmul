using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using MixLib.Events;

namespace MixLib.Type
{
	public class Word : ICloneable, IWord
	{
		public event WordValueChangedEventHandler WordValueChanged;

		private readonly ReaderWriterLock accessLock;
		private readonly MixByte[] bytes;
		private Signs sign;

		public int ByteCount { get; private set; }

		public Word(int byteCount) : this(byteCount, Signs.Positive)
		{
			for (int i = 0; i < byteCount; i++)
				this.bytes[i] = new MixByte(0);
		}

		public Word(MixByte[] bytes, Signs sign) : this(bytes.Length, sign)
		{
			for (int i = 0; i < ByteCount; i++)
				this.bytes[i] = bytes[i];
		}

		private Word(int byteCount, Signs sign)
		{
			ByteCount = byteCount;
			this.sign = sign;
			this.bytes = new MixByte[byteCount];
			this.accessLock = new ReaderWriterLock();
		}

		public void Load(string text)
		{
			int count = text == null ? 0 : Math.Min(ByteCount, text.Length);
			int index = 0;

			for (; index < count; index++)
				this.bytes[index] = text[index];

			while (index < ByteCount)
				this.bytes[index++] = 0;
		}

		public virtual bool IsEmpty
			=> MagnitudeLongValue == 0 && Sign.IsPositive();

		public int MaxByteCount
			=> ByteCount;

		public int BitCount
			=> ByteCount * MixByte.BitCount;

		public long MaxMagnitude
			=> (1 << BitCount) - 1;

		public override string ToString()
			=> ToString(false);

		public IEnumerator GetEnumerator()
			=> this.bytes.GetEnumerator();

		IEnumerator<MixByte> IEnumerable<MixByte>.GetEnumerator()
			=> ((IEnumerable<MixByte>)this.bytes).GetEnumerator();

		public MixByte[] ToArray()
			=> Magnitude;

		public static long BytesToLong(params MixByte[] bytes)
			=> BytesToLong(Signs.Positive, bytes);

		protected void OnWordValueChanged()
			=> WordValueChanged?.Invoke(this);

		public static long BytesToLong(Signs sign, params MixByte[] bytes)
		{
			long longValue = 0L;

			for (int i = 0; i < bytes.Length; i++)
				longValue = (longValue << MixByte.BitCount) + bytes[i];

			longValue = sign.ApplyTo(longValue);

			return longValue;
		}

		public void InvertSign()
			=> Sign = Sign.Invert();

		public string ToString(bool asChars)
		{
			string stringValue;

			if (asChars)
			{
				stringValue = string.Empty;

				foreach (MixByte mixByte in this.bytes)
					stringValue += (char)mixByte;
			}
			else
			{
				stringValue = "" + Sign.ToChar();

				foreach (MixByte mixByte in this.bytes)
					stringValue += ' ' + mixByte;
			}

			return stringValue;
		}

		public MixByte this[int index]
		{
			get
			{
				MixByte selectedByte;

				this.accessLock.AcquireReaderLock(Timeout.Infinite);

				try
				{
					selectedByte = this.bytes[index];
				}
				finally
				{
					this.accessLock.ReleaseReaderLock();
				}

				return selectedByte;
			}
			set
			{
				this.accessLock.AcquireWriterLock(Timeout.Infinite);

				try
				{
					MixByte oldValue = this.bytes[index];

					this.bytes[index] = value ?? new MixByte();

					if (oldValue.ByteValue == this.bytes[index].ByteValue)
						return;
				}
				finally
				{
					this.accessLock.ReleaseWriterLock();
				}

				OnWordValueChanged();
			}
		}

		public long LongValue
		{
			get
			{
				long longValue;
				this.accessLock.AcquireReaderLock(Timeout.Infinite);

				try
				{
					longValue = BytesToLong(Sign, this.bytes);
				}
				finally
				{
					this.accessLock.ReleaseReaderLock();
				}

				return longValue;
			}
			set
			{
				this.accessLock.AcquireWriterLock(Timeout.Infinite);

				try
				{
					if (value == LongValue)
						return;

					SetMagnitudeLongValue(value);
					this.sign = value.GetSign();
				}
				finally
				{
					this.accessLock.ReleaseWriterLock();
				}

				OnWordValueChanged();
			}
		}

		private long SetMagnitudeLongValue(long magnitude)
		{
			long oldValue = MagnitudeLongValue;

			magnitude = magnitude.GetMagnitude();

			if (oldValue == magnitude)
				return magnitude;

			for (int i = ByteCount - 1; i >= 0; i--)
			{
				this.bytes[i] = new MixByte((byte)(magnitude & MixByte.MaxValue));
				magnitude >>= MixByte.BitCount;
			}

			return oldValue;
		}

		public MixByte[] Magnitude
		{
			get
			{
				MixByte[] bytes;

				this.accessLock.AcquireReaderLock(Timeout.Infinite);

				try
				{
					bytes = (MixByte[])this.bytes.Clone();
				}
				finally
				{
					this.accessLock.ReleaseReaderLock();
				}

				return bytes;
			}
			set
			{
				this.accessLock.AcquireWriterLock(Timeout.Infinite);

				try
				{
					long oldValue = MagnitudeLongValue;

					int thisStartIndex = 0;
					int valueStartIndex = 0;

					if (value.Length < ByteCount)
					{
						thisStartIndex = ByteCount - value.Length;

						for (int i = 0; i < thisStartIndex; i++)
							this.bytes[i] = new MixByte(0);
					}
					else if (value.Length > ByteCount)
						valueStartIndex = value.Length - ByteCount;

					while (thisStartIndex < ByteCount)
					{
						this.bytes[thisStartIndex] = value[valueStartIndex];
						thisStartIndex++;
						valueStartIndex++;
					}

					if (oldValue == MagnitudeLongValue)
						return;
				}
				finally
				{
					this.accessLock.ReleaseWriterLock();
				}

				OnWordValueChanged();
			}
		}

		public long MagnitudeLongValue
		{
			get
			{
				long longValue;

				this.accessLock.AcquireReaderLock(Timeout.Infinite);

				try
				{
					longValue = BytesToLong(this.bytes);
				}
				finally
				{
					this.accessLock.ReleaseReaderLock();
				}

				return longValue;
			}
			set
			{
				this.accessLock.AcquireWriterLock(Timeout.Infinite);

				try
				{
					if (SetMagnitudeLongValue(value) == value.GetMagnitude())
						return;
				}
				finally
				{
					this.accessLock.ReleaseWriterLock();
				}

				OnWordValueChanged();
			}
		}

		public Signs Sign
		{
			get
			{
				this.accessLock.AcquireReaderLock(Timeout.Infinite);

				try
				{
					return this.sign;
				}
				finally
				{
					this.accessLock.ReleaseReaderLock();
				}
			}
			set
			{
				this.accessLock.AcquireWriterLock(Timeout.Infinite);

				try
				{
					if (value == this.sign)
						return;

					this.sign = value;
				}
				finally
				{
					this.accessLock.ReleaseWriterLock();
				}

				OnWordValueChanged();
			}
		}

		public enum Signs
		{
			Positive,
			Negative
		}

		public virtual object Clone()
		{
			var copy = new Word(ByteCount);
			CopyTo(copy);

			return copy;
		}

		protected void CopyTo(Word target)
		{
			if (target.ByteCount != ByteCount)
				throw new ArgumentException("Target Word must have same bytecount as this Word", nameof(target));

			for (int i = 0; i < ByteCount; i++)
				target.bytes[i] = this.bytes[i].ByteValue;

			target.sign = this.sign;
		}
	}
}
