﻿using System;
using MixAssembler.Value;
using MixLib.Instruction;
using MixLib.Type;

namespace MixAssembler.Instruction
{
  /// <summary>
  /// This class contains the parameters that are required to create an instance object of a specific loader instruction "template".
  /// 
  /// The (static) ParseAddressField method is to be called during the parsing of the MIXAL source (first assembly pass), to create a parameter object based on the loader instruction's address field.
  /// The CreateInstance method should be called during the second assembly pass, to create the actual loader instruction instance.
  /// </summary>
  /// <remarks>
  /// Constructor for this class. 
  /// </remarks>
  /// <param name="value">The parameter value associated with the instruction. The type of the value depends on the loader instruction it is connected with</param>
  /// <param name="textLength">The length (character count) of the value in the source code it was parsed from</param>
  public class LoaderInstructionParameters(IValue value, int textLength) : IInstructionParameters
	{
		/// <summary>
		/// Create a loader instruction instance with the parameters contained in this object.
		/// </summary>
		/// <param name="instruction">LoaderInstruction to create an instance of. This method will throw an exception if this parameter is of a different instruction type.</param>
		/// <param name="status">AssemblingStatus object reflecting the current state of the assembly process</param>
		/// <returns></returns>
		public InstructionInstanceBase CreateInstance(InstructionBase instruction, AssemblingStatus status)
		{
			if (instruction is not LoaderInstruction loaderInstruction)
				throw new ArgumentException("instruction must be a LoaderInstruction", nameof(instruction));

			if (!value.IsValueDefined(status.LocationCounter))
			{
				status.ReportParsingError(LineSection.AddressField, 0, textLength, "value is not defined");
				return null;
			}

			return new LoaderInstruction.Instance(loaderInstruction, value.GetSign(status.LocationCounter), value.GetMagnitude(status.LocationCounter));
		}

		/// <summary>
		/// Creates an instance of this class by parsing the address field of a loader instruction. 
		/// </summary>
		/// <param name="instruction">LoaderInstruction to parse the address field for. This method will throw an exception if this parameter is of a different instruction type.</param>
		/// <param name="addressField">The address field to parse.</param>
		/// <param name="status">ParsingStatus object reflecting the current state of the parse process</param>
		/// <returns></returns>
		public static IInstructionParameters ParseAddressField(InstructionBase instruction, string addressField, ParsingStatus status)
		{
			if (instruction is not LoaderInstruction loaderInstruction)
				throw new ArgumentException("instruction must be a LoaderInstruction", nameof(instruction));

			IValue address = loaderInstruction.Alphanumeric ? CharacterConstantValue.ParseValue(addressField, 0, status) : WValue.ParseValue(addressField, 0, status);

			if (address == null)
			{
				status.ReportParsingError(0, addressField.Length, "unable to parse value");
				return null;
			}

			return new LoaderInstructionParameters(address, addressField.Length);
		}
	}
}
