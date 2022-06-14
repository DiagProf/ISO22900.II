using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming
// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable IdentifierTypo

namespace ISO22900.II.UnSafeCStructs
{
    // Basic data types used in ISO22900-2
    // C# using alias directive     //C typedef...
    using UNUM8 = System.Byte;      //typedef unsigned char UNUM8;      // Unsigned numeric 8 bits
    using SNUM8 = System.SByte;     //typedef signed char SNUM8;        // Signed numeric 8 bits
    using UNUM16 = System.UInt16;   //typedef unsigned short UNUM16;    // Unsigned numeric 16 bits
    using SNUM16 = System.Int16;    //typedef signed short SNUM16;      // Signed numeric 16 bits
    using UNUM32 = System.UInt32;   //typedef unsigned long UNUM32;     // Unsigned numeric 32 bits
    using SNUM32 = System.Int32;    //typedef signed long SNUM32;       // Signed numeric 32 bits
    using CHAR8 = System.Byte;      //typedef char CHAR8;               // ASCII-coded 8-bit character value (ISO8859-1 (Latin 1))

    // Structure for flag data
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct PDU_FLAG_DATA
    {
     public UNUM32 NumFlagBytes;                /* number of bytes in pFlagData array*/
     public UNUM8* pFlagData;                   /* Pointer to flag bytes used for TxFlag, RxFlag, and CllCreateFlag.  See section D.2 Flag definitions */
    }
    
    
}