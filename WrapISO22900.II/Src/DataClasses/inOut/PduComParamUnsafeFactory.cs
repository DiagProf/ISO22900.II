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
    internal class PduComParamUnsafeFactory : PduComParamFactory
    {
        private unsafe PDU_PARAM_ITEM* _pointerPduParamItem;

        internal unsafe PduComParam GetPduComParamFromPduParamItemPointer(PDU_PARAM_ITEM* pPduParamItem)
        {
            _pointerPduParamItem = pPduParamItem;
            return PduComParamFromParamType(pPduParamItem->ComParamDataType);
        }

        protected unsafe void* PullComParamGeneralDataFromUnmanagedMemory()
        {
            ComParamId = _pointerPduParamItem->ComParamId;
            ComParamClass = _pointerPduParamItem->ComParamClass;
            return _pointerPduParamItem->pComParamData;
        }

        protected override unsafe PduComParam CreatePduComParamOfTypeByte()
        {
            var comParamData = *(byte*)PullComParamGeneralDataFromUnmanagedMemory();
            return new PduComParamOfTypeByte(ComParamId, ComParamClass, comParamData);
        }

        protected override unsafe PduComParam CreatePduComParamOfTypeByteField()
        {
            var pComParamData = (PDU_PARAM_BYTEFIELD_DATA*) PullComParamGeneralDataFromUnmanagedMemory();
            var paramMaxLen = pComParamData->ParamMaxLen;
            var paramActLen = pComParamData->ParamActLen;
            var pointerDataArray = pComParamData->pDataArray;
            var dataArray = new byte[paramActLen];
            for (var index = 0; index < paramActLen; index++) 
                dataArray[index] = pointerDataArray[index];

            return new PduComParamOfTypeByteField(ComParamId, ComParamClass, new PduParamByteFieldData(dataArray, paramMaxLen));
        }

        protected override unsafe PduComParam CreatePduComParamOfTypeInt()
        {
            var comParamData = *(int*)PullComParamGeneralDataFromUnmanagedMemory();
            return new PduComParamOfTypeInt(ComParamId, ComParamClass, comParamData);
        }

        protected override unsafe PduComParam CreatePduComParamOfTypeSbyte()
        {
            var comParamData = *(sbyte*)PullComParamGeneralDataFromUnmanagedMemory();
            return new PduComParamOfTypeSbyte(ComParamId, ComParamClass, comParamData);
        }

        protected override unsafe PduComParam CreatePduComParamOfTypeShort()
        {
            var comParamData = *(short*)PullComParamGeneralDataFromUnmanagedMemory();
            return new PduComParamOfTypeShort(ComParamId, ComParamClass, comParamData);
        }

        protected override unsafe PduComParam CreatePduComParamOfTypeStructField()
        {
            var pComParamData = (PDU_PARAM_STRUCTFIELD_DATA*)PullComParamGeneralDataFromUnmanagedMemory();
            var paramStructType = pComParamData->ComParamStructType;
            var paramMaxEntries = pComParamData->ParamMaxEntries;
            var paramActEntries = pComParamData->ParamActEntries;
            var pointerStructArray = pComParamData->pStructArray;

            PduParamStructData[] dataArray;
            if (paramStructType == PduCpSt.PDU_CPST_SESSION_TIMING)
                dataArray = CreatePduParamStructSessionTimingField(paramMaxEntries, paramActEntries,
                    (PDU_PARAM_STRUCT_SESS_TIMING*) pointerStructArray);
            else
                dataArray = CreatePduParamStructAccessTimingField(paramMaxEntries, paramActEntries,
                    (PDU_PARAM_STRUCT_ACCESS_TIMING*) pointerStructArray);

            var comParamData = new PduParamStructFieldData(paramStructType, dataArray, paramMaxEntries);
            return new PduComParamOfTypeStructField(ComParamId, ComParamClass, comParamData);
        }

        protected static unsafe PduParamStructSessionTiming[] CreatePduParamStructSessionTimingField(uint paramMaxEntries, uint paramActEntries,
            PDU_PARAM_STRUCT_SESS_TIMING* pointerStructArray)
        {
            var dataArray = new PduParamStructSessionTiming[paramActEntries];
            for (var index = 0; index < paramActEntries; index++)
                dataArray[index] = new PduParamStructSessionTiming(pointerStructArray[index].session,
                    pointerStructArray[index].P2Max_high, pointerStructArray[index].P2Max_low,
                    pointerStructArray[index].P2Star_high, pointerStructArray[index].P2Star_low);
            return dataArray;
        }

        protected static unsafe PduParamStructAccessTiming[] CreatePduParamStructAccessTimingField(uint paramMaxEntries, uint paramActEntries,
            PDU_PARAM_STRUCT_ACCESS_TIMING* pointerStructArray)
        {
            var dataArray = new PduParamStructAccessTiming[paramActEntries];
            for (var index = 0; index < paramActEntries; index++)
            {
                dataArray[index].P2Min = pointerStructArray[index].P2Min;
                dataArray[index].P2Max = pointerStructArray[index].P2Max;
                dataArray[index].P3Min = pointerStructArray[index].P3Min;
                dataArray[index].P4Min = pointerStructArray[index].P4Min;
                dataArray[index].TimingSet = pointerStructArray[index].TimingSet;
            }
            return dataArray;
        }
        protected override unsafe PduComParam CreatePduComParamOfTypeUint()
        {
            var comParamData = *(uint*) PullComParamGeneralDataFromUnmanagedMemory();
            return new PduComParamOfTypeUint(ComParamId, ComParamClass, comParamData);
        }

        protected override unsafe PduComParam CreatePduComParamOfTypeUintField()
        {
            var pComParamData = (PDU_PARAM_LONGFIELD_DATA*)PullComParamGeneralDataFromUnmanagedMemory();
            var paramMaxLen = pComParamData->ParamMaxLen;
            var paramActLen = pComParamData->ParamActLen;
            var pointerDataArray = pComParamData->pDataArray;
            
            var dataArray = new uint[paramActLen];
            for (var index = 0; index < paramActLen; index++) 
                dataArray[index] = pointerDataArray[index];

            return new PduComParamOfTypeUintField(ComParamId, ComParamClass, new PduParamUintFieldData(dataArray, paramMaxLen));
        }

        protected override unsafe PduComParam CreatePduComParamOfTypeUshort()
        {
            var comParamData = *(ushort*)PullComParamGeneralDataFromUnmanagedMemory();
            return new PduComParamOfTypeUshort(ComParamId, ComParamClass, comParamData);
        }

    }
}