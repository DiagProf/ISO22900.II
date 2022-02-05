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
    public abstract class PduComParam : ICloneable<PduComParam>
    {
        /// <summary>
        ///     ComParam Short Name
        /// </summary>
        public string ComParamShortName { get; internal set; }

        /// <summary>
        ///     ComParamId Value from MDF of MVCI Protocol Module
        /// </summary>
        internal uint ComParamId { get; set; }

        /// <summary>
        ///     ComParam Class type.  The class type is used by the D-PDU API for special ComParam handling cases.
        ///     (BusType (physical ComParams) and Unique ID ComParams)). See section B.2.2
        /// </summary>
        public PduPc ComParamClass { get; init; }

        /// <summary>
        ///     Defines the data type of the ComParam See section B.2.3 ComParam data type
        /// </summary>
        public PduPt ComParamDataType { get; }

        internal abstract void Accept(IVisitorPduComParamAndUniqueRespIdTable visitorPduComParamAndUniqueRespIdTable);

        protected PduComParam(PduPc comParamClass, PduPt comParamDataType, uint comParamId, string comParamShortName)
        {
            ComParamId = comParamId;
            ComParamClass = comParamClass;
            ComParamDataType = comParamDataType;
            ComParamShortName = comParamShortName;
        }

        public PduComParam UpdateData(long data)
        {
            switch (ComParamDataType)
            {
                case PduPt.PDU_PT_UNUM8:
                    ((PduComParamOfTypeByte)this).ComParamData = (byte)data;
                    break;
                case PduPt.PDU_PT_SNUM8:
                    ((PduComParamOfTypeSbyte)this).ComParamData = (sbyte)data;
                    break;
                case PduPt.PDU_PT_UNUM16:
                    ((PduComParamOfTypeUshort)this).ComParamData = (ushort)data;
                    break;
                case PduPt.PDU_PT_SNUM16:
                    ((PduComParamOfTypeShort)this).ComParamData = (short)data;
                    break;
                case PduPt.PDU_PT_UNUM32:
                    ((PduComParamOfTypeUint)this).ComParamData = (uint)data;
                    break;
                case PduPt.PDU_PT_SNUM32:
                    ((PduComParamOfTypeInt)this).ComParamData = (int)data;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return this;
        }

        public PduComParam UpdateData(byte[] data)
        {
            switch (ComParamDataType)
            {
                case PduPt.PDU_PT_BYTEFIELD:
                    ((PduComParamOfTypeByteField)this).ComParamData = new PduParamByteFieldData(data);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return this;
        }

        public PduComParam UpdateData(uint[] data)
        {
            switch (ComParamDataType)
            {
                case PduPt.PDU_PT_LONGFIELD:
                    ((PduComParamOfTypeUintField)this).ComParamData = new PduParamUintFieldData(data);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return this;
        }

        public PduComParam UpdateData(PduParamStructFieldData data)
        {
            switch (ComParamDataType)
            {
                case PduPt.PDU_PT_STRUCTFIELD:
                    ((PduComParamOfTypeStructField)this).ComParamData = data;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return this;
        }


        public abstract PduComParam Clone();
        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}