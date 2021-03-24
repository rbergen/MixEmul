using MixAssembler.Value;
using MixLib.Instruction;
using MixLib.Type;
using System;

namespace MixAssembler.Instruction
{
	/// <summary>
	/// This class contains the parameters that are required to create an instance object of a specific MIX instruction "template".
	/// 
	/// The (static) ParseAddressField method is to be called during the parsing of the MIXAL source (first assembly pass), to create a parameter object based on the MIX instruction's address field.
	/// The CreateInstance method should be called during the second assembly pass, to create the actual MIX instruction instance.
	/// </summary>
	public class MixInstructionParameters : IInstructionParameters
	{
		private readonly IValue mAddress;
		private readonly IValue mField;
		private readonly int mFieldPartCharIndex;
		private readonly IValue mIndex;
		private readonly int mIndexPartCharIndex;
		private readonly int mTextLength;

		private MixInstructionParameters(IValue address, int indexPartCharIndex, IValue index, int fieldPartCharIndex, IValue field, int textLength)
		{
			mAddress = address;
			mIndex = index;
			mField = field;
			mIndexPartCharIndex = indexPartCharIndex;
			mFieldPartCharIndex = fieldPartCharIndex;
			mTextLength = textLength;
		}

		private bool AreValuesDefined(AssemblingStatus status)
		{
			if (!mAddress.IsValueDefined(status.LocationCounter))
			{
				status.ReportParsingError(LineSection.AddressField, 0, mIndexPartCharIndex, "address value undefined");
				return false;
			}

			if (!mIndex.IsValueDefined(status.LocationCounter))
			{
				status.ReportParsingError(LineSection.AddressField, mIndexPartCharIndex, mFieldPartCharIndex - mIndexPartCharIndex, "index value undefined");
				return false;
			}

			if (!mAddress.IsValueDefined(status.LocationCounter))
			{
				status.ReportParsingError(LineSection.AddressField, mFieldPartCharIndex, mTextLength - mFieldPartCharIndex, "field value undefined");
				return false;
			}

			return true;
		}

		/// <summary>
		/// Create a MIX instruction instance with the parameters contained in this object.
		/// </summary>
		/// <param name="instruction">MixInstruction to create an instance of. This method will throw an exception if this parameter is of a different instruction type.</param>
		/// <param name="status">AssemblingStatus object reflecting the current state of the assembly process</param>
		/// <returns></returns>
		public InstructionInstanceBase CreateInstance(InstructionBase instruction, AssemblingStatus status)
		{
			if (instruction is not MixInstruction)
				throw new ArgumentException("instruction must be a MixInstruction", nameof(instruction));

			var mixInstruction = (MixInstruction)instruction;

			if (!AreValuesDefined(status))
				return null;

			var addressMagnitude = mAddress.GetMagnitude(status.LocationCounter);
			var word = new Word(MixInstruction.AddressByteCount);
			if (addressMagnitude > word.MaxMagnitude)
			{
				status.ReportError(LineSection.AddressField, 0, mIndexPartCharIndex, new MixAssembler.Finding.ParsingError("address value " + addressMagnitude + " invalid", (int)-word.MaxMagnitude, (int)word.MaxMagnitude));
				return null;
			}

			word.MagnitudeLongValue = addressMagnitude;
			var fieldSpecValue = GetFieldSpecValue(status, mixInstruction);
			if (fieldSpecValue == null)
				return null;

			var instructionWord = new FullWord
			{
				Sign = mAddress.GetSign(status.LocationCounter)
			};

			for (int i = 0; i < word.ByteCount; i++)
				instructionWord[i] = word[i];

			instructionWord[MixInstruction.IndexByte] = (int)mIndex.GetValue(status.LocationCounter);
			instructionWord[MixInstruction.FieldSpecByte] = fieldSpecValue;
			instructionWord[MixInstruction.OpcodeByte] = mixInstruction.Opcode;

			var instance = mixInstruction.CreateInstance(instructionWord);
			ReportInstanceErrors(status, instance);

			return instance;
		}

		private MixByte GetFieldSpecValue(AssemblingStatus status, MixInstruction mixInstruction)
		{
			var fieldValue = mField.GetValue(status.LocationCounter);

			switch (mixInstruction.MetaFieldSpec.Presence)
			{
				case MetaFieldSpec.Presences.Forbidden:
					if (fieldValue == long.MinValue)
						return mixInstruction.FieldSpec.MixByteValue;

					status.ReportParsingError(LineSection.AddressField, mFieldPartCharIndex, mTextLength - mFieldPartCharIndex, "fieldspec forbidden for this instruction");
					return null;

				case MetaFieldSpec.Presences.Optional:
					if (fieldValue == long.MinValue)
						return mixInstruction.MetaFieldSpec.DefaultFieldSpec.MixByteValue;

					return (int)fieldValue;

				case MetaFieldSpec.Presences.Mandatory:
					if (fieldValue != long.MinValue)
						return (int)fieldValue;

					status.ReportParsingError(LineSection.AddressField, mFieldPartCharIndex, mTextLength - mFieldPartCharIndex, "fieldspec mandatory for this instruction");
					return null;
			}
			return null;
		}

		/// <summary>
		/// Creates an instance of this class by parsing the address field of a MIX instruction. 
		/// </summary>
		/// <param name="instruction">MixInstruction to parse the address field for. This method will throw an exception if this parameter is of a different instruction type.</param>
		/// <param name="addressField">The address field to parse.</param>
		/// <param name="status">ParsingStatus object reflecting the current state of the parse process</param>
		/// <returns></returns>
		public static IInstructionParameters ParseAddressField(InstructionBase instruction, string addressField, ParsingStatus status)
		{
			var indexCharIndex = addressField.IndexOf(',');
			var sectionCharIndex = addressField.IndexOf('(', Math.Max(indexCharIndex, 0));

			if (sectionCharIndex == -1)
				sectionCharIndex = addressField.Length;

			if (indexCharIndex == -1)
				indexCharIndex = sectionCharIndex;

			var address = APartValue.ParseValue(addressField.Substring(0, indexCharIndex), 0, status);
			if (address == null)
			{
				status.ReportParsingError(0, indexCharIndex, "unable to parse address");
				return null;
			}

			var index = IPartValue.ParseValue(addressField[indexCharIndex..sectionCharIndex], indexCharIndex, status);
			if (index == null)
			{
				status.ReportParsingError(indexCharIndex, sectionCharIndex - indexCharIndex, "unable to parse index");
				return null;
			}

			var field = FPartValue.ParseValue(addressField[sectionCharIndex..], sectionCharIndex, status);
			if (field == null)
			{
				status.ReportParsingError(sectionCharIndex, addressField.Length - sectionCharIndex, "unable to parse field");
				return null;
			}

			return new MixInstructionParameters(address, indexCharIndex, index, sectionCharIndex, field, addressField.Length);
		}

		private void ReportInstanceErrors(AssemblingStatus status, MixInstruction.Instance instance)
		{
			var errorArray = instance.Validate();

			if (errorArray == null)
				return;

			int causeStartIndex = 0;
			int causeLength = 0;

			foreach (InstanceValidationError error in errorArray)
			{
				switch (error.Source)
				{
					case InstanceValidationError.Sources.Address:
						causeStartIndex = 0;
						causeLength = mIndexPartCharIndex;
						break;

					case InstanceValidationError.Sources.Index:
						causeStartIndex = mIndexPartCharIndex;
						causeLength = mFieldPartCharIndex - mIndexPartCharIndex;
						break;

					case InstanceValidationError.Sources.FieldSpec:
						causeStartIndex = mFieldPartCharIndex;
						causeLength = mTextLength - mFieldPartCharIndex;
						break;
				}

				status.ReportError(LineSection.AddressField, causeStartIndex, causeLength, error);
			}
		}
	}
}
