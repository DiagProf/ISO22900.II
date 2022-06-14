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
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming
// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable IdentifierTypo

namespace ISO22900.II.SafeCStructs
{
    // Item for ComParam data (used by PDUGetComParam, PDUSetComParam)
    [StructLayout(LayoutKind.Sequential)]
    internal struct PDU_PARAM_ITEM
    {
        internal PduIt ItemType; /* value= PDU_IT_PARAM */
        
        internal uint ComParamId; /* ComParam Id.  Value from MDF of MVCI Protocol Module */

        internal PduPt ComParamDataType; /* Defines the data type of the ComParam See section B.2.3 ComParam data type */

        internal PduPc ComParamClass; /* ComParam Class type.  The class type is used by the D-PDU API for special ComParam handling cases. (BusType (physical ComParams) and Unique ID ComParams)). See section B.2.2 */

        internal IntPtr pComParamData; /* pointer to ComParam data of type ComParamDataType */
    }
}