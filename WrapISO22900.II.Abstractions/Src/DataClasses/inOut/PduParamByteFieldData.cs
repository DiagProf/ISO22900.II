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
    public class PduParamByteFieldData : ICloneable<PduParamByteFieldData>
    {
        private byte[] _dataArray;
        public uint ParamMaxLen { get; init; }
        
        public uint ParamActLen => (uint)_dataArray.Length;

        public byte[] DataArray
        {
            get => _dataArray;
            set
            {
                if (value.Length <= ParamMaxLen)
                {
                    _dataArray = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        internal void Accept(IVisitorPduComParamAndUniqueRespIdTable visitorPduComParamAndUniqueRespIdTable)
        {
            visitorPduComParamAndUniqueRespIdTable.VisitConcretePduParamByteFieldData(this);
        }

        public PduParamByteFieldData(byte[] byteField, uint paramMaxLen = 0x0C)
        {
            if (byteField.Length > paramMaxLen)
                throw new ArgumentOutOfRangeException();
            ParamMaxLen = paramMaxLen;
            _dataArray = byteField;
            
        }

        public PduParamByteFieldData Clone()
        {
            return new PduParamByteFieldData((byte[])DataArray.Clone(),ParamMaxLen);
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}