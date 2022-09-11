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
using System.Text;

namespace ISO22900.II
{
    // Basic data types used in ISO22900-2
    // C# using alias directive     //C typedef...
    //using UNUM8 = System.Byte;      //typedef unsigned char UNUM8;      // Unsigned numeric 8 bits
    //using SNUM8 = System.SByte;     //typedef signed char SNUM8;        // Signed numeric 8 bits
    //using UNUM16 = System.UInt16;   //typedef unsigned short UNUM16;    // Unsigned numeric 16 bits
    //using SNUM16 = System.Int16;    //typedef signed short SNUM16;      // Signed numeric 16 bits
    using UNUM32 = System.UInt32;   //typedef unsigned long UNUM32;     // Unsigned numeric 32 bits
    //using SNUM32 = System.Int32;    //typedef signed long SNUM32;       // Signed numeric 32 bits
    using CHAR8 = System.Byte;      //typedef char CHAR8;               // ASCII-coded 8-bit character value (ISO8859-1 (Latin 1))
    
    internal class ApiCallPduGetObjectIdUnsafe : ApiCallPduGetObjectId
    {
        internal override unsafe uint PduGetObjectId(PduObjt pduObjectType, string shortName)
        {
            var asc2 = Encoding.Latin1.GetBytes(shortName);
            //ODX ShortName is not larger than 128 bytes.
            //Stop at 128 so there is no stack overflow
            var length = asc2.Length < 128 ? asc2.Length : 128; 
            var pStack = stackalloc CHAR8[length + 1];
            for (var i = 0; i < length; i++)
            {
                pStack[i] = asc2[i];
            }
            pStack[length] = 0; //zero‐terminated character field

            UNUM32 pduObjectId;
            CheckResultThrowException(PDUGetObjectId(pduObjectType, pStack, &pduObjectId));
            return pduObjectId;
        }

        internal ApiCallPduGetObjectIdUnsafe(IntPtr handleToLoadedNativeLibrary) : base(handleToLoadedNativeLibrary)
        {
        }

        // should look like the C function as much as possible.
        // ReSharper disable InconsistentNaming
        // ReSharper disable BuiltInTypeReferenceStyle
        // ReSharper disable IdentifierTypo
        private unsafe PduError PDUGetObjectId(PduObjt pduObjectType, CHAR8* pShortname, UNUM32* pPduObjectId) => ((delegate* unmanaged[Stdcall]<PduObjt, CHAR8 *, UNUM32 *, PduError>)AddressOfNativeMethod)(pduObjectType, pShortname, pPduObjectId);
        // ReSharper restore IdentifierTypo
        // ReSharper restore InconsistentNaming
        // ReSharper restore BuiltInTypeReferenceStyle
    }
}
