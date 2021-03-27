using MixAssembler.Value;
using MixLib.Type;

namespace MixAssembler.Symbol
{
	public class LiteralConstantSymbol : SymbolBase
	{
		private readonly long _literalMagnitude;
		private readonly Word.Signs _literalSign;
		private long _magnitude;
		private Word.Signs _sign;
		private bool _valueDefined;

		private LiteralConstantSymbol(Word.Signs literalSign, long literalMagnitude, string name) : base(name)
		{
			_literalSign = literalSign;
			_literalMagnitude = literalMagnitude;
			_magnitude = -1L;
			_sign = Word.Signs.Positive;
			_valueDefined = false;
		}

		public override bool IsSymbolDefined
			=> _valueDefined;

		public override long MemoryWordMagnitude
			=> _literalMagnitude;

		public override Word.Signs MemoryWordSign
			=> _literalSign;

		public override long MemoryWordValue
			=> _literalSign.ApplyTo(_literalMagnitude);

		public override long GetValue(int currentAddress)
			=> _sign.ApplyTo(_magnitude);

		public override long GetMagnitude(int currentAddress)
			=> _magnitude;

		public override Word.Signs GetSign(int currentAddress)
			=> _sign;

		public override bool IsValueDefined(int currentAddress)
			=> _valueDefined;

		private static string GetName(Word.Signs literalSign, long literalMagnitude, int count)
			=> string.Concat("=", literalSign.IsNegative() ? "-" : "", literalMagnitude, '=', count);

		public static IValue ParseValue(string text, int sectionCharIndex, ParsingStatus status)
		{
			if (text.Length < 2 || text[0] != '=' || text[^1] != '=')
				return null;

			var expressionValue = WValue.ParseValue(text[1..^1], sectionCharIndex + 1, status);

			if (expressionValue == null)
				return null;

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
			_sign = value.GetSign();
			_magnitude = value.GetMagnitude();
			_valueDefined = true;
		}

		public override void SetValue(Word.Signs sign, long magnitude)
		{
			_sign = sign;
			_magnitude = magnitude;
			_valueDefined = true;
		}
	}
}
