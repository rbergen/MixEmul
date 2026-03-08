using MixAssembler.Value;
using MixLib.Type;

namespace MixAssembler.Tests.Value;

public class NumberValueTests
{
	[Theory]
	[InlineData(0L, Word.Signs.Positive, 0L)]
	[InlineData(42L, Word.Signs.Positive, 42L)]
	[InlineData(100L, Word.Signs.Positive, 100L)]
	[InlineData(-42L, Word.Signs.Negative, 42L)]
	[InlineData(-100L, Word.Signs.Negative, 100L)]
	public void Constructor_WithLongValue_ShouldSetSignAndMagnitude(long value, Word.Signs expectedSign, long expectedMagnitude)
	{
		var numberValue = new NumberValue(value);

		numberValue.Sign.Should().Be(expectedSign);
		numberValue.Magnitude.Should().Be(expectedMagnitude);
	}

	[Theory]
	[InlineData(Word.Signs.Positive, 42L)]
	[InlineData(Word.Signs.Negative, 100L)]
	[InlineData(Word.Signs.Positive, 0L)]
	public void Constructor_WithSignAndMagnitude_ShouldSetProperties(Word.Signs sign, long magnitude)
	{
		var numberValue = new NumberValue(sign, magnitude);

		numberValue.Sign.Should().Be(sign);
		numberValue.Magnitude.Should().Be(magnitude);
	}

	[Theory]
	[InlineData(Word.Signs.Positive, 42L, 42L)]
	[InlineData(Word.Signs.Negative, 42L, -42L)]
	[InlineData(Word.Signs.Positive, 0L, 0L)]
	[InlineData(Word.Signs.Negative, 0L, 0L)]
	public void GetValue_ShouldApplySignToMagnitude(Word.Signs sign, long magnitude, long expectedValue)
	{
		var numberValue = new NumberValue(sign, magnitude);

		var result = numberValue.GetValue(0);

		result.Should().Be(expectedValue);
	}

	[Fact]
	public void GetValue_CurrentAddressParameter_IsIgnored()
	{
		var numberValue = new NumberValue(42);

		var result1 = numberValue.GetValue(0);
		var result2 = numberValue.GetValue(1000);
		var result3 = numberValue.GetValue(-500);

		result1.Should().Be(result2).And.Be(result3);
	}

	[Fact]
	public void IsValueDefined_ShouldAlwaysReturnTrue()
	{
		var numberValue = new NumberValue(42);

		numberValue.IsValueDefined(0).Should().BeTrue();
		numberValue.IsValueDefined(100).Should().BeTrue();
		numberValue.IsValueDefined(-100).Should().BeTrue();
	}

	[Theory]
	[InlineData(42L, 42L)]
	[InlineData(-42L, 42L)]
	[InlineData(0L, 0L)]
	public void GetMagnitude_ShouldReturnMagnitude(long value, long expectedMagnitude)
	{
		var numberValue = new NumberValue(value);

		var result = numberValue.GetMagnitude(0);

		result.Should().Be(expectedMagnitude);
	}

	[Theory]
	[InlineData(42L, Word.Signs.Positive)]
	[InlineData(-42L, Word.Signs.Negative)]
	[InlineData(0L, Word.Signs.Positive)]
	public void GetSign_ShouldReturnSign(long value, Word.Signs expectedSign)
	{
		var numberValue = new NumberValue(value);

		var result = numberValue.GetSign(0);

		result.Should().Be(expectedSign);
	}

	[Theory]
	[InlineData("0")]
	[InlineData("42")]
	[InlineData("100")]
	[InlineData("1234567890")] // Exactly 10 digits - max length
	public void ParseValue_WithValidNumericString_ShouldReturnNumberValue(string text)
	{
		var result = NumberValue.ParseValue(text, 0, null);

		result.Should().NotBeNull();
		result.Should().BeOfType<NumberValue>();
		((NumberValue)result).Magnitude.Should().Be(long.Parse(text));
	}

	[Theory]
	[InlineData("")] // Empty
	[InlineData("12345678901")] // 11 digits - too long
	[InlineData("abc")] // Non-numeric
	[InlineData("12a34")] // Mixed
	[InlineData("-42")] // Negative sign
	[InlineData("+42")] // Positive sign
	[InlineData("1 2 3")] // Spaces
	public void ParseValue_WithInvalidString_ShouldReturnNull(string text)
	{
		var result = NumberValue.ParseValue(text, 0, null);

		result.Should().BeNull();
	}

	[Fact]
	public void ParseValue_WithTenDigits_ShouldHandleCorrectly()
	{
		var tenDigits = "1234567890";

		var result = NumberValue.ParseValue(tenDigits, 0, null);

		result.Should().NotBeNull();
		((NumberValue)result).Magnitude.Should().Be(1234567890L);
	}

	[Fact]
	public void ParseValue_SectionCharIndexParameter_IsIgnored()
	{
		var result1 = NumberValue.ParseValue("42", 0, null);
		var result2 = NumberValue.ParseValue("42", 100, null);

		result1.Should().NotBeNull();
		result2.Should().NotBeNull();
		((NumberValue)result1).Magnitude.Should().Be(((NumberValue)result2).Magnitude);
	}

	[Theory]
	[InlineData("0", 0L)]
	[InlineData("00042", 42L)]
	[InlineData("0000000001", 1L)]
	public void ParseValue_WithLeadingZeros_ShouldParseCorrectly(string text, long expectedValue)
	{
		var result = NumberValue.ParseValue(text, 0, null);

		result.Should().NotBeNull();
		((NumberValue)result).Magnitude.Should().Be(expectedValue);
	}
}
