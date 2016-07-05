using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FaunaDB.Collections
{
    public class ArrayList<T> : IList<T>, IReadOnlyList<T>
    {
        public static readonly ArrayList<T> Empty = new ArrayList<T>();

        List<T> list;

        internal ArrayList(List<T> list)
        {
            this.list = list;
        }

        public ArrayList() : this(new List<T>()) { }

        public ArrayList(IEnumerable<T> values) : this(new List<T>(values)) { }

        public T this[int index]
        {
            get
            {
                return list[index];
            }
            set
            {
                list[index] = value;
            }
        }

        public int Count
        {
            get
            {
                return list.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                IList<T> l = list;
                return l.IsReadOnly;
            }
        }

        public void Add(T item) =>
            list.Add(item);

        public void Clear() =>
            list.Clear();

        public bool Contains(T item) =>
            list.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) =>
            list.CopyTo(array, arrayIndex);

        public IEnumerator<T> GetEnumerator() =>
            list.GetEnumerator();

        public int IndexOf(T item) =>
            list.IndexOf(item);

        public void Insert(int index, T item) =>
            list.Insert(index, item);

        public bool Remove(T item) =>
            list.Remove(item);

        public void RemoveAt(int index) =>
            list.RemoveAt(index);

        IEnumerator IEnumerable.GetEnumerator() =>
            list.GetEnumerator();

        public override int GetHashCode() =>
            list.GetHashCode();

        public override bool Equals(object obj)
        {
            var other = obj as ArrayList<T>;
            return other != null && list.SequenceEqual(other.list);
        }

        public override string ToString() =>
            $"[{string.Join(", ", list)}]";
    }

    public sealed class ImmutableList
    {
        public static IReadOnlyList<T> Empty<T>() =>
            ArrayList<T>.Empty;

        public static IReadOnlyList<T> Of<T>(params T[] values)
        {
            List<T> list = new List<T>(values);
            return new ArrayList<T>(list.AsReadOnly());
        }
    }
}
