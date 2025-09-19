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

        internal override unsafe uint PduStartComPrimitive(uint moduleHandle, uint comLogicalLinkHandle,
            PduCopt copType, byte[] copData, PduCopCtrlData copCtrlData, uint copTag)
        {
            // Inside DLL nobody dereferences the pointer, so we can use it like a 32 bit id.
            // Downsides only if copTag is zero; that should be avoided.
            var pCopTag = copTag == 0 ? null : ((IntPtr)copTag).ToPointer();

            uint comPrimitiveHandle = 0;

            if (copType == PduCopt.PDU_COPT_UPDATEPARAM || copType == PduCopt.PDU_COPT_RESTORE_PARAM)
            {
                CheckResultThrowException(PDUStartComPrimitive(moduleHandle, comLogicalLinkHandle, copType,
                    0, null, null, pCopTag, &comPrimitiveHandle));
                return comPrimitiveHandle;
            }

            // We keep the try/catch for documentation purposes, even though:
            // IMPORTANT: A real StackOverflowException will normally terminate the process before this catch executes.
            try
            {
                UNUM32 length = (uint)(copData?.LongLength ?? 0);

                // Local function for the common native invocation part.
                // Prepares control data on stack and invokes the native PDUStartComPrimitive.
                // Pass the handle pointer (&ComPrimitiveHandle) in so we do not take the address of a captured variable (ComPrimitiveHandle) inside.
                void CallNativeCommonPart(byte* pCoPData, uint dataLength, uint* pComPrimitiveHandle)
                {
                    _memorySizeVisitor.MemorySize = 0;
                    copCtrlData.Accept(_memorySizeVisitor);

                    // Control data goes on stack (fast, ephemeral).
                    void* pCopCtrlDataOnStack = stackalloc byte[_memorySizeVisitor.MemorySize];

                    _visitorPduComPrimitiveControlData.PointerForCopControlData = pCopCtrlDataOnStack;
                    copCtrlData.Accept(_visitorPduComPrimitiveControlData);

                    CheckResultThrowException(PDUStartComPrimitive(
                        moduleHandle,
                        comLogicalLinkHandle,
                        copType,
                        dataLength,
                        dataLength == 0 ? null : (UNUM8*)pCoPData,
                        (PDU_COP_CTRL_DATA*)pCopCtrlDataOnStack,
                        pCopTag,
                        pComPrimitiveHandle));
                }

                const int stackAllocThresholdBytes = 512 * 1024; // 512 KB threshold for stack allocation of request data
                // Rationale:
                // - stackalloc is fast for small / medium buffers -> avoids GC pressure and
                //   is more than enough for ISO-TP with max 4096 bytes and even sufficient for
                //   D-PDU-API raw mode where the CAN ID is still included in the data.
                //
                // - For large DoIP request the stack risk (overflow) increases; above the threshold we pin
                //   the existing array with 'fixed' (no extra allocation, no copy) to stay safe.
                //
                // - For control data structure (copCtrlData related) stackalloc is always used because is typically very small
                //
                //
                // By the way:
                // - NOTE: Single arrays (e.g. byte[]) in .NET (including x64) are limited to ~2 GB
                //   (practically 2_147_483_591 elements for byte[]). On 32-bit the smaller virtual
                //   address space additionally constrains usable sizes; on 64-bit the per-object
                //   limit still applies.
                //
                // - It usually does not make practical sense to push extremely LARGE (> tens of MB)
                //   diagnostic request through such an D-PDU API.
                //   You CANNOT present meaningful INCREMENTAL PROGRESS to the user, because transmit takes time.
                //   And imagine that even copying 1 GB in a fast desktop environment takes a noticeable amount of time.
                //   In automotive environments the involved controllers (ECUs) and transport
                //   channels are typically much slower (cost-optimized microcontrollers, slower media), amplifying the issue.
                //
                //   So keep in mind: "Small requests keep the diagnostic application responsive."
                //   Nobody wants to see a "ECU Flash Progress" that looks like the application hangs between each step, or starts and then hangs and then immediately goes to 100%.


                if (length <= stackAllocThresholdBytes)
                {
                    // Small / medium payload: copy to stack buffer.
                    byte* pCoPData = stackalloc byte[(int)length];
                    for (int i = 0; i < length; i++)
                    {
                        pCoPData[i] = copData![i];
                    }

                    CallNativeCommonPart(pCoPData, length, &comPrimitiveHandle);
                }
                else
                {
                    // Large payload: avoid stack usage; pin the existing managed array.
                    // 'fixed' gives a stable pointer for the duration of the call.
                    // No extra allocation, no copy.
                    fixed (byte* pCoPData = copData)
                    {
                        CallNativeCommonPart(pCoPData, length, &comPrimitiveHandle);
                    }
                }
            }
            catch (StackOverflowException e)
            {
                // NOTE: This catch is mostly illustrative because a severe stack overflow will normally bypass managed recovery.
                _logger.LogCritical(e, "Unusually large control data size. Possibly 'stackAllocThresholdBytes' was chosen too large; consider lowering the threshold so large payloads use pinned heap memory earlier to reduce stack pressure.");
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
        private unsafe PduError PDUStartComPrimitive(UNUM32 hMod, UNUM32 hCLL, PduCopt CoPType, UNUM32 CoPDataSize,
            UNUM8* pCoPData, PDU_COP_CTRL_DATA* pCopCtrlData, void* pCoPTag, UNUM32* phCoP)
        {
            return ((delegate* unmanaged[Stdcall]<UNUM32, UNUM32, PduCopt, UNUM32, UNUM8*, PDU_COP_CTRL_DATA*, void*,
                UNUM32*, PduError>)AddressOfNativeMethod)(hMod, hCLL, CoPType, CoPDataSize, pCoPData, pCopCtrlData,
                pCoPTag, phCoP);
        }
        // ReSharper restore InconsistentNaming
    }
}
