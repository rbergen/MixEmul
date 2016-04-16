using MixLib.Type;

namespace MixLib.Instruction
{
	public class InstructionText
	{
		private MixInstruction.Instance mInstance;

		public InstructionText(MixInstruction.Instance instance)
		{
			mInstance = instance;
		}

		private string getFieldText()
		{
			FieldSpec fieldSpec = mInstance.FieldSpec;
			if (mInstance.MixInstruction.MetaFieldSpec.FieldIsRange && fieldSpec.IsValid)
			{
				return (fieldSpec.LowBound.ToString() + ":" + fieldSpec.HighBound.ToString());
			}
			return fieldSpec.MixByteValue.ByteValue.ToString();
		}

		public string Address
		{
			get
			{
				string addressText = mInstance.AddressMagnitude.ToString();
				if (mInstance.AddressSign == Word.Signs.Negative)
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
				if (str != null)
				{
					return ("(" + str + ")");
				}
				return "";
			}
		}

		public string Index
		{
			get
			{
				if (mInstance.Index != 0)
				{
					return ("," + mInstance.Index);
				}
				return "";
			}
		}

		public string InstanceText
		{
			get
			{
				return (Mnemonic + " " + Address + Index + Field);
			}
		}

		public string Mnemonic
		{
			get
			{
				return mInstance.MixInstruction.Mnemonic;
			}
		}
	}
}
