using System.Collections.Generic;

namespace FaunaDB.Collections
{
    public static class ImmutableDictionary
    {
        public static IReadOnlyDictionary<TKey, TValue> Empty<TKey, TValue>() =>
            OrderedDictionary<TKey, TValue>.Empty;

        public static IReadOnlyDictionary<TKey1, TValue1> Of<TKey1, TValue1>(TKey1 k0, TValue1 v0)
        {
            var dic = new OrderedDictionary<TKey1, TValue1>();
            dic.Add(k0, v0);
            return dic;
        }

        public static IReadOnlyDictionary<TKey1, TValue1> Of<TKey1, TValue1>(TKey1 k0, TValue1 v0, TKey1 k1, TValue1 v1)
        {
            var dic = new OrderedDictionary<TKey1, TValue1>();
            dic.Add(k0, v0);
            dic.Add(k1, v1);
            return dic;
        }

        public static IReadOnlyDictionary<TKey1, TValue1> Of<TKey1, TValue1>(TKey1 k0, TValue1 v0, TKey1 k1, TValue1 v1, TKey1 k2, TValue1 v2)
        {
            var dic = new OrderedDictionary<TKey1, TValue1>();
            dic.Add(k0, v0);
            dic.Add(k1, v1);
            dic.Add(k2, v2);
            return dic;
        }

        public static IReadOnlyDictionary<TKey1, TValue1> Of<TKey1, TValue1>(TKey1 k0, TValue1 v0, TKey1 k1, TValue1 v1, TKey1 k2, TValue1 v2, TKey1 k3, TValue1 v3)
        {
            var dic = new OrderedDictionary<TKey1, TValue1>();
            dic.Add(k0, v0);
            dic.Add(k1, v1);
            dic.Add(k2, v2);
            dic.Add(k3, v3);
            return dic;
        }

        public static IReadOnlyDictionary<TKey1, TValue1> Of<TKey1, TValue1>(TKey1 k0, TValue1 v0, TKey1 k1, TValue1 v1, TKey1 k2, TValue1 v2, TKey1 k3, TValue1 v3, TKey1 k4, TValue1 v4)
        {
            var dic = new OrderedDictionary<TKey1, TValue1>();
            dic.Add(k0, v0);
            dic.Add(k1, v1);
            dic.Add(k2, v2);
            dic.Add(k3, v3);
            dic.Add(k4, v4);
            return dic;
        }

        public static IReadOnlyDictionary<TKey1, TValue1> Of<TKey1, TValue1>(TKey1 k0, TValue1 v0, TKey1 k1, TValue1 v1, TKey1 k2, TValue1 v2, TKey1 k3, TValue1 v3, TKey1 k4, TValue1 v4, TKey1 k5, TValue1 v5)
        {
            var dic = new OrderedDictionary<TKey1, TValue1>();
            dic.Add(k0, v0);
            dic.Add(k1, v1);
            dic.Add(k2, v2);
            dic.Add(k3, v3);
            dic.Add(k4, v4);
            dic.Add(k5, v5);
            return dic;
        }

        public static IReadOnlyDictionary<TKey1, TValue1> Of<TKey1, TValue1>(TKey1 k0, TValue1 v0, TKey1 k1, TValue1 v1, TKey1 k2, TValue1 v2, TKey1 k3, TValue1 v3, TKey1 k4, TValue1 v4, TKey1 k5, TValue1 v5, TKey1 k6, TValue1 v6)
        {
            var dic = new OrderedDictionary<TKey1, TValue1>();
            dic.Add(k0, v0);
            dic.Add(k1, v1);
            dic.Add(k2, v2);
            dic.Add(k3, v3);
            dic.Add(k4, v4);
            dic.Add(k5, v5);
            dic.Add(k6, v6);
            return dic;
        }

    }
}
