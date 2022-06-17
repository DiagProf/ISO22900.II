
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming
// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable IdentifierTypo

namespace ISO22900.II.UnSafeCStructs
{
    // Basic data types used in ISO22900-2
    // C# using alias directive     //C typedef...
    //using UNUM8 = System.Byte;    //typedef unsigned char UNUM8;      // Unsigned numeric 8 bits
    //using SNUM8 = System.SByte;   //typedef signed char SNUM8;        // Signed numeric 8 bits
    //using UNUM16 = System.UInt16; //typedef unsigned short UNUM16;    // Unsigned numeric 16 bits
    //using SNUM16 = System.Int16;  //typedef signed short SNUM16;      // Signed numeric 16 bits
    using UNUM32 = System.UInt32;   //typedef unsigned long UNUM32;     // Unsigned numeric 32 bits
    using SNUM32 = System.Int32;    //typedef signed long SNUM32;       // Signed numeric 32 bits
    //using CHAR8 = System.Byte;    //typedef char CHAR8;               // ASCII-coded 8-bit character value (ISO8859-1 (Latin 1))


    /// <summary>
    /// Structure to control a ComPrimitiveLevel's operation (used by PDUStartComPrimitive)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct PDU_COP_CTRL_DATA
    {
        /// <summary>
        /// Cycle time in ms for cyclic send operation or delay time for PDU_COPT_DELAY ComPrimitiveLevel
        /// </summary>
        internal UNUM32 Time;

        /// <summary>
        /// number of send cycles to be performed;
        /// -1 for infinite cyclic send operation
        /// </summary>
        internal SNUM32 NumSendCycles;

        /// <summary>
        /// number of receive cycles to be performed;
        /// -1 (IS-CYCLIC) for infinite receive operation,
        /// -2 (IS-MULTIPLE) for multiple expected responses from 1 or more ECUs
        /// </summary>
        internal SNUM32 NumReceiveCycles;

        /// <summary>
        /// /* Temporary ComParam and temporary UniqueRespIdTable settings for the ComPrimitiveLevel:        */
        /// /* 0 = Do not use temporary ComParams for this ComPrimitiveLevel.                                */
        /// /*     The ComPrimitiveLevel shall attach the "Active" ComParam buffer to the ComPrimitiveLevel.      */
        /// /*     This buffer shall be in effect for the ComPrimitiveLevel until it is finished.            */
        /// /*     The ComParams for the ComPrimitiveLevel will not change even if the "Active" buffer is    */
        /// /*     modified by a subsequent ComPrimitiveLevel type of PDU_COPT_UPDATEPARAM.                  */
        /// /* 1 = Use temporary ComParams for this ComPrimitiveLevel; The ComPrimitiveLevel shall attach the     */
        /// /*     ComParam "Working" buffer "Working" table to the ComPrimitiveLevel.                       */
        /// /*     This buffer shall be in effect for the ComPrimitiveLevel until it is finished.            */
        /// /*     The ComParams for the ComPrimitiveLevel will not change even if the "Active" or           */
        /// /*     "Working" buffers are modified by any subsequent calls to PDUSetComParam.            */
        /// /* NOTE: If TempParamUpdate is set to 1, the ComParam Working Buffer is restored to         */
        /// /* the Active Buffer when this PDUStartComPrimitive function call returns.                  */
        /// </summary>
        internal UNUM32 TempParamUpdate;


        /// <summary>
        /// Transmit Flag used to indicate protocol specific elements for the ComPrimitiveLevel's
        /// execution. (See section D.2.1 TxFlag definition.)
        /// </summary>
        internal PDU_FLAG_DATA TxFlag;

        /// <summary>
        /// number of entries in pExpectedResponseArray
        /// </summary>
        internal UNUM32 NumPossibleExpectedResponses;

        /// <summary>
        /// pointer to an array of expected responses (see 11.1.3.18 Structure for expected response)
        /// </summary>
        internal PDU_EXP_RESP_DATA* pExpectedResponseArray;
    }
}