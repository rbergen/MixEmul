using System.Collections.Generic;

namespace MixLib.Utils
{
	public static class CollectionHelper
	{
		public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) where TValue : new()
		{
			TValue value;
			if (!dictionary.TryGetValue(key, out value))
			{
				value = new TValue();
				dictionary[key] = value;
			}

			return value;
		}
	}
}
