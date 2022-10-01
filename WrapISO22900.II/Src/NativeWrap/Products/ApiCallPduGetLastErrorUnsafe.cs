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

using System;

// ReSharper disable BuiltInTypeReferenceStyle

namespace ISO22900.II
{
    // Basic data types used in ISO22900-2
    // C# using alias directive     //C typedef...
    //using UNUM8 = System.Byte;      //typedef unsigned char UNUM8;      // Unsigned numeric 8 bits
    //using SNUM8 = System.SByte;     //typedef signed char SNUM8;        // Signed numeric 8 bits
    //using UNUM16 = System.UInt16;   //typedef unsigned short UNUM16;    // Unsigned numeric 16 bits
    //using SNUM16 = System.Int16;    //typedef signed short SNUM16;      // Signed numeric 16 bits
    using UNUM32 = UInt32; //typedef unsigned long UNUM32;     // Unsigned numeric 32 bits
    //using SNUM32 = System.Int32;    //typedef signed long SNUM32;       // Signed numeric 32 bits
    //using CHAR8 = System.Byte;      //typedef char CHAR8;               // ASCII-coded 8-bit character value (ISO8859-1 (Latin 1))

    internal class ApiCallPduGetLastErrorUnsafe : ApiCallPduGetLastError
    {
        internal override unsafe PduExLastErrorData PduGetLastError(uint moduleHandle, uint comLogicalLinkHandle)
        {
            UNUM32 comPrimitiveHandle;  //be careful in this context it is an output parameter
            UNUM32 errorCode;
            UNUM32 timestamp;
            UNUM32 extraInfo;
            CheckResultThrowException(PDUGetLastError(moduleHandle, comLogicalLinkHandle, 
                &comPrimitiveHandle, &errorCode, &timestamp, &extraInfo));
            return new PduExLastErrorData(comPrimitiveHandle, (PduErrEvt)errorCode, timestamp, extraInfo);
        }

        internal ApiCallPduGetLastErrorUnsafe(IntPtr handleToLoadedNativeLibrary) : base(handleToLoadedNativeLibrary)
        {
        }

        // should look like the C function as much as possible.
        // ReSharper disable InconsistentNaming
        private unsafe PduError PDUGetLastError(UNUM32 hMod, UNUM32 hCLL, UNUM32* phCoP, UNUM32* pLastErrorCode,
            UNUM32* pTimestamp, UNUM32* pExtraInfo)
        {
            return ((delegate* unmanaged[Stdcall]<UNUM32, UNUM32, UNUM32*, UNUM32*, UNUM32*, UNUM32*, PduError >)
                AddressOfNativeMethod)(hMod, hCLL, phCoP, pLastErrorCode, pTimestamp, pExtraInfo);
        }
        // ReSharper restore InconsistentNaming
    }
}
