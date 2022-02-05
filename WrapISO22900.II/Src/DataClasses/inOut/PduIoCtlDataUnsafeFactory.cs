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

namespace ISO22900.II
{
    internal class PduIoCtlDataUnsafeFactory : PduIoCtlDataFactory
    {
        private unsafe PDU_DATA_ITEM* _pointerPduDataItem;

        internal unsafe PduIoCtlData GetPduIoCtlDataFromIoCtlDataItemPointer(PDU_DATA_ITEM* pduDataItem)
        {
            _pointerPduDataItem = pduDataItem;
            return PduIoCtlDataFromItemType(pduDataItem->ItemType);
        }

        protected unsafe void* PointerToStartOfData()
        {
            return _pointerPduDataItem->pData;
        }

        protected override unsafe PduIoCtlDataUnum32 CreatePduIoCtlDataUnum32()
        {
            var uInt32 = *(uint*)PointerToStartOfData();
            return new PduIoCtlDataUnum32(uInt32);
        }

        protected override PduIoCtlData CreatePduIoCtlDataProgVoltage()
        {
            throw new System.NotImplementedException();
        }

        protected override PduIoCtlData CreatePduIoCtlDataByteArray()
        {
            throw new System.NotImplementedException();
        }

        protected override PduIoCtlData CreatePduIoCtlDataFilter()
        {
            throw new System.NotImplementedException();
        }
    }
}