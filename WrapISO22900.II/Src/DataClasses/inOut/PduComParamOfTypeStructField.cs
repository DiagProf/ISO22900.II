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
    public class PduComParamOfTypeStructField : PduComParam
    {
        /// <summary>
        ///     ComParam data of type PduParamStructFieldData
        /// </summary>
        public PduParamStructFieldData ComParamData { get; set; }

        internal override void Accept(IVisitorPduComParamAndUniqueRespIdTable visitorPduComParamAndUniqueRespIdTable)
        {
            visitorPduComParamAndUniqueRespIdTable.VisitConcretePduComParamOfTypeStructField(this);
        }

        internal PduComParamOfTypeStructField(uint comParamId, PduPc comParamClass, PduParamStructFieldData comParamData, string comParamShortName = "")
            : base( comParamClass, PduPt.PDU_PT_STRUCTFIELD, comParamId, comParamShortName)
        {
            ComParamData = comParamData;
        }

        internal PduComParamOfTypeStructField(string comParamShortName, PduPc comParamClass, PduParamStructFieldData comParamData)
            : this(PduConst.PDU_ID_UNDEF, comParamClass, comParamData, comParamShortName)
        {
        }

        public override PduComParam Clone()
        {
            return new PduComParamOfTypeStructField(ComParamId, ComParamClass, ComParamData.Clone(), ComParamShortName);
        }
    }
}