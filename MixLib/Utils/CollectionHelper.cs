using System.Collections.Generic;

namespace MixLib.Utils
{
	public static class CollectionHelper
	{
		public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) where TValue : new()
		{
            if (!dictionary.TryGetValue(key, out TValue value))
            {
                value = new TValue();
                dictionary[key] = value;
            }

            return value;
		}
	}
}
