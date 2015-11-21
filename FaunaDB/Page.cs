using System;

using FaunaDB.Values;

namespace FaunaDB
{
    /// <summary>
    /// A single pagination result.
    /// See <c>paginate</c> in the <see href="https://faunadb.com/documentation/queries#read_functions">docs</see>. 
    /// </summary>
    public struct Page : IEquatable<Page>
    {
        /// <summary>
        /// Pagination results. What these are depends on the query.
        /// </summary>
        public ArrayV Data { get; }
        /// <summary>
        /// Nullable cursor to the previous page.
        /// </summary>
        public Cursor? Before { get; }
        /// <summary>
        /// Nullable cursor to the next page.
        /// </summary>
        public Cursor? After { get; }

        public Page(ArrayV data, Cursor? before = null, Cursor? after = null)
        {
            Data = data;
            Before = before;
            After = after;
        }

        /// <summary>
        /// Use this on a value that you know represents a Page.
        /// </summary>
        public static explicit operator Page(Value v)
        {
            var obj = (ObjectV) v;
            return new Page((ArrayV) obj["data"], GetCursor(obj, "before"), GetCursor(obj, "after"));
        }

        static Cursor? GetCursor(ObjectV obj, string direction)
        {
            var value = obj.GetOrNull(direction);
            return value == null ? null : (Cursor?) new Cursor((ArrayV) value);
        }

        public static implicit operator Value(Page p)
        {
            return ObjectV.WithoutNullValues(ObjectV.Pairs("data", p.Data), ObjectV.Pairs("before", p.Before?.Value, "after", p.After?.Value));
        }

        #region boilerplate
        public override bool Equals(object obj)
        {
            return obj is Page && Equals((Page) obj);
        }

        public bool Equals(Page p)
        {
            return p.Data == Data && p.Before == Before && p.After == After;
        }

        public static bool operator==(Page a, Page b)
        {
            return object.Equals(a, b);
        }

        public static bool operator!=(Page a, Page b)
        {
            return !object.Equals(a, b);
        }

        public override int GetHashCode()
        {
            return HashUtil.Hash(Data, Before, After);
        }

        public override string ToString()
        {
            return string.Format("Page(Data: {0}, Before: {1}, After: {2})", Data, Before, After);
        }
        #endregion
    }

    /// <summary>
    /// An opaque value enabling you to get a previous/next page.
    /// </summary>
    public struct Cursor : IEquatable<Cursor>
    {
        /// <summary>
        /// Cursor value. You usually won't need to access this.
        /// </summary>
        public ArrayV Value { get; }

        public Cursor(ArrayV value)
        {
            if (value == null)
                throw new NullReferenceException();
            Value = value;
        }

        public static implicit operator Value(Cursor c)
        {
            return c.Value;
        }

        #region boilerplate
        public override bool Equals(object obj)
        {
            return obj is Cursor && Equals((Cursor) obj);
        }

        public bool Equals(Cursor c)
        {
            return Value == c.Value;
        }

        public static bool operator ==(Cursor a, Cursor b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Cursor a, Cursor b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("Cursor({0})", Value);
        }
        #endregion
    }
}
