using System;
using System.Collections;
using System.Collections.Generic;

namespace FaunaDB.Collections
{
    static class ImmutableDictionary
    {
        public static IReadOnlyDictionary<TKey, TValue> Empty<TKey, TValue>() =>
            new Dictionary<TKey, TValue>();

        public static IReadOnlyDictionary<TKey, TValue> Of<TKey, TValue>(TKey k0, TValue v0)
        {
            return new Dictionary<TKey, TValue> {
                {k0, v0}
            }.FilterNulls();
        }

        public static IReadOnlyDictionary<TKey, TValue> Of<TKey, TValue>(TKey k0, TValue v0, TKey k1, TValue v1)
        {
            return new Dictionary<TKey, TValue> {
                {k0, v0},
                {k1, v1}
            }.FilterNulls();
        }

        public static IReadOnlyDictionary<TKey, TValue> Of<TKey, TValue>(TKey k0, TValue v0, TKey k1, TValue v1, TKey k2, TValue v2)
        {
            return new Dictionary<TKey, TValue> {
                {k0, v0},
                {k1, v1},
                {k2, v2}
            }.FilterNulls();
        }

        public static IReadOnlyDictionary<TKey, TValue> Of<TKey, TValue>(TKey k0, TValue v0, TKey k1, TValue v1, TKey k2, TValue v2, TKey k3, TValue v3)
        {
            return new Dictionary<TKey, TValue> {
                {k0, v0},
                {k1, v1},
                {k2, v2},
                {k3, v3}
            }.FilterNulls();
        }

        public static IReadOnlyDictionary<TKey, TValue> Of<TKey, TValue>(TKey k0, TValue v0, TKey k1, TValue v1, TKey k2, TValue v2, TKey k3, TValue v3, TKey k4, TValue v4)
        {
            return new Dictionary<TKey, TValue> {
                {k0, v0},
                {k1, v1},
                {k2, v2},
                {k3, v3},
                {k4, v4}
            }.FilterNulls();
        }

        public static IReadOnlyDictionary<TKey, TValue> Of<TKey, TValue>(TKey k0, TValue v0, TKey k1, TValue v1, TKey k2, TValue v2, TKey k3, TValue v3, TKey k4, TValue v4, TKey k5, TValue v5)
        {
            return new Dictionary<TKey, TValue> {
                {k0, v0},
                {k1, v1},
                {k2, v2},
                {k3, v3},
                {k4, v4},
                {k5, v5}
            }.FilterNulls();
        }

        public static IReadOnlyDictionary<TKey, TValue> Of<TKey, TValue>(TKey k0, TValue v0, TKey k1, TValue v1, TKey k2, TValue v2, TKey k3, TValue v3, TKey k4, TValue v4, TKey k5, TValue v5, TKey k6, TValue v6)
        {
            return new Dictionary<TKey, TValue> {
                {k0, v0},
                {k1, v1},
                {k2, v2},
                {k3, v3},
                {k4, v4},
                {k5, v5},
                {k6, v6}
            }.FilterNulls();
        }
    }
}
