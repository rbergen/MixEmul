using MixLib.Type;

namespace MixLib.Utils
{
	public static class TextHelper
	{
		public static int FindMatch(this string text, SearchParameters options, int startIndex)
		{
			if (text == null || startIndex >= text.Length)
			{
				return -1;
			}

			if (startIndex < 0)
			{
				startIndex = 0;
			}

			int index = text.IndexOf(options.SearchText, startIndex);

			while (index >= 0)
			{
				if (!options.MatchWholeWord
					|| (options.SearchText.Length == text.Length
							|| ((index == 0 || !char.IsLetterOrDigit(text, index - 1))
									&& (index + options.SearchText.Length == text.Length || !char.IsLetterOrDigit(text, index + options.SearchText.Length)))))
				{
					return index;
				}

				index = text.IndexOf(options.SearchText, index + 1);
			}

			return -1;
		}
	}
}
