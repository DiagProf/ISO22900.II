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
using System.Collections.Generic;

namespace ISO22900.II
{
    public static class DiagPduApiComParamFactory
    {
        public static PduComParam Create(string name, long value, PduPt pduPt, PduPc pduPc)
        {
            PduComParam cp;

            switch (pduPt)
            {
                case PduPt.PDU_PT_UNUM8:
                    cp = new PduComParamOfTypeByte(name, pduPc, (byte)value);
                    break;
                case PduPt.PDU_PT_SNUM8:
                    cp = new PduComParamOfTypeSbyte(name, pduPc, (sbyte)value);
                    break;
                case PduPt.PDU_PT_UNUM16:
                    cp = new PduComParamOfTypeUshort(name, pduPc, (ushort)value);
                    break;
                case PduPt.PDU_PT_SNUM16:
                    cp = new PduComParamOfTypeShort(name, pduPc, (short)value);
                    break;
                case PduPt.PDU_PT_UNUM32:
                    cp = new PduComParamOfTypeUint(name, pduPc, (uint)value);
                    break;
                case PduPt.PDU_PT_SNUM32:
                    cp = new PduComParamOfTypeInt(name, pduPc, (int)value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return cp;
        }


        public static PduComParam Create(string name, byte[] value, PduPt pduPt, PduPc pduPc)
        {
            PduComParam cp;

            switch (pduPt)
            {
                case PduPt.PDU_PT_BYTEFIELD:
                    cp = new PduComParamOfTypeByteField(name, pduPc, new PduParamByteFieldData(value));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return cp;
        }

        public static PduComParam Create(string name, uint[] value, PduPt pduPt, PduPc pduPc)
        {
            PduComParam cp;

            switch (pduPt)
            {
                case PduPt.PDU_PT_LONGFIELD:
                    cp = new PduComParamOfTypeUintField(name, pduPc, new PduParamUintFieldData(value));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return cp;
        }

        public static PduComParam Create(string name, PduParamStructFieldData value, PduPt pduPt, PduPc pduPc)
        {
            PduComParam cp;

            switch (pduPt)
            {
                case PduPt.PDU_PT_STRUCTFIELD:
                    cp = new PduComParamOfTypeStructField(name, pduPc, value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return cp;
        }
    }
}
