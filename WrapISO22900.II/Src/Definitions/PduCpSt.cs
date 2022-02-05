using System;
// ReSharper disable IdentifierTypo
// ReSharper disable BuiltInTypeReferenceStyle

namespace ISO22900.II
{
    // D-PDU API item type values
    public enum PduCpSt : UInt32   //ENUM are also treated as a 32-bit value under 64-bit OS, this is more or less a reminder
    {
        PDU_CPST_SESSION_TIMING = 0x00000001,  /* See ComParam struct type PDU_PARAM_STRUCT_SESS_TIMING */
        PDU_CPST_ACCESS_TIMING = 0x00000002    /* See ComParam struct type PDU_PARAM_STRUCT_ACCESS_TIMING */
    }
}