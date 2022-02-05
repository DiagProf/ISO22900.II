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
    /// <summary>
    ///     Is called Longfield in ISO22900
    ///     I have renamed it so that there are no associations with the c# long data type
    /// </summary>
    public class PduComParamOfTypeUintField : PduComParam
    {
        /// <summary>
        ///     ComParam data of type ComParamDataType
        /// </summary>
        public PduParamUintFieldData ComParamData { get; set; }

        internal override void Accept(IVisitorPduComParamAndUniqueRespIdTable visitorPduComParamAndUniqueRespIdTable)
        {
            visitorPduComParamAndUniqueRespIdTable.VisitConcretePduComParamOfTypeUintField(this);
        }

        internal PduComParamOfTypeUintField(uint comParamId, PduPc comParamClass, PduParamUintFieldData comParamData, string comParamShortName = "") :
            base(comParamClass, PduPt.PDU_PT_LONGFIELD, comParamId, comParamShortName)
        {
            ComParamData = comParamData;
        }

        internal PduComParamOfTypeUintField(string comParamShortName, PduPc comParamClass, PduParamUintFieldData comParamData)
            : this(PduConst.PDU_ID_UNDEF, comParamClass, comParamData, comParamShortName)
        {
        }

        public override PduComParam Clone()
        {
            return new PduComParamOfTypeUintField(ComParamId, ComParamClass, ComParamData.Clone(), ComParamShortName);
        }
    }
}