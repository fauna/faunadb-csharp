using FaunaDB.Client;
using NUnit.Framework;

namespace Test
{
    [TestFixture]
    public class EnvironmentHeaderTest
    {
#if NETCOREAPP3_1
        [Test]
        public void TestNetCore31Runtime()
        {
            string actual = RuntimeEnvironmentHeader.Construct();
            Assert.That(actual, Does.Contain(".net core 3.1"));
        }
#endif

#if NETCOREAPP2_1
        [Test]
        public void TestNetCore21Runtime()
        {
            string actual = RuntimeEnvironmentHeader.Construct();
            Assert.That(actual, Does.Contain(".net core 2.1"));
        }
#endif
    }
}