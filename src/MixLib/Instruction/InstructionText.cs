using MixLib.Type;

namespace MixLib.Instruction
{
	public class InstructionText
	{
		private readonly MixInstruction.Instance instance;

		public InstructionText(MixInstruction.Instance instance) 
			=> this.instance = instance;

		public string Index 
			=> this.instance.Index != 0 ? "," + this.instance.Index : "";

		public string InstanceText 
			=> Mnemonic + " " + Address + Index + Field;

		public string Mnemonic 
			=> this.instance.MixInstruction.Mnemonic;

		private string FieldText
		{
			get
			{
				FieldSpec fieldSpec = this.instance.FieldSpec;

				if (this.instance.MixInstruction.MetaFieldSpec.FieldIsRange && fieldSpec.IsValid)
					return (fieldSpec.LowBound + ":" + fieldSpec.HighBound);

				return fieldSpec.MixByteValue.ByteValue.ToString();
			}
		}

		public string Address
		{
			get
			{
				var addressText = this.instance.AddressMagnitude.ToString();
				
				if (this.instance.AddressSign.IsNegative())
					addressText = '-' + addressText;

				return addressText;
			}
		}

		public string Field
		{
			get
			{
				string str = null;
				switch (this.instance.MixInstruction.MetaFieldSpec.Presence)
				{
					case MetaFieldSpec.Presences.Optional:
						if (this.instance.FieldSpec != this.instance.MixInstruction.MetaFieldSpec.DefaultFieldSpec)
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
