using System;
// ReSharper disable IdentifierTypo
// ReSharper disable BuiltInTypeReferenceStyle

namespace ISO22900.II
{
    //Event error codes
    public enum PduErrEvt : UInt32   //ENUM are also treated as a 32-bit value under 64-bit OS, this is more or less a reminder
    {
        PDU_ERR_EVT_NOERROR = 0x00000000,  /* No Error. Event type only returned on a PDUGetLastError if there were no previous errors for the requested handle */
        PDU_ERR_EVT_FRAME_STRUCT = 0x00000100,  /* CLL/CoP Error: The structure of the received protocol frame is incorrect (e.g. wrong frame number, missing FC...). */
        PDU_ERR_EVT_TX_ERROR = 0x00000101,  /* CLL/CoP Error: Error encountered during tramsmit of a ComPrimitiveLevel PDU */
        PDU_ERR_EVT_TESTER_PRESENT_ERROR = 0x00000102,  /* CLL/CoP Error: Error encountered in transmitting a Tester Present message or in receiving an expected response to a Tester Present message. */
        PDU_ERR_EVT_RSC_LOCKED = 0x00000109,  /* CLL Error: A physical ComParam was not set because of a physical  ComParam lock. */
        PDU_ERR_EVT_RX_TIMEOUT = 0x00000103,  /* CLL/CoP Error: Receive timer (e.g. P2Max) expired with no expected responses received from the vehicle. */
        PDU_ERR_EVT_RX_ERROR = 0x00000104,  /* CLL/CoP Error: Error encountered in receiving a mesage from the vehicle bus (e.g. checksum error ...). */
        PDU_ERR_EVT_PROT_ERR = 0x00000105,  /* CLL/CoP Error: Protocol error encountered during handling of a ComPrimitiveLevel (e.g. if the protocol cannot handle the length of a ComPrimitiveLevel. */
        PDU_ERR_EVT_LOST_COMM_TO_VCI = 0x00000106,  /* Module Error: Communication to a MVCI protocol module has been lost. */
        PDU_ERR_EVT_VCI_HARDWARE_FAULT = 0x00000107,  /* Module Error: The MVCI protocol module has detected a hardware error. */
        PDU_ERR_EVT_INIT_ERROR = 0x00000108   /* CLL/CoP Error: A failure occurred during a protocol initialization sequence. */
    }
}