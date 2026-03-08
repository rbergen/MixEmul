using MixLib.Type;

namespace MixLib.Tests.Type;

public class MixByteTests
{
	[Fact]
	public void Constructor_DefaultConstructor_ShouldCreateZeroValue()
	{
		var mixByte = new MixByte();

		mixByte.ByteValue.Should().Be(0);
	}

	[Theory]
	[InlineData(0)]
	[InlineData(1)]
	[InlineData(32)]
	[InlineData(63)]
	public void Constructor_WithValidByteValue_ShouldSetValue(byte value)
	{
		var mixByte = new MixByte(value);

		mixByte.ByteValue.Should().Be(value);
	}

	[Fact]
	public void Constructor_WithValueGreaterThanMaxValue_ShouldThrowArgumentException()
	{
		byte invalidValue = MixByte.MaxValue + 1;

		var act = () => new MixByte(invalidValue);

		act.Should().Throw<ArgumentException>()
			.WithParameterName("value")
			.WithMessage("value too large for MixByte*");
	}

	[Theory]
	[InlineData(' ', 0)]
	[InlineData('A', 1)]
	[InlineData('I', 9)]
	[InlineData('0', 30)]
	[InlineData('9', 39)]
	public void Constructor_WithValidChar_ShouldSetCorrectByteValue(char ch, byte expectedValue)
	{
		var mixByte = new MixByte(ch);

		mixByte.ByteValue.Should().Be(expectedValue);
	}

	[Fact]
	public void Constructor_WithUnknownChar_ShouldSetMaxValue()
	{
		var mixByte = new MixByte('~');

		mixByte.ByteValue.Should().Be(MixByte.MaxValue);
	}

	[Theory]
	[InlineData(0, ' ')]
	[InlineData(1, 'A')]
	[InlineData(9, 'I')]
	[InlineData(30, '0')]
	[InlineData(39, '9')]
	public void CharValue_ShouldReturnCorrectCharacter(byte value, char expectedChar)
	{
		var mixByte = new MixByte(value);

		mixByte.CharValue.Should().Be(expectedChar);
	}

	[Fact]
	public void CharValue_WithValueGreaterThanCharsLength_ShouldReturnLastChar()
	{
		var mixByte = new MixByte(MixByte.MaxValue);

		mixByte.CharValue.Should().Be(MixByte.MixChars[^1]);
	}

	[Theory]
	[InlineData(10, 5, 15)]
	[InlineData(0, 10, 10)]
	[InlineData(63, 0, 63)]
	public void AddOperator_ShouldReturnCorrectSum(byte byteValue, byte delta, int expected)
	{
		var mixByte = new MixByte(byteValue);

		var result = mixByte + delta;

		result.Should().Be(expected);
	}

	[Theory]
	[InlineData(20, 4, 5)]
	[InlineData(63, 1, 63)]
	[InlineData(0, 1, 0)]
	public void DivideOperator_ShouldReturnCorrectQuotient(byte byteValue, byte divisor, int expected)
	{
		var mixByte = new MixByte(byteValue);

		var result = mixByte / divisor;

		result.Should().Be(expected);
	}

	[Theory]
	[InlineData(10, 5, 5)]
	[InlineData(20, 3, 17)]
	[InlineData(5, 5, 0)]
	public void SubtractOperator_ShouldReturnCorrectDifference(byte byteValue, byte delta, int expected)
	{
		var mixByte = new MixByte(byteValue);

		var result = mixByte - delta;

		result.Should().Be(expected);
	}

	[Theory]
	[InlineData(20, 6, 2)]
	[InlineData(63, 10, 3)]
	public void ModuloOperator_ShouldReturnCorrectRemainder(byte byteValue, byte divisor, int expected)
	{
		var mixByte = new MixByte(byteValue);

		var result = mixByte % divisor;

		result.Should().Be(expected);
	}

	[Fact]
	public void EqualityOperator_WithEqualValues_ShouldReturnTrue()
	{
		var mixByte = new MixByte(42);

		(mixByte == 42).Should().BeTrue();
	}

	[Fact]
	public void EqualityOperator_WithDifferentValues_ShouldReturnFalse()
	{
		var mixByte = new MixByte(42);

		(mixByte == 10).Should().BeFalse();
	}

	[Fact]
	public void InequalityOperator_WithDifferentValues_ShouldReturnTrue()
	{
		var mixByte = new MixByte(42);

		(mixByte != 10).Should().BeTrue();
	}

	[Fact]
	public void GreaterThanOperator_ShouldCompareCorrectly()
	{
		var mixByte = new MixByte(42);

		(mixByte > 10).Should().BeTrue();
		(mixByte > 42).Should().BeFalse();
		(mixByte > 50).Should().BeFalse();
	}

	[Fact]
	public void LessThanOperator_ShouldCompareCorrectly()
	{
		var mixByte = new MixByte(42);

		(mixByte < 50).Should().BeTrue();
		(mixByte < 42).Should().BeFalse();
		(mixByte < 10).Should().BeFalse();
	}

	[Fact]
	public void ImplicitConversionToByte_ShouldReturnByteValue()
	{
		var mixByte = new MixByte(42);

		byte result = mixByte;

		result.Should().Be(42);
	}

	[Fact]
	public void ImplicitConversionToChar_ShouldReturnCharValue()
	{
		var mixByte = new MixByte(1);

		char result = mixByte;

		result.Should().Be('A');
	}

	[Fact]
	public void ImplicitConversionToInt_ShouldReturnByteValue()
	{
		var mixByte = new MixByte(42);

		int result = mixByte;

		result.Should().Be(42);
	}

	[Fact]
	public void ImplicitConversionFromByte_ShouldCreateMixByte()
	{
		MixByte mixByte = (byte)42;

		mixByte.ByteValue.Should().Be(42);
	}

	[Fact]
	public void ImplicitConversionFromChar_ShouldCreateMixByte()
	{
		MixByte mixByte = 'A';

		mixByte.ByteValue.Should().Be(1);
	}

	[Fact]
	public void ImplicitConversionFromInt_ShouldCreateMixByte()
	{
		MixByte mixByte = 42;

		mixByte.ByteValue.Should().Be(42);
	}

	[Fact]
	public void ToString_ShouldReturnTwoDigitString()
	{
		var mixByte = new MixByte(5);

		mixByte.ToString().Should().Be("05");
	}

	[Fact]
	public void Equals_WithSameValue_ShouldReturnTrue()
	{
		var mixByte1 = new MixByte(42);
		var mixByte2 = new MixByte(42);

		mixByte1.Equals(mixByte2).Should().BeTrue();
	}

	[Fact]
	public void Equals_WithDifferentValue_ShouldReturnFalse()
	{
		var mixByte1 = new MixByte(42);
		var mixByte2 = new MixByte(10);

		mixByte1.Equals(mixByte2).Should().BeFalse();
	}

	[Fact]
	public void GetHashCode_WithSameValue_ShouldReturnSameHashCode()
	{
		var mixByte1 = new MixByte(42);
		var mixByte2 = new MixByte(42);

		mixByte1.GetHashCode().Should().Be(mixByte2.GetHashCode());
	}

	[Fact]
	public void MaxValue_ShouldBe63()
	{
		MixByte.MaxValue.Should().Be(63);
	}

	[Fact]
	public void MinValue_ShouldBe0()
	{
		MixByte.MinValue.Should().Be(0);
	}

	[Fact]
	public void BitCount_ShouldBe6()
	{
		MixByte.BitCount.Should().Be(6);
	}
}
