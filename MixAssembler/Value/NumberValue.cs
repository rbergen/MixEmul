using System.Linq;
using MixLib.Type;

namespace MixAssembler.Value
{
	/// <summary>
	/// This class represents a literal numeric value
	/// </summary>
	public class NumberValue : IValue
	{
		public long Magnitude { get; set; }
		public Word.Signs Sign { get; set; }

		public NumberValue(long value)
		{
			if (value < 0)
			{
				Sign = Word.Signs.Negative;
				value = -value;
			}
			else
				Sign = Word.Signs.Positive;

			Magnitude = value;

		}

		public NumberValue(Word.Signs sign, long magnitude)
		{
			Sign = sign;
			Magnitude = magnitude;
		}

		public virtual long GetValue(int currentAddress)
			=> Sign.ApplyTo(Magnitude);

		public virtual bool IsValueDefined(int currentAddress)
			=> true;

		public long GetMagnitude(int currentAddress)
			=> Magnitude;

		public Word.Signs GetSign(int currentAddress)
			=> Sign;

		public static IValue ParseValue(string text, int sectionCharIndex, ParsingStatus status)
			=> text.Length != 0 && text.Length <= 10 && text.All(c => char.IsNumber(c)) && long.TryParse(text, out long longValue)
			? new NumberValue(longValue)
			: null;
	}
}
