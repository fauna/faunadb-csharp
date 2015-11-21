using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace FaunaDB
{
    static class ImmutableUtil
    {
        public static ImmutableDictionary<TKey, TVal> Create<TKey, TVal>(params KeyValuePair<TKey, TVal>[] pairs)
        {
            return BuildDict<TKey, TVal>(Add =>
            {
                foreach (var kv in pairs)
                    Add(kv.Key, kv.Value);
            });
        }

        public static ImmutableDictionary<TKey, TVal> DictWithoutNullValues<TKey, TVal>(
            IEnumerable<KeyValuePair<TKey, TVal>> notNullPairs,
            IEnumerable<KeyValuePair<TKey, TVal>> nullablePairs)
            where TVal : class
        {
            return BuildDict<TKey, TVal>(Add =>
            {
                foreach (var kv in notNullPairs)
                    Add(kv.Key, kv.Value);
                foreach (var kv in nullablePairs)
                    if (kv.Value != null)
                        Add(kv.Key, kv.Value);
            });
        }

        public static ImmutableDictionary<TKey, TVal> DictWithoutNullValues<TKey, TVal>(
            params KeyValuePair<TKey, TVal>[] nullablePairs)
            where TVal : class
        {
            return DictWithoutNullValues<TKey, TVal>(Enumerable.Empty<KeyValuePair<TKey, TVal>>(), nullablePairs);
        }

        public static ImmutableDictionary<TKey, TVal> BuildDict<TKey, TVal>(Action<Action<TKey, TVal>> builder)
        {
            var d = ImmutableDictionary.CreateBuilder<TKey, TVal>();
            builder((k, v) => d.Add(new KeyValuePair<TKey, TVal>(k, v)));
            return d.ToImmutable();
        }

        public static ImmutableArray<T> BuildArray<T>(Action<Action<T>> builder)
        {
            var a = ImmutableArray.CreateBuilder<T>();
            builder(a.Add);
            return a.ToImmutable();
        }

        public static bool DictEquals<TKey, TVal>(ImmutableDictionary<TKey, TVal> a, ImmutableDictionary<TKey, TVal> b)
        {
            if (a.Count != b.Count)
                return false;
            foreach (var kv in a)
            {
                TVal valueB;
                if (!b.TryGetValue(kv.Key, out valueB))
                    return false;
                if (!object.Equals(kv.Value, valueB))
                    return false;
            }
            return true;
        }
    }
}

