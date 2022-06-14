
using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming
// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable IdentifierTypo

namespace ISO22900.II.SafeCStructs
{
    /// <summary>
    /// Structure to control a ComPrimitiveLevel's operation (used by PDUStartComPrimitive)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct PDU_COP_CTRL_DATA
    {
        /// <summary>
        /// Cycle time in ms for cyclic send operation or delay time for PDU_COPT_DELAY ComPrimitiveLevel
        /// </summary>
        internal uint Time;

        /// <summary>
        /// number of send cycles to be performed;
        /// -1 for infinite cyclic send operation
        /// </summary>
        internal int NumSendCycles;

        /// <summary>
        /// number of receive cycles to be performed;
        /// -1 (IS-CYCLIC) for infinite receive operation,
        /// -2 (IS-MULTIPLE) for multiple expected responses from 1 or more ECUs
        /// </summary>
        internal int NumReceiveCycles;

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
        internal uint TempParamUpdate;


        /// <summary>
        /// Transmit Flag used to indicate protocol specific elements for the ComPrimitiveLevel's
        /// execution. (See section D.2.1 TxFlag definition.)
        /// </summary>
        internal PDU_FLAG_DATA TxFlag;

        /// <summary>
        /// number of entries in pExpectedResponseArray
        /// </summary>
        internal uint NumPossibleExpectedResponses;

        /// <summary>
        /// pointer to an array of expected responses (see 11.1.3.18 Structure for expected response)
        /// </summary>
        internal PDU_EXP_RESP_DATA[] pExpectedResponseArray;
    }
}