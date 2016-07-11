using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace FaunaDB.Collections
{
    class OrderedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    {
        public static readonly OrderedDictionary<TKey, TValue> Empty = new OrderedDictionary<TKey, TValue>();

        readonly OrderedDictionary dictionary;

        internal OrderedDictionary(OrderedDictionary dictionary)
        {
            this.dictionary = dictionary;
        }

        public OrderedDictionary() : this(new OrderedDictionary()) { }

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

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys
        {
            get
            {
                return Keys;
            }
        }

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
        {
            get
            {
                return Values;
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

        private static bool DictEquals(OrderedDictionary<TKey, TValue> a, OrderedDictionary<TKey, TValue> b)
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
