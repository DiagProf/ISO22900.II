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
    public partial class ISO_15765_4
    {
        protected class CpIso157654UniqueRespIdTables : CpUniqueRespIdTables, ISO_15765_4.IIso157654CpUniqueRespIdTables
        {
            public ISO_15765_4.IIso157654UniqueComParams this[string cpEcuLayerShortName]
            {
                get
                {
                    if (Exists(table => table.CP_ECULayerShortName.Equals("")))
                    {
                        var defaultSheet = Find(table => table.CP_ECULayerShortName.Equals(""));
                        defaultSheet.CP_ECULayerShortName = cpEcuLayerShortName;
                    }

                    if ( !Exists(table => table.CP_ECULayerShortName.Equals(cpEcuLayerShortName)) )
                    {
                        Add(new ISO_15765_4.CpIso157654UniqueRespIdTable(cpEcuLayerShortName, HashAlgo));
                    }

                    return (ISO_15765_4.CpIso157654UniqueRespIdTable)Find(table => table.CP_ECULayerShortName.Equals(cpEcuLayerShortName));
                }
            }

            public new ISO_15765_4.CpIso157654UniqueRespIdTable this[int index] => (ISO_15765_4.CpIso157654UniqueRespIdTable)base[index];

            public CpIso157654UniqueRespIdTables(HashRuleUniqueRespIdentifierFromCpEcuLayerShortName hashAlgo) : base(hashAlgo)
            {
            }
        }
    }
}
