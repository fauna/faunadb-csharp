using System;
using System.Threading;

/// <summary>
///   A thread-safe static helper used to store the last seen
///   transaction time returned by the database.
/// </summary>
namespace FaunaDB.Client {
    public class LastSeen {
        private long lastSeenTxn = 0;

        /// <summary>
        ///   Returns the last seen transaction time, if any.
        /// </summary>
        public long? Txn
        {
            get {
                if (lastSeenTxn > 0) {
                    return lastSeenTxn;
                } else {
                    return null;
                }
            }
        }

        /// <summary>
        ///   Set the last seen transaction time to the given time, if
        ///   time is greater than the previously-seen time.
        /// </summary>
        public void SetTxn(long time) {
            long initial;

            do
            {
                initial = lastSeenTxn;

                // don't move the last seen time backward
                if (time <= initial) {
                    return;
                }
            } while (initial != Interlocked.CompareExchange(ref lastSeenTxn, time, initial));
        }
    }
}
