using System;

namespace MixLib.Type
{
	public class WordField : Word
	{
        FieldSpec mFieldSpec;

        WordField(FieldSpec fieldSpec, int byteCount)
            : base(byteCount)
        {
            mFieldSpec = fieldSpec;
        }

        public void ApplyToFullWord(IFullWord word)
		{
			int byteCount = mFieldSpec.ByteCount;
			int lowBoundByteIndex = mFieldSpec.LowBoundByteIndex;

			for (int i = 0; i < byteCount; i++)
			{
				word[lowBoundByteIndex + i] = base[i];
			}

			if (mFieldSpec.IncludesSign)
			{
				word.Sign = Sign;
			}
		}

		public void ApplyToRegister(Register register)
		{
			int fieldSpecByteCount = mFieldSpec.ByteCount;
			if (fieldSpecByteCount > register.ByteCountWithPadding)
			{
				throw new ArgumentOutOfRangeException(nameof(register), register, "bytecount too large for this register");
			}

			int paddingByteCount = register.ByteCount - fieldSpecByteCount;
			for (int i = 0; i < paddingByteCount; i++)
			{
				register[i] = 0;
			}

			int fromWordByteCount = Math.Max(0, -paddingByteCount);
			fieldSpecByteCount--;

			while (fieldSpecByteCount >= fromWordByteCount)
			{
				register[paddingByteCount + fieldSpecByteCount] = base[fieldSpecByteCount];
				fieldSpecByteCount--;
			}

			register.Sign = Sign;
		}

		public int CompareTo(IFullWord toCompare)
		{
			WordField field = LoadFromFullWord(mFieldSpec, toCompare);

			long wordValue = LongValue;
			long fieldValue = field.LongValue;

			return wordValue.CompareTo(fieldValue);
		}

		public static WordField LoadFromFullWord(FieldSpec fieldSpec, IFullWord word)
		{
			int fieldSpecByteCount = fieldSpec.ByteCount;

			var field = new WordField(fieldSpec, fieldSpecByteCount);
			int lowBoundByteIndex = fieldSpec.LowBoundByteIndex;

			for (int i = 0; i < fieldSpecByteCount; i++)
			{
				field[i] = word[lowBoundByteIndex + i];
			}

			if (fieldSpec.IncludesSign)
			{
				field.Sign = word.Sign;
			}

			return field;
		}

		public static WordField LoadFromRegister(FieldSpec fieldSpec, Register register)
		{
			int fieldSpecByteCount = fieldSpec.ByteCount;
			if (fieldSpecByteCount > register.ByteCountWithPadding)
			{
				throw new ArgumentOutOfRangeException(nameof(fieldSpec), fieldSpec, "bytecount too large for this register");
			}

			var field = new WordField(fieldSpec, fieldSpecByteCount);
			int fromRegisterStartIndex = register.ByteCountWithPadding - fieldSpecByteCount;
			fieldSpecByteCount--;

			while (fieldSpecByteCount >= 0)
			{
				field[fieldSpecByteCount] = register.GetByteWithPadding(fromRegisterStartIndex + fieldSpecByteCount);
				fieldSpecByteCount--;
			}

			if (fieldSpec.IncludesSign)
			{
				field.Sign = register.Sign;
			}

			return field;
		}
	}
}
