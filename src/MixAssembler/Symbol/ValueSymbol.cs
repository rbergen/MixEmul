using MixLib.Instruction;
using MixLib.Type;

namespace MixAssembler.Symbol
{
	public class ValueSymbol(string name) : SymbolBase(name), IValueSymbol
	{
		public const long MaxValue = (1 << MixByte.BitCount * MixInstruction.AddressByteCount) - 1;
		public const long MinValue = -MaxValue;
		public const int MaxNameLength = 10;

		private bool isDefined = false;

		public long Magnitude { get; private set; }
		public Word.Signs Sign { get; private set; } = Word.Signs.Positive;

		public override bool IsSymbolDefined
			=> this.isDefined;

		public long Value
			=> Sign.ApplyTo(Magnitude);

		public static SymbolBase ParseDefinition(string text)
			=> !IsValueSymbolName(text) ? null : new ValueSymbol(text);

		public override long GetValue(int currentAddress)
			=> Value;

		public override long GetMagnitude(int currentAddress)
			=> Magnitude;

		public override Word.Signs GetSign(int currentAddress)
			=> Sign;

		public override bool IsValueDefined(int currentAddress)
			=> this.isDefined;

		public static bool IsValueSymbolName(string text)
		{
			if (text.Length == 0 || text.Length > MaxNameLength)
				return false;

			bool letterFound = false;

			for (int i = 0; i < text.Length; i++)
			{
				if (char.IsLetter(text[i]))
					letterFound = true;

				else if (!char.IsNumber(text[i]))
					return false;
			}

			return letterFound;
		}

		public static SymbolBase ParseDefinition(string text, int sectionCharIndex, ParsingStatus status)
		{
			var symbol = LocalSymbol.ParseDefinition(text, sectionCharIndex, status);

			if (symbol != null)
				return symbol;

			return ParseDefinition(text);
		}

		public static IValue ParseValue(string text, int sectionCharIndex, ParsingStatus status)
		{
			var symbolValue = LocalSymbol.ParseValue(text, sectionCharIndex, status);

			if (symbolValue != null)
				return symbolValue;

			if (!IsValueSymbolName(text))
				return null;

			SymbolBase symbol = status.Symbols[text];
			if (symbol == null)
			{
				symbol = new ValueSymbol(text);
				status.Symbols.Add(symbol);
			}

			return symbol;
		}

		public override void SetValue(long value)
		{
			Sign = value.GetSign();
			Magnitude = value.GetMagnitude();
			this.isDefined = true;
		}

		public override void SetValue(Word.Signs sign, long magnitude)
		{
			Sign = sign;
			Magnitude = magnitude;
			this.isDefined = true;
		}
	}
}
