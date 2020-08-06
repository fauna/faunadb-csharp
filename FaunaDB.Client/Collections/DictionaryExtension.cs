using System;
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

        public static string Debug<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> source)
            where TKey: IComparable
        {
            List<TKey> elements = source.Keys.ToList();
            elements.Sort();

            string[] result = new string[elements.Count];

            for (int i = 0; i < result.Length; i++)
            {
                var key = elements[i];
                var value = source[key].ToString();
                result[i] = $"\"{key}\": {value}";
            }

            return string.Join(", ", result);
        }
    }
}

