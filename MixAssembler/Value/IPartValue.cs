using MixLib.Type;

namespace MixAssembler.Value
{
	/// <summary>
	/// This class represents the index part of an instruction's address section
	/// </summary>
	public static class IPartValue
	{
		public const char TagCharacter = ',';

		public static IValue ParseValue(string text, int sectionCharIndex, ParsingStatus status)
		{
			// an empty index is equal to 0
			if (text.Length == 0)
			{
				return new NumberValue(0L);
			}

			// it can only be an index if it starts with a comma
			if (text[0] == TagCharacter)
			{
				// the actual fieldspec can be any expression...
				IValue expression = ExpressionValue.ParseValue(text.Substring(1), sectionCharIndex + 1, status);

				if (expression == null)
				{
					return null;
				}

				long value = expression.GetValue(status.LocationCounter);

				// ... as long as it is within the range of valid MIX byte values
				if (value >= MixByte.MinValue && value <= MixByte.MaxValue)
				{
					return expression;
				}

				status.ReportParsingError(sectionCharIndex, text.Length, "index value invalid");
			}

			return null;
		}
	}
}
