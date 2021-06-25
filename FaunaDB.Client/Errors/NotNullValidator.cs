using System;

namespace FaunaDB.Errors
{
    internal static class NotNullValidator
    {
        public static void AssertNotNull(this object value, string valueName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(valueName);
            }
        }
    }
}
