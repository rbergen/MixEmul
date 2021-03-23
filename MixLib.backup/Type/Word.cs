using MixLib.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace MixLib.Type
{
	public class Word : ICloneable, IWord
	{
		public event WordValueChangedEventHandler WordValueChanged;

		ReaderWriterLock mAccessLock;
		readonly MixByte[] mBytes;
		Signs mSign;

		public int ByteCount { get; private set; }

		public Word(int byteCount) : this(byteCount, Signs.Positive)
		{
			for (int i = 0; i < byteCount; i++)
			{
				mBytes[i] = new MixByte(0);
			}
		}

		public Word(MixByte[] bytes, Signs sign) : this(bytes.Length, sign)
		{
			for (int i = 0; i < ByteCount; i++)
			{
				mBytes[i] = bytes[i];
			}
		}

		Word(int byteCount, Signs sign)
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

		public virtual bool IsEmpty => MagnitudeLongValue == 0 && Sign.IsPositive();

		public int MaxByteCount => ByteCount;

		public int BitCount => ByteCount * MixByte.BitCount;

		public long MaxMagnitude => (1 << BitCount) - 1;

		public override string ToString() => ToString(false);

		public IEnumerator GetEnumerator() => mBytes.GetEnumerator();

		IEnumerator<MixByte> IEnumerable<MixByte>.GetEnumerator() => ((IEnumerable<MixByte>)mBytes).GetEnumerator();

		public MixByte[] ToArray() => Magnitude;

		public static long BytesToLong(params MixByte[] bytes) => BytesToLong(Signs.Positive, bytes);

		protected void OnWordValueChanged() => WordValueChanged?.Invoke(this);

		public static long BytesToLong(Signs sign, params MixByte[] bytes)
		{
			long longValue = 0L;

			for (int i = 0; i < bytes.Length; i++)
			{
				longValue = (longValue << MixByte.BitCount) + bytes[i];
			}

			longValue = sign.ApplyTo(longValue);

			return longValue;
		}

		public void InvertSign()
		{
			Sign = Sign.Invert();
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
				stringValue = "" + Sign.ToChar();

				foreach (MixByte mixByte in mBytes)
				{
					stringValue += ' ' + mixByte;
				}
			}

			return stringValue;
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

					mBytes[index] = value ?? new MixByte();

					if (oldValue.ByteValue == mBytes[index].ByteValue) return;
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
					if (value == LongValue) return;

					SetMagnitudeLongValue(value);
					mSign = value.GetSign();
				}
				finally
				{
					mAccessLock.ReleaseWriterLock();
				}

				OnWordValueChanged();
			}
		}

		long SetMagnitudeLongValue(long magnitude)
		{
			long oldValue = MagnitudeLongValue;

			magnitude = magnitude.GetMagnitude();

			if (oldValue == magnitude) return magnitude;

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

					if (oldValue == MagnitudeLongValue) return;
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
					if (SetMagnitudeLongValue(value) == value.GetMagnitude()) return;
				}
				finally
				{
					mAccessLock.ReleaseWriterLock();
				}

				OnWordValueChanged();
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
					if (value == mSign) return;

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

		public virtual object Clone()
		{
			var copy = new Word(ByteCount);
			CopyTo(copy);

			return copy;
		}

		protected void CopyTo(Word target)
		{
			if (target.ByteCount != ByteCount)
			{
				throw new ArgumentException("Target Word must have same bytecount as this Word", nameof(target));
			}

			for (int i = 0; i < ByteCount; i++)
			{
				target.mBytes[i] = mBytes[i].ByteValue;
			}

			target.mSign = mSign;
		}
	}
}
