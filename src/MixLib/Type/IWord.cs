using System.Collections;
using System.Collections.Generic;

namespace MixLib.Type
{
	public interface IWord : IEnumerable<MixByte>, IMixByteCollection
	{
		void InvertSign();

		bool IsEmpty { get; }
		int BitCount { get; }
		int ByteCount { get; }
		long LongValue { get; set; }
		MixByte[] Magnitude { get; set; }
		long MagnitudeLongValue { get; set; }
		long MaxMagnitude { get; }
		Word.Signs Sign { get; set; }
	}
}
