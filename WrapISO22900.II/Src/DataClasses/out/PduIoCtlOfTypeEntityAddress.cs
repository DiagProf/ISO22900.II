﻿#region License

// // MIT License
// //
// // Copyright (c) 2022 Joerg Frank
// // http://www.diagprof.com/
// //
// // Permission is hereby granted, free of charge, to any person obtaining a copy
// // of this software and associated documentation files (the "Software"), to deal
// // in the Software without restriction, including without limitation the rights
// // to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// // copies of the Software, and to permit persons to whom the Software is
// // furnished to do so, subject to the following conditions:
// //
// // The above copyright notice and this permission notice shall be included in all
// // copies or substantial portions of the Software.
// //
// // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// // IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// // FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// // AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// // LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// // OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// // SOFTWARE.

#endregion

namespace ISO22900.II
{
    public class PduIoCtlOfTypeEntityAddress : PduIoCtl
    {
        /// <summary>
        /// Logical address of DoIP entity to access
        /// </summary>
        public uint LogicalAddress { get; set; }

        /// <summary>
        /// Timeout in milliseconds to wait for the response from the DoIP entity (corresponds  to A_DoIP_Ctrl in ISO 13400)
        /// </summary>
        public uint DoIpCtrlTimeout { get; set; }

        public PduIoCtlOfTypeEntityAddress(uint logicalAddress, uint doIpCtrlTimeout) : base(PduIt.PDU_IT_IO_ENTITY_ADDRESS)
        {
            LogicalAddress = logicalAddress;
            DoIpCtrlTimeout = doIpCtrlTimeout;
        }

        internal override void Accept(IVisitorPduIoCtl visitorPduIoCtl)
        {
            visitorPduIoCtl.VisitConcretePduIoCtlOfTypeEntityAddress(this);
        }
    }
}
