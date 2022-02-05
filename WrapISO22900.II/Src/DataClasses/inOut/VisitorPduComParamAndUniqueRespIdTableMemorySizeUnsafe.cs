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
    internal class VisitorPduComParamAndUniqueRespIdTableMemorySizeUnsafe : IVisitorPduComParamAndUniqueRespIdTable
    {
        internal int MemorySize { get; set; }
        
        
        #region UsedInsidePduSetComParam

        public void VisitConcretePduComParamOfTypeByte(PduComParamOfTypeByte cp)
        {
            MemorySize += CalculateSizeOfComParamData() + sizeof(byte);
        }

        public void VisitConcretePduComParamOfTypeByteField(PduComParamOfTypeByteField cp)
        {
            MemorySize += CalculateSizeOfComParamData();
            cp.ComParamData.Accept(this); //this function it self sets the Memory size
        }

        public void VisitConcretePduComParamOfTypeInt(PduComParamOfTypeInt cp)
        {
            MemorySize += CalculateSizeOfComParamData() + sizeof(int);
        }

        public void VisitConcretePduComParamOfTypeUintField(PduComParamOfTypeUintField cp)
        {
            MemorySize += CalculateSizeOfComParamData();
            cp.ComParamData.Accept(this); //this function it self sets the Memory size
        }

        public void VisitConcretePduComParamOfTypeSbyte(PduComParamOfTypeSbyte cp)
        {
            MemorySize += CalculateSizeOfComParamData() + sizeof(sbyte);
        }

        public void VisitConcretePduComParamOfTypeShort(PduComParamOfTypeShort cp)
        {
            MemorySize += CalculateSizeOfComParamData() + sizeof(short);
        }

        public void VisitConcretePduComParamOfTypeStructField(PduComParamOfTypeStructField cp)
        {
            MemorySize += CalculateSizeOfComParamData();
            cp.ComParamData.Accept(this); //this function it self sets the Memory size
        }

        public void VisitConcretePduComParamOfTypeUint(PduComParamOfTypeUint cp)
        {
            MemorySize += CalculateSizeOfComParamData() + sizeof(uint);
        }

        public void VisitConcretePduComParamOfTypeUshort(PduComParamOfTypeUshort cp)
        {
            MemorySize += CalculateSizeOfComParamData() + sizeof(ushort);
        }

        //Param data types which have no equal native type
        public unsafe void VisitConcretePduParamByteFieldData(PduParamByteFieldData pd)
        {
            MemorySize += sizeof(PDU_PARAM_BYTEFIELD_DATA) + (int)pd.ParamMaxLen * sizeof(byte);
        }

        public unsafe void VisitConcretePduParamUintFieldData(PduParamUintFieldData pd)
        {
            MemorySize += sizeof(PDU_PARAM_LONGFIELD_DATA) + (int)pd.ParamMaxLen * sizeof(uint);
        }

        public unsafe void VisitConcretePduParamStructFieldData(PduParamStructFieldData pd)
        {
            MemorySize += sizeof(PDU_PARAM_STRUCTFIELD_DATA);
            foreach (var entry in pd.StructArray)
            {
                entry.Accept(this);
            }
        }

        //Param data types which are a sub of PduParamStructFieldData
        public unsafe void VisitConcretePduParamStructSessionTiming(PduParamStructSessionTiming pd)
        {
            MemorySize += sizeof(PDU_PARAM_STRUCT_SESS_TIMING);
        }

        public unsafe void VisitConcretePduParamStructAccessTiming(PduParamStructAccessTiming pd)
        {
            MemorySize += sizeof(PDU_PARAM_STRUCT_ACCESS_TIMING);
        }

        public unsafe void VisitConcretePduUniqueRespIdTable(PduUniqueRespIdTable pduUniqueRespIdTable)
        {
            MemorySize += sizeof(PDU_UNIQUE_RESP_ID_TABLE_ITEM);
            foreach (var entry in pduUniqueRespIdTable.TableEntries)
            {
                entry.Accept(this);
            }
        }

        public unsafe void VisitConcretePduEcuUniqueRespData(PduEcuUniqueRespData pduEcuUniqueRespData)
        {
            MemorySize += sizeof(PDU_ECU_UNIQUE_RESP_DATA);
            foreach (var entry in pduEcuUniqueRespData.ComParams)
            {
                entry.Accept(this);
            }
        }

        #endregion

        protected static unsafe int CalculateSizeOfComParamData()
        {
            return sizeof(PDU_PARAM_ITEM);
        }
    }
}