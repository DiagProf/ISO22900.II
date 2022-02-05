using System;
using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming
// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable IdentifierTypo
namespace ISO22900.II
{
    // Basic data types used in ISO22900-2
    // C# using alias directive     //C typedef...
    using UNUM8 = System.Byte;      //typedef unsigned char UNUM8;      // Unsigned numeric 8 bits
    //using SNUM8 = System.SByte;     //typedef signed char SNUM8;        // Signed numeric 8 bits
    //using UNUM16 = System.UInt16;   //typedef unsigned short UNUM16;    // Unsigned numeric 16 bits
    //using SNUM16 = System.Int16;    //typedef signed short SNUM16;      // Signed numeric 16 bits
    using UNUM32 = System.UInt32;   //typedef unsigned long UNUM32;     // Unsigned numeric 32 bits
    //using SNUM32 = System.Int32;    //typedef signed long SNUM32;       // Signed numeric 32 bits
    //using CHAR8 = System.Byte;      //typedef char CHAR8;               // ASCII-coded 8-bit character value (ISO8859-1 (Latin 1))

    /// <summary>
    /// Structure for expected response
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct PDU_EXP_RESP_DATA
    {
        /// <summary>
        /// 0=positive response; 1 = negative response
        /// </summary>
        internal UNUM32 ResponseType;

        /// <summary>
        /// ID assigned by application to be returned in PDU_RESULT_DATA,
        /// which indicates which expected response matched
        /// </summary>
        internal UNUM32 AcceptanceId;

        /// <summary>
        /// number of bytes in the Mask Data and Pattern Data
        /// </summary>
        internal UNUM32 NumMaskPatternBytes;

        /// <summary>
        /// Pointer to Mask Data. Bits set to a '1' are care bits, '0' are don't care bits.
        /// </summary>
        internal UNUM8* pMaskData;

        /// <summary>
        /// Pointer to Pattern Data. Bytes to compare after the mask is applied
        /// </summary>
        internal UNUM8* pPatternData;

        /// <summary>
        /// Number of items in the following array of unique response identifiers.
        /// If the number is set to 0, then responses with any unique response identifier are considered,
        /// when trying to match them to this expected response.
        /// </summary>
        internal UNUM32 NumUniqueRespIds;

        /// <summary>
        /// Array containing unique response identifiers. Only responses with a unique response identifier found in this array are considered, when trying to match them to this expected response.
        /// </summary>
        internal UNUM32* pUniqueRespIds;
    }
}