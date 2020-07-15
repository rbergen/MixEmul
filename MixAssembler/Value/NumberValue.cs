using MixLib.Type;
using System;

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
			{
				Sign = Word.Signs.Positive;
			}

			Magnitude = value;

		}

		public NumberValue(Word.Signs sign, long magnitude)
		{
			Sign = sign;
			Magnitude = magnitude;
		}

		public virtual long GetValue(int currentAddress) => Sign.ApplyTo(Magnitude);

		public virtual bool IsValueDefined(int currentAddress) => true;

		public long GetMagnitude(int currentAddress) => Magnitude;

		public Word.Signs GetSign(int currentAddress) => Sign;

		public static IValue ParseValue(string text, int sectionCharIndex, ParsingStatus status)
		{
			if (text.Length != 0 && text.Length <= 10)
			{
				// all characters must be numeric for this to be a literal numeric value
				for (int i = 0; i < text.Length; i++)
				{
					if (!char.IsNumber(text[i])) return null;
				}

				// it seems to be numeric, so we just parse it
				try
				{
					return new NumberValue(long.Parse(text));
				}
				catch (FormatException)
				{
				}
			}

			return null;
		}
	}
}
