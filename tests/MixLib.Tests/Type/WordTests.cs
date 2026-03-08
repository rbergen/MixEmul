using System.Collections.Generic;
using MixLib.Type;

namespace MixLib.Tests.Type;

public class WordTests
{
	[Theory]
	[InlineData(1)]
	[InlineData(2)]
	[InlineData(5)]
	public void Constructor_WithByteCount_ShouldCreateWordWithZeros(int byteCount)
	{
		var word = new Word(byteCount);

		word.ByteCount.Should().Be(byteCount);
		word.Sign.Should().Be(Word.Signs.Positive);
		word.LongValue.Should().Be(0);
	}

	[Fact]
	public void Constructor_WithBytesAndSign_ShouldSetValues()
	{
		var bytes = new MixByte[] { 10, 20, 30 };
		
		var word = new Word(bytes, Word.Signs.Negative);

		word.ByteCount.Should().Be(3);
		word.Sign.Should().Be(Word.Signs.Negative);
		word[0].ByteValue.Should().Be(10);
		word[1].ByteValue.Should().Be(20);
		word[2].ByteValue.Should().Be(30);
	}

	[Fact]
	public void BytesToLong_WithPositiveSign_ShouldCalculateCorrectly()
	{
		var bytes = new MixByte[] { 0, 0, 1, 2, 3 };
		
		var result = Word.BytesToLong(bytes);

		result.Should().Be((1 << 12) + (2 << 6) + 3);
	}

	[Fact]
	public void BytesToLong_WithSignParameter_ShouldApplySign()
	{
		var bytes = new MixByte[] { 0, 0, 1, 2, 3 };
		
		var result = Word.BytesToLong(Word.Signs.Negative, bytes);

		result.Should().BeNegative();
		result.Should().Be(-((1 << 12) + (2 << 6) + 3));
	}

	[Fact]
	public void Slice_ShouldReturnSubArray()
	{
		var word = new Word(5);
		word[0] = 10;
		word[1] = 20;
		word[2] = 30;
		word[3] = 40;
		word[4] = 50;

		var slice = word.Slice(1, 3);

		slice.Length.Should().Be(3);
		slice[0].ByteValue.Should().Be(20);
		slice[1].ByteValue.Should().Be(30);
		slice[2].ByteValue.Should().Be(40);
	}

	[Fact]
	public void InvertSign_ShouldToggleSign()
	{
		var word = new Word(3);
		word.Sign = Word.Signs.Positive;

		word.InvertSign();

		word.Sign.Should().Be(Word.Signs.Negative);
		
		word.InvertSign();
		
		word.Sign.Should().Be(Word.Signs.Positive);
	}

	[Fact]
	public void ToString_WithAsCharsFalse_ShouldIncludeSignAndBytes()
	{
		var word = new Word(3);
		word[0] = 1;
		word[1] = 2;
		word[2] = 3;
		word.Sign = Word.Signs.Positive;

		var result = word.ToString(false);

		result.Should().StartWith("+");
		result.Should().Contain("1");
		result.Should().Contain("2");
		result.Should().Contain("3");
	}

	[Fact]
	public void ToString_WithAsCharsTrue_ShouldReturnCharacterString()
	{
		var word = new Word(5);
		word.Load("HELLO");

		var result = word.ToString(true);

		result.Should().Be("HELLO");
	}

	[Fact]
	public void Magnitude_Get_ShouldReturnByteArrayCopy()
	{
		var word = new Word(3);
		word[0] = 10;
		word[1] = 20;
		word[2] = 30;

		var magnitude = word.Magnitude;

		magnitude.Length.Should().Be(3);
		magnitude[0].ByteValue.Should().Be(10);
		magnitude[1].ByteValue.Should().Be(20);
		magnitude[2].ByteValue.Should().Be(30);
	}

	[Fact]
	public void Magnitude_Set_WithSmallerArray_ShouldPadWithZeros()
	{
		var word = new Word(5);
		word.Magnitude = new MixByte[] { 10, 20 };

		word[0].ByteValue.Should().Be(0);
		word[1].ByteValue.Should().Be(0);
		word[2].ByteValue.Should().Be(0);
		word[3].ByteValue.Should().Be(10);
		word[4].ByteValue.Should().Be(20);
	}

	[Fact]
	public void Magnitude_Set_WithLargerArray_ShouldTruncate()
	{
		var word = new Word(3);
		word.Magnitude = new MixByte[] { 10, 20, 30, 40, 50 };

		word.ByteCount.Should().Be(3);
		word[0].ByteValue.Should().Be(30);
		word[1].ByteValue.Should().Be(40);
		word[2].ByteValue.Should().Be(50);
	}

	[Fact]
	public void MagnitudeLongValue_ShouldReturnAbsoluteValue()
	{
		var word = new Word(3);
		word.Sign = Word.Signs.Negative;
		word[0] = 1;
		word[1] = 2;
		word[2] = 3;

		var magnitude = word.MagnitudeLongValue;

		magnitude.Should().BeGreaterThan(0);
		magnitude.Should().Be((1 << 12) + (2 << 6) + 3);
	}

	[Fact]
	public void MagnitudeLongValue_Set_ShouldNotChangeSign()
	{
		var word = new Word(3);
		word.Sign = Word.Signs.Negative;

		word.MagnitudeLongValue = 42;

		word.Sign.Should().Be(Word.Signs.Negative);
		word.MagnitudeLongValue.Should().Be(42);
	}

	[Fact]
	public void ToArray_ShouldReturnMagnitude()
	{
		var word = new Word(3);
		word[0] = 10;
		word[1] = 20;
		word[2] = 30;

		var array = word.ToArray();

		array.Should().BeEquivalentTo(word.Magnitude);
	}

	[Fact]
	public void IsEmpty_WithZeroAndPositive_ShouldBeTrue()
	{
		var word = new Word(3);

		word.IsEmpty.Should().BeTrue();
	}

	[Fact]
	public void IsEmpty_WithZeroAndNegative_ShouldBeFalse()
	{
		var word = new Word(3);
		word.Sign = Word.Signs.Negative;

		word.IsEmpty.Should().BeFalse();
	}

	[Fact]
	public void IsEmpty_WithNonZero_ShouldBeFalse()
	{
		var word = new Word(3);
		word[0] = 1;

		word.IsEmpty.Should().BeFalse();
	}

	[Fact]
	public void Count_ShouldReturnByteCount()
	{
		var word = new Word(7);

		word.Count.Should().Be(7);
	}

	[Fact]
	public void MaxByteCount_ShouldReturnByteCount()
	{
		var word = new Word(7);

		word.MaxByteCount.Should().Be(7);
	}

	[Fact]
	public void BitCount_ShouldBeByteCountTimesSix()
	{
		var word = new Word(5);

		word.BitCount.Should().Be(30);
	}

	[Fact]
	public void MaxMagnitude_ShouldBeCorrect()
	{
		var word = new Word(2);

		word.MaxMagnitude.Should().Be((1 << 12) - 1);
	}

	[Fact]
	public void Enumeration_ShouldIterateBytes()
	{
		var word = new Word(3);
		word[0] = 10;
		word[1] = 20;
		word[2] = 30;

		var list = new List<byte>();
		foreach (MixByte b in word)
		{
			list.Add(b.ByteValue);
		}

		list.Should().Equal(10, 20, 30);
	}

	[Fact]
	public void Sign_SetAndGet_ShouldWork()
	{
		var word = new Word(3);

		word.Sign = Word.Signs.Negative;

		word.Sign.Should().Be(Word.Signs.Negative);
	}
}
