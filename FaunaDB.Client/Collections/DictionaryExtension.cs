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
    }
}

