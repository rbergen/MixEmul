using System.Collections.Generic;
using MixLib;
using MixLib.Type;

namespace MixLib.Tests;

public class MemoryTests
{
	[Fact]
	public void Constructor_ShouldSetMinAndMaxIndices()
	{
		var memory = new Memory(-100, 100);

		memory.MinWordIndex.Should().Be(-100);
		memory.MaxWordIndex.Should().Be(100);
	}

	[Fact]
	public void WordCount_ShouldReturnCorrectCount()
	{
		var memory = new Memory(-100, 100);

		memory.WordCount.Should().Be(201); // -100 to 100 inclusive
	}

	[Fact]
	public void Indexer_Get_WithinBounds_ShouldReturnWord()
	{
		var memory = new Memory(0, 100);

		var word = memory[50];

		word.Should().NotBeNull();
		word.Should().BeAssignableTo<IMemoryFullWord>();
	}

	[Fact]
	public void Indexer_Get_BelowMinIndex_ShouldThrowArgumentOutOfRangeException()
	{
		var memory = new Memory(0, 100);

		var act = () => memory[-1];

		act.Should().Throw<ArgumentOutOfRangeException>()
			.WithParameterName("index")
			.WithMessage("*must be between MinWordIndex*");
	}

	[Fact]
	public void Indexer_Get_AboveMaxIndex_ShouldThrowArgumentOutOfRangeException()
	{
		var memory = new Memory(0, 100);

		var act = () => memory[101];

		act.Should().Throw<ArgumentOutOfRangeException>()
			.WithParameterName("index");
	}

	[Fact]
	public void Indexer_Set_ShouldStoreWord()
	{
		var memory = new Memory(0, 100);
		var word = memory.GetRealWord(50);
		word.LongValue = 42;

		memory[50].LongValue.Should().Be(42);
	}

	[Fact]
	public void Indexer_Set_BelowMinIndex_ShouldThrowIndexOutOfRangeException()
	{
		var memory = new Memory(0, 100);
		var word = memory.GetRealWord(50);

		var act = () => memory[-1] = word;

		act.Should().Throw<IndexOutOfRangeException>()
			.WithMessage("*must be between MinWordIndex*");
	}

	[Fact]
	public void Indexer_Set_AboveMaxIndex_ShouldThrowIndexOutOfRangeException()
	{
		var memory = new Memory(0, 100);
		var word = memory.GetRealWord(50);

		var act = () => memory[101] = word;

		act.Should().Throw<IndexOutOfRangeException>();
	}

	[Fact]
	public void HasContents_WithStoredWord_ShouldReturnTrue()
	{
		var memory = new Memory(0, 100);
		memory.GetRealWord(50).LongValue = 42;

		var result = memory.HasContents(50);

		result.Should().BeTrue();
	}

	[Fact]
	public void HasContents_WithEmptyWord_ShouldReturnFalse()
	{
		var memory = new Memory(0, 100);
		memory.GetRealWord(50).LongValue = 0;

		var result = memory.HasContents(50);

		result.Should().BeFalse();
	}

	[Fact]
	public void HasContents_WithoutStoredWord_ShouldReturnFalse()
	{
		var memory = new Memory(0, 100);

		var result = memory.HasContents(50);

		result.Should().BeFalse();
	}

	[Fact]
	public void Reset_ShouldClearAllMemory()
	{
		var memory = new Memory(0, 100);
		memory.GetRealWord(10).LongValue = 42;
		memory.GetRealWord(20).LongValue = 99;

		memory.Reset();

		memory.HasContents(10).Should().BeFalse();
		memory.HasContents(20).Should().BeFalse();
	}

	[Fact]
	public void GetRealWord_ShouldReturnMemoryFullWord()
	{
		var memory = new Memory(0, 100);

		var word = memory.GetRealWord(50);

		word.Should().NotBeNull();
		word.Should().BeOfType<MemoryFullWord>();
	}

	[Fact]
	public void GetRealWord_CalledTwice_ShouldReturnSameInstance()
	{
		var memory = new Memory(0, 100);

		var word1 = memory.GetRealWord(50);
		var word2 = memory.GetRealWord(50);

		word1.Should().BeSameAs(word2);
	}

	[Fact]
	public void DefaultFieldSpec_ShouldCoverFullWord()
	{
		var fieldSpec = Memory.DefaultFieldSpec;

		fieldSpec.LowBound.Should().Be(0);
		fieldSpec.HighBound.Should().Be(5);
	}

	[Fact]
	public void Indexer_Set_WithSign_ShouldPreserveSign()
	{
		var memory = new Memory(0, 100);
		var word = memory.GetRealWord(50);
		word.LongValue = -42;

		memory[50].Sign.Should().Be(Word.Signs.Negative);
		memory[50].LongValue.Should().Be(-42);
	}

	[Fact]
	public void Indexer_Set_WithBytes_ShouldPreserveBytes()
	{
		var memory = new Memory(0, 100);
		var word = memory.GetRealWord(10);
		word[0] = 10;
		word[1] = 20;
		word[2] = 30;
		word[3] = 40;
		word[4] = 50;

		memory[10][0].ByteValue.Should().Be(10);
		memory[10][1].ByteValue.Should().Be(20);
		memory[10][2].ByteValue.Should().Be(30);
		memory[10][3].ByteValue.Should().Be(40);
		memory[10][4].ByteValue.Should().Be(50);
	}

	[Fact]
	public void Enumeration_ShouldIterateStoredWords()
	{
		var memory = new Memory(0, 100);
		memory.GetRealWord(10).LongValue = 42;
		memory.GetRealWord(20).LongValue = 99;

		var list = new List<IMemoryFullWord>();
		foreach (IMemoryFullWord word in memory)
		{
			list.Add(word);
		}

		list.Should().HaveCount(2);
	}
}
