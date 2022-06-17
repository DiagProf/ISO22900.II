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
using ISO22900.II.UnSafeCStructs;

namespace ISO22900.II
{
    internal class PduUniqueRespIdTableUnsafeFactory
    {
        internal unsafe List<PduEcuUniqueRespData> GetListOfPduEcuUniqueRespDataFromPduUniqueRespIdTablePointer(PDU_UNIQUE_RESP_ID_TABLE_ITEM* pPduUniqueRespIdTable)
        {
            if (pPduUniqueRespIdTable->ItemType != PduIt.PDU_IT_UNIQUE_RESP_ID_TABLE)
            {
                throw new ArgumentOutOfRangeException(nameof(GetListOfPduEcuUniqueRespDataFromPduUniqueRespIdTablePointer));
            }

            var pUniqueData = pPduUniqueRespIdTable->pUniqueData;
            var numEntries = pPduUniqueRespIdTable->NumEntries;
            var pduEcuUniqueRespDatas = new List<PduEcuUniqueRespData>((int)numEntries);

            for (var index = 0; index < numEntries; index++)
            {
                var numParams = pUniqueData[index].NumParamItems;
                var pduComParams = new List<PduComParam>((int) numParams);
                
                for (uint indexParam = 0; indexParam < numParams; indexParam++)
                {
                    pduComParams.Add(PduComParamFactory.GetPduComParamFromPduParamItemPointer(&pUniqueData[index].pParams[indexParam]));
                }
                
                pduEcuUniqueRespDatas.Add(new PduEcuUniqueRespData(pUniqueData[index].UniqueRespIdentifier, pduComParams));
            }
            return pduEcuUniqueRespDatas;
        }

        public PduUniqueRespIdTableUnsafeFactory()
        {
            PduComParamFactory = new PduComParamUnsafeFactory();
        }
        
        private PduComParamUnsafeFactory PduComParamFactory { get; }

    }
}