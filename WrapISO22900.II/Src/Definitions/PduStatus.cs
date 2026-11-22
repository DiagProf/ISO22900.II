using System;
// ReSharper disable IdentifierTypo
// ReSharper disable BuiltInTypeReferenceStyle

namespace ISO22900.II
{
    // Status values
    public enum PduStatus : UInt32   //ENUM are also treated as a 32-bit value under 64-bit OS, this is more or less a reminder
    {
        /*  ComPrimitiveLevel status */
        PDU_COPST_IDLE = 0x8010, /* ComPrimitiveLevel is in the CommLogicalLink's ComPrimitiveLevel Queue and has not been acted upon. */
        PDU_COPST_EXECUTING = 0x8011, /* ComPrimitiveLevel has been pulled from the CommLogicalLink's ComPrimitiveLevel Queue  and is in an active state. */
        PDU_COPST_FINISHED = 0x8012, /* ComPrimitiveLevel is finished. No further event items will be generated for this ComPrimitiveLevel. */
        PDU_COPST_CANCELLED = 0x8013, /* ComPrimitiveLevel was cancelled by a PDUCancelComPrimitive request. No further event items will be generated for this ComPrimitiveLevel. */
        PDU_COPST_WAITING = 0x8014, /* A periodic send ComPrimitiveLevel (NumSendCycles > 1) has finished its periodic cycle and is waiting for its next cyclic time for transmission. */

        /*  ComLogicalLinkLayer status */
        PDU_CLLST_OFFLINE = 0x8050, /* ComLogicalLinkLayer is in communication state "offline".*/
        PDU_CLLST_ONLINE = 0x8051, /* ComLogicalLinkLayer is in communication state "online".*/
        PDU_CLLST_COMM_STARTED = 0x8052, /* ComLogicalLinkLayer is in communication state "communication started". A PDU_COPT_STARTCOMM ComPrimitiveLevel has been commanded. The ComLogicalLinkLayer is in a transmit/receive state. */

        /*  Module status */
        PDU_MODST_READY = 0x8060, /* The MVCI Protocol Module is ready for communication. The MVCI Protocol Module has been connected by this D-PDU API Session (See PDUModuleConnect) */
        PDU_MODST_NOT_READY = 0x8061, /* The MVCI Protocol Module is not ready for communication. Additional information about the cause may be provided via an additional vendor specific status code returned in pExtraInfo. */
        /* Refer to description of PDUGetStatus. EXAMPLE: After running a PDU_IOCTL_RESET command on the module, it may take some time for the module until it becomes ready. */
        /* Module is connected by this D-PDU API Session, but it is not ready for communication. */
        PDU_MODST_NOT_AVAIL = 0x8062, /* The MVCI Protocol Module is unavailable for connection. E.g. MVCI Protocol Module is in use by another D-PDU API connection or communication was lost after previously being in a PDU_MODST_READY state. */
        PDU_MODST_AVAIL = 0x8063 /* The MVCI Protocol Module is available for connection (i.e. not yet connected by a D-PDU API session.) See PDUModuleConnect and PDUModuleDisconnect) */
    }
}