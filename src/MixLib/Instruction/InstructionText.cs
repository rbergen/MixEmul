using MixLib.Type;

namespace MixLib.Instruction
{
	public class InstructionText(MixInstruction.Instance instance)
  {
		public string Index 
			=> instance.Index != 0 ? "," + instance.Index : string.Empty;

		public string InstanceText 
			=> Mnemonic + " " + Address + Index + Field;

		public string Mnemonic 
			=> instance.MixInstruction.Mnemonic;

		private string FieldText
		{
			get
			{
				FieldSpec fieldSpec = instance.FieldSpec;

				if (instance.MixInstruction.MetaFieldSpec.FieldIsRange && fieldSpec.IsValid)
					return (fieldSpec.LowBound + ":" + fieldSpec.HighBound);

				return fieldSpec.MixByteValue.ByteValue.ToString();
			}
		}

		public string Address
		{
			get
			{
				var addressText = instance.AddressMagnitude.ToString();
				
				if (instance.AddressSign.IsNegative())
					addressText = '-' + addressText;

				return addressText;
			}
		}

		public string Field
		{
			get
			{
				string str = null;
				switch (instance.MixInstruction.MetaFieldSpec.Presence)
				{
					case MetaFieldSpec.Presences.Optional:
						if (instance.FieldSpec != instance.MixInstruction.MetaFieldSpec.DefaultFieldSpec)
							str = FieldText;

						break;

					case MetaFieldSpec.Presences.Mandatory:
						str = FieldText;
						break;
				}

				return str != null ? "(" + str + ")" : string.Empty;
			}
		}
	}
}
