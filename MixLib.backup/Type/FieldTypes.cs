using System;

namespace MixLib.Type
{
	[Flags]
	public enum FieldTypes : byte
	{
		None = 0,
		LastByte = 1,
		Word = 2,
		Value = 4,
		Chars = 8,
		Instruction = 16
	}
}