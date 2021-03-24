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

		private readonly ReaderWriterLock _accessLock;
		private readonly MixByte[] _bytes;
		private Signs _sign;

		public int ByteCount { get; private set; }

		public Word(int byteCount) : this(byteCount, Signs.Positive)
		{
			for (int i = 0; i < byteCount; i++)
				_bytes[i] = new MixByte(0);
		}

		public Word(MixByte[] bytes, Signs sign) : this(bytes.Length, sign)
		{
			for (int i = 0; i < ByteCount; i++)
				_bytes[i] = bytes[i];
		}

		private Word(int byteCount, Signs sign)
		{
			ByteCount = byteCount;
			_sign = sign;
			_bytes = new MixByte[byteCount];
			_accessLock = new ReaderWriterLock();
		}

		public void Load(string text)
		{
			int count = text == null ? 0 : Math.Min(ByteCount, text.Length);
			int index = 0;

			for (; index < count; index++)
				_bytes[index] = text[index];

			while (index < ByteCount)
				_bytes[index++] = 0;
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
			=> _bytes.GetEnumerator();

		IEnumerator<MixByte> IEnumerable<MixByte>.GetEnumerator()
			=> ((IEnumerable<MixByte>)_bytes).GetEnumerator();

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

				foreach (MixByte mixByte in _bytes)
					stringValue += (char)mixByte;
			}
			else
			{
				stringValue = "" + Sign.ToChar();

				foreach (MixByte mixByte in _bytes)
					stringValue += ' ' + mixByte;
			}

			return stringValue;
		}

		public MixByte this[int index]
		{
			get
			{
				MixByte selectedByte;

				_accessLock.AcquireReaderLock(Timeout.Infinite);

				try
				{
					selectedByte = _bytes[index];
				}
				finally
				{
					_accessLock.ReleaseReaderLock();
				}

				return selectedByte;
			}
			set
			{
				_accessLock.AcquireWriterLock(Timeout.Infinite);

				try
				{
					MixByte oldValue = _bytes[index];

					_bytes[index] = value ?? new MixByte();

					if (oldValue.ByteValue == _bytes[index].ByteValue)
						return;
				}
				finally
				{
					_accessLock.ReleaseWriterLock();
				}

				OnWordValueChanged();
			}
		}

		public long LongValue
		{
			get
			{
				long longValue;
				_accessLock.AcquireReaderLock(Timeout.Infinite);

				try
				{
					longValue = BytesToLong(Sign, _bytes);
				}
				finally
				{
					_accessLock.ReleaseReaderLock();
				}

				return longValue;
			}
			set
			{
				_accessLock.AcquireWriterLock(Timeout.Infinite);

				try
				{
					if (value == LongValue)
						return;

					SetMagnitudeLongValue(value);
					_sign = value.GetSign();
				}
				finally
				{
					_accessLock.ReleaseWriterLock();
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
				_bytes[i] = new MixByte((byte)(magnitude & MixByte.MaxValue));
				magnitude >>= MixByte.BitCount;
			}

			return oldValue;
		}

		public MixByte[] Magnitude
		{
			get
			{
				MixByte[] bytes;

				_accessLock.AcquireReaderLock(Timeout.Infinite);

				try
				{
					bytes = (MixByte[])_bytes.Clone();
				}
				finally
				{
					_accessLock.ReleaseReaderLock();
				}

				return bytes;
			}
			set
			{
				_accessLock.AcquireWriterLock(Timeout.Infinite);

				try
				{
					long oldValue = MagnitudeLongValue;

					int thisStartIndex = 0;
					int valueStartIndex = 0;

					if (value.Length < ByteCount)
					{
						thisStartIndex = ByteCount - value.Length;

						for (int i = 0; i < thisStartIndex; i++)
							_bytes[i] = new MixByte(0);
					}
					else if (value.Length > ByteCount)
						valueStartIndex = value.Length - ByteCount;

					while (thisStartIndex < ByteCount)
					{
						_bytes[thisStartIndex] = value[valueStartIndex];
						thisStartIndex++;
						valueStartIndex++;
					}

					if (oldValue == MagnitudeLongValue)
						return;
				}
				finally
				{
					_accessLock.ReleaseWriterLock();
				}

				OnWordValueChanged();
			}
		}

		public long MagnitudeLongValue
		{
			get
			{
				long longValue;

				_accessLock.AcquireReaderLock(Timeout.Infinite);

				try
				{
					longValue = BytesToLong(_bytes);
				}
				finally
				{
					_accessLock.ReleaseReaderLock();
				}

				return longValue;
			}
			set
			{
				_accessLock.AcquireWriterLock(Timeout.Infinite);

				try
				{
					if (SetMagnitudeLongValue(value) == value.GetMagnitude())
						return;
				}
				finally
				{
					_accessLock.ReleaseWriterLock();
				}

				OnWordValueChanged();
			}
		}

		public Signs Sign
		{
			get
			{
				_accessLock.AcquireReaderLock(Timeout.Infinite);

				try
				{
					return _sign;
				}
				finally
				{
					_accessLock.ReleaseReaderLock();
				}
			}
			set
			{
				_accessLock.AcquireWriterLock(Timeout.Infinite);

				try
				{
					if (value == _sign)
						return;

					_sign = value;
				}
				finally
				{
					_accessLock.ReleaseWriterLock();
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
				target._bytes[i] = _bytes[i].ByteValue;

			target._sign = _sign;
		}
	}
}
