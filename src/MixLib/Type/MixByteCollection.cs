using System;
using System.Collections.Generic;
using System.Text;

namespace MixLib.Type
{
	public class MixByteCollection : IMixByteCollection
	{
		private readonly int maxByteCount;
		private readonly MixByte[] bytes;

		public MixByteCollection(int maxByteCount)
		{
			this.maxByteCount = maxByteCount;
			this.bytes = new MixByte[this.maxByteCount];

			for (int i = 0; i < this.maxByteCount; i++)
				this.bytes[i] = 0;
		}

		public MixByteCollection(MixByte[] bytes)
		{
			this.maxByteCount = bytes.Length;
			this.bytes = bytes;
		}

		public int MaxByteCount 
			=> this.maxByteCount;

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() 
			=> this.bytes.GetEnumerator();

		public IEnumerator<MixByte> GetEnumerator() 
			=> ((IEnumerable<MixByte>)this.bytes).GetEnumerator();

		public MixByte[] ToArray() 
			=> (MixByte[])this.bytes.Clone();

		public void Load(string text)
		{
			int count = text == null ? 0 : Math.Min(this.maxByteCount, text.Length);
			int index = 0;

			for (; index < count; index++)
				this.bytes[index] = text[index];

			while (index < this.maxByteCount)
				this.bytes[index++] = 0;
		}

		public string ToString(bool asChars)
		{
			if (asChars)
			{
				var builder = new StringBuilder();

				foreach (MixByte mixByte in this.bytes)
					builder.Append((char)mixByte);

				return builder.ToString().TrimEnd();
			}

			return ToString();
		}

		public MixByte this[int index]
		{
			get => this.bytes[index];
			set => this.bytes[index] = value;
		}

		#region ICloneable Members

		public object Clone()
		{
			var clone = new MixByteCollection(this.maxByteCount);

			for (int i = 0; i < this.maxByteCount; i++)
				clone.bytes[i] = this.bytes[i].ByteValue;

			return clone;
		}

		#endregion
	}
}
