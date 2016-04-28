using MixLib.Type;

namespace MixAssembler.Value
{
	// This class represents the fieldspec part of an instruction's address section
	public static class FPartValue
	{
		public const long Default = long.MinValue;
		public const char TagCharacter = '(';

		public static IValue ParseValue(string text, int sectionCharIndex, ParsingStatus status)
		{
			// an empty fieldspec means we use the default
			if (text.Length == 0) return new NumberValue(Default);

			// it can only be a fieldspec if it starts and ends with brackets
			if (text.Length >= 2 && text[0] == TagCharacter && text[text.Length - 1] == ')')
			{
				// the actual fieldspec can be any expression...
				IValue expression = ExpressionValue.ParseValue(text.Substring(1, text.Length - 2), sectionCharIndex + 1, status);

				if (expression == null) return null;

				long value = expression.GetValue(status.LocationCounter);

				// ... as long as it is within the range of valid MIX byte values
				if (value >= MixByte.MinValue && value <= MixByte.MaxValue) return expression;

				status.ReportParsingError(sectionCharIndex, text.Length, "field value invalid");
			}

			return null;
		}
	}
}
