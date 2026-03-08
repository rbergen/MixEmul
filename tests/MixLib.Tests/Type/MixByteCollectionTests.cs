using MixLib.Type;
using System.Collections.Generic;

namespace MixLib.Tests.Type;

public class MixByteCollectionTests
{
	[Theory]
	[InlineData(1)]
	[InlineData(5)]
	[InlineData(10)]
	[InlineData(100)]
	public void Constructor_WithByteCount_ShouldCreateCollectionWithZeros(int byteCount)
	{
		var collection = new MixByteCollection(byteCount);

		collection.MaxByteCount.Should().Be(byteCount);

		for (int i = 0; i < byteCount; i++)
		{
			collection[i].ByteValue.Should().Be(0);
		}
	}

	[Fact]
	public void Constructor_WithByteArray_ShouldCopyArray()
	{
		var bytes = new MixByte[] { 10, 20, 30, 40, 50 };

		var collection = new MixByteCollection(bytes);

		collection.MaxByteCount.Should().Be(5);
		collection[0].ByteValue.Should().Be(10);
		collection[1].ByteValue.Should().Be(20);
		collection[2].ByteValue.Should().Be(30);
		collection[3].ByteValue.Should().Be(40);
		collection[4].ByteValue.Should().Be(50);
	}

	[Fact]
	public void Indexer_Get_ShouldReturnByte()
	{
		var collection = new MixByteCollection(5);
		collection[2] = new MixByte(42);

		var result = collection[2];

		result.ByteValue.Should().Be(42);
	}

	[Fact]
	public void Indexer_Set_ShouldUpdateByte()
	{
		var collection = new MixByteCollection(5);

		collection[2] = new MixByte(42);

		collection[2].ByteValue.Should().Be(42);
	}

	[Fact]
	public void Load_WithText_ShouldLoadCharacters()
	{
		var collection = new MixByteCollection(5);

		collection.Load("HELLO");

		collection[0].CharValue.Should().Be('H');
		collection[1].CharValue.Should().Be('E');
		collection[2].CharValue.Should().Be('L');
		collection[3].CharValue.Should().Be('L');
		collection[4].CharValue.Should().Be('O');
	}

	[Fact]
	public void Load_WithShortText_ShouldPadWithZeros()
	{
		var collection = new MixByteCollection(5);

		collection.Load("HI");

		collection[0].CharValue.Should().Be('H');
		collection[1].CharValue.Should().Be('I');
		collection[2].ByteValue.Should().Be(0);
		collection[3].ByteValue.Should().Be(0);
		collection[4].ByteValue.Should().Be(0);
	}

	[Fact]
	public void Load_WithLongText_ShouldTruncate()
	{
		var collection = new MixByteCollection(3);

		collection.Load("HELLO");

		collection.MaxByteCount.Should().Be(3);
		collection[0].CharValue.Should().Be('H');
		collection[1].CharValue.Should().Be('E');
		collection[2].CharValue.Should().Be('L');
	}

	[Fact]
	public void Load_WithNull_ShouldFillWithZeros()
	{
		var collection = new MixByteCollection(5);
		collection.Load("TEST");

		collection.Load(null);

		for (int i = 0; i < 5; i++)
		{
			collection[i].ByteValue.Should().Be(0);
		}
	}

	[Fact]
	public void Load_WithEmptyString_ShouldFillWithZeros()
	{
		var collection = new MixByteCollection(5);
		collection.Load("TEST");

		collection.Load("");

		for (int i = 0; i < 5; i++)
		{
			collection[i].ByteValue.Should().Be(0);
		}
	}

	[Fact]
	public void ToString_WithAsCharsTrue_ShouldReturnCharacterString()
	{
		var collection = new MixByteCollection(5);
		collection.Load("HELLO");

		var result = collection.ToString(true);

		result.Should().Be("HELLO");
	}

	[Fact]
	public void ToString_WithAsCharsTrue_ShouldTrimTrailingSpaces()
	{
		var collection = new MixByteCollection(5);
		collection.Load("HI");

		var result = collection.ToString(true);

		result.Should().Be("HI");
	}

	[Fact]
	public void ToArray_ShouldReturnClonedArray()
	{
		var collection = new MixByteCollection(3);
		collection[0] = 10;
		collection[1] = 20;
		collection[2] = 30;

		var array = collection.ToArray();

		array.Length.Should().Be(3);
		array[0].ByteValue.Should().Be(10);
		array[1].ByteValue.Should().Be(20);
		array[2].ByteValue.Should().Be(30);
	}

	[Fact]
	public void ToArray_ShouldReturnIndependentCopy()
	{
		var collection = new MixByteCollection(3);
		collection[0] = 10;

		var array = collection.ToArray();
		collection[0] = 20;

		array[0].ByteValue.Should().Be(10);
		collection[0].ByteValue.Should().Be(20);
	}

	[Fact]
	public void Clone_ShouldCreateIndependentCopy()
	{
		var original = new MixByteCollection(3);
		original[0] = 10;
		original[1] = 20;
		original[2] = 30;

		var clone = (MixByteCollection)original.Clone();

		clone.MaxByteCount.Should().Be(original.MaxByteCount);
		clone[0].ByteValue.Should().Be(10);
		clone[1].ByteValue.Should().Be(20);
		clone[2].ByteValue.Should().Be(30);
	}

	[Fact]
	public void Clone_ShouldBeIndependent()
	{
		var original = new MixByteCollection(3);
		original[0] = 10;

		var clone = (MixByteCollection)original.Clone();
		clone[0] = 20;

		original[0].ByteValue.Should().Be(10);
		clone[0].ByteValue.Should().Be(20);
	}

	[Fact]
	public void GetEnumerator_ShouldIterateAllBytes()
	{
		var collection = new MixByteCollection(3);
		collection[0] = 10;
		collection[1] = 20;
		collection[2] = 30;

		var list = new List<byte>();
		foreach (MixByte b in collection)
		{
			list.Add(b.ByteValue);
		}

		list.Should().Equal(10, 20, 30);
	}

	[Fact]
	public void MaxByteCount_ShouldReturnCorrectValue()
	{
		var collection = new MixByteCollection(7);

		collection.MaxByteCount.Should().Be(7);
	}
}
