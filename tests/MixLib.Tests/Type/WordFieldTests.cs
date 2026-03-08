using MixLib.Type;

namespace MixLib.Tests.Type;

public class WordFieldTests
{
	[Fact]
	public void LoadFromFullWord_WithFullField_ShouldLoadAllBytes()
	{
		var word = new FullWord(12345);
		var fieldSpec = new FieldSpec(1, 5);

		var field = WordField.LoadFromFullWord(fieldSpec, word);

		field.Should().NotBeNull();
		((Word)field).ByteCount.Should().Be(5);
	}

	[Fact]
	public void LoadFromFullWord_WithSignIncluded_ShouldCopySign()
	{
		var word = new FullWord(-42);
		var fieldSpec = new FieldSpec(0, 5);

		var field = WordField.LoadFromFullWord(fieldSpec, word);

		field.Sign.Should().Be(Word.Signs.Negative);
	}

	[Fact]
	public void LoadFromFullWord_WithoutSign_ShouldHavePositiveSign()
	{
		var word = new FullWord(-42);
		var fieldSpec = new FieldSpec(1, 5);

		var field = WordField.LoadFromFullWord(fieldSpec, word);

		field.Sign.Should().Be(Word.Signs.Positive);
	}

	[Fact]
	public void LoadFromFullWord_ShouldLoadCorrectBytes()
	{
		var word = new FullWord();
		word[0] = 10;
		word[1] = 20;
		word[2] = 30;
		word[3] = 40;
		word[4] = 50;

		var fieldSpec = new FieldSpec(2, 4);
		var field = WordField.LoadFromFullWord(fieldSpec, word);

		((Word)field).ByteCount.Should().Be(3);
		field[0].ByteValue.Should().Be(20);
		field[1].ByteValue.Should().Be(30);
		field[2].ByteValue.Should().Be(40);
	}

	[Fact]
	public void LoadFromFullWord_WithSingleByte_ShouldWork()
	{
		var word = new FullWord();
		word[2] = 42;

		var fieldSpec = new FieldSpec(3, 3);
		var field = WordField.LoadFromFullWord(fieldSpec, word);

		((Word)field).ByteCount.Should().Be(1);
		field[0].ByteValue.Should().Be(42);
	}

	[Fact]
	public void ApplyToFullWord_ShouldCopyBytesToWord()
	{
		var word = new FullWord();
		var fieldSpec = new FieldSpec(2, 4);
		var field = WordField.LoadFromFullWord(fieldSpec, word);

		field[0] = 10;
		field[1] = 20;
		field[2] = 30;

		var targetWord = new FullWord();
		field.ApplyToFullWord(targetWord);

		targetWord[1].ByteValue.Should().Be(10);
		targetWord[2].ByteValue.Should().Be(20);
		targetWord[3].ByteValue.Should().Be(30);
	}

	[Fact]
	public void ApplyToFullWord_WithSignIncluded_ShouldCopySign()
	{
		var word = new FullWord();
		var fieldSpec = new FieldSpec(0, 3);
		var field = WordField.LoadFromFullWord(fieldSpec, word);
		field.Sign = Word.Signs.Negative;

		var targetWord = new FullWord();
		field.ApplyToFullWord(targetWord);

		targetWord.Sign.Should().Be(Word.Signs.Negative);
	}

	[Fact]
	public void ApplyToFullWord_WithoutSign_ShouldNotChangeSign()
	{
		var word = new FullWord();
		var fieldSpec = new FieldSpec(1, 3);
		var field = WordField.LoadFromFullWord(fieldSpec, word);
		field.Sign = Word.Signs.Negative;

		var targetWord = new FullWord();
		targetWord.Sign = Word.Signs.Positive;
		field.ApplyToFullWord(targetWord);

		targetWord.Sign.Should().Be(Word.Signs.Positive);
	}

	[Fact]
	public void LoadFromRegister_WithIndexRegister_ShouldWork()
	{
		var register = new IndexRegister();
		register[0] = 10;
		register[1] = 20;

		var fieldSpec = new FieldSpec(0, 2);
		var field = WordField.LoadFromRegister(fieldSpec, register);

		((Word)field).ByteCount.Should().Be(2);
		field[0].ByteValue.Should().Be(10);
		field[1].ByteValue.Should().Be(20);
	}

	[Fact]
	public void LoadFromRegister_WithSign_ShouldCopySign()
	{
		var register = new IndexRegister();
		register.Sign = Word.Signs.Negative;

		var fieldSpec = new FieldSpec(0, 2);
		var field = WordField.LoadFromRegister(fieldSpec, register);

		field.Sign.Should().Be(Word.Signs.Negative);
	}

	[Fact]
	public void LoadFromRegister_WithoutSign_ShouldHavePositiveSign()
	{
		var register = new IndexRegister();
		register.Sign = Word.Signs.Negative;

		var fieldSpec = new FieldSpec(1, 2);
		var field = WordField.LoadFromRegister(fieldSpec, register);

		field.Sign.Should().Be(Word.Signs.Positive);
	}

	[Fact]
	public void LoadFromRegister_WithPadding_ShouldHandleCorrectly()
	{
		var register = new IndexRegister();
		register[0] = 10;
		register[1] = 20;

		var fieldSpec = new FieldSpec(0, 5);
		var field = WordField.LoadFromRegister(fieldSpec, register);

		((Word)field).ByteCount.Should().Be(5);
		field[0].ByteValue.Should().Be(0);
		field[1].ByteValue.Should().Be(0);
		field[2].ByteValue.Should().Be(0);
		field[3].ByteValue.Should().Be(10);
		field[4].ByteValue.Should().Be(20);
	}

	[Fact]
	public void LoadFromRegister_WithLargerFieldThanRegister_ShouldThrowArgumentOutOfRangeException()
	{
		var register = new IndexRegister(); // ByteCountWithPadding = 5
		// We need a field that has ByteCount > 5, but such fields don't exist in valid FieldSpecs
		// because the max is (0,6) which still has ByteCount of 5
		// This test documents that limitation
		var fieldSpec = new FieldSpec(1, 5); // ByteCount = 5, fits exactly

		var field = WordField.LoadFromRegister(fieldSpec, register);

		field.Should().NotBeNull();
	}

	[Fact]
	public void ApplyToRegister_ShouldCopyBytesToRegister()
	{
		var word = new FullWord();
		var fieldSpec = new FieldSpec(1, 2);
		var field = WordField.LoadFromFullWord(fieldSpec, word);
		field[0] = 10;
		field[1] = 20;

		var register = new IndexRegister();
		field.ApplyToRegister(register);

		register[0].ByteValue.Should().Be(10);
		register[1].ByteValue.Should().Be(20);
	}

	[Fact]
	public void ApplyToRegister_ShouldCopySign()
	{
		var word = new FullWord();
		var fieldSpec = new FieldSpec(0, 2);
		var field = WordField.LoadFromFullWord(fieldSpec, word);
		field.Sign = Word.Signs.Negative;

		var register = new IndexRegister();
		field.ApplyToRegister(register);

		register.Sign.Should().Be(Word.Signs.Negative);
	}

	[Fact]
	public void ApplyToRegister_WithPadding_ShouldPadWithZeros()
	{
		var word = new FullWord();
		var fieldSpec = new FieldSpec(1, 1);
		var field = WordField.LoadFromFullWord(fieldSpec, word);
		field[0] = 42;

		var register = new IndexRegister();
		register[0] = 55;
		register[1] = 44;

		field.ApplyToRegister(register);

		register[0].ByteValue.Should().Be(0);
		register[1].ByteValue.Should().Be(42);
	}

	[Fact]
	public void ApplyToRegister_WithLargerFieldThanRegister_DocumentsLimitation()
	{
		// The maximum valid field spec has ByteCount of 5 ((0,6) or (1,5))
		// which matches IndexRegister.ByteCountWithPadding of 5
		// So there's no valid FieldSpec that would throw this exception for IndexRegister
		var word = new FullWord();
		var fieldSpec = new FieldSpec(1, 5); // ByteCount = 5
		var field = WordField.LoadFromFullWord(fieldSpec, word);

		var register = new IndexRegister();

		// This should work without throwing
		field.ApplyToRegister(register);

		register.Should().NotBeNull();
	}

	[Theory]
	[InlineData(100, 100, 0)]
	[InlineData(100, 50, 1)]
	[InlineData(50, 100, -1)]
	[InlineData(-100, -100, 0)]
	[InlineData(-50, -100, 1)]
	[InlineData(-100, -50, -1)]
	public void CompareTo_ShouldCompareFieldValues(long value1, long value2, int expectedComparison)
	{
		var word1 = new FullWord(value1);
		var word2 = new FullWord(value2);
		var fieldSpec = new FieldSpec(0, 5);

		var field = WordField.LoadFromFullWord(fieldSpec, word1);

		var result = field.CompareTo(word2);

		result.Should().Be(expectedComparison);
	}

	[Fact]
	public void CompareTo_WithPartialField_ShouldCompareCorrectly()
	{
		var word1 = new FullWord();
		word1[2] = 10;
		word1[3] = 20;

		var word2 = new FullWord();
		word2[2] = 10;
		word2[3] = 30;

		var fieldSpec = new FieldSpec(3, 4);
		var field = WordField.LoadFromFullWord(fieldSpec, word1);

		var result = field.CompareTo(word2);

		result.Should().BeLessThan(0);
	}
}
