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

		private ReaderWriterLock mAccessLock;
		private MixByte[] mBytes;
		private Signs mSign;

        public int ByteCount { get; private set; }

        public Word(int byteCount)
			: this(byteCount, Signs.Positive)
		{
			for (int i = 0; i < byteCount; i++)
			{
				mBytes[i] = new MixByte(0);
			}
		}

		public Word(MixByte[] bytes, Signs sign)
			: this(bytes.Length, sign)
		{
			for (int i = 0; i < ByteCount; i++)
			{
				mBytes[i] = bytes[i];
			}
		}

		private Word(int byteCount, Signs sign)
		{
			ByteCount = byteCount;
			mSign = sign;
			mBytes = new MixByte[byteCount];
			mAccessLock = new ReaderWriterLock();
		}

		public void Load(string text)
		{
			int count = text == null ? 0 : Math.Min(ByteCount, text.Length);
			int index = 0;
			for (; index < count; index++)
			{
				mBytes[index] = text[index];
			}

			while (index < ByteCount)
			{
				mBytes[index++] = 0;
			}
		}

		public static long BytesToLong(params MixByte[] bytes)
		{
			return BytesToLong(Signs.Positive, bytes);
		}

		public static long BytesToLong(Signs sign, params MixByte[] bytes)
		{
			long longValue = 0L;

			for (int i = 0; i < bytes.Length; i++)
			{
				longValue = (longValue << MixByte.BitCount) + (long)bytes[i];
			}

			if (sign == Signs.Negative)
			{
				longValue = -longValue;
			}

			return longValue;
		}

		public void NegateSign()
		{
			Sign = ChangeSign(Sign);
		}

		public static Signs ChangeSign(Signs sign)
		{
			return (sign == Signs.Positive) ? Signs.Negative : Signs.Positive;
		}

		public string ToString(bool asChars)
		{
			string stringValue;

			if (asChars)
			{
				stringValue = string.Empty;

				foreach (MixByte mixByte in mBytes)
				{
					stringValue += (char)mixByte;
				}
			}
			else
			{
				stringValue = Sign == Signs.Negative ? "-" : "+";

				foreach (MixByte mixByte in mBytes)
				{
					stringValue += ' ' + mixByte;
				}
			}

			return stringValue;
		}

		public override string ToString()
		{
			return ToString(false);
		}

		public int BitCount
		{
			get
			{
				return ByteCount * MixByte.BitCount;
			}
		}

		public MixByte this[int index]
		{
			get
			{
				MixByte selectedByte;

				mAccessLock.AcquireReaderLock(Timeout.Infinite);

				try
				{
					selectedByte = mBytes[index];
				}
				finally
				{
					mAccessLock.ReleaseReaderLock();
				}

				return selectedByte;
			}
			set
			{
				mAccessLock.AcquireWriterLock(Timeout.Infinite);

				try
				{
					MixByte oldValue = mBytes[index];

					mBytes[index] = (value != null) ? value : new MixByte();

					if (oldValue.ByteValue == mBytes[index].ByteValue)
					{
						return;
					}
				}
				finally
				{
					mAccessLock.ReleaseWriterLock();
				}

				OnWordValueChanged();
			}
		}

		public long LongValue
		{
			get
			{
				long longValue;
				mAccessLock.AcquireReaderLock(Timeout.Infinite);

				try
				{
					longValue = BytesToLong(Sign, mBytes);
				}
				finally
				{
					mAccessLock.ReleaseReaderLock();
				}

				return longValue;
			}
			set
			{
				mAccessLock.AcquireWriterLock(Timeout.Infinite);

				try
				{
					if (value == LongValue)
					{
						return;
					}

					setMagnitudeLongValue(value);
					mSign = value < 0L ? Signs.Negative : Signs.Positive;
				}
				finally
				{
					mAccessLock.ReleaseWriterLock();
				}

				OnWordValueChanged();
			}
		}

		private long setMagnitudeLongValue(long magnitude)
		{
			long oldValue = MagnitudeLongValue;

			if (magnitude < 0L)
			{
				magnitude = -magnitude;
			}

			if (oldValue == magnitude)
			{
				return magnitude;
			}

			for (int i = ByteCount - 1; i >= 0; i--)
			{
				mBytes[i] = new MixByte((byte)(magnitude & MixByte.MaxValue));
				magnitude = magnitude >> MixByte.BitCount;
			}

			return oldValue;
		}

		public MixByte[] Magnitude
		{
			get
			{
				MixByte[] bytes;

				mAccessLock.AcquireReaderLock(Timeout.Infinite);

				try
				{
					bytes = (MixByte[])mBytes.Clone();
				}
				finally
				{
					mAccessLock.ReleaseReaderLock();
				}

				return bytes;
			}
			set
			{
				mAccessLock.AcquireWriterLock(Timeout.Infinite);

				try
				{
					long oldValue = MagnitudeLongValue;

					int thisStartIndex = 0;
					int valueStartIndex = 0;

					if (value.Length < ByteCount)
					{
						thisStartIndex = ByteCount - value.Length;

						for (int i = 0; i < thisStartIndex; i++)
						{
							mBytes[i] = new MixByte(0);
						}
					}
					else if (value.Length > ByteCount)
					{
						valueStartIndex = value.Length - ByteCount;
					}

					while (thisStartIndex < ByteCount)
					{
						mBytes[thisStartIndex] = value[valueStartIndex];
						thisStartIndex++;
						valueStartIndex++;
					}

					if (oldValue == MagnitudeLongValue)
					{
						return;
					}
				}
				finally
				{
					mAccessLock.ReleaseWriterLock();
				}

				OnWordValueChanged();
			}
		}

		public long MagnitudeLongValue
		{
			get
			{
				long longValue;

				mAccessLock.AcquireReaderLock(Timeout.Infinite);

				try
				{
					longValue = BytesToLong(mBytes);
				}
				finally
				{
					mAccessLock.ReleaseReaderLock();
				}

				return longValue;
			}
			set
			{
				mAccessLock.AcquireWriterLock(Timeout.Infinite);

				try
				{
					if (setMagnitudeLongValue(value) == Math.Abs(value))
					{
						return;
					}
				}
				finally
				{
					mAccessLock.ReleaseWriterLock();
				}

				OnWordValueChanged();
			}
		}

		public long MaxMagnitude
		{
			get
			{
				return (long)((1 << BitCount) - 1);
			}
		}

		public Signs Sign
		{
			get
			{
				mAccessLock.AcquireReaderLock(Timeout.Infinite);

				try
				{
					return mSign;
				}
				finally
				{
					mAccessLock.ReleaseReaderLock();
				}
			}
			set
			{
				mAccessLock.AcquireWriterLock(Timeout.Infinite);

				try
				{
					if (value == mSign)
					{
						return;
					}

					mSign = value;
				}
				finally
				{
					mAccessLock.ReleaseWriterLock();
				}

				OnWordValueChanged();
			}
		}

		public enum Signs
		{
			Positive,
			Negative
		}

		protected void OnWordValueChanged()
		{
			if (WordValueChanged != null)
			{
				WordValueChanged(this);
			}
		}

		public virtual Object Clone()
		{
			Word copy = new Word(ByteCount);
			CopyTo(copy);

			return copy;
		}

		protected void CopyTo(Word target)
		{
			if (target.ByteCount != ByteCount)
			{
				throw new ArgumentException("Target Word must have same bytecount as this Word", "target");
			}

			for (int i = 0; i < ByteCount; i++)
			{
				target.mBytes[i] = mBytes[i].ByteValue;
			}

			target.mSign = mSign;
		}

		public virtual bool IsEmpty
		{
			get
			{
				return MagnitudeLongValue == 0 && Sign == Signs.Positive;
			}
		}

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return mBytes.GetEnumerator();
		}

		#endregion

		#region IEnumerable<MixByte> Members

		IEnumerator<MixByte> IEnumerable<MixByte>.GetEnumerator()
		{
			return ((IEnumerable<MixByte>)mBytes).GetEnumerator();
		}

		#endregion

		#region IMixByteCollection Members

		public int MaxByteCount
		{
			get
			{
				return ByteCount;
			}
		}

		#endregion

		#region IMixByteCollection Members


		public MixByte[] ToArray()
		{
			return Magnitude;
		}

		#endregion
	}
}
