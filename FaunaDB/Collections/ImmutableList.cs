using System.Collections.Generic;

namespace FaunaDB.Collections
{
    public static class ImmutableList
    {
        public static IReadOnlyList<T> Empty<T>() =>
            ArrayList<T>.Empty;

        public static IReadOnlyList<T> Of<T>(params T[] values) =>
            new ArrayList<T>(values);
    }
}
