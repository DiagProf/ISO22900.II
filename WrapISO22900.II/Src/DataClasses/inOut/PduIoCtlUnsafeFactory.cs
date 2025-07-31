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

using ISO22900.II.UnSafeCStructs;
using System;
using System.Runtime.InteropServices;

namespace ISO22900.II
{
    internal class PduIoCtlUnsafeFactory : PduIoCtlFactory
    {
        private unsafe PDU_DATA_ITEM* _pointerPduDataItem;

        internal unsafe PduIoCtl GetPduIoCtlFromIoCtlItemPointer(PDU_DATA_ITEM* pduDataItem)
        {
            _pointerPduDataItem = pduDataItem;
            return PduIoCtlFromItemType(pduDataItem->ItemType);
        }

        protected unsafe void* PointerToStartOfData()
        {
            return _pointerPduDataItem->pData;
        }

        protected override unsafe PduIoCtlOfTypeUint CreatePduIoCtlUint()
        {
            var uInt32 = *(uint*)PointerToStartOfData();
            return new PduIoCtlOfTypeUint(uInt32);
        }

        protected override unsafe PduIoCtlOfTypeEntityStatus CreatePduIoCtlEntityStatus()
        {
            var eStatusData = *(PDU_IO_ENTITY_STATUS_DATA*)PointerToStartOfData();
            return new PduIoCtlOfTypeEntityStatus(new PduIoCtlEntityStatusData(entityType: eStatusData.EntityType, tcpClients: eStatusData.TcpClients,
                tcpClientsMax: eStatusData.TcpClientsMax, maxDataSize: eStatusData.MaxDataSize));
        }

        protected override unsafe PduIoCtlOfTypeByteField CreatePduIoCtlByteArray()
        {
            var byteArrayData = *(PDU_IO_BYTEARRAY_DATA*)PointerToStartOfData();
            int len = (int)byteArrayData.DataSize;
            
            var data = new byte[len];
            Marshal.Copy((IntPtr)(0x10 + ptr), data, 0, len);
            return new PduIoCtlOfTypeByteField(data);
        }
    }
}
