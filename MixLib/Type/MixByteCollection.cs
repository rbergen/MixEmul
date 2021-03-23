using System;
using System.Collections.Generic;
using System.Text;

namespace MixLib.Type
{
	public class MixByteCollection : IMixByteCollection
	{
		readonly int mMaxByteCount;
		readonly MixByte[] mBytes;

		public MixByteCollection(int maxByteCount)
		{
			mMaxByteCount = maxByteCount;
			mBytes = new MixByte[mMaxByteCount];
			for (int i = 0; i < mMaxByteCount; i++)
			{
				mBytes[i] = 0;
			}
		}

		public MixByteCollection(MixByte[] bytes)
		{
			mMaxByteCount = bytes.Length;
			mBytes = bytes;
		}

		public int MaxByteCount => mMaxByteCount;

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return mBytes.GetEnumerator();
		}

		public IEnumerator<MixByte> GetEnumerator()
		{
			return ((IEnumerable<MixByte>)mBytes).GetEnumerator();
		}

		public MixByte[] ToArray()
		{
			return (MixByte[])mBytes.Clone();
		}

		public void Load(string text)
		{
			int count = text == null ? 0 : Math.Min(mMaxByteCount, text.Length);
			int index = 0;
			for (; index < count; index++)
			{
				mBytes[index] = text[index];
			}

			while (index < mMaxByteCount)
			{
				mBytes[index++] = 0;
			}
		}

		public string ToString(bool asChars)
		{
			if (asChars)
			{
				var builder = new StringBuilder();

				foreach (MixByte mixByte in mBytes)
				{
					builder.Append((char)mixByte);
				}

				return builder.ToString().TrimEnd();
			}

			return ToString();
		}

		public MixByte this[int index]
		{
			get => mBytes[index];
			set => mBytes[index] = value;
		}

		#region ICloneable Members

		public object Clone()
		{
			var clone = new MixByteCollection(mMaxByteCount);
			for (int i = 0; i < mMaxByteCount; i++)
			{
				clone.mBytes[i] = mBytes[i].ByteValue;
			}

			return clone;
		}

		#endregion
	}
}
