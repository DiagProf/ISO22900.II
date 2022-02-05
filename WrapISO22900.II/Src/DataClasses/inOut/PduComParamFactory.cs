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
    internal abstract class PduComParamFactory
    {
        #region UsedInsidePduGetComParam
        
        protected uint ComParamId;

        protected PduPc ComParamClass;
        protected PduComParam PduComParamFromParamType(PduPt pduPt)
        {
            return pduPt switch
            {
                PduPt.PDU_PT_UNUM8 => CreatePduComParamOfTypeByte(),
                PduPt.PDU_PT_SNUM8 => CreatePduComParamOfTypeSbyte(),
                PduPt.PDU_PT_UNUM16 => CreatePduComParamOfTypeUshort(),
                PduPt.PDU_PT_SNUM16 => CreatePduComParamOfTypeShort(),
                PduPt.PDU_PT_UNUM32 => CreatePduComParamOfTypeUint(),
                PduPt.PDU_PT_SNUM32 => CreatePduComParamOfTypeInt(),
                PduPt.PDU_PT_BYTEFIELD => CreatePduComParamOfTypeByteField(),
                PduPt.PDU_PT_STRUCTFIELD => CreatePduComParamOfTypeStructField(),
                PduPt.PDU_PT_LONGFIELD => CreatePduComParamOfTypeUintField(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        protected abstract PduComParam CreatePduComParamOfTypeByte();

        protected abstract PduComParam CreatePduComParamOfTypeByteField();

        protected abstract PduComParam CreatePduComParamOfTypeInt();

        protected abstract PduComParam CreatePduComParamOfTypeSbyte();

        protected abstract PduComParam CreatePduComParamOfTypeShort();

        protected abstract PduComParam CreatePduComParamOfTypeStructField();

        protected abstract PduComParam CreatePduComParamOfTypeUint();

        protected abstract PduComParam CreatePduComParamOfTypeUintField();

        protected abstract PduComParam CreatePduComParamOfTypeUshort();

        #endregion
    }
}