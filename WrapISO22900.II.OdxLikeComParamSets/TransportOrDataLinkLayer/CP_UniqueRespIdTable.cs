#region License

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

#endregion


using System.Collections.Generic;

namespace ISO22900.II.OdxLikeComParamSets.TransportOrDataLinkLayer
{
    public abstract class CpUniqueRespIdTable : ComParamHolder
    {
        private readonly HashRuleUniqueRespIdentifierFromCpEcuLayerShortName _hashAlgo;
        private string _cpEcuLayerShortName;

        protected internal List<PduComParam> ComParamList => CpList;

        public string CP_ECULayerShortName
        {
            get => _cpEcuLayerShortName;
            set => _cpEcuLayerShortName = string.IsNullOrWhiteSpace(_cpEcuLayerShortName) ? value : "";
        }

        public uint UniqueRespIdentifier => _hashAlgo(_cpEcuLayerShortName);

        protected CpUniqueRespIdTable(string cpEcuLayerShortName, HashRuleUniqueRespIdentifierFromCpEcuLayerShortName hashAlgo)
        {
            CP_ECULayerShortName = cpEcuLayerShortName;
            _hashAlgo = hashAlgo;
        }
    }
}
