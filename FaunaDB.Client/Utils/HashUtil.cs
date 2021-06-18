﻿using System;
using System.Collections.Generic;
using FaunaDB.Errors;

namespace FaunaDB.Utils
{
    // "Fnv" and "System.Data.HashFunction" packages work on byte[].
    // I couldn't find any packages that work generically.
    internal static class HashUtil
    {
        public static int Hash(params object[] values) =>
            Hash((IEnumerable<object>)values);

        public static int Hash(IEnumerable<object> values)
        {
            values.AssertNotNull(nameof(values));

            int hash = 17;
            foreach (object x in values)
            {
                if (x != null)
                {
                    hash = (hash * 23) + x.GetHashCode();
                }
            }

            return hash;
        }
    }
}