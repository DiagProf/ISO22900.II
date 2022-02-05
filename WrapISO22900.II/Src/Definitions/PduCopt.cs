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

namespace ISO22900.II
{
    /// <summary>
    /// ComPrimitiveLevel type values
    /// </summary>
    public enum PduCopt : UInt32   //ENUM are also treated as a 32-bit value under 64-bit OS, this is more or less a reminder
    {
        /// <summary>
        /// Start communication with ECU by sending an optional request.
        /// The detailed behaviour is protocol dependent. For certain protocols
        /// (.e.g. ISO 14230), this ComPrimitiveLevel is required as the first
        /// ComPrimitiveLevel. This ComPrimitiveLevel is also required to put the
        /// ComLogicalLinkLayer into the state PDU_CLLST_COMM_STARTED
        /// which allows for tester present messages to be enabled (see
        /// CP_TesterPresentHandling). Once tester present handling is
        /// enabled the message is sent immediately, prior to the initial tester
        /// present cyclic time (CP_TesterPresentTime)
        /// </summary>
        PDU_COPT_STARTCOMM = 0x8001,

        /// <summary>
        /// Stop communication with ECU by sending an optional request.
        /// The detailed behaviour is protocol dependent. After successful
        /// completion of this ComPrimitiveLevel type, the ComLogicalLinkLayer is
        /// placed into PDU_CLLST_ONLINE state and no further tester
        /// presents will be sent. A PDU_COPT_STARTCOMM ComPrimitiveLevel
        /// might be required by some protocols (e.g. ISO 14230) to begin
        /// communications again.
        /// </summary>
        PDU_COPT_STOPCOMM = 0x8002,

        /// <summary>
        /// Copies ComParams related to a ComLogicalLinkLayer from the
        /// working buffer to the active buffer. Prior to update, the values need
        /// to be passed to the D-PDU API by calling PDUSetComParam,
        /// which modifies the ComParams in the working buffer. If the
        /// physical ComParams are locked by another ComLogicalLinkLayer, then
        /// a PDU_COPT_UPDATEPARAM will generate an error event
        /// (PDU_ERR_EVT_RSC_LOCKED) if physical ComParams are to
        /// be modified.
        /// NOTE 1 If the CLL is in the PDU_CLLST_COMM_STARTED
        /// state and tester present handling is enabled (see
        /// CP_TesterPresentHandling) any changes to one of the tester
        /// present ComParams will cause the tester present message to be
        /// sent immediately, prior to the initial tester present cyclic time.
        /// NOTE 2 Protocol handler always waits the proper P3Min time
        /// before allowing any transmit. See CP_P3Min, CP_P3Func,
        /// CP_P3Phys.
        /// </summary>
        PDU_COPT_UPDATEPARAM = 0x8003,
        
        /// <summary>
        /// Send request data and/or receive corresponding response data
        /// (single or multiple responses). See 10.1.4.17 for detailed settings
        /// of the PDU_COP_CTRL_DATA structure.
        /// </summary>
        PDU_COPT_SENDRECV = 0x8004,

        /// <summary>
        /// Wait the given time span before executing the next ComPrimitiveLevel
        /// </summary>
        PDU_COPT_DELAY = 0x8005,

        /// <summary>
        /// Copies ComParams related to a ComLogicalLinkLayer from active buffer to working buffer.
        /// (Converse functionality of PDU_COPT_UPDATEPARAM.)
        /// </summary>
        PDU_COPT_RESTORE_PARAM = 0x8006
    }
}