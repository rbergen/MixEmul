using MixLib;
using MixLib.Type;

namespace MixLib.Tests;

public class RegistersTests
{
	[Fact]
	public void Constructor_ShouldInitializeAllRegisters()
	{
		var registers = new Registers();

		registers.RA.Should().NotBeNull();
		registers.RX.Should().NotBeNull();
		registers.RJ.Should().NotBeNull();
		registers.RI1.Should().NotBeNull();
		registers.RI2.Should().NotBeNull();
		registers.RI3.Should().NotBeNull();
		registers.RI4.Should().NotBeNull();
		registers.RI5.Should().NotBeNull();
		registers.RI6.Should().NotBeNull();
	}

	[Fact]
	public void Constructor_ShouldSetDefaultIndicators()
	{
		var registers = new Registers();

		registers.CompareIndicator.Should().Be(Registers.CompValues.Equal);
		registers.OverflowIndicator.Should().BeFalse();
	}

	[Fact]
	public void RA_ShouldBeFullWordRegister()
	{
		var registers = new Registers();

		registers.RA.Should().BeOfType<FullWordRegister>();
		registers.RA.ByteCount.Should().Be(5);
	}

	[Fact]
	public void RX_ShouldBeFullWordRegister()
	{
		var registers = new Registers();

		registers.RX.Should().BeOfType<FullWordRegister>();
		registers.RX.ByteCount.Should().Be(5);
	}

	[Fact]
	public void RJ_ShouldBeAddressRegister()
	{
		var registers = new Registers();

		registers.RJ.Should().BeOfType<AddressRegister>();
		registers.RJ.ByteCount.Should().Be(2);
	}

	[Theory]
	[InlineData(Registers.Offset.rI1)]
	[InlineData(Registers.Offset.rI2)]
	[InlineData(Registers.Offset.rI3)]
	[InlineData(Registers.Offset.rI4)]
	[InlineData(Registers.Offset.rI5)]
	[InlineData(Registers.Offset.rI6)]
	public void IndexRegisters_ShouldBeIndexRegister(Registers.Offset offset)
	{
		var registers = new Registers();

		registers[offset].Should().BeOfType<IndexRegister>();
		registers[offset].ByteCount.Should().Be(2);
	}

	[Fact]
	public void Indexer_WithIntOffset_ShouldAccessRegister()
	{
		var registers = new Registers();

		registers[(int)Registers.Offset.rA].Should().BeSameAs(registers.RA);
		registers[(int)Registers.Offset.rI1].Should().BeSameAs(registers.RI1);
	}

	[Fact]
	public void Indexer_WithEnumOffset_ShouldAccessRegister()
	{
		var registers = new Registers();

		registers[Registers.Offset.rA].Should().BeSameAs(registers.RA);
		registers[Registers.Offset.rI1].Should().BeSameAs(registers.RI1);
	}

	[Theory]
	[InlineData(100, 0)]
	[InlineData(100, 1)]
	[InlineData(100, 2)]
	public void GetIndexedAddress_WithZeroIndex_ShouldReturnBaseAddress(int baseAddress, int registerOffset)
	{
		var registers = new Registers();
		if (registerOffset > 0)
		{
			registers[(int)Registers.Offset.rI1 + registerOffset - 1].LongValue = 50;
		}

		var result = registers.GetIndexedAddress(baseAddress, 0);

		result.Should().Be(baseAddress);
	}

	[Theory]
	[InlineData(100, 1, 10, 110)]
	[InlineData(100, 2, 20, 120)]
	[InlineData(100, 6, -10, 90)]
	public void GetIndexedAddress_WithIndexRegister_ShouldAddRegisterValue(int baseAddress, int index, int registerValue, int expected)
	{
		var registers = new Registers();
		registers[(int)Registers.Offset.rI1 + index - 1].LongValue = registerValue;

		var result = registers.GetIndexedAddress(baseAddress, index);

		result.Should().Be(expected);
	}

	[Theory]
	[InlineData(-1)]
	[InlineData(7)]
	[InlineData(10)]
	public void GetIndexedAddress_WithInvalidIndex_ShouldThrowArgumentException(int invalidIndex)
	{
		var registers = new Registers();

		var act = () => registers.GetIndexedAddress(100, invalidIndex);

		act.Should().Throw<ArgumentException>()
			.WithParameterName("index");
	}

	[Fact]
	public void CompareIndicator_SetAndGet_ShouldWork()
	{
		var registers = new Registers();

		registers.CompareIndicator = Registers.CompValues.Less;

		registers.CompareIndicator.Should().Be(Registers.CompValues.Less);
	}

	[Fact]
	public void OverflowIndicator_SetAndGet_ShouldWork()
	{
		var registers = new Registers();

		registers.OverflowIndicator = true;

		registers.OverflowIndicator.Should().BeTrue();
	}

	[Fact]
	public void RegisterCount_ShouldBeNine()
	{
		Registers.RegisterCount.Should().Be(9); // rA, rX, rJ, rI1-rI6
	}

	[Fact]
	public void MaxOffset_ShouldBeSeven()
	{
		Registers.MaxOffset.Should().Be(7);
	}

	[Fact]
	public void PropertyAccessors_ShouldReturnCorrectRegisters()
	{
		var registers = new Registers();

		registers.RI1.Should().BeSameAs(registers[(int)Registers.Offset.rI1]);
		registers.RI2.Should().BeSameAs(registers[(int)Registers.Offset.rI2]);
		registers.RI3.Should().BeSameAs(registers[(int)Registers.Offset.rI3]);
		registers.RI4.Should().BeSameAs(registers[(int)Registers.Offset.rI4]);
		registers.RI5.Should().BeSameAs(registers[(int)Registers.Offset.rI5]);
		registers.RI6.Should().BeSameAs(registers[(int)Registers.Offset.rI6]);
	}
}
