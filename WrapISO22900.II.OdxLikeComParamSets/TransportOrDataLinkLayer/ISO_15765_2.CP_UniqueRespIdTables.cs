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

namespace ISO22900.II.OdxLikeComParamSets.TransportOrDataLinkLayer
{
    public partial class ISO_15765_2
    {
        protected class CpIso157652UniqueRespIdTables : CpUniqueRespIdTables, IIso157652CpUniqueRespIdTables
        {
            public IIso157652UniqueComParams this[string cpEcuLayerShortName]
            {
                get
                {
                    if ( !Exists(table => table.CP_ECULayerShortName.Equals(cpEcuLayerShortName)) )
                    {
                        Add(new CpIso157652UniqueRespIdTable(cpEcuLayerShortName, HashAlgo));
                    }

                    return (CpIso157652UniqueRespIdTable)Find(table => table.CP_ECULayerShortName.Equals(cpEcuLayerShortName));
                }
            }

            public new CpIso157652UniqueRespIdTable this[int index] => (CpIso157652UniqueRespIdTable)base[index];

            public CpIso157652UniqueRespIdTables(HashRuleUniqueRespIdentifierFromCpEcuLayerShortName hashAlgo) : base(hashAlgo)
            {
            }
        }
    }
}
