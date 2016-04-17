using MixLib.Instruction;
using MixLib.Type;

namespace MixAssembler.Symbol
{
	public class ValueSymbol : SymbolBase, IValueSymbol
	{
		public const long MaxValue = (1 << MixByte.BitCount * MixInstruction.AddressByteCount) - 1;
		public const long MinValue = -MaxValue;
		public const int MaxNameLength = 10;

		private bool mIsDefined;

        public long Magnitude { get; private set; }
        public Word.Signs Sign { get; private set; }

        public ValueSymbol(string name)
			: base(name)
		{
			mIsDefined = false;
			Sign = Word.Signs.Positive;
		}

		public override long GetValue(int currentAddress)
		{
			return Value;
		}

		public override long GetMagnitude(int currentAddress)
		{
			return Magnitude;
		}

		public override Word.Signs GetSign(int currentAddress)
		{
			return Sign;
		}

		public override bool IsValueDefined(int currentAddress)
		{
			return mIsDefined;
		}

		public static bool IsValueSymbolName(string text)
		{
			if (text.Length == 0 || text.Length > MaxNameLength)
			{
				return false;
			}

			bool letterFound = false;

			for (int i = 0; i < text.Length; i++)
			{
				if (char.IsLetter(text[i]))
				{
					letterFound = true;
				}
				else if (!char.IsNumber(text[i]))
				{
					return false;
				}
			}

			return letterFound;
		}

		public static SymbolBase ParseDefinition(string text)
		{
			if (!IsValueSymbolName(text))
			{
				return null;
			}

			return new ValueSymbol(text);
		}


		public static SymbolBase ParseDefinition(string text, int sectionCharIndex, ParsingStatus status)
		{
			SymbolBase symbol = LocalSymbol.ParseDefinition(text, sectionCharIndex, status);

			if (symbol != null)
			{
				return symbol;
			}

			return ParseDefinition(text);
		}

		public static IValue ParseValue(string text, int sectionCharIndex, ParsingStatus status)
		{
			IValue symbolValue = LocalSymbol.ParseValue(text, sectionCharIndex, status);

			if (symbolValue != null)
			{
				return symbolValue;
			}

			if (!IsValueSymbolName(text))
			{
				return null;
			}

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
			mIsDefined = true;
		}

		public override void SetValue(Word.Signs sign, long magnitude)
		{
			Sign = sign;
			Magnitude = magnitude;
			mIsDefined = true;
		}

		public override bool IsSymbolDefined
		{
			get
			{
				return mIsDefined;
			}
		}

		public long Value
		{
			get
			{
				return Sign.ApplyTo(Magnitude);
			}
		}
	}
}
