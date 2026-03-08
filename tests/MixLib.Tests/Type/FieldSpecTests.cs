using MixLib.Type;

namespace MixLib.Tests.Type;

public class FieldSpecTests
{
	[Theory]
	[InlineData(0, 5, true)]
	[InlineData(1, 5, true)]
	[InlineData(0, 0, true)]
	[InlineData(3, 5, true)]
	[InlineData(1, 1, true)]
	[InlineData(0, 6, true)] // Special case for floating point
	public void Constructor_WithValidBounds_ShouldCreateFieldSpec(int lowBound, int highBound, bool shouldBeValid)
	{
		var fieldSpec = new FieldSpec(lowBound, highBound);

		fieldSpec.Should().NotBeNull();
		fieldSpec.LowBound.Should().Be(lowBound);
		fieldSpec.HighBound.Should().Be(highBound);
		fieldSpec.IsValid.Should().Be(shouldBeValid);
	}

	[Theory]
	[InlineData(6, 5)] // Low > high
	[InlineData(-1, 5)] // Low < 0
	[InlineData(0, 7)] // High > 6 (not floating point)
	[InlineData(1, 6)] // High = 6 but low != 0
	public void Constructor_WithInvalidBounds_ShouldThrowArgumentException(int lowBound, int highBound)
	{
		var act = () => new FieldSpec(lowBound, highBound);

		act.Should().Throw<ArgumentException>()
			.WithMessage("low and/or high bounds are invalid");
	}

	[Fact]
	public void Constructor_WithMixByte_ShouldSetMixByteValue()
	{
		var mixByte = new MixByte(13); // 1:5
		var fieldSpec = new FieldSpec(mixByte);

		fieldSpec.MixByteValue.ByteValue.Should().Be(13);
		fieldSpec.LowBound.Should().Be(1);
		fieldSpec.HighBound.Should().Be(5);
	}

	[Fact]
	public void IncludesSign_WithLowBoundZero_ShouldBeTrue()
	{
		var fieldSpec = new FieldSpec(0, 5);

		fieldSpec.IncludesSign.Should().BeTrue();
	}

	[Fact]
	public void IncludesSign_WithLowBoundNotZero_ShouldBeFalse()
	{
		var fieldSpec = new FieldSpec(1, 5);

		fieldSpec.IncludesSign.Should().BeFalse();
	}

	[Theory]
	[InlineData(0, 5, 0, 4)] // Sign + all bytes
	[InlineData(1, 5, 0, 4)] // All bytes, no sign
	[InlineData(3, 5, 2, 4)] // Partial field
	[InlineData(0, 0, 0, -1)] // Sign only -> special case
	public void ByteIndices_ShouldCalculateCorrectly(int lowBound, int highBound, int expectedLowIndex, int expectedHighIndex)
	{
		var fieldSpec = new FieldSpec(lowBound, highBound);

		fieldSpec.LowBoundByteIndex.Should().Be(expectedLowIndex);
		if (expectedHighIndex >= 0)
		{
			fieldSpec.HighBoundByteIndex.Should().Be(expectedHighIndex);
		}
	}

	[Theory]
	[InlineData(0, 5, 5)]
	[InlineData(1, 5, 5)]
	[InlineData(3, 5, 3)]
	[InlineData(1, 1, 1)]
	public void ByteCount_ShouldReturnCorrectCount(int lowBound, int highBound, int expectedCount)
	{
		var fieldSpec = new FieldSpec(lowBound, highBound);

		fieldSpec.ByteCount.Should().Be(expectedCount);
	}

	[Fact]
	public void FloatingPoint_WithBounds0And6_ShouldBeTrue()
	{
		var fieldSpec = new FieldSpec(0, 6);

		fieldSpec.FloatingPoint.Should().BeTrue();
	}

	[Theory]
	[InlineData(0, 5)]
	[InlineData(1, 5)]
	[InlineData(1, 1)]
	public void FloatingPoint_WithOtherBounds_ShouldBeFalse(int lowBound, int highBound)
	{
		var fieldSpec = new FieldSpec(lowBound, highBound);

		fieldSpec.FloatingPoint.Should().BeFalse();
	}

	[Theory]
	[InlineData(0, 5, "(0:5)")]
	[InlineData(1, 5, "(1:5)")]
	[InlineData(3, 3, "(3:3)")]
	public void ToString_WithValidSpec_ShouldFormatCorrectly(int lowBound, int highBound, string expected)
	{
		var fieldSpec = new FieldSpec(lowBound, highBound);

		fieldSpec.ToString().Should().Be(expected);
	}

	[Fact]
	public void Equals_WithSameValues_ShouldReturnTrue()
	{
		var fieldSpec1 = new FieldSpec(1, 5);
		var fieldSpec2 = new FieldSpec(1, 5);

		fieldSpec1.Equals(fieldSpec2).Should().BeTrue();
		(fieldSpec1 == fieldSpec2).Should().BeTrue();
	}

	[Fact]
	public void Equals_WithDifferentValues_ShouldReturnFalse()
	{
		var fieldSpec1 = new FieldSpec(1, 5);
		var fieldSpec2 = new FieldSpec(0, 5);

		fieldSpec1.Equals(fieldSpec2).Should().BeFalse();
		(fieldSpec1 != fieldSpec2).Should().BeTrue();
	}

	[Fact]
	public void GetHashCode_WithSameValues_ShouldReturnSameHash()
	{
		var fieldSpec1 = new FieldSpec(1, 5);
		var fieldSpec2 = new FieldSpec(1, 5);

		fieldSpec1.GetHashCode().Should().Be(fieldSpec2.GetHashCode());
	}

	[Theory]
	[InlineData(0, 5, true)]
	[InlineData(1, 5, true)]
	[InlineData(0, 6, true)]
	[InlineData(6, 5, false)]
	[InlineData(-1, 5, false)]
	public void IsValidFieldSpec_StaticMethod_ShouldValidateCorrectly(int lowBound, int highBound, bool expectedValid)
	{
		var isValid = FieldSpec.IsValidFieldSpec(lowBound, highBound);

		isValid.Should().Be(expectedValid);
	}

	[Theory]
	[InlineData(13, true)]  // 1:5
	[InlineData(5, true)]   // 0:5
	[InlineData(63, false)] // 7:7 - invalid
	public void IsValidFieldSpec_WithMixByte_ShouldValidateCorrectly(byte value, bool expectedValid)
	{
		var mixByte = new MixByte(value);
		var isValid = FieldSpec.IsValidFieldSpec(mixByte);

		isValid.Should().Be(expectedValid);
	}

	[Fact]
	public void LowBound_WithInvalidFieldSpec_ShouldThrowInvalidOperationException()
	{
		var invalidMixByte = new MixByte(63); // 7:7
		var fieldSpec = new FieldSpec(invalidMixByte);

		var act = () => fieldSpec.LowBound;

		act.Should().Throw<InvalidOperationException>()
			.WithMessage("this MixFieldSpec is not valid");
	}

	[Fact]
	public void HighBound_WithInvalidFieldSpec_ShouldThrowInvalidOperationException()
	{
		var invalidMixByte = new MixByte(63); // 7:7
		var fieldSpec = new FieldSpec(invalidMixByte);

		var act = () => fieldSpec.HighBound;

		act.Should().Throw<InvalidOperationException>()
			.WithMessage("this MixFieldSpec is not valid");
	}

	[Fact]
	public void MixByteValue_ShouldEncodeFieldSpecCorrectly()
	{
		var fieldSpec = new FieldSpec(1, 5);

		fieldSpec.MixByteValue.ByteValue.Should().Be(13); // (1 * 8) + 5 = 13
	}

	[Theory]
	[InlineData(0, 0, 0)]
	[InlineData(1, 5, 13)]
	[InlineData(0, 5, 5)]
	[InlineData(3, 3, 27)]
	public void MixByteValue_ForVariousBounds_ShouldEncodeCorrectly(int lowBound, int highBound, byte expectedValue)
	{
		var fieldSpec = new FieldSpec(lowBound, highBound);

		fieldSpec.MixByteValue.ByteValue.Should().Be(expectedValue);
	}
}
