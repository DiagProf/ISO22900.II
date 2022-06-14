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
    [StructLayout(LayoutKind.Sequential)]
    internal struct PDU_PARAM_LONGFIELD_DATA
    {
        internal uint ParamMaxLen;   /* Contains the maximum number of bytes the ComParam can contain in
                               pDataArray. This is also the amount of memory the D-PDU API allocates prior to a
                               call of PDUGetComParam. */
        internal uint ParamActLen;   /* Contains the actual number of UNUM32 entries in pDataArray */
        internal uint[] pDataArray;   /* Pointer to an array of UNUM32 values */
    }
}