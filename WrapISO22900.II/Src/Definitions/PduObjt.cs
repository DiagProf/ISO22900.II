using System;

namespace ISO22900.II
{
    public enum PduObjt : UInt32   //ENUM are also treated as a 32-bit value under 64-bit OS, this is more or less a reminder
    {
        PDU_OBJT_PROTOCOL = 0x8021,  // Object type for object PROTOCOL of MDF.
        PDU_OBJT_BUSTYPE = 0x8022,  // Object type for object BUSTYPE of MDF.
        PDU_OBJT_IO_CTRL = 0x8023,  // Object type for object IO_CTRL of MDF.
        PDU_OBJT_COMPARAM = 0x8024,  // Object type for object COMPARAM of MDF.
        PDU_OBJT_PINTYPE = 0x8025,  // Object type for object PINTYPE of MDF.
        PDU_OBJT_RESOURCE = 0x8026   // Object type for object RESOURCE of MDF.
    }
}