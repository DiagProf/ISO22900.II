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
using ISO22900.II.UnSafeCStructs;

// ReSharper disable BuiltInTypeReferenceStyle

namespace ISO22900.II
{
    // Basic data types used in ISO22900-2
    // C# using alias directive     //C typedef...
    //using UNUM8 = Byte; //typedef unsigned char UNUM8;      // Unsigned numeric 8 bits
    //using SNUM8 = System.SByte;     //typedef signed char SNUM8;        // Signed numeric 8 bits
    //using UNUM16 = System.UInt16;   //typedef unsigned short UNUM16;    // Unsigned numeric 16 bits
    //using SNUM16 = System.Int16;    //typedef signed short SNUM16;      // Signed numeric 16 bits
    using UNUM32 = UInt32; //typedef unsigned long UNUM32;     // Unsigned numeric 32 bits
    //using SNUM32 = System.Int32;    //typedef signed long SNUM32;       // Signed numeric 32 bits
    //using CHAR8 = System.Byte;      //typedef char CHAR8;               // ASCII-coded 8-bit character value (ISO8859-1 (Latin 1))

    internal class ApiCallPduIoCtlUnsafe : ApiCallPduIoCtl
    {
        private readonly PduIoCtlUnsafeFactory _pduIoCtlUnsafeFactory;
        private readonly VisitorPduIoCtlMemorySizeUnsafe _memorySizeVisitor;
        private readonly VisitorPduIoCtlToUnmanagedMemoryUnsafe _visitorPduIoCtl;

        internal override unsafe PduIoCtl PduIoCtl(uint moduleHandle, uint comLogicalLinkHandle,
            uint ioCtlCommandId, PduIoCtl input)
        {
            void* pIoCtlDataOnStack = null;
            if (input != null)
            {
                _memorySizeVisitor.MemorySize = 0;
                input.Accept(_memorySizeVisitor);
                var p = stackalloc byte[_memorySizeVisitor.MemorySize];
                pIoCtlDataOnStack = p;
                _visitorPduIoCtl.PointerForIoCtlData = pIoCtlDataOnStack;
                input.Accept(_visitorPduIoCtl);
            }

            PduIoCtl pduIoCtlDataResult = null;
            PDU_DATA_ITEM* pOutputData = null;
            try
            {
                CheckResultThrowException(PDUIoCtl(moduleHandle, comLogicalLinkHandle, ioCtlCommandId,
                    (PDU_DATA_ITEM*)pIoCtlDataOnStack, &pOutputData));
            }
            finally
            {
                if (pOutputData != null)
                {
                    pduIoCtlDataResult = _pduIoCtlUnsafeFactory.GetPduIoCtlFromIoCtlItemPointer(pOutputData);
                    CheckResultThrowException(PDUDestroyItem((PDU_ITEM*)pOutputData));
                }
            }
            return pduIoCtlDataResult;
        }

        internal ApiCallPduIoCtlUnsafe(IntPtr handleToLoadedNativeLibrary) : base(handleToLoadedNativeLibrary)
        {
            _pduIoCtlUnsafeFactory = new PduIoCtlUnsafeFactory();
            _memorySizeVisitor = new VisitorPduIoCtlMemorySizeUnsafe();
            _visitorPduIoCtl = new VisitorPduIoCtlToUnmanagedMemoryUnsafe();
        }

        //should look like the C function as much as possible.
        // ReSharper disable InconsistentNaming
        private unsafe PduError PDUIoCtl(UNUM32 hMod, UNUM32 hCLL, UNUM32 ioCtlCommandId, PDU_DATA_ITEM* pInputData, PDU_DATA_ITEM** pOutputData)
        {
            return ((delegate* unmanaged[Stdcall]<UNUM32, UNUM32, UNUM32, PDU_DATA_ITEM*, PDU_DATA_ITEM**, PduError >) AddressOfNativeMethod)(hMod, hCLL, ioCtlCommandId, pInputData, pOutputData);
        }

        private unsafe PduError PDUDestroyItem(PDU_ITEM* pItem)
        {
            return ((delegate* unmanaged[Stdcall]<PDU_ITEM*, PduError>) AddressOfNativeMethodDestroyItem)(pItem);
        }
        // ReSharper restore InconsistentNaming
    }


}
