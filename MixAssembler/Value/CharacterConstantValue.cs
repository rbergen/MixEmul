using MixLib.Type;

namespace MixAssembler.Value
{
	public class CharacterConstantValue
	{
		public static IValue ParseValue(string text, int sectionCharIndex, ParsingStatus status)
		{
			if (text.Length >= 1 && text[0] == '"')
			{
				int length = text.Length - 1;

				// a character constant can (but is not required to) end with a double quote
				if (text[length] == '"')
				{
					length--;
				}

				text = text.Substring(1, length);
				sectionCharIndex++;
			}

			// if the character constant is shorter than the number of bytes in a full word, we pad it at the right with spaces
			if (text.Length < FullWord.ByteCount)
			{
				text = text + new string(' ', FullWord.ByteCount - text.Length);
			}

			FullWord word = new FullWord();
			for (int i = 0; i < FullWord.ByteCount; i++)
			{
				word[i] = text[i];
			}

			return new NumberValue(word.LongValue);
		}
	}
}
