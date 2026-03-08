using MixLib.Type;

namespace MixLib.Tests.Type;

public class FullWordRegisterTests
{
	[Fact]
	public void Constructor_ShouldCreateFiveByteRegister()
	{
		var register = new FullWordRegister();

		register.ByteCount.Should().Be(5);
		register.Sign.Should().Be(Word.Signs.Positive);
	}

	[Fact]
	public void ByteCountWithPadding_ShouldBeFive()
	{
		var register = new FullWordRegister();

		register.ByteCountWithPadding.Should().Be(5);
	}

	[Fact]
	public void PaddingByteCount_ShouldBeZero()
	{
		FullWordRegister.PaddingByteCount.Should().Be(0);
	}

	[Fact]
	public void RegisterByteCount_ShouldBeFive()
	{
		FullWordRegister.RegisterByteCount.Should().Be(5);
	}

	[Fact]
	public void GetByteWithPadding_ShouldReturnActualBytes()
	{
		var register = new FullWordRegister();
		register[0] = 10;
		register[1] = 20;
		register[2] = 30;
		register[3] = 40;
		register[4] = 50;

		register.GetByteWithPadding(0).ByteValue.Should().Be(10);
		register.GetByteWithPadding(1).ByteValue.Should().Be(20);
		register.GetByteWithPadding(2).ByteValue.Should().Be(30);
		register.GetByteWithPadding(3).ByteValue.Should().Be(40);
		register.GetByteWithPadding(4).ByteValue.Should().Be(50);
	}

	[Fact]
	public void FullWordValue_ShouldMatchRegisterExactly()
	{
		var register = new FullWordRegister();
		register[0] = 10;
		register[1] = 20;
		register[2] = 30;
		register[3] = 40;
		register[4] = 50;
		register.Sign = Word.Signs.Negative;

		var fullWord = register.FullWordValue;

		fullWord[0].ByteValue.Should().Be(10);
		fullWord[1].ByteValue.Should().Be(20);
		fullWord[2].ByteValue.Should().Be(30);
		fullWord[3].ByteValue.Should().Be(40);
		fullWord[4].ByteValue.Should().Be(50);
		fullWord.Sign.Should().Be(Word.Signs.Negative);
	}

	[Fact]
	public void DefaultFieldSpec_ShouldCoverAllFiveBytes()
	{
		var fieldSpec = FullWordRegister.DefaultFieldSpec;

		fieldSpec.LowBound.Should().Be(0);
		fieldSpec.HighBound.Should().Be(5);
	}

	[Theory]
	[InlineData(0L, 0L)]
	[InlineData(12345L, 12345L)]
	[InlineData(-12345L, -12345L)]
	[InlineData(1073741823L, 1073741823L)]
	public void LongValue_SetAndGet_ShouldWork(long value, long expected)
	{
		var register = new FullWordRegister();

		register.LongValue = value;

		register.LongValue.Should().Be(expected);
	}

	[Fact]
	public void Indexer_ShouldAccessAllBytes()
	{
		var register = new FullWordRegister();

		register[0] = 1;
		register[1] = 2;
		register[2] = 3;
		register[3] = 4;
		register[4] = 5;

		register[0].ByteValue.Should().Be(1);
		register[1].ByteValue.Should().Be(2);
		register[2].ByteValue.Should().Be(3);
		register[3].ByteValue.Should().Be(4);
		register[4].ByteValue.Should().Be(5);
	}

	[Fact]
	public void Sign_SetAndGet_ShouldWork()
	{
		var register = new FullWordRegister();

		register.Sign = Word.Signs.Negative;

		register.Sign.Should().Be(Word.Signs.Negative);
	}
}
