using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace FaunaDB.Collections
{
    class ImmutableDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    {
        public static readonly ImmutableDictionary<TKey, TValue> Empty =
            new ImmutableDictionary<TKey, TValue>();

        readonly OrderedDictionary dictionary = new OrderedDictionary();

        internal ImmutableDictionary(params KeyValuePair<TKey, TValue>[] values)
            : this((IEnumerable<KeyValuePair<TKey, TValue>>)values)
        { }

        internal ImmutableDictionary(IEnumerable<KeyValuePair<TKey, TValue>> values)
        {
            foreach (var kv in values)
            {
                if (kv.Value == null)
                    continue;

                dictionary.Add(kv.Key, kv.Value);
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                if (key == null)
                    throw new ArgumentNullException(nameof(key));

                if (!ContainsKey(key))
                    throw new KeyNotFoundException();

                return (TValue)dictionary[key];
            }
        }

        public int Count
        {
            get
            {
                return dictionary.Count;
            }
        }

        public IEnumerable<TKey> Keys
        {
            get
            {
                TKey[] array = new TKey[dictionary.Keys.Count];
                dictionary.Keys.CopyTo(array, 0);
                return new List<TKey>(array);
            }
        }

        public IEnumerable<TValue> Values
        {
            get
            {
                TValue[] array = new TValue[dictionary.Values.Count];
                dictionary.Values.CopyTo(array, 0);
                return new List<TValue>(array);
            }
        }

        public bool ContainsKey(TKey key)
        {
            return dictionary.Contains(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (dictionary.Contains(key))
            {
                value = this[key];
                return true;
            }

            value = default(TValue);
            return false;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var key in Keys)
            {
                yield return new KeyValuePair<TKey, TValue>(key, this[key]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
    }

    static class ImmutableDictionary
    {
        public static IReadOnlyDictionary<TKey, TValue> Empty<TKey, TValue>() =>
            ImmutableDictionary<TKey, TValue>.Empty;

        public static IReadOnlyDictionary<TKey, TValue> Of<TKey, TValue>(TKey k0, TValue v0)
        {
            return new ImmutableDictionary<TKey, TValue>(
                new KeyValuePair<TKey, TValue>(k0, v0)
            );
        }

        public static IReadOnlyDictionary<TKey, TValue> Of<TKey, TValue>(TKey k0, TValue v0, TKey k1, TValue v1)
        {
            return new ImmutableDictionary<TKey, TValue>(
                new KeyValuePair<TKey, TValue>(k0, v0),
                new KeyValuePair<TKey, TValue>(k1, v1)
            );
        }

        public static IReadOnlyDictionary<TKey, TValue> Of<TKey, TValue>(TKey k0, TValue v0, TKey k1, TValue v1, TKey k2, TValue v2)
        {
            return new ImmutableDictionary<TKey, TValue>(
                new KeyValuePair<TKey, TValue>(k0, v0),
                new KeyValuePair<TKey, TValue>(k1, v1),
                new KeyValuePair<TKey, TValue>(k2, v2)
            );
        }

        public static IReadOnlyDictionary<TKey, TValue> Of<TKey, TValue>(TKey k0, TValue v0, TKey k1, TValue v1, TKey k2, TValue v2, TKey k3, TValue v3)
        {
            return new ImmutableDictionary<TKey, TValue>(
                new KeyValuePair<TKey, TValue>(k0, v0),
                new KeyValuePair<TKey, TValue>(k1, v1),
                new KeyValuePair<TKey, TValue>(k2, v2),
                new KeyValuePair<TKey, TValue>(k3, v3)
            );
        }

        public static IReadOnlyDictionary<TKey, TValue> Of<TKey, TValue>(TKey k0, TValue v0, TKey k1, TValue v1, TKey k2, TValue v2, TKey k3, TValue v3, TKey k4, TValue v4)
        {
            return new ImmutableDictionary<TKey, TValue>(
                new KeyValuePair<TKey, TValue>(k0, v0),
                new KeyValuePair<TKey, TValue>(k1, v1),
                new KeyValuePair<TKey, TValue>(k2, v2),
                new KeyValuePair<TKey, TValue>(k3, v3),
                new KeyValuePair<TKey, TValue>(k4, v4)
            );
        }

        public static IReadOnlyDictionary<TKey, TValue> Of<TKey, TValue>(TKey k0, TValue v0, TKey k1, TValue v1, TKey k2, TValue v2, TKey k3, TValue v3, TKey k4, TValue v4, TKey k5, TValue v5)
        {
            return new ImmutableDictionary<TKey, TValue>(
                new KeyValuePair<TKey, TValue>(k0, v0),
                new KeyValuePair<TKey, TValue>(k1, v1),
                new KeyValuePair<TKey, TValue>(k2, v2),
                new KeyValuePair<TKey, TValue>(k3, v3),
                new KeyValuePair<TKey, TValue>(k4, v4),
                new KeyValuePair<TKey, TValue>(k5, v5)
            );
        }

        public static IReadOnlyDictionary<TKey, TValue> Of<TKey, TValue>(TKey k0, TValue v0, TKey k1, TValue v1, TKey k2, TValue v2, TKey k3, TValue v3, TKey k4, TValue v4, TKey k5, TValue v5, TKey k6, TValue v6)
        {
            return new ImmutableDictionary<TKey, TValue>(
                new KeyValuePair<TKey, TValue>(k0, v0),
                new KeyValuePair<TKey, TValue>(k1, v1),
                new KeyValuePair<TKey, TValue>(k2, v2),
                new KeyValuePair<TKey, TValue>(k3, v3),
                new KeyValuePair<TKey, TValue>(k4, v4),
                new KeyValuePair<TKey, TValue>(k5, v5),
                new KeyValuePair<TKey, TValue>(k6, v6)
            );
        }
    }
}
