using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming
// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable IdentifierTypo

namespace ISO22900.II.SafeCStructs
{
    // Structure for flag data
    [StructLayout(LayoutKind.Sequential)]
    internal struct PDU_FLAG_DATA
    {
     public uint NumFlagBytes;                /* number of bytes in pFlagData array*/
     public byte[] pFlagData;                   /* Pointer to flag bytes used for TxFlag, RxFlag, and CllCreateFlag.  See section D.2 Flag definitions */
    }
    
    
}