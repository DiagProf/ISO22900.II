using System;
// ReSharper disable IdentifierTypo
// ReSharper disable BuiltInTypeReferenceStyle

namespace ISO22900.II
{
    // D-PDU API events
    public enum PduEvtData : UInt32   //ENUM are also treated as a 32-bit value under 64-bit OS, this is more or less a reminder
    {
        PDU_EVT_DATA_AVAILABLE = 0x0801,  /* This event indicates that there is event data available to be read by the Application. */ 
                                          /* The data could be an error, status, or result item. */
                                          /* The application must call PDUGetEventItem to retrieve the item. */
                                          
        PDU_EVT_DATA_LOST = 0x0802        /* This event indicates that the Com Logical Link has lost data due to a buffer (queue) overrun. */
                                          /* No event data is stored in the event queue. */
                                          /* This is for information only. */
    }
}