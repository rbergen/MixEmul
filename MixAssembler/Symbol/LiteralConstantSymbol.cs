using MixAssembler.Value;
using MixLib.Type;

namespace MixAssembler.Symbol
{
	public class LiteralConstantSymbol : SymbolBase
	{
        long mLiteralMagnitude;
        Word.Signs mLiteralSign;
        long mMagnitude;
        Word.Signs mSign;
        bool mValueDefined;

        LiteralConstantSymbol(Word.Signs literalSign, long literalMagnitude, string name)
            : base(name)
        {
            mLiteralSign = literalSign;
            mLiteralMagnitude = literalMagnitude;
            mMagnitude = -1L;
            mSign = Word.Signs.Positive;
            mValueDefined = false;
        }

        public override bool IsSymbolDefined => mValueDefined;

        public override long MemoryWordMagnitude => mLiteralMagnitude;

        public override Word.Signs MemoryWordSign => mLiteralSign;

        public override long MemoryWordValue => mLiteralSign.ApplyTo(mLiteralMagnitude);

        public override long GetValue(int currentAddress) => mSign.ApplyTo(mMagnitude);

		public override long GetMagnitude(int currentAddress) => mMagnitude;

		public override Word.Signs GetSign(int currentAddress) => mSign;

		public override bool IsValueDefined(int currentAddress) => mValueDefined;

        static string getName(Word.Signs literalSign, long literalMagnitude, int count) =>
            string.Concat("=", (literalSign.IsNegative() ? "-" : ""), literalMagnitude, '=', count);

        public static IValue ParseValue(string text, int sectionCharIndex, ParsingStatus status)
		{
			if (text.Length < 2 || text[0] != '=' || text[text.Length - 1] != '=')
			{
				return null;
			}

			IValue expressionValue = WValue.ParseValue(text.Substring(1, text.Length - 2), sectionCharIndex + 1, status);
			if (expressionValue == null)
			{
				return null;
			}

			long literalMagnitude = expressionValue.GetMagnitude(status.LocationCounter);
			Word.Signs literalSign = expressionValue.GetSign(status.LocationCounter);

			int count = 0;
			string name = getName(literalSign, literalMagnitude, count);

			while (status.Symbols[name] != null)
			{
				count++;
				name = getName(literalSign, literalMagnitude, count);
			}

			SymbolBase literalConstantSymbol = new LiteralConstantSymbol(literalSign, literalMagnitude, name);
			status.Symbols.Add(literalConstantSymbol);
			return literalConstantSymbol;
		}

		public override void SetValue(long value)
		{
            mSign = value.GetSign();
			mMagnitude = value.GetMagnitude();
			mValueDefined = true;
		}

		public override void SetValue(Word.Signs sign, long magnitude)
		{
			mSign = sign;
			mMagnitude = magnitude;
			mValueDefined = true;
		}
	}
}
