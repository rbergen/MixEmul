using MixLib.Type;

namespace MixLib.Tests.Type;

public class ExtensionMethodsTests
{
	[Theory]
	[InlineData(Word.Signs.Positive, 100L, 100L)]
	[InlineData(Word.Signs.Negative, 100L, -100L)]
	[InlineData(Word.Signs.Positive, 0L, 0L)]
	[InlineData(Word.Signs.Negative, 0L, 0L)]
	public void ApplyTo_ShouldApplySignToMagnitude(Word.Signs sign, long magnitude, long expected)
	{
		var result = sign.ApplyTo(magnitude);

		result.Should().Be(expected);
	}

	[Theory]
	[InlineData(100L, Word.Signs.Positive, 100L)]
	[InlineData(100L, Word.Signs.Negative, -100L)]
	public void Apply_ShouldApplySignToMagnitude(long magnitude, Word.Signs sign, long expected)
	{
		var result = magnitude.Apply(sign);

		result.Should().Be(expected);
	}

	[Theory]
	[InlineData(42L, Word.Signs.Positive)]
	[InlineData(-42L, Word.Signs.Negative)]
	[InlineData(0L, Word.Signs.Positive)]
	public void GetSign_Long_ShouldReturnCorrectSign(long value, Word.Signs expectedSign)
	{
		var result = value.GetSign();

		result.Should().Be(expectedSign);
	}

	[Theory]
	[InlineData(42L, 42L)]
	[InlineData(-42L, 42L)]
	[InlineData(0L, 0L)]
	public void GetMagnitude_Long_ShouldReturnAbsoluteValue(long value, long expected)
	{
		var result = value.GetMagnitude();

		result.Should().Be(expected);
	}

	[Theory]
	[InlineData(42, 42)]
	[InlineData(-42, 42)]
	[InlineData(0, 0)]
	public void GetMagnitude_Int_ShouldReturnAbsoluteValue(int value, int expected)
	{
		var result = value.GetMagnitude();

		result.Should().Be(expected);
	}

	[Theory]
	[InlineData(42.5, Word.Signs.Positive)]
	[InlineData(-42.5, Word.Signs.Negative)]
	[InlineData(0.0, Word.Signs.Positive)]
	public void GetSign_Decimal_ShouldReturnCorrectSign(double valueDouble, Word.Signs expectedSign)
	{
		var value = (decimal)valueDouble;
		var result = value.GetSign();

		result.Should().Be(expectedSign);
	}

	[Theory]
	[InlineData(42.5, 42.5)]
	[InlineData(-42.5, 42.5)]
	[InlineData(0.0, 0.0)]
	public void GetMagnitude_Decimal_ShouldReturnAbsoluteValue(double valueDouble, double expectedDouble)
	{
		var value = (decimal)valueDouble;
		var expected = (decimal)expectedDouble;

		var result = value.GetMagnitude();

		result.Should().Be(expected);
	}

	[Theory]
	[InlineData(Word.Signs.Positive, Word.Signs.Negative)]
	[InlineData(Word.Signs.Negative, Word.Signs.Positive)]
	public void Invert_ShouldInvertSign(Word.Signs sign, Word.Signs expectedSign)
	{
		var result = sign.Invert();

		result.Should().Be(expectedSign);
	}

	[Theory]
	[InlineData(Word.Signs.Positive, '+')]
	[InlineData(Word.Signs.Negative, '-')]
	public void ToChar_Sign_ShouldReturnCorrectCharacter(Word.Signs sign, char expectedChar)
	{
		var result = sign.ToChar();

		result.Should().Be(expectedChar);
	}

	[Theory]
	[InlineData(Word.Signs.Positive, true)]
	[InlineData(Word.Signs.Negative, false)]
	public void IsPositive_ShouldReturnCorrectValue(Word.Signs sign, bool expected)
	{
		var result = sign.IsPositive();

		result.Should().Be(expected);
	}

	[Theory]
	[InlineData(Word.Signs.Positive, false)]
	[InlineData(Word.Signs.Negative, true)]
	public void IsNegative_ShouldReturnCorrectValue(Word.Signs sign, bool expected)
	{
		var result = sign.IsNegative();

		result.Should().Be(expected);
	}

	[Theory]
	[InlineData('+', Word.Signs.Positive)]
	[InlineData('-', Word.Signs.Negative)]
	[InlineData('x', Word.Signs.Positive)] // Non-minus defaults to positive
	public void ToSign_Char_ShouldReturnCorrectSign(char ch, Word.Signs expectedSign)
	{
		var result = ch.ToSign();

		result.Should().Be(expectedSign);
	}

	[Theory]
	[InlineData(Registers.CompValues.Equal, Registers.CompValues.Greater)]
	[InlineData(Registers.CompValues.Greater, Registers.CompValues.Less)]
	[InlineData(Registers.CompValues.Less, Registers.CompValues.Equal)]
	public void Next_CompValues_ShouldCycle(Registers.CompValues current, Registers.CompValues expected)
	{
		var result = current.Next();

		result.Should().Be(expected);
	}

	[Theory]
	[InlineData(Registers.CompValues.Less, '<')]
	[InlineData(Registers.CompValues.Equal, '=')]
	[InlineData(Registers.CompValues.Greater, '>')]
	public void ToChar_CompValues_ShouldReturnCorrectCharacter(Registers.CompValues compValue, char expectedChar)
	{
		var result = compValue.ToChar();

		result.Should().Be(expectedChar);
	}

	[Theory]
	[InlineData(-1, Registers.CompValues.Less)]
	[InlineData(0, Registers.CompValues.Equal)]
	[InlineData(1, Registers.CompValues.Greater)]
	[InlineData(-100, Registers.CompValues.Less)]
	[InlineData(100, Registers.CompValues.Greater)]
	public void ToCompValue_ShouldConvertIntToCompValue(int value, Registers.CompValues expected)
	{
		var result = value.ToCompValue();

		result.Should().Be(expected);
	}
}
