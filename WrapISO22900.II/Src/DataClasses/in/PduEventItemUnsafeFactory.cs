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

namespace ISO22900.II
{
    internal class PduEventItemUnsafeFactory : PduEventItemFactory
    {
        private unsafe PDU_EVENT_ITEM* _pointerPduEventItem;

        internal unsafe PduEventItem GetPduEventItemFromPduEventItemPointer(PDU_EVENT_ITEM* pPduEventItem)
        {
            _pointerPduEventItem = pPduEventItem;
            return PduEventItemFromItemType(pPduEventItem->ItemType);
        }

        protected unsafe void* PullEventItemGeneralDataFromUnmanagedMemory()
        {
            CopHandle = _pointerPduEventItem->hCop;
            Timestamp = _pointerPduEventItem->Timestamp;
            CopTag = (uint) _pointerPduEventItem->pCoPTag;
            return _pointerPduEventItem->pData;
        }

        protected override unsafe PduEventItem CreatePduEventItemStatus()
        {
            var pduStatus = *(PduStatus*)PullEventItemGeneralDataFromUnmanagedMemory();
            return new PduEventItemStatus(CopHandle,CopTag,Timestamp, pduStatus);
        }

        protected override unsafe PduEventItem CreatePduEventItemResult()
        {
            var pPduResultData = (PDU_RESULT_DATA*)PullEventItemGeneralDataFromUnmanagedMemory();

            var rxFlagData =
                ExtractByteArrayBytes(pPduResultData->RxFlag.pFlagData, pPduResultData->RxFlag.NumFlagBytes);

            var timeFlagData = 
                ExtractByteArrayBytes(pPduResultData->TimestampFlags.pFlagData, pPduResultData->TimestampFlags.NumFlagBytes);

            var dataBytes =
                ExtractByteArrayBytes(pPduResultData->pDataBytes, pPduResultData->NumDataBytes);

            PduResultData resultData;
            if (pPduResultData->pExtraInfo != null)
            {
                var extraInfoHeaderBytes = ExtractByteArrayBytes(pPduResultData->pExtraInfo->pHeaderBytes,
                    pPduResultData->pExtraInfo->NumHeaderBytes);

                var extraInfoFooterBytes = ExtractByteArrayBytes(pPduResultData->pExtraInfo->pFooterBytes,
                    pPduResultData->pExtraInfo->NumFooterBytes);

                resultData = new PduResultData(new PduFlagDataRxFlag(rxFlagData),
                    pPduResultData->UniqueRespIdentifier, pPduResultData->AcceptanceId,
                    new PduFlagDataTimestampFlag(timeFlagData), pPduResultData->TxMsgDoneTimestamp,
                    pPduResultData->StartMsgTimestamp,
                    dataBytes, extraInfoHeaderBytes, extraInfoFooterBytes);
            }
            else
            {
                resultData = new PduResultData(new PduFlagDataRxFlag(rxFlagData),
                    pPduResultData->UniqueRespIdentifier, pPduResultData->AcceptanceId,
                    new PduFlagDataTimestampFlag(timeFlagData), pPduResultData->TxMsgDoneTimestamp,
                    pPduResultData->StartMsgTimestamp,
                    dataBytes);
            }

            return new PduEventItemResult(CopHandle, CopTag, Timestamp, resultData);
        }

        private static unsafe byte[] ExtractByteArrayBytes(byte* pData, uint numBytes)
        {
            var data = new byte[numBytes];
            for (var index = 0; index < numBytes; index++)
                data[index] = pData[index];
            return data;
        }

        protected override unsafe PduEventItem CreatePduEventItemError()
        {
            var pPduErrorData = (PDU_ERROR_DATA*) PullEventItemGeneralDataFromUnmanagedMemory();
            return new PduEventItemError(CopHandle, CopTag, Timestamp, pPduErrorData->ErrorCodeId,
                pPduErrorData->ExtraErrorInfoId);
        }

        protected override unsafe PduEventItem CreatePduEventItemInfo()
        {
            var pPduInfoData = (PDU_INFO_DATA*)PullEventItemGeneralDataFromUnmanagedMemory();
            return new PduEventItemInfo(CopHandle, CopTag, Timestamp, pPduInfoData->InfoCode,
                pPduInfoData->ExtraInfoData);
        }
    }
}