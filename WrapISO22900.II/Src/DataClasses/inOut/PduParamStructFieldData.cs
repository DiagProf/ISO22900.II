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
    public class PduParamStructFieldData : ICloneable<PduParamStructFieldData>
    {
        private PduParamStructData[] _structArray;
        public PduCpSt ComParamStructType { get; init; } /* type of ComParam Structure being used. */

        public uint ParamMaxEntries { get; init; }
        
        public uint ParamActEntries => (uint)_structArray.Length;

        public PduParamStructData[] StructArray
        {
            get => _structArray;
            set
            {
                if (value.Length <= ParamMaxEntries)
                {
                    _structArray = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(PduParamStructData), "");
                }
            }
        }

        internal void Accept(IVisitorPduComParamAndUniqueRespIdTable visitorPduComParamAndUniqueRespIdTable)
        {
            visitorPduComParamAndUniqueRespIdTable.VisitConcretePduParamStructFieldData(this);
        }

        private PduParamStructFieldData(PduCpSt comParamStructType, PduParamStructData[] structField, uint paramMaxEntries)
        {
            ComParamStructType = comParamStructType;
            ParamMaxEntries = paramMaxEntries;
            _structArray = structField;
        }

        public PduParamStructFieldData( PduParamStructAccessTiming[] structField, uint paramMaxEntries)
        {
            ComParamStructType = PduCpSt.PDU_CPST_ACCESS_TIMING;
            ParamMaxEntries = paramMaxEntries;
            _structArray = structField;
        }

        public PduParamStructFieldData(PduParamStructSessionTiming[] structField, uint paramMaxEntries)
        {
            ComParamStructType = PduCpSt.PDU_CPST_SESSION_TIMING;
            ParamMaxEntries = paramMaxEntries;
            _structArray = structField;
        }

        public PduParamStructFieldData Clone()
        {
            return new PduParamStructFieldData(ComParamStructType, (PduParamStructData[])StructArray.Clone(), ParamMaxEntries);
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}