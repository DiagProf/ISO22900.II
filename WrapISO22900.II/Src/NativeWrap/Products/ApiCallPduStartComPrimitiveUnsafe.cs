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
using Microsoft.Extensions.Logging;

namespace ISO22900.II
{
    // Basic data types used in ISO22900-2
    // C# using alias directive     //C typedef...
    using UNUM8 = System.Byte;      //typedef unsigned char UNUM8;      // Unsigned numeric 8 bits
    //using SNUM8 = System.SByte;     //typedef signed char SNUM8;        // Signed numeric 8 bits
    //using UNUM16 = System.UInt16;   //typedef unsigned short UNUM16;    // Unsigned numeric 16 bits
    //using SNUM16 = System.Int16;    //typedef signed short SNUM16;      // Signed numeric 16 bits
    using UNUM32 = System.UInt32;   //typedef unsigned long UNUM32;     // Unsigned numeric 32 bits
    //using SNUM32 = System.Int32;    //typedef signed long SNUM32;       // Signed numeric 32 bits
    //using CHAR8 = System.Byte;      //typedef char CHAR8;               // ASCII-coded 8-bit character value (ISO8859-1 (Latin 1))
    
    internal class ApiCallPduStartComPrimitiveUnsafe : ApiCallPduStartComPrimitive
    {
        private readonly ILogger _logger = ApiLibLogging.CreateLogger<ApiCallPduStartComPrimitiveUnsafe>();
        private readonly VisitorPduComPrimitiveControlDataMemorySizeUnsafe _memorySizeVisitor;
      private readonly VisitorPduComPrimitiveControlDataToUnmanagedMemoryUnsafe _visitorPduComPrimitiveControlData;

      internal override unsafe uint PduStartComPrimitive(uint moduleHandle, uint comLogicalLinkHandle, PduCopt copType,
          byte[] copData, PduCopCtrlData copCtrlData, uint copTag)
      {
          //inside dll nobody dereferences the pointer,so we can use it like a 32 bit id (uint(32bit) is well for 64 and 32 bit DLLs).
          //Downsides is only if cllTag with zero, that should be avoided
          var pCopTag = copTag == 0 ? null : ((IntPtr) copTag).ToPointer();

          uint comPrimitiveHandle = 0;

          if ( copType == PduCopt.PDU_COPT_UPDATEPARAM || copType == PduCopt.PDU_COPT_RESTORE_PARAM )
          {
              CheckResultThrowException(PDUStartComPrimitive(moduleHandle, comLogicalLinkHandle, copType,
                  0, null, null, pCopTag,
                  &comPrimitiveHandle));
          }
          else
          {
              try
              {
                  var pCoPData = stackalloc byte[copData.Length];
                  for ( uint index = 0; index < copData.Length; index++ )
                  {
                      pCoPData[index] = copData[index];
                  }


                  _memorySizeVisitor.MemorySize = 0;
                  copCtrlData.Accept(_memorySizeVisitor);
                  void* pComParamDataOnStack = stackalloc byte[_memorySizeVisitor.MemorySize];
                  _visitorPduComPrimitiveControlData.PointerForCopControlData = pComParamDataOnStack;
                  copCtrlData.Accept(_visitorPduComPrimitiveControlData);
                  CheckResultThrowException(PDUStartComPrimitive(moduleHandle, comLogicalLinkHandle, copType,
                      (uint)copData.Length, copData.Length == 0 ? null : pCoPData, (PDU_COP_CTRL_DATA*)pComParamDataOnStack, pCopTag,
                      &comPrimitiveHandle));
              }
              catch ( StackOverflowException e )
              {
                  _logger.LogCritical(e, "This should not happen with most protocols. Because the size of the data does not exceed ~4096. But with DoIP it could theoretically happen");
              }
          }

          return comPrimitiveHandle;
      }

        internal ApiCallPduStartComPrimitiveUnsafe(IntPtr handleToLoadedNativeLibrary) : base(handleToLoadedNativeLibrary)
        {
            _memorySizeVisitor = new VisitorPduComPrimitiveControlDataMemorySizeUnsafe();
            _visitorPduComPrimitiveControlData = new VisitorPduComPrimitiveControlDataToUnmanagedMemoryUnsafe();
        }

        // should look like the C function as much as possible.
        // ReSharper disable InconsistentNaming
        private unsafe PduError PDUStartComPrimitive(UNUM32 hMod, UNUM32 hCLL, PduCopt CoPType, UNUM32 CoPDataSize, UNUM8* pCoPData, PDU_COP_CTRL_DATA* pCopCtrlData, void* pCoPTag, UNUM32* phCoP)
        {
            return ((delegate* unmanaged[Stdcall]<UNUM32, UNUM32, PduCopt, UNUM32, UNUM8*, PDU_COP_CTRL_DATA*, void*, UNUM32*, PduError >) AddressOfNativeMethod)(hMod, hCLL, CoPType, CoPDataSize, pCoPData, pCopCtrlData, pCoPTag, phCoP);
        }
        // ReSharper restore InconsistentNaming
    }
}
