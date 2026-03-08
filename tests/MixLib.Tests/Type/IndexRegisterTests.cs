using MixLib.Type;

namespace MixLib.Tests.Type;

public class IndexRegisterTests
{
	[Fact]
	public void Constructor_ShouldCreateTwoByteRegister()
	{
		var register = new IndexRegister();

		register.ByteCount.Should().Be(2);
		register.Sign.Should().Be(Word.Signs.Positive);
	}

	[Fact]
	public void ByteCountWithPadding_ShouldBeFive()
	{
		var register = new IndexRegister();

		register.ByteCountWithPadding.Should().Be(5);
	}

	[Fact]
	public void PaddingByteCount_ShouldBeThree()
	{
		IndexRegister.PaddingByteCount.Should().Be(3);
	}

	[Fact]
	public void RegisterByteCount_ShouldBeTwo()
	{
		IndexRegister.RegisterByteCount.Should().Be(2);
	}

	[Fact]
	public void GetByteWithPadding_WithinPadding_ShouldReturnZero()
	{
		var register = new IndexRegister();
		register[0] = 10;
		register[1] = 20;

		register.GetByteWithPadding(0).ByteValue.Should().Be(0);
		register.GetByteWithPadding(1).ByteValue.Should().Be(0);
		register.GetByteWithPadding(2).ByteValue.Should().Be(0);
	}

	[Fact]
	public void GetByteWithPadding_AfterPadding_ShouldReturnActualByte()
	{
		var register = new IndexRegister();
		register[0] = 10;
		register[1] = 20;

		register.GetByteWithPadding(3).ByteValue.Should().Be(10);
		register.GetByteWithPadding(4).ByteValue.Should().Be(20);
	}

	[Fact]
	public void FullWordValue_ShouldIncludePadding()
	{
		var register = new IndexRegister();
		register[0] = 10;
		register[1] = 20;
		register.Sign = Word.Signs.Negative;

		var fullWord = register.FullWordValue;

		fullWord[0].ByteValue.Should().Be(0);
		fullWord[1].ByteValue.Should().Be(0);
		fullWord[2].ByteValue.Should().Be(0);
		fullWord[3].ByteValue.Should().Be(10);
		fullWord[4].ByteValue.Should().Be(20);
		fullWord.Sign.Should().Be(Word.Signs.Negative);
	}

	[Fact]
	public void DefaultFieldSpec_ShouldCoverFullWord()
	{
		var fieldSpec = IndexRegister.DefaultFieldSpec;

		fieldSpec.LowBound.Should().Be(0);
		fieldSpec.HighBound.Should().Be(5);
	}

	[Theory]
	[InlineData(0, 0)]
	[InlineData(100, 100)]
	[InlineData(-100, -100)]
	public void LongValue_SetAndGet_ShouldWork(long value, long expected)
	{
		var register = new IndexRegister();

		register.LongValue = value;

		register.LongValue.Should().Be(expected);
	}

	[Fact]
	public void Indexer_ShouldAccessBytes()
	{
		var register = new IndexRegister();

		register[0] = 42;
		register[1] = 24;

		register[0].ByteValue.Should().Be(42);
		register[1].ByteValue.Should().Be(24);
	}
}
