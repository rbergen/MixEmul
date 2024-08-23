using MixAssembler.Value;
using MixLib.Type;

namespace MixAssembler.Symbol
{
	public class LiteralConstantSymbol(Word.Signs literalSign, long literalMagnitude, string name) : SymbolBase(name)
	{
		private long magnitude;
		private Word.Signs sign;
		private bool valueDefined;

		public override bool IsSymbolDefined
			=> this.valueDefined;

		public override long MemoryWordMagnitude
			=> literalMagnitude;

		public override Word.Signs MemoryWordSign
			=> literalSign;

		public override long MemoryWordValue
			=> literalSign.ApplyTo(literalMagnitude);

		public override long GetValue(int currentAddress)
			=> this.sign.ApplyTo(this.magnitude);

		public override long GetMagnitude(int currentAddress)
			=> this.magnitude;

		public override Word.Signs GetSign(int currentAddress)
			=> this.sign;

		public override bool IsValueDefined(int currentAddress)
			=> this.valueDefined;

		private static string GetName(Word.Signs literalSign, long literalMagnitude, int count)
			=> string.Concat("=", literalSign.IsNegative() ? "-" : string.Empty, literalMagnitude, '=', count);

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
			this.sign = value.GetSign();
			this.magnitude = value.GetMagnitude();
			this.valueDefined = true;
		}

		public override void SetValue(Word.Signs sign, long magnitude)
		{
			this.sign = sign;
			this.magnitude = magnitude;
			this.valueDefined = true;
		}
	}
}
