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
    
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct PDU_RSC_DATA
    {
        public UNUM32 BusTypeId;                 /* Bus Type Id (IN parameter) */
        public UNUM32 ProtocolId;                /* Protocol Id (IN parameter) */
        public UNUM32 NumPinData;                /* Number of items in the following array */
        public PDU_PIN_DATA* pDLCPinData;         /* Pointer to array of PDU_PIN_DATA structures*/
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct PDU_PIN_DATA
    {
        public UNUM32 DLCPinNumber;              /* Pin number on DLC */
        public UNUM32 DLCPinTypeId;              /* Pin ID */
    }
}