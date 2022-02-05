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





// ReSharper disable InconsistentNaming

using System;
using System.Collections.Generic;

namespace ISO22900.II.OdxLikeComParamSets.TransportOrDataLinkLayer
{
    public delegate uint HashRuleUniqueRespIdentifierFromCpEcuLayerShortName(string cpEcuLayerShortName);

    public abstract class AbstractTransportOrDataLinkLayer : ComParamHolder
    {
        protected abstract CpUniqueRespIdTables UniqueRespIdTables { get; }

        protected HashRuleUniqueRespIdentifierFromCpEcuLayerShortName HashAlgo { get; set; }
        ////default
            = delegate(string cpEcuLayerShortName)
            {
                if ( string.IsNullOrWhiteSpace(cpEcuLayerShortName) )
                {
                    return PduConst.PDU_ID_UNDEF;
                }

                var uniqueRespIdentifier = (uint)cpEcuLayerShortName.GetHashCode();
                if ( uniqueRespIdentifier == PduConst.PDU_ID_UNDEF )
                {
                    //Edgecase
                    throw new ArgumentException("CP_ECULayerShortName to hash... result is PduConst.PDU_ID_UNDEF");
                }

                return uniqueRespIdentifier;
            };

        public void SetUpComParamOfTypeUniqueRespIdTable(ComLogicalLink link)
        {
            var ecuUniqueRespDatas = new List<PduEcuUniqueRespData>();
            foreach ( var uniqueRespIdTable in UniqueRespIdTables )
            {
                ecuUniqueRespDatas.Add(new PduEcuUniqueRespData(uniqueRespIdTable.UniqueRespIdentifier, uniqueRespIdTable.ComParamList));
            }

            link.SetUniqueRespIdTable(ecuUniqueRespDatas);
        }
    }
}
