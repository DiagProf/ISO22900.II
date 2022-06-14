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


using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming
// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable IdentifierTypo

namespace ISO22900.II.SafeCStructs
{
    // Items for unique response identification (used by PDUGetUniqueRespIdTable and PDUSetUniqueRespIdTable)

    [StructLayout(LayoutKind.Sequential)]
    internal struct PDU_UNIQUE_RESP_ID_TABLE_ITEM
    {
        internal PduIt ItemType; /* value= PDU_IT_UNIQUE_RESP_ID_TABLE */
        internal uint NumEntries; /* number of entries in the table */
        internal PDU_ECU_UNIQUE_RESP_DATA[] pUniqueData; /* pointer to array of table entries for each ECU response */
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct PDU_ECU_UNIQUE_RESP_DATA
    {
        internal uint UniqueRespIdentifier; /* filled out by Application */
        internal uint NumParamItems; /* number of ComParams for the Unique Identifier */
        internal PDU_PARAM_ITEM[] pParams; /* pointer to array of ComParam items to uniquely define a ECU response.  The list is protocol specific */
    }
}