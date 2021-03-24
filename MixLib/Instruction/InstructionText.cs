using MixLib.Type;

namespace MixLib.Instruction
{
	public class InstructionText
	{
		private readonly MixInstruction.Instance _instance;

		public InstructionText(MixInstruction.Instance instance) 
			=> _instance = instance;

		public string Index 
			=> _instance.Index != 0 ? "," + _instance.Index : "";

		public string InstanceText 
			=> Mnemonic + " " + Address + Index + Field;

		public string Mnemonic 
			=> _instance.MixInstruction.Mnemonic;

		private string FieldText
		{
			get
			{
				FieldSpec fieldSpec = _instance.FieldSpec;

				if (_instance.MixInstruction.MetaFieldSpec.FieldIsRange && fieldSpec.IsValid)
					return (fieldSpec.LowBound + ":" + fieldSpec.HighBound);

				return fieldSpec.MixByteValue.ByteValue.ToString();
			}
		}

		public string Address
		{
			get
			{
				var addressText = _instance.AddressMagnitude.ToString();
				
				if (_instance.AddressSign.IsNegative())
					addressText = '-' + addressText;

				return addressText;
			}
		}

		public string Field
		{
			get
			{
				string str = null;
				switch (_instance.MixInstruction.MetaFieldSpec.Presence)
				{
					case MetaFieldSpec.Presences.Optional:
						if (_instance.FieldSpec != _instance.MixInstruction.MetaFieldSpec.DefaultFieldSpec)
							str = FieldText;

						break;

					case MetaFieldSpec.Presences.Mandatory:
						str = FieldText;
						break;
				}

				return str != null ? "(" + str + ")" : "";
			}
		}
	}
}
