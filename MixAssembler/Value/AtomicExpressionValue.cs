using MixAssembler.Symbol;
using MixLib.Type;

namespace MixAssembler.Value
{
	/// <summary>
	/// This class represents an atomic expression (number, location counter or value symbol)
	/// </summary>
	public static class AtomicExpressionValue
	{
		public static IValue ParseValue(string text, int sectionCharIndex, ParsingStatus status)
		{
			IValue value = null;

			value = NumberValue.ParseValue(text, sectionCharIndex, status);

			if (value == null)
			{
				value = LocationCounterValue.ParseValue(text, sectionCharIndex, status);
				if (value != null) return value;

				value = ValueSymbol.ParseValue(text, sectionCharIndex, status);
				if (value != null && !value.IsValueDefined(status.LocationCounter))
				{
					status.ReportParsingError(sectionCharIndex, text.Length, "symbol " + text + " not defined");
				}
			}

			return value;
		}
	}
}
