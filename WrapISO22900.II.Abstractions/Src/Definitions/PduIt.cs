using System;
// ReSharper disable IdentifierTypo
// ReSharper disable BuiltInTypeReferenceStyle

namespace ISO22900.II
{
    // D-PDU API item type values
    public enum PduIt : UInt32   //ENUM are also treated as a 32-bit value under 64-bit OS, this is more or less a reminder
    {
        PDU_IT_IO_UNUM32 = 0x1000,  // IOCTL UNUM32 item.
        PDU_IT_IO_PROG_VOLTAGE = 0x1001,  // IOCTL Program Voltage item.
        PDU_IT_IO_BYTEARRAY = 0x1002,  // IOCTL Byte Array item.
        PDU_IT_IO_FILTER = 0x1003,  // IOCTL Filter item.
        PDU_IT_IO_EVENT_QUEUE_PROPERTY = 0x1004,  // IOCTL Event Queue Property item.
        PDU_IT_IO_TLS_CERTIFICATE = 0x1005, // IOCTL Set certificate structure.
        PDU_IT_RSC_STATUS = 0x1100,  // Resource Status item
        PDU_IT_PARAM = 0x1200,  // ComParam item
        PDU_IT_RESULT = 0x1300,  // Result item
        PDU_IT_STATUS = 0x1301,  // Status notification item
        PDU_IT_ERROR = 0x1302,  // Error notification item
        PDU_IT_INFO = 0x1303,  // Information notification item
        PDU_IT_RSC_ID = 0x1400,  // Resource ID item
        PDU_IT_RSC_CONFLICT = 0x1500,  // Resource Conflict Item
        PDU_IT_MODULE_ID = 0x1600,  // Module ID item
        PDU_IT_UNIQUE_RESP_ID_TABLE = 0x1700,  // Unique Response Id Table Item
        PDU_IT_IO_VEHICLE_ID_REQUEST = 0x1800,  // IOCTL DoIP Vehicle Identification Request.
        PDU_IT_IO_ETH_SWITCH_STATE = 0x1801,  // IOCTL Switch Ethernet activation pin.
        PDU_IT_IO_ENTITY_ADDRESS = 0x1802,  // DoIP entity addressing item
        PDU_IT_IO_ENTITY_STATUS = 0x1803   // DoIP entity status item
    }
}