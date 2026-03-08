using MixLib.Type;

namespace MixLib.Tests.Type;

public class FullWordTests
{
	[Fact]
	public void Constructor_Default_ShouldCreateZeroValue()
	{
		var word = new FullWord();

		word.LongValue.Should().Be(0);
		word.Sign.Should().Be(Word.Signs.Positive);
		((Word)word).ByteCount.Should().Be(5);
	}

	[Theory]
	[InlineData(0L)]
	[InlineData(1L)]
	[InlineData(100L)]
	[InlineData(-100L)]
	[InlineData(1073741823L)] // Max positive value for 5 bytes (2^30 - 1)
	[InlineData(-1073741823L)]
	public void Constructor_WithLongValue_ShouldSetValue(long value)
	{
		var word = new FullWord(value);

		word.LongValue.Should().Be(value);
	}

	[Theory]
	[InlineData(Word.Signs.Positive, 100L, 100L)]
	[InlineData(Word.Signs.Negative, 100L, -100L)]
	[InlineData(Word.Signs.Positive, 0L, 0L)]
	public void Constructor_WithSignAndMagnitude_ShouldSetCorrectValue(Word.Signs sign, long magnitude, long expectedValue)
	{
		var word = new FullWord(sign, magnitude);

		word.LongValue.Should().Be(expectedValue);
		word.Sign.Should().Be(sign);
	}

	[Fact]
	public void ImplicitConversion_FromLong_ShouldCreateFullWord()
	{
		FullWord word = 42L;

		word.LongValue.Should().Be(42);
	}

	[Fact]
	public void Clone_ShouldCreateIndependentCopy()
	{
		var original = new FullWord(42);
		var clone = (FullWord)original.Clone();

		clone.LongValue.Should().Be(original.LongValue);

		clone.LongValue = 100;

		original.LongValue.Should().Be(42);
		clone.LongValue.Should().Be(100);
	}

	[Fact]
	public void ByteCount_ShouldBeFive()
	{
		var word = new FullWord();

		((Word)word).ByteCount.Should().Be(5);
	}

	[Fact]
	public void LongValue_Set_ShouldUpdateValue()
	{
		var word = new FullWord(42);

		word.LongValue = 100;

		word.LongValue.Should().Be(100);
	}

	[Fact]
	public void Sign_NegativeValue_ShouldBeNegative()
	{
		var word = new FullWord(-42);

		word.Sign.Should().Be(Word.Signs.Negative);
	}

	[Fact]
	public void Sign_PositiveValue_ShouldBePositive()
	{
		var word = new FullWord(42);

		word.Sign.Should().Be(Word.Signs.Positive);
	}

	[Fact]
	public void InvertSign_ShouldChangeSign()
	{
		var word = new FullWord(42);

		word.InvertSign();

		word.Sign.Should().Be(Word.Signs.Negative);
		word.LongValue.Should().Be(-42);
	}

	[Fact]
	public void IsEmpty_WithZeroValue_ShouldBeTrue()
	{
		var word = new FullWord(0);

		word.IsEmpty.Should().BeTrue();
	}

	[Fact]
	public void IsEmpty_WithNonZeroValue_ShouldBeFalse()
	{
		var word = new FullWord(42);

		word.IsEmpty.Should().BeFalse();
	}

	[Fact]
	public void MagnitudeLongValue_ShouldReturnAbsoluteValue()
	{
		var positiveWord = new FullWord(42);
		var negativeWord = new FullWord(-42);

		positiveWord.MagnitudeLongValue.Should().Be(42);
		negativeWord.MagnitudeLongValue.Should().Be(42);
	}

	[Fact]
	public void Indexer_ShouldAccessIndividualBytes()
	{
		var word = new FullWord();

		word[0] = new MixByte(10);
		word[1] = new MixByte(20);

		word[0].ByteValue.Should().Be(10);
		word[1].ByteValue.Should().Be(20);
	}

	[Theory]
	[InlineData(42, "+ 00 00 00 00 42")]
	[InlineData(-42, "- 00 00 00 00 42")]
	[InlineData(0, "+ 00 00 00 00 00")]
	public void ToString_WithoutChars_ShouldFormatCorrectly(long value, string expected)
	{
		var word = new FullWord(value);

		word.ToString(false).Should().Be(expected);
	}

	[Fact]
	public void Load_WithText_ShouldLoadCharacters()
	{
		var word = new FullWord();

		word.Load("HELLO");

		word[0].CharValue.Should().Be('H');
		word[1].CharValue.Should().Be('E');
		word[2].CharValue.Should().Be('L');
		word[3].CharValue.Should().Be('L');
		word[4].CharValue.Should().Be('O');
	}

	[Fact]
	public void Load_WithShortText_ShouldPadWithZeros()
	{
		var word = new FullWord();

		word.Load("HI");

		word[0].CharValue.Should().Be('H');
		word[1].CharValue.Should().Be('I');
		word[2].ByteValue.Should().Be(0);
		word[3].ByteValue.Should().Be(0);
		word[4].ByteValue.Should().Be(0);
	}

	[Fact]
	public void MaxMagnitude_ShouldBeCorrect()
	{
		var word = new FullWord();

		word.MaxMagnitude.Should().Be((1L << 30) - 1);
	}
}
