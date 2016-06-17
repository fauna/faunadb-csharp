using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using FaunaDB.Query;

namespace FaunaDB.Utils
{
    public class OrderedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private OrderedDictionary dictionary;

        internal OrderedDictionary(OrderedDictionary dictionary)
        {
            this.dictionary = dictionary;
        }

        public OrderedDictionary() : this(new OrderedDictionary()) { }

        public OrderedDictionary<TKey, TValue> ToImmutable() =>
            new OrderedDictionary<TKey, TValue>(dictionary.AsReadOnly());

        public TValue this[TKey key]
        {
            get
            {
                if (!ContainsKey(key))
                    throw new KeyNotFoundException();

                return (TValue)dictionary[key];
            }

            set
            {
                dictionary[key] = value;
            }
        }

        public int Count
        {
            get
            {
                return dictionary.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return dictionary.IsReadOnly;
            }
        }

        public ICollection<TKey> Keys
        {
            get
            {
                TKey[] array = new TKey[dictionary.Keys.Count];
                dictionary.Keys.CopyTo(array, 0);
                return new List<TKey>(array);
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                TValue[] array = new TValue[dictionary.Values.Count];
                dictionary.Values.CopyTo(array, 0);
                return new List<TValue>(array);
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            if (item.Value != null)
                dictionary.Add(item.Key, item.Value);
        }

        public void Add(TKey key, TValue value)
        {
            if (value != null)
                dictionary.Add(key, value);
        }

        public void Clear() =>
            dictionary.Clear();

        public bool Contains(KeyValuePair<TKey, TValue> item) =>
            dictionary.Contains(item.Key) && dictionary[item.Key].Equals(item.Value);

        public bool ContainsKey(TKey key) =>
            dictionary.Contains(key);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) =>
            dictionary.CopyTo(array, arrayIndex);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var key in Keys)
            {
                yield return new KeyValuePair<TKey, TValue>(key, this[key]);
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item) =>
            Remove(item.Key);

        public bool Remove(TKey key)
        {
            if (dictionary.Contains(key))
            {
                dictionary.Remove(key);
                return true;
            }
            return false;
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

        IEnumerator IEnumerable.GetEnumerator() =>
            dictionary.GetEnumerator();

        public override int GetHashCode() =>
            dictionary.GetHashCode();

        public override bool Equals(object obj)
        {
            var other = obj as OrderedDictionary<TKey, TValue>;
            return other != null && DictEquals(this, other);
        }

        public static bool DictEquals(OrderedDictionary<TKey, TValue> a, OrderedDictionary<TKey, TValue> b)
        {
            if (a.Count != b.Count)
                return false;
            foreach (var kv in a)
            {
                TValue valueB;
                if (!b.TryGetValue(kv.Key, out valueB))
                    return false;
                if (!object.Equals(kv.Value, valueB))
                    return false;
            }
            return true;
        }
    }

    public class ImmutableDictionary {
        public static OrderedDictionary<TKey1, TValue1> Of<TKey1, TValue1>(TKey1 k0, TValue1 v0)
        {
            OrderedDictionary<TKey1, TValue1> dic = new OrderedDictionary<TKey1, TValue1>();
            dic.Add(k0, v0);
            return dic.ToImmutable();
        }

        public static OrderedDictionary<TKey1, TValue1> Of<TKey1, TValue1>(TKey1 k0, TValue1 v0, TKey1 k1, TValue1 v1)
        {
            OrderedDictionary<TKey1, TValue1> dic = new OrderedDictionary<TKey1, TValue1>();
            dic.Add(k0, v0);
            dic.Add(k1, v1);
            return dic.ToImmutable();
        }

        public static OrderedDictionary<TKey1, TValue1> Of<TKey1, TValue1>(TKey1 k0, TValue1 v0, TKey1 k1, TValue1 v1, TKey1 k2, TValue1 v2)
        {
            OrderedDictionary<TKey1, TValue1> dic = new OrderedDictionary<TKey1, TValue1>();
            dic.Add(k0, v0);
            dic.Add(k1, v1);
            dic.Add(k2, v2);
            return dic.ToImmutable();
        }

        public static OrderedDictionary<TKey1, TValue1> Of<TKey1, TValue1>(TKey1 k0, TValue1 v0, TKey1 k1, TValue1 v1, TKey1 k2, TValue1 v2, TKey1 k3, TValue1 v3)
        {
            OrderedDictionary<TKey1, TValue1> dic = new OrderedDictionary<TKey1, TValue1>();
            dic.Add(k0, v0);
            dic.Add(k1, v1);
            dic.Add(k2, v2);
            dic.Add(k3, v3);
            return dic.ToImmutable();
        }

        public static OrderedDictionary<TKey1, TValue1> Of<TKey1, TValue1>(TKey1 k0, TValue1 v0, TKey1 k1, TValue1 v1, TKey1 k2, TValue1 v2, TKey1 k3, TValue1 v3, TKey1 k4, TValue1 v4)
        {
            OrderedDictionary<TKey1, TValue1> dic = new OrderedDictionary<TKey1, TValue1>();
            dic.Add(k0, v0);
            dic.Add(k1, v1);
            dic.Add(k2, v2);
            dic.Add(k3, v3);
            dic.Add(k4, v4);
            return dic.ToImmutable();
        }

        public static OrderedDictionary<TKey1, TValue1> Of<TKey1, TValue1>(TKey1 k0, TValue1 v0, TKey1 k1, TValue1 v1, TKey1 k2, TValue1 v2, TKey1 k3, TValue1 v3, TKey1 k4, TValue1 v4, TKey1 k5, TValue1 v5)
        {
            OrderedDictionary<TKey1, TValue1> dic = new OrderedDictionary<TKey1, TValue1>();
            dic.Add(k0, v0);
            dic.Add(k1, v1);
            dic.Add(k2, v2);
            dic.Add(k3, v3);
            dic.Add(k4, v4);
            dic.Add(k5, v5);
            return dic.ToImmutable();
        }

        public static OrderedDictionary<TKey1, TValue1> Of<TKey1, TValue1>(TKey1 k0, TValue1 v0, TKey1 k1, TValue1 v1, TKey1 k2, TValue1 v2, TKey1 k3, TValue1 v3, TKey1 k4, TValue1 v4, TKey1 k5, TValue1 v5, TKey1 k6, TValue1 v6)
        {
            OrderedDictionary<TKey1, TValue1> dic = new OrderedDictionary<TKey1, TValue1>();
            dic.Add(k0, v0);
            dic.Add(k1, v1);
            dic.Add(k2, v2);
            dic.Add(k3, v3);
            dic.Add(k4, v4);
            dic.Add(k5, v5);
            dic.Add(k6, v6);
            return dic.ToImmutable();
        }

    }
}
