using System;
using System.Collections.Generic;
using System.Text;

namespace MixLib.Type
{
	public class MixByteCollection : IMixByteCollection
	{
		private readonly int _maxByteCount;
		private readonly MixByte[] _bytes;

		public MixByteCollection(int maxByteCount)
		{
			_maxByteCount = maxByteCount;
			_bytes = new MixByte[_maxByteCount];

			for (int i = 0; i < _maxByteCount; i++)
				_bytes[i] = 0;
		}

		public MixByteCollection(MixByte[] bytes)
		{
			_maxByteCount = bytes.Length;
			_bytes = bytes;
		}

		public int MaxByteCount 
			=> _maxByteCount;

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() 
			=> _bytes.GetEnumerator();

		public IEnumerator<MixByte> GetEnumerator() 
			=> ((IEnumerable<MixByte>)_bytes).GetEnumerator();

		public MixByte[] ToArray() 
			=> (MixByte[])_bytes.Clone();

		public void Load(string text)
		{
			int count = text == null ? 0 : Math.Min(_maxByteCount, text.Length);
			int index = 0;

			for (; index < count; index++)
				_bytes[index] = text[index];

			while (index < _maxByteCount)
				_bytes[index++] = 0;
		}

		public string ToString(bool asChars)
		{
			if (asChars)
			{
				var builder = new StringBuilder();

				foreach (MixByte mixByte in _bytes)
					builder.Append((char)mixByte);

				return builder.ToString().TrimEnd();
			}

			return ToString();
		}

		public MixByte this[int index]
		{
			get => _bytes[index];
			set => _bytes[index] = value;
		}

		#region ICloneable Members

		public object Clone()
		{
			var clone = new MixByteCollection(_maxByteCount);

			for (int i = 0; i < _maxByteCount; i++)
				clone._bytes[i] = _bytes[i].ByteValue;

			return clone;
		}

		#endregion
	}
}
