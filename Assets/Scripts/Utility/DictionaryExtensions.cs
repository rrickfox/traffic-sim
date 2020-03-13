using System.Collections.Generic;

namespace Utility
{
    public static class DictionaryExtensions
    {
        // returns specified element in dictionary and removes it
        public static TValue Pop<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        {
            var value = dictionary[key];
            dictionary.Remove(key);
            return value;
        }
    }
}