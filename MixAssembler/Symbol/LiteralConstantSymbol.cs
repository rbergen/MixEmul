using MixAssembler.Value;
using MixLib.Type;

namespace MixAssembler.Symbol
{
	public class LiteralConstantSymbol : SymbolBase
	{
		readonly long mLiteralMagnitude;
		readonly Word.Signs mLiteralSign;
		long mMagnitude;
		Word.Signs mSign;
		bool mValueDefined;

		LiteralConstantSymbol(Word.Signs literalSign, long literalMagnitude, string name) : base(name)
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

		static string GetName(Word.Signs literalSign, long literalMagnitude, int count)
		{
			return string.Concat("=", (literalSign.IsNegative() ? "-" : ""), literalMagnitude, '=', count);
		}

		public static IValue ParseValue(string text, int sectionCharIndex, ParsingStatus status)
		{
			if (text.Length < 2 || text[0] != '=' || text[text.Length - 1] != '=')
			{
				return null;
			}

			var expressionValue = WValue.ParseValue(text.Substring(1, text.Length - 2), sectionCharIndex + 1, status);
			if (expressionValue == null)
			{
				return null;
			}

			var literalMagnitude = expressionValue.GetMagnitude(status.LocationCounter);
			var literalSign = expressionValue.GetSign(status.LocationCounter);

			int count = 0;
			var name = GetName(literalSign, literalMagnitude, count);

			while (status.Symbols[name] != null)
			{
				count++;
				name = GetName(literalSign, literalMagnitude, count);
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
