#region License

// // MIT License
// //
// // Copyright (c) 2022 Joerg Frank
// // http://www.diagprof.com/
// //
// // Permission is hereby granted, free of charge, to any person obtaining a copy
// // of this software and associated documentation files (the "Software"), to deal
// // in the Software without restriction, including without limitation the rights
// // to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// // copies of the Software, and to permit persons to whom the Software is
// // furnished to do so, subject to the following conditions:
// //
// // The above copyright notice and this permission notice shall be included in all
// // copies or substantial portions of the Software.
// //
// // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// // IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// // FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// // AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// // LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// // OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// // SOFTWARE.

#endregion

namespace ISO22900.II.OdxLikeComParamSets.TransportOrDataLinkLayer
{
    public partial class ISO_13400_2
    {
        protected class CpIso134002UniqueRespIdTables : CpUniqueRespIdTables, IIso134002CpUniqueRespIdTables
        {
            public IIso134002UniqueComParams this[string cpEcuLayerShortName]
            {
                get
                {
                    if ( !Exists(table => table.CP_ECULayerShortName.Equals(cpEcuLayerShortName)) )
                    {
                        Add(new CpIso134002UniqueRespIdTable(cpEcuLayerShortName, HashAlgo));
                    }

                    return (CpIso134002UniqueRespIdTable)Find(table => table.CP_ECULayerShortName.Equals(cpEcuLayerShortName));
                }
            }

            public new CpIso134002UniqueRespIdTable this[int index] => (CpIso134002UniqueRespIdTable)base[index];

            public CpIso134002UniqueRespIdTables(HashRuleUniqueRespIdentifierFromCpEcuLayerShortName hashAlgo) : base(hashAlgo)
            {
            }
        }
    }
}
