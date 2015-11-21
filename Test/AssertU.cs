using System;
using System.Threading.Tasks;

namespace Test
{
    public static class AssertU
    {
        // Assert.Throws isn't available until NUnit 3, and that won't work in Xamarin Studio yet.
        public static TException Throws<TException>(Action action) where TException : Exception
        {
            try
            {
                action();
            }
            catch (TException e)
            {
                return e;
            }
            throw new Exception("Code did not throw.");
        }

        public static async Task<TException> Throws<TException>(Func<Task> action) where TException : Exception
        {
            try
            {
                await action();
            }
            catch (TException e)
            {
                return e;
            }
            throw new Exception("Code did not throw.");
        }
    }
}

