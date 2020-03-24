using System;
namespace FaunaDB.Client.Utils
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Will return a Fauna <see cref="Types.TimeV"/> object.
        /// </summary>
        /// <returns> <see cref="Types.TimeV"/></returns>
        public static Types.TimeV ToFaunaTime(this DateTime dt)
        {
            return new Types.TimeV(dt);
        }

        /// <summary>
        /// Will return a Fauna <see cref="Types.DateV"/> object.
        /// </summary>
        /// <returns> <see cref="Types.DateV"/></returns>
        public static Types.DateV ToFaunaDate(this DateTime dt)
        {
            return new Types.DateV(dt.Date);
        }

        /// <summary>
        /// Will return a Fauna <see cref="Types.TimeV"/> object.
        /// </summary>
        /// <returns> <see cref="Types.TimeV"/></returns>
        public static Types.TimeV ToFaunaTime(this DateTimeOffset dt)
        {
            return new Types.TimeV(dt.UtcDateTime);
        }

        /// <summary>
        /// Will return a Fauna <see cref="Types.DateV"/> object.
        /// </summary>
        /// <returns> <see cref="Types.DateV"/></returns>
        public static Types.DateV ToFaunaDate(this DateTimeOffset dt)
        {
            return new Types.DateV(dt.UtcDateTime.Date);
        }
    }
}
