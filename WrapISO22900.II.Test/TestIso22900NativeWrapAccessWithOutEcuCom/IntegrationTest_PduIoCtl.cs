using System.Threading;
using ISO22900.II;
using NUnit.Framework;

namespace ISO22900.II.Test
{
    public partial class IntegrationTest_SetupAndTearDown
    {
        [Test]
        public void TestPduIoCtlReadPduIoCtlReadVbatt()
        {
            //Attention some VCIs do not support this!!
            //Then something goes wrong here, of course
            var ioCtlCommandId = _dPduApi.PduGetObjectId(PduObjt.PDU_OBJT_IO_CTRL, "PDU_IOCTL_READ_VBATT");
            var pduIoCtlData = _dPduApi.PduIoCtl(_moduleOne, PduConst.PDU_HANDLE_UNDEF, ioCtlCommandId, null);
            Assert.AreEqual(PduIt.PDU_IT_IO_UNUM32, pduIoCtlData.PduItemType);
            if (pduIoCtlData.PduItemType == PduIt.PDU_IT_IO_UNUM32)
            {
                if (pduIoCtlData is PduIoCtlDataUnum32 ctlDataUnum32) 
                    Assert.GreaterOrEqual(ctlDataUnum32.Value, 0);
            }
        }

        [Test]
        public void TestPduIoCtlReadPduIoReadIgnitionSenseState()
        {
            //Attention some VCIs do not support this!!
            //Then something goes wrong here, of course
            var ioCtlCommandId = _dPduApi.PduGetObjectId(PduObjt.PDU_OBJT_IO_CTRL, "PDU_IOCTL_READ_IGNITION_SENSE_STATE");

            var input = new PduIoCtlDataUnum32(1);

            var output = _dPduApi.PduIoCtl(_moduleOne, PduConst.PDU_HANDLE_UNDEF, ioCtlCommandId, input);
            Assert.AreEqual(PduIt.PDU_IT_IO_UNUM32, output.PduItemType);
            if (output.PduItemType == PduIt.PDU_IT_IO_UNUM32)
            {
                if (output is PduIoCtlDataUnum32 ctlDataUnum32)
                    Assert.GreaterOrEqual(ctlDataUnum32.Value, 0);
            }
        }

    }
}