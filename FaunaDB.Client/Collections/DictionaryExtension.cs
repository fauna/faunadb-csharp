using System.Linq;
using System.Collections.Generic;

namespace FaunaDB.Collections
{
    static class DictionaryExtension
    {
        public static bool DictEquals<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> a, IReadOnlyDictionary<TKey, TValue> b)
        {
            if (a.Count != b.Count)
                return false;

            foreach (var kv in a)
            {
                TValue valueB;
                if (!b.TryGetValue(kv.Key, out valueB))
                    return false;

                if (!Equals(kv.Value, valueB))
                    return false;
            }

            return true;
        }

        public static Dictionary<TKey, TValue> FilterNulls<TKey, TValue>(this Dictionary<TKey, TValue> dict) =>
            dict.Where(kv => kv.Value != null).ToDictionary(kv => kv.Key, kv => kv.Value);
    }
}

