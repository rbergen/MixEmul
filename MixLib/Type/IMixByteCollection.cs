using System;
using System.Collections.Generic;

namespace MixLib.Type
{
	public interface IMixByteCollection : IEnumerable<MixByte>, ICloneable
	{
		MixByte this[int index] { get; set; }

        int MaxByteCount { get; }

		void Load(string text);

		string ToString(bool asChars);

		MixByte[] ToArray();
	}
}
