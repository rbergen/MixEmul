using MixAssembler.Value;
using MixLib.Type;

namespace MixAssembler.Symbol
{
	public class LiteralConstantSymbol : SymbolBase
	{
		private long mLiteralMagnitude;
		private Word.Signs mLiteralSign;
		private long mMagnitude;
		private Word.Signs mSign;
		private bool mValueDefined;

		private LiteralConstantSymbol(Word.Signs literalSign, long literalMagnitude, string name)
			: base(name)
		{
			mLiteralSign = literalSign;
			mLiteralMagnitude = literalMagnitude;
			mMagnitude = -1L;
			mSign = Word.Signs.Positive;
			mValueDefined = false;
		}

		private static string getName(Word.Signs literalSign, long literalMagnitude, int count)
		{
			return string.Concat("=", (literalSign.IsNegative() ? "-" : ""), literalMagnitude, '=', count);
		}

		public override long GetValue(int currentAddress)
		{
			return mSign.ApplyTo(mMagnitude);
		}

		public override long GetMagnitude(int currentAddress)
		{
			return mMagnitude;
		}

		public override Word.Signs GetSign(int currentAddress)
		{
			return mSign;
		}

		public override bool IsValueDefined(int currentAddress)
		{
			return mValueDefined;
		}

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

		public override bool IsSymbolDefined
		{
			get
			{
				return mValueDefined;
			}
		}

		public override long MemoryWordMagnitude
		{
			get
			{
				return mLiteralMagnitude;
			}
		}

		public override Word.Signs MemoryWordSign
		{
			get
			{
				return mLiteralSign;
			}
		}

		public override long MemoryWordValue
		{
			get
			{
				return mLiteralSign.ApplyTo(mLiteralMagnitude);
			}
		}
	}
}
