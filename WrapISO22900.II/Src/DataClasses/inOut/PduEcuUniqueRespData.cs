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
using System.Linq;

namespace ISO22900.II
{
    public class PduEcuUniqueRespData : ICloneable<PduEcuUniqueRespData>
    {
        /// <summary>
        ///     UniqueRespIdentifier (filled out by Application)
        ///     You can think of it as the page/worksheet "name" of a worksheet in excel
        /// </summary>
        public uint UniqueRespIdentifier { get; set; }

        /// <summary>
        ///     ComParamItems for the Unique Identifier
        /// </summary>
        public List<PduComParam> ComParams { get; init; }

        internal void Accept(IVisitorPduComParamAndUniqueRespIdTable visitorPduEcuUniqueResp)
        {
            visitorPduEcuUniqueResp.VisitConcretePduEcuUniqueRespData(this);
        }

        public PduEcuUniqueRespData(uint uniqueRespIdentifier, List<PduComParam> comParamsItems)
        {
            UniqueRespIdentifier = uniqueRespIdentifier;
            ComParams = comParamsItems;
        }

        public void ComParamUpdateData(string comParamName, long data)
        {
            ComParams.Find(param => param.ComParamShortName.Equals(comParamName))?.UpdateData(data);
        }

        public PduEcuUniqueRespData Clone()
        {
            return new PduEcuUniqueRespData(UniqueRespIdentifier, ComParams.Clone());
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }

    //public abstract class PduEcuUniqueRespDataUnsafe: PduEcuUniqueRespData
    //{

    //    protected PduEcuUniqueRespDataUnsafe(uint uniqueRespIdentifier, List<PduComParam> comParamsItems) : base(uniqueRespIdentifier, comParamsItems)
    //    {
            
    //    }

    //    #region UsedInsidePduGetComParam

    //    #endregion

    //    #region UsedInsidePduSetComParam

    //    //internal override unsafe int CalculateSizeOfComParamData()
    //    //{
    //    //    //return sizeof(PDU_ECU_UNIQUE_RESP_DATA) + ComParam.Sum(comParamItem =>
    //    //    //    comParamItem.CalculateSizeOfComParamData());
    //    //    return 0;
    //    //}
    //    ///// <summary>
    //    /////     Bridge between PduComParamItemdd and the Unsafe World
    //    ///// </summary>
    //    //internal override unsafe void PushComParamDataUnmanagedMemory(IntPtr pComParamDataOnStack)
    //    //{
    //    //    PushComParamDataUnmanagedMemory((PDU_UNIQUE_RESP_ID_TABLE_ITEM*)pComParamDataOnStack.ToPointer());
    //    //}
    //    //internal virtual unsafe void PushComParamDataUnmanagedMemory(PDU_UNIQUE_RESP_ID_TABLE_ITEM* ptrMemory)
    //    //{
    //    //    var pComParamData = ptrMemory->pUniqueData;
    //    //}

    //    #endregion
    //}
}