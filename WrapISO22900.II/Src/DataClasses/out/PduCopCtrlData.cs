#region License

// /*
// MIT License
// 
// Copyright (c) 2022 Joerg Frank
// 
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// */

#endregion

using System;
using System.Collections.Generic;

namespace ISO22900.II
{
    /// <summary>
    /// Class to control a ComPrimitiveLevel's operation (used by PDUStartComPrimitive)
    /// </summary>
    public class PduCopCtrlData
    {
        /// <summary>
        /// Cycle time in ms for cyclic send operation or delay time for PDU_COPT_DELAY ComPrimitiveLevel
        /// </summary>
        public uint Time;

        /// <summary>
        /// number of send cycles to be performed;
        /// -1 for infinite cyclic send operation
        /// </summary>
        public int NumSendCycles;

        /// <summary>
        /// number of receive cycles to be performed;
        /// -1 (IS-CYCLIC) for infinite receive operation,
        /// -2 (IS-MULTIPLE) for multiple expected responses from 1 or more ECUs
        /// </summary>
        public int NumReceiveCycles;

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
        public uint TempParamUpdate;


        /// <summary>
        /// Transmit Flag used to indicate protocol specific elements for the ComPrimitiveLevel's
        /// execution. (See section D.2.1 TxFlag definition.)
        /// </summary>
        public PduFlagDataTxFlag TxFlag;

        /// <summary>
        /// number of entries in pExpectedResponseArray
        /// </summary>
        // internal uint NumPossibleExpectedResponses;
        public PduExpectedResponseData[] PduExpectedResponseDatas;
        

        internal void Accept(IVisitorPduComPrimitiveControlData visitorPduComPrimitiveControlData)
        {
            visitorPduComPrimitiveControlData.VisitConcretePduCopCtrlData(this);
        }

        /// <summary>
        /// Use me if you create ComPrimitiveLevel of type delay an set delayTime in ms 
        /// </summary>
        /// <param name="delayTimeMs"></param>
        public PduCopCtrlData(uint delayTimeMs)
        {
            Time = delayTimeMs;
            NumSendCycles = 0;
            NumReceiveCycles = 0;
            TempParamUpdate = 0;
            TxFlag = new PduFlagDataTxFlag();
            PduExpectedResponseDatas = Array.Empty<PduExpectedResponseData>();
        }

        /// <summary>
        /// I would say that is the 90% case how the PduCopCtrlData has to be set
        /// </summary>
        /// <param name="expectedResponseDatas"></param>
        public PduCopCtrlData(PduExpectedResponseData[] expectedResponseDatas)
        {
            NumSendCycles = 1;
            NumReceiveCycles = 1;
            TempParamUpdate = 0;
            Time = 0;
            TxFlag = new PduFlagDataTxFlag();
            PduExpectedResponseDatas = expectedResponseDatas;
        }

        /// <summary>
        /// Raw element everything still has to be set,
        /// this is useful, for example, if all information comes from ODX
        /// </summary>
        public PduCopCtrlData()
        {
            
        }
    }
}