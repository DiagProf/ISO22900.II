using System.Threading;
using ISO22900.II;
using NUnit.Framework;

namespace ISO22900.II.Test
{
    public partial class IntegrationTest_SetupAndTearDown
    {
        [Test]
        public void TestPduGetStatusModule()
        {
            var statusData = _dPduApi.PduGetStatus(_moduleOne, PduConst.PDU_HANDLE_UNDEF, PduConst.PDU_HANDLE_UNDEF);
            Assert.AreEqual(PduStatus.PDU_MODST_READY, statusData.Status );
        }

        [Test]
        public void TestPduGetStatusComLogicalLink()
        {
            var statusData = _dPduApi.PduGetStatus(_moduleOne, _cll, PduConst.PDU_HANDLE_UNDEF);
            Assert.AreEqual(PduStatus.PDU_CLLST_OFFLINE, statusData.Status);
        }


        [Test]
        public void TestPduGetStatusComPrimitive()
        {
            _dPduApi.PduConnect(_moduleOne, _cll);
            uint delay = 200;
            var copControlDelay = new PduCopCtrlData(delay);
            var hCoP = _dPduApi.PduStartComPrimitive(_moduleOne, _cll, PduCopt.PDU_COPT_DELAY, new byte[] { },
                copControlDelay,
                0);
            
            var statusData = _dPduApi.PduGetStatus(_moduleOne, _cll, hCoP);
            Assert.AreEqual(PduStatus.PDU_COPST_IDLE, statusData.Status);
            _dPduApi.PduDisconnect(_moduleOne, _cll);
        }
    }
}