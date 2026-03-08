using MixLib.Misc;

namespace MixLib.Tests.Misc;

public class ValidationErrorTests
{
	[Fact]
	public void Constructor_WithMessageOnly_ShouldSetMessage()
	{
		var error = new ValidationError("Invalid value");

		error.Message.Should().Be("Invalid value");
		error.BoundsSpecified.Should().BeFalse();
	}

	[Fact]
	public void Constructor_WithBoundsOnly_ShouldSetBounds()
	{
		var error = new ValidationError(0, 100);

		error.ValidLowerBound.Should().Be(0);
		error.ValidUpperBound.Should().Be(100);
		error.BoundsSpecified.Should().BeTrue();
		error.Message.Should().BeNull();
	}

	[Fact]
	public void Constructor_WithMessageAndBounds_ShouldSetBoth()
	{
		var error = new ValidationError("Invalid range", -100, 100);

		error.Message.Should().Be("Invalid range");
		error.ValidLowerBound.Should().Be(-100);
		error.ValidUpperBound.Should().Be(100);
		error.BoundsSpecified.Should().BeTrue();
	}

	[Fact]
	public void CompiledMessage_WithMessageOnly_ShouldReturnMessage()
	{
		var error = new ValidationError("Test message");

		error.CompiledMessage.Should().Be("Test message");
	}

	[Fact]
	public void CompiledMessage_WithBoundsOnly_ShouldFormatBounds()
	{
		var error = new ValidationError(0, 63);

		error.CompiledMessage.Should().Be("value must be between 0 and 63");
	}

	[Fact]
	public void CompiledMessage_WithMessageAndBounds_ShouldCombineMessageAndBounds()
	{
		var error = new ValidationError("Field value out of range", 0, 5);

		error.CompiledMessage.Should().Be("Field value out of range (must be between 0 and 5)");
	}

	[Fact]
	public void CompiledMessage_WithNegativeBounds_ShouldFormatCorrectly()
	{
		var error = new ValidationError(-3999, 3999);

		error.CompiledMessage.Should().Be("value must be between -3999 and 3999");
	}

	[Fact]
	public void BoundsSpecified_WithMessageOnlyConstructor_ShouldBeFalse()
	{
		var error = new ValidationError("Message");

		error.BoundsSpecified.Should().BeFalse();
	}

	[Fact]
	public void BoundsSpecified_WithBoundsConstructor_ShouldBeTrue()
	{
		var error = new ValidationError(0, 100);

		error.BoundsSpecified.Should().BeTrue();
	}

	[Fact]
	public void ValidLowerBound_WithoutBounds_ShouldBeUnspecified()
	{
		var error = new ValidationError("Message");

		error.ValidLowerBound.Should().Be(ValidationError.Unspecified);
	}

	[Fact]
	public void ValidUpperBound_WithoutBounds_ShouldBeUnspecified()
	{
		var error = new ValidationError("Message");

		error.ValidUpperBound.Should().Be(ValidationError.Unspecified);
	}

	[Fact]
	public void Unspecified_ShouldBeIntMinValue()
	{
		ValidationError.Unspecified.Should().Be(int.MinValue);
	}
}
