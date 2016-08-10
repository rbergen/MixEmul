using MixLib.Type;

namespace MixLib.Instruction
{
	public class InstructionText
	{
        readonly MixInstruction.Instance mInstance;

        public InstructionText(MixInstruction.Instance instance)
		{
			mInstance = instance;
		}

        public string Index => mInstance.Index != 0 ? "," + mInstance.Index : "";

        public string InstanceText => Mnemonic + " " + Address + Index + Field;

        public string Mnemonic => mInstance.MixInstruction.Mnemonic;

        string getFieldText()
        {
            FieldSpec fieldSpec = mInstance.FieldSpec;
            if (mInstance.MixInstruction.MetaFieldSpec.FieldIsRange && fieldSpec.IsValid)
            {
                return (fieldSpec.LowBound + ":" + fieldSpec.HighBound);
            }
            return fieldSpec.MixByteValue.ByteValue.ToString();
        }

        public string Address
		{
			get
			{
				var addressText = mInstance.AddressMagnitude.ToString();
				if (mInstance.AddressSign.IsNegative())
				{
					addressText = '-' + addressText;
				}

				return addressText;
			}
		}

		public string Field
		{
			get
			{
				string str = null;
				switch (mInstance.MixInstruction.MetaFieldSpec.Presence)
				{
					case MetaFieldSpec.Presences.Optional:
						if (mInstance.FieldSpec != mInstance.MixInstruction.MetaFieldSpec.DefaultFieldSpec)
						{
							str = getFieldText();
						}
						break;

					case MetaFieldSpec.Presences.Mandatory:
						str = getFieldText();
						break;
				}

				return str != null ? "(" + str + ")" : "";
			}
		}
	}
}
