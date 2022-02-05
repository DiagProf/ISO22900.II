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

namespace ISO22900.II
{
    // Basic data types used in ISO22900-2
    // C# using alias directive     //C typedef...
    using UNUM8 = System.Byte;      //typedef unsigned char UNUM8;      // Unsigned numeric 8 bits
    using SNUM8 = System.SByte;     //typedef signed char SNUM8;        // Signed numeric 8 bits
    using UNUM16 = System.UInt16;   //typedef unsigned short UNUM16;    // Unsigned numeric 16 bits
    using SNUM16 = System.Int16;    //typedef signed short SNUM16;      // Signed numeric 16 bits
    using UNUM32 = System.UInt32;   //typedef unsigned long UNUM32;     // Unsigned numeric 32 bits
    using SNUM32 = System.Int32;    //typedef signed long SNUM32;       // Signed numeric 32 bits
    using CHAR8 = System.Byte;      //typedef char CHAR8;               // ASCII-coded 8-bit character value (ISO8859-1 (Latin 1))
    
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct PDU_PARAM_STRUCTFIELD_DATA
    {
        internal PduCpSt ComParamStructType;  /* type of ComParam Structure being used. */
        internal UNUM32 ParamMaxEntries;     /* Contains the maximum number of struct entries the ComParam can contain in
                                       pStructArray. This is also the amount of memory the D-PDU API allocates prior to
                                       a call of PDUGetComParam. The value of this element is given in the MDF file by
                                       the entry MAX_VALUE of the corresponding COMPARAM_REF */
        internal UNUM32 ParamActEntries;     /* Contains the actual number of struct entries in pStructArray. The value range of
                                       this element must be between the MDF file entries MIN_VALUE and
                                       MAX_VALUE of the corresponding COMPARAM_REF */
        internal void* pStructArray;        /* Pointer to an array of structs (typecasted to the ComParamStructType) */
    }

    
    [StructLayout(LayoutKind.Sequential)]
    internal struct PDU_PARAM_STRUCT_SESS_TIMING
    {
        internal UNUM16 session;       /* Session number, for the diagnostic session of ISO 15765-3 */
        internal UNUM8 P2Max_high;    /* 1ms resolution, default P2Can_Server_max timing supported by the
                               server for the activated diagnostic session. Used for ComParam CP_P2Max. */
        internal UNUM8 P2Max_low;     /* 1ms resolution. Used for ComParam CP_P2Min. */
        internal UNUM8 P2Star_high;   /* 10ms resolution. Enhanced (NRC 78hex) P2Can_Server_max supported by the
                               server for the activated diagnostic session. Used for ComParam CP_P2Star */
        internal UNUM8 P2Star_low;    /* 10ms resolution. Used for internal ECU use only. */
    }

    
    [StructLayout(LayoutKind.Sequential)]
    internal struct PDU_PARAM_STRUCT_ACCESS_TIMING
    {
        internal UNUM8 P2Min;         /* 0.5ms resolution. Minimum time between tester request and ECU response(s). Used
                               for ComParam_P2Min */
        internal UNUM8 P2Max;         /* Resolution see table 5 of ISO14230-2. Maximum time between tester request and ECU
                               response(s). User for ComParam CP_P2Max */
        internal UNUM8 P3Min;         /* 0.5ms resolution. Minimum time between end of ECU responses and start of new
                               tester request. User for ComParam CP_P3Min */
        internal UNUM8 P3Max;         /* 250ms resolution. Maximum time between ECU responses and start of new tester
                               request used for ComParam CP_P3Max_Ecu or CP_P2Star for the tester */
        internal UNUM8 P4Min;         /* 0.5ms resolution. Minimum inter byte time for tester request. Use for ComParam
                               CP_P4Min */
        internal UNUM8 TimingSet;     /* Set number allowing multiple parameters (ex: default, normal) */
    }

}