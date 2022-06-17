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
using ISO22900.II.UnSafeCStructs;

namespace ISO22900.II
{
    internal class VisitorPduComParamAndUniqueRespIdTableToUnmanagedMemoryUnsafe : IVisitorPduComParamAndUniqueRespIdTable
    {
        private unsafe PDU_PARAM_ITEM* _pointerGeneralData;
        private unsafe void* _pointerSpecialData; 

        private unsafe PDU_UNIQUE_RESP_ID_TABLE_ITEM* _pointerTable;
        private unsafe PDU_ECU_UNIQUE_RESP_DATA* _pointerUniqueData;

        /// <summary>
        /// only used inside PduSetComParam
        /// </summary>
        internal unsafe void* PointerForSetComParam
        {
            set
            {
                _pointerGeneralData = (PDU_PARAM_ITEM*) value;
                _pointerSpecialData = _pointerGeneralData + 1; //&_pointerGeneralData[1];
            }
        }

        /// <summary>
        /// only used inside PduSetUniqueRespIdTable
        /// </summary>
        internal unsafe void* PointerForUniqueRespIdTable
        {
            set { _pointerTable = (PDU_UNIQUE_RESP_ID_TABLE_ITEM*) value; }
        }

        #region UsedInsidePduSetComParam

        public unsafe void VisitConcretePduComParamOfTypeByte(PduComParamOfTypeByte cp)
        {
            PushComParamGeneralDataUnmanagedMemory(cp);
            (*(byte*) _pointerSpecialData) = cp.ComParamData;
            _pointerSpecialData = (byte*) _pointerSpecialData + sizeof(byte);
        }

        public void VisitConcretePduComParamOfTypeByteField(PduComParamOfTypeByteField cp)
        {
            PushComParamGeneralDataUnmanagedMemory(cp);
            cp.ComParamData.Accept(this);
        }

        public unsafe void VisitConcretePduComParamOfTypeInt(PduComParamOfTypeInt cp)
        {
            PushComParamGeneralDataUnmanagedMemory(cp);
            (*(int*) _pointerSpecialData) = cp.ComParamData;
            _pointerSpecialData = (byte*) _pointerSpecialData + sizeof(int);
        }

        public void VisitConcretePduComParamOfTypeUintField(PduComParamOfTypeUintField cp)
        {
            PushComParamGeneralDataUnmanagedMemory(cp);
            cp.ComParamData.Accept(this);
        }

        public unsafe void VisitConcretePduComParamOfTypeSbyte(PduComParamOfTypeSbyte cp)
        {
            PushComParamGeneralDataUnmanagedMemory(cp);
            (*(sbyte*) _pointerSpecialData) = cp.ComParamData;
            _pointerSpecialData = (byte*) _pointerSpecialData + sizeof(sbyte);
        }

        public unsafe void VisitConcretePduComParamOfTypeShort(PduComParamOfTypeShort cp)
        {
            PushComParamGeneralDataUnmanagedMemory(cp);
            (*(short*) _pointerSpecialData) = cp.ComParamData;
            _pointerSpecialData = (byte*) _pointerSpecialData + sizeof(short);
        }

        public void VisitConcretePduComParamOfTypeStructField(PduComParamOfTypeStructField cp)
        {
            PushComParamGeneralDataUnmanagedMemory(cp);
            cp.ComParamData.Accept(this);
        }

        public unsafe void VisitConcretePduComParamOfTypeUint(PduComParamOfTypeUint cp)
        {
            PushComParamGeneralDataUnmanagedMemory(cp);
            (*(uint*) _pointerSpecialData) = cp.ComParamData;
            _pointerSpecialData = (byte*) _pointerSpecialData + sizeof(uint);
        }

        public unsafe void VisitConcretePduComParamOfTypeUshort(PduComParamOfTypeUshort cp)
        {
            PushComParamGeneralDataUnmanagedMemory(cp);
            (*(ushort*) _pointerSpecialData) = cp.ComParamData;
            _pointerSpecialData = (byte*) _pointerSpecialData + sizeof(ushort);
        }

        //Param data types which have no equal native type
        public unsafe void VisitConcretePduParamByteFieldData(PduParamByteFieldData pd)
        {
            var pComParamData = (PDU_PARAM_BYTEFIELD_DATA*) _pointerSpecialData;
            _pointerSpecialData = (byte*) _pointerSpecialData + sizeof(PDU_PARAM_BYTEFIELD_DATA);
            pComParamData->ParamMaxLen = pd.ParamMaxLen;
            pComParamData->ParamActLen = pd.ParamActLen;
            pComParamData->pDataArray = (byte*) _pointerSpecialData;

            for (var index = 0; index < pd.ParamActLen; index++)
                pComParamData->pDataArray[index] = pd.DataArray[index];

            _pointerSpecialData = (byte*) _pointerSpecialData + sizeof(byte) * pd.ParamMaxLen;
        }

        public unsafe void VisitConcretePduParamUintFieldData(PduParamUintFieldData pd)
        {
            var pComParamData = (PDU_PARAM_LONGFIELD_DATA*) _pointerSpecialData;
            _pointerSpecialData = (byte*) _pointerSpecialData + sizeof(PDU_PARAM_LONGFIELD_DATA);
            pComParamData->ParamMaxLen = pd.ParamMaxLen;
            pComParamData->ParamActLen = pd.ParamActLen;
            pComParamData->pDataArray = (uint*) _pointerSpecialData;

            for (var index = 0; index < pd.ParamActLen; index++)
                pComParamData->pDataArray[index] = pd.DataArray[index];

            _pointerSpecialData = (byte*) _pointerSpecialData + sizeof(uint) * pd.ParamMaxLen;
        }

        public unsafe void VisitConcretePduParamStructFieldData(PduParamStructFieldData pd)
        {
            var pComParamData = (PDU_PARAM_STRUCTFIELD_DATA*) _pointerSpecialData;
            _pointerSpecialData = (byte*) _pointerSpecialData + sizeof(PDU_PARAM_STRUCTFIELD_DATA);
            pComParamData->ComParamStructType = pd.ComParamStructType;
            pComParamData->ParamMaxEntries = pd.ParamMaxEntries;
            pComParamData->ParamActEntries = pd.ParamActEntries;
            pComParamData->pStructArray = _pointerSpecialData;

            for (var index = 0; index < pd.ParamActEntries; index++)
                pd.StructArray[index].Accept(this);
        }

        //Param data types which are a sub of PduParamStructFieldData
        public unsafe void VisitConcretePduParamStructSessionTiming(PduParamStructSessionTiming pd)
        {
            var pComParamData = (PDU_PARAM_STRUCT_SESS_TIMING*) _pointerSpecialData;
            _pointerSpecialData = (byte*) _pointerSpecialData + sizeof(PDU_PARAM_STRUCT_SESS_TIMING);
            pComParamData->session = pd.Session;
            pComParamData->P2Max_high = pd.P2MaxHigh;
            pComParamData->P2Max_low = pd.P2MaxLow;
            pComParamData->P2Star_high = pd.P2StarHigh;
            pComParamData->P2Star_low = pd.P2StarLow;
        }

        public unsafe void VisitConcretePduParamStructAccessTiming(PduParamStructAccessTiming pd)
        {
            var pComParamData = (PDU_PARAM_STRUCT_ACCESS_TIMING*)_pointerSpecialData;
            _pointerSpecialData = (byte*) _pointerSpecialData + sizeof(PDU_PARAM_STRUCT_ACCESS_TIMING);
            pComParamData->P2Min = pd.P2Min;
            pComParamData->P2Max = pd.P2Max;
            pComParamData->P3Min = pd.P3Min;
            pComParamData->P3Max = pd.P3Max;
            pComParamData->P4Min = pd.P4Min;
            pComParamData->TimingSet = pd.TimingSet;
        }

        public unsafe void VisitConcretePduUniqueRespIdTable(PduUniqueRespIdTable pduUniqueRespIdTable)
        {
            var tableEntriesCount = (uint) pduUniqueRespIdTable.TableEntries.Count;
            _pointerUniqueData = (PDU_ECU_UNIQUE_RESP_DATA*) ((byte*) _pointerTable + sizeof(PDU_UNIQUE_RESP_ID_TABLE_ITEM));
            _pointerTable->ItemType = PduIt.PDU_IT_UNIQUE_RESP_ID_TABLE;
            _pointerTable->NumEntries = tableEntriesCount;
            _pointerTable->pUniqueData = _pointerUniqueData;
            
            SetComParamOffSet(&_pointerUniqueData[tableEntriesCount],
                tableEntriesCount * (uint) pduUniqueRespIdTable.TableEntries[0].ComParams.Count);

            foreach (var entry in pduUniqueRespIdTable.TableEntries) entry.Accept(this);
        }

        public unsafe void VisitConcretePduEcuUniqueRespData(PduEcuUniqueRespData pduEcuUniqueRespData)
        {
            _pointerUniqueData->UniqueRespIdentifier = pduEcuUniqueRespData.UniqueRespIdentifier;
            _pointerUniqueData->NumParamItems = (uint) pduEcuUniqueRespData.ComParams.Count;
            _pointerUniqueData->pParams = _pointerGeneralData; //_pointerGeneralData is set inside SetComParamOffSet()

            foreach (var entry in pduEcuUniqueRespData.ComParams)
            {
                entry.Accept(this);
            }

            _pointerUniqueData++;
        }

        #endregion

        protected unsafe void PushComParamGeneralDataUnmanagedMemory(PduComParam cp)
        {
            var pPduParamItem = _pointerGeneralData;
            pPduParamItem->ItemType = PduIt.PDU_IT_PARAM;
            pPduParamItem->ComParamId = cp.ComParamId;
            pPduParamItem->ComParamDataType = cp.ComParamDataType;
            pPduParamItem->ComParamClass = cp.ComParamClass;
            pPduParamItem->pComParamData = _pointerSpecialData;

            _pointerGeneralData++;
        }

        protected unsafe void SetComParamOffSet(void* start, uint size)
        {
            _pointerGeneralData = (PDU_PARAM_ITEM*)start;
            _pointerSpecialData = _pointerGeneralData + size; //&_pointerGeneralData[size];
        }
    }
}