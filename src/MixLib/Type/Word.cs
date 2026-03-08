using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MixLib.Events;

namespace MixLib.Type
{
	public class Word : ICloneable, IWord
	{
		public event WordValueChangedEventHandler WordValueChanged;

		private readonly Lock accessLock = new();
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

		public int Count
			=> ByteCount;

		public int MaxByteCount
			=> ByteCount;

		public int BitCount
			=> ByteCount * MixByte.BitCount;

		public long MaxMagnitude
			=> (1 << BitCount) - 1;

		public override string ToString()
			=> ToString(false);

		IEnumerator IEnumerable.GetEnumerator()
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

		public MixByte[] Slice(int startIndex, int count)
		{
			MixByte[] slice = new MixByte[count];

			for (int i = 0; i < count; i++)
				slice[i] = this.bytes[startIndex + i];

			return slice;
		}

		public void InvertSign()
			=> Sign = Sign.Invert();

		public string ToString(bool asChars)
		{
			if (asChars)
			{
				return new string([.. this.bytes.Select(b => (char)b)]);
			}
			else
			{
				var sb = new StringBuilder();
				sb.Append(Sign.ToChar());

				foreach (MixByte mixByte in this.bytes)
				{
					sb.Append(' ');
					sb.Append(mixByte.ToString());
				}

				return sb.ToString();
			}
		}

		public MixByte this[int index]
		{
			get
			{
				lock (accessLock)
				{
					return this.bytes[index];
				}
			}
			set
			{
				lock (accessLock)
				{
					MixByte oldValue = this.bytes[index];
					this.bytes[index] = value ?? new MixByte();

					if (oldValue.ByteValue == this.bytes[index].ByteValue)
						return;
				}

				OnWordValueChanged();
			}
		}

		public long LongValue
		{
			get
			{
				lock (accessLock)
				{
					return BytesToLong(Sign, this.bytes);
				}
			}
			set
			{
				lock (accessLock)
				{
					if (value == LongValue)
						return;

					SetMagnitudeLongValue(value);
					this.sign = value.GetSign();
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
				lock (accessLock)
				{
					return (MixByte[])this.bytes.Clone();
				}
			}
			set
			{
				lock (accessLock)
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

				OnWordValueChanged();
			}
		}

		public long MagnitudeLongValue
		{
			get
			{
				lock (accessLock)
				{
					return BytesToLong(this.bytes);
				}
			}
			set
			{
				lock (accessLock)
				{
					if (SetMagnitudeLongValue(value) == value.GetMagnitude())
						return;
				}

				OnWordValueChanged();
			}
		}

		public Signs Sign
		{
			get
			{
				lock (accessLock)
				{
					return this.sign;
				}
			}
			set
			{
				lock (accessLock)
				{
					if (value == this.sign)
						return;

					this.sign = value;
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
