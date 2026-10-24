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
    /// <summary>
    ///     Is called Longfield in ISO22900
    ///     I have renamed it so that there are no associations with the c# long data type
    /// </summary>
    public class PduParamUintFieldData : ICloneable<PduParamUintFieldData>
    {
        private uint[] _dataArray;
        public uint ParamMaxLen { get; init; }
        public uint ParamActLen => (uint)_dataArray.Length;

        public uint[] DataArray
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

        //public uint this[int index]
        //{
        //    get => DataArray[index];
        //    set => DataArray[index] = value;
        //}

        internal void Accept(IVisitorPduComParamAndUniqueRespIdTable visitorPduComParamAndUniqueRespIdTable)
        {
            visitorPduComParamAndUniqueRespIdTable.VisitConcretePduParamUintFieldData(this);
        }

        /// <summary>
        /// UintField is called LongField in ISO22900
        /// but a long in C # is a different size.
        /// In order to avoid confusion,I gave it the name UintField
        /// </summary>
        /// <param name="uintField"></param>
        /// <param name="paramMaxLen"></param>
        public PduParamUintFieldData(uint[] uintField, uint paramMaxLen = 0x0C)
        {
            if (uintField.Length > paramMaxLen)
                throw new ArgumentOutOfRangeException(nameof(PduParamUintFieldData));
            ParamMaxLen = paramMaxLen;
            _dataArray = uintField;
            
        }

        public PduParamUintFieldData Clone()
        {
            return new PduParamUintFieldData((uint[])DataArray.Clone(), ParamMaxLen);
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}