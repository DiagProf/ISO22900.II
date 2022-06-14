
using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming
// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable IdentifierTypo
namespace ISO22900.II.SafeCStructs
{
    /// <summary>
    /// Structure for expected response
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct PDU_EXP_RESP_DATA
    {
        /// <summary>
        /// 0=positive response; 1 = negative response
        /// </summary>
        internal uint ResponseType;

        /// <summary>
        /// ID assigned by application to be returned in PDU_RESULT_DATA,
        /// which indicates which expected response matched
        /// </summary>
        internal uint AcceptanceId;

        /// <summary>
        /// number of bytes in the Mask Data and Pattern Data
        /// </summary>
        internal uint NumMaskPatternBytes;

        /// <summary>
        /// Pointer to Mask Data. Bits set to a '1' are care bits, '0' are don't care bits.
        /// </summary>
        internal byte[] pMaskData;

        /// <summary>
        /// Pointer to Pattern Data. Bytes to compare after the mask is applied
        /// </summary>
        internal byte[] pPatternData;

        /// <summary>
        /// Number of items in the following array of unique response identifiers.
        /// If the number is set to 0, then responses with any unique response identifier are considered,
        /// when trying to match them to this expected response.
        /// </summary>
        internal uint NumUniqueRespIds;

        /// <summary>
        /// Array containing unique response identifiers. Only responses with a unique response identifier found in this array are considered, when trying to match them to this expected response.
        /// </summary>
        internal uint[] pUniqueRespIds;
    }
}