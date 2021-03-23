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

			var index = text.IndexOf(options.SearchText, startIndex, System.StringComparison.Ordinal);

			while (index >= 0)
			{
				// if we're not doing a full word search or the search text is the full text, we have a match
				if (!options.MatchWholeWord || options.SearchText.Length == text.Length)
				{
					return index;
				}

				// ...otherwise check if the character before and after the search text (insofar present) are not alphanumeric
				if ((index == 0 || !char.IsLetterOrDigit(text, index - 1))
					&& (index + options.SearchText.Length == text.Length
												|| !char.IsLetterOrDigit(text, index + options.SearchText.Length)))
				{
					return index;
				}

				index = text.IndexOf(options.SearchText, index + 1, System.StringComparison.Ordinal);
			}

			return -1;
		}
	}
}
