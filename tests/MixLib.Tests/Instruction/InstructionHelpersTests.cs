using MixLib.Instruction;

namespace MixLib.Tests.Instruction;

public class InstructionHelpersTests
{
	[Fact]
	public void InvalidAddress_ShouldBeIntMinValue()
	{
		InstructionHelpers.InvalidAddress.Should().Be(int.MinValue);
	}

	// Note: The ValidateIndex and ValidateIndexAndFieldSpec methods require
	// MixInstruction.Instance objects which are complex to construct in unit tests.
	// These methods are better tested through integration tests with actual instructions.
	// The constant validation above ensures the InvalidAddress sentinel value is correct.
}
