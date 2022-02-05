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
    internal class VisitorPduComPrimitiveControlDataToUnmanagedMemoryUnsafe : IVisitorPduComPrimitiveControlData
    {
        private unsafe PDU_EXP_RESP_DATA* _pointerGeneralData;
        private unsafe void* _pointerSpecialData;

        private unsafe PDU_COP_CTRL_DATA* _pointerCopControlData;
        private unsafe PDU_EXP_RESP_DATA* _pointerStartOfPduExpRespData;


        internal unsafe void* PointerForCopControlData
        {
            set { _pointerCopControlData = (PDU_COP_CTRL_DATA*) value; }
        }

        #region UsedInsidePduStartComPrimitive

        protected unsafe void SetComParamOffSet(PDU_EXP_RESP_DATA* start, uint size)
        {
            _pointerGeneralData = (PDU_EXP_RESP_DATA*)start;
            _pointerSpecialData = &_pointerGeneralData[size];
        }

        public unsafe void VisitConcretePduCopCtrlData(PduCopCtrlData copCtrlData)
        {
            var pduExpectedLength = (uint)copCtrlData.PduExpectedResponseDatas.Length;

            _pointerCopControlData->Time = copCtrlData.Time;
            _pointerCopControlData->NumSendCycles = copCtrlData.NumSendCycles;
            _pointerCopControlData->NumReceiveCycles = copCtrlData.NumReceiveCycles;
            _pointerCopControlData->TempParamUpdate = copCtrlData.TempParamUpdate;
            _pointerCopControlData->TxFlag.NumFlagBytes = (uint)copCtrlData.TxFlag.FlagData.Length;
            _pointerCopControlData->TxFlag.pFlagData = (byte*)_pointerCopControlData + sizeof(PDU_COP_CTRL_DATA);
            for ( var index = 0; index < copCtrlData.TxFlag.FlagData.Length; index++ )
            {
                _pointerCopControlData->TxFlag.pFlagData[index] = copCtrlData.TxFlag.FlagData[index];
            }

            _pointerCopControlData->NumPossibleExpectedResponses = pduExpectedLength;

            _pointerStartOfPduExpRespData = (PDU_EXP_RESP_DATA*)((byte*)_pointerCopControlData +
                                                                 sizeof(PDU_COP_CTRL_DATA) +
                                                                 copCtrlData.TxFlag.FlagData.Length * sizeof(byte));

            _pointerCopControlData->pExpectedResponseArray = _pointerStartOfPduExpRespData;
            SetComParamOffSet(_pointerStartOfPduExpRespData, pduExpectedLength);

            foreach ( var entry in copCtrlData.PduExpectedResponseDatas )
            {
                entry.Accept(this);
            }
        }

        public unsafe void VisitConcretePduExpectedResponseData(PduExpectedResponseData pduExpectedResponseData)
        {
            _pointerGeneralData->ResponseType = (uint) pduExpectedResponseData.ResponseType;
            _pointerGeneralData->AcceptanceId = pduExpectedResponseData.AcceptanceId;
            _pointerGeneralData->NumMaskPatternBytes = pduExpectedResponseData.MaskAndPatternPair.NumMaskPatternBytes;
            if ( pduExpectedResponseData.MaskAndPatternPair.PatternDataArray.Length == 0 )
            {
                _pointerGeneralData->pMaskData = null;
                _pointerGeneralData->pPatternData = null;
                _pointerGeneralData->NumUniqueRespIds = 0;
                _pointerGeneralData->pUniqueRespIds = null;
            }
            else
            {
                _pointerGeneralData->pMaskData = (byte*)_pointerSpecialData;
                _pointerGeneralData->pPatternData = (byte*)_pointerSpecialData + pduExpectedResponseData.MaskAndPatternPair.NumMaskPatternBytes;
                pduExpectedResponseData.MaskAndPatternPair.Accept(this);
                _pointerGeneralData->NumUniqueRespIds = pduExpectedResponseData.UniqueRespIds.NumberOfUniqueRespIds;
                _pointerGeneralData->pUniqueRespIds = (uint*)_pointerSpecialData;
                pduExpectedResponseData.UniqueRespIds.Accept(this);
            }

            _pointerGeneralData++;
        }

        public unsafe void VisitConcretePduMaskAndPatternBytes(MaskAndPatternBytes maskAndPatternBytes)
        {
            var pointeByte = (byte*) _pointerSpecialData;
            for (var index = 0; index < maskAndPatternBytes.NumMaskPatternBytes; index++)
                (*pointeByte++) = maskAndPatternBytes.MaskDataArray[index];
            for (var index = 0; index < maskAndPatternBytes.NumMaskPatternBytes; index++)
                (*pointeByte++) = maskAndPatternBytes.PatternDataArray[index];
            _pointerSpecialData = pointeByte;
        }

        public unsafe void VisitConcretePduUniqueRespIds(UniqueRespIds uniqueRespIds)
        {
            var pointerUint = (uint*)_pointerSpecialData;
            for (var index = 0; index < uniqueRespIds.NumberOfUniqueRespIds; index++)
                (*pointerUint++) = uniqueRespIds[index];
            _pointerSpecialData = pointerUint;
        }

        #endregion
    }
}