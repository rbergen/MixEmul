using MixLib.Type;

namespace MixLib.Tests.Type;

public class AddressRegisterTests
{
	[Fact]
	public void Constructor_ShouldCreateTwoByteRegister()
	{
		var register = new AddressRegister();

		register.ByteCount.Should().Be(2);
		register.Sign.Should().Be(Word.Signs.Positive);
	}

	[Fact]
	public void ByteCountWithPadding_ShouldBeTwo()
	{
		var register = new AddressRegister();

		register.ByteCountWithPadding.Should().Be(2);
	}

	[Fact]
	public void GetByteWithPadding_ShouldReturnActualBytes()
	{
		var register = new AddressRegister();
		register[0] = 10;
		register[1] = 20;

		register.GetByteWithPadding(0).ByteValue.Should().Be(10);
		register.GetByteWithPadding(1).ByteValue.Should().Be(20);
	}

	[Fact]
	public void FullWordValue_ShouldPadToFiveBytes()
	{
		var register = new AddressRegister();
		register[0] = 10;
		register[1] = 20;
		register.Sign = Word.Signs.Negative;

		var fullWord = register.FullWordValue;

		fullWord[0].ByteValue.Should().Be(10);
		fullWord[1].ByteValue.Should().Be(20);
		fullWord[2].ByteValue.Should().Be(0);
		fullWord[3].ByteValue.Should().Be(0);
		fullWord[4].ByteValue.Should().Be(0);
		fullWord.Sign.Should().Be(Word.Signs.Negative);
	}

	[Fact]
	public void DefaultFieldSpec_ShouldCoverTwoBytes()
	{
		var fieldSpec = AddressRegister.DefaultFieldSpec;

		fieldSpec.LowBound.Should().Be(0);
		fieldSpec.HighBound.Should().Be(2);
	}

	[Theory]
	[InlineData(0, 0)]
	[InlineData(100, 100)]
	[InlineData(-100, -100)]
	public void LongValue_SetAndGet_ShouldWork(long value, long expected)
	{
		var register = new AddressRegister();

		register.LongValue = value;

		register.LongValue.Should().Be(expected);
	}

	[Fact]
	public void Indexer_ShouldAccessBytes()
	{
		var register = new AddressRegister();

		register[0] = 42;
		register[1] = 24;

		register[0].ByteValue.Should().Be(42);
		register[1].ByteValue.Should().Be(24);
	}

	[Fact]
	public void Sign_SetAndGet_ShouldWork()
	{
		var register = new AddressRegister();

		register.Sign = Word.Signs.Negative;

		register.Sign.Should().Be(Word.Signs.Negative);
	}
}
