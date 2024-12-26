using System;
using System.Threading;
using NUnit.Framework;

namespace ISO22900.II.Test
{
    public partial class IntegrationTest_SetupAndTearDown
    {
        [Test]
        public void TestPduGetTimestamp()
        {
            var timestamp1 = _dPduApi.PduGetTimestamp(_moduleOne);
            Thread.Sleep(TimeSpan.FromMilliseconds(1)); //just a little wait
            var timestamp2 = _dPduApi.PduGetTimestamp(_moduleOne);

            //If an overflow occurs exactly at the moment of test. It will fail. But that is the most unlikely case :-)
            Assert.That(timestamp1, Is.LessThan(timestamp2));
        }
    }
}