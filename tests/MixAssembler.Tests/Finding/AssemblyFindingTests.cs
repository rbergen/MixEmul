using MixAssembler.Finding;
using MixLib.Misc;

namespace MixAssembler.Tests.Finding;

public class AssemblyFindingTests
{
	[Fact]
	public void AssemblyError_ShouldHaveErrorSeverity()
	{
		var error = new ValidationError("Test error");
		var assemblyError = new AssemblyError(10, LineSection.OpField, 5, 3, error);

		assemblyError.Severity.Should().Be(Severity.Error);
	}

	[Fact]
	public void AssemblyWarning_ShouldHaveWarningSeverity()
	{
		var error = new ValidationError("Test warning");
		var assemblyWarning = new AssemblyWarning(10, LineSection.OpField, 5, 3, error);

		assemblyWarning.Severity.Should().Be(Severity.Warning);
	}

	[Fact]
	public void AssemblyError_ShouldStoreLineNumber()
	{
		var error = new ValidationError("Test error");
		var assemblyError = new AssemblyError(42, LineSection.OpField, 5, 3, error);

		assemblyError.LineNumber.Should().Be(42);
	}

	[Fact]
	public void AssemblyError_ShouldStoreLineSection()
	{
		var error = new ValidationError("Test error");
		var assemblyError = new AssemblyError(10, LineSection.AddressField, 5, 3, error);

		assemblyError.LineSection.Should().Be(LineSection.AddressField);
	}

	[Fact]
	public void AssemblyError_ShouldStoreStartCharIndex()
	{
		var error = new ValidationError("Test error");
		var assemblyError = new AssemblyError(10, LineSection.OpField, 15, 3, error);

		assemblyError.StartCharIndex.Should().Be(15);
	}

	[Fact]
	public void AssemblyError_ShouldStoreLength()
	{
		var error = new ValidationError("Test error");
		var assemblyError = new AssemblyError(10, LineSection.OpField, 5, 7, error);

		assemblyError.Length.Should().Be(7);
	}

	[Fact]
	public void ValidationFinding_Message_ShouldReturnValidationErrorMessage()
	{
		var error = new ValidationError("Invalid instruction");
		var assemblyError = new AssemblyError(10, LineSection.OpField, 5, 3, error);

		assemblyError.Message.Should().Be("Invalid instruction");
	}

	[Fact]
	public void ValidationFinding_WithBoundsError_MessageShouldIncludeBounds()
	{
		var error = new ValidationError(0, 100);
		var assemblyError = new AssemblyError(10, LineSection.AddressField, 5, 3, error);

		assemblyError.Message.Should().Be("value must be between 0 and 100");
	}

	[Fact]
	public void ValidationFinding_WithMessageAndBounds_MessageShouldIncludeBoth()
	{
		var error = new ValidationError("Address out of range", -3999, 3999);
		var assemblyError = new AssemblyError(10, LineSection.AddressField, 5, 4, error);

		assemblyError.Message.Should().Be("Address out of range (must be between -3999 and 3999)");
	}

	[Fact]
	public void ValidationFinding_Error_ShouldReturnValidationError()
	{
		var error = new ValidationError("Test error");
		var assemblyError = new AssemblyError(10, LineSection.OpField, 5, 3, error);

		assemblyError.Error.Should().BeSameAs(error);
	}
}
