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
    [StructLayout(LayoutKind.Sequential)]
    internal struct PDU_PARAM_STRUCTFIELD_DATA
    {
        internal PduCpSt ComParamStructType;  /* type of ComParam Structure being used. */
        internal uint ParamMaxEntries;     /* Contains the maximum number of struct entries the ComParam can contain in
                                       pStructArray. This is also the amount of memory the D-PDU API allocates prior to
                                       a call of PDUGetComParam. The value of this element is given in the MDF file by
                                       the entry MAX_VALUE of the corresponding COMPARAM_REF */
        internal uint ParamActEntries;     /* Contains the actual number of struct entries in pStructArray. The value range of
                                       this element must be between the MDF file entries MIN_VALUE and
                                       MAX_VALUE of the corresponding COMPARAM_REF */
        internal IntPtr pStructArray;        /* Pointer to an array of structs (typecasted to the ComParamStructType) */
    }

    
    [StructLayout(LayoutKind.Sequential)]
    internal struct PDU_PARAM_STRUCT_SESS_TIMING
    {
        internal UInt16 session;       /* Session number, for the diagnostic session of ISO 15765-3 */
        internal byte P2Max_high;    /* 1ms resolution, default P2Can_Server_max timing supported by the
                               server for the activated diagnostic session. Used for ComParam CP_P2Max. */
        internal byte P2Max_low;     /* 1ms resolution. Used for ComParam CP_P2Min. */
        internal byte P2Star_high;   /* 10ms resolution. Enhanced (NRC 78hex) P2Can_Server_max supported by the
                               server for the activated diagnostic session. Used for ComParam CP_P2Star */
        internal byte P2Star_low;    /* 10ms resolution. Used for internal ECU use only. */
    }

    
    [StructLayout(LayoutKind.Sequential)]
    internal struct PDU_PARAM_STRUCT_ACCESS_TIMING
    {
        internal byte P2Min;         /* 0.5ms resolution. Minimum time between tester request and ECU response(s). Used
                               for ComParam_P2Min */
        internal byte P2Max;         /* Resolution see table 5 of ISO14230-2. Maximum time between tester request and ECU
                               response(s). User for ComParam CP_P2Max */
        internal byte P3Min;         /* 0.5ms resolution. Minimum time between end of ECU responses and start of new
                               tester request. User for ComParam CP_P3Min */
        internal byte P3Max;         /* 250ms resolution. Maximum time between ECU responses and start of new tester
                               request used for ComParam CP_P3Max_Ecu or CP_P2Star for the tester */
        internal byte P4Min;         /* 0.5ms resolution. Minimum inter byte time for tester request. Use for ComParam
                               CP_P4Min */
        internal byte TimingSet;     /* Set number allowing multiple parameters (ex: default, normal) */
    }

}