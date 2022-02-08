using System;
using Microsoft.Extensions.Logging;

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

    internal class ApiCallPduRegisterEventCallbackUnsafe : ApiCallPduRegisterEventCallback
    {

        internal override unsafe void PduRegisterEventCallback(uint moduleHandle, uint comLogicalLinkHandle,
            Action<PduEvtData, uint, uint, uint, uint> eventHandler)
        {
            if (eventHandler == null)
            {
                try
                {
                    CheckResultThrowException(PDURegisterEventCallback(moduleHandle, comLogicalLinkHandle, null));
                }
                finally
                {
                    _callbackfncs.TryRemove(((ulong)moduleHandle << 32) | comLogicalLinkHandle, out var callback);
                }
            }
            else
            {
                var callback = new CALLBACKFNC((eventType, hMod, hCll, pCllTag, pApiTag) =>
                {
                    var localCopyEventHandler = eventHandler;
                    uint cllTag = 0;
                    //if (pCllTag != IntPtr.Zero)
                    cllTag = (uint) pCllTag.ToInt32();

                    uint apiTag = 0;
                    //if (pApiTag != IntPtr.Zero)
                    apiTag = (uint) pApiTag.ToInt32();

                    localCopyEventHandler(eventType, hMod, hCll, cllTag, apiTag);

                });

                CheckResultThrowException(PDURegisterEventCallback(moduleHandle, comLogicalLinkHandle, callback));

                _callbackfncs.AddOrUpdate(((ulong) moduleHandle << 32) | comLogicalLinkHandle, callback,
                    (id, existingCallback) => { return existingCallback = callback; });
                
            }

        }

        internal ApiCallPduRegisterEventCallbackUnsafe(IntPtr handleToLoadedNativeLibrary) : base(handleToLoadedNativeLibrary)
        {
        }

        // should look like the C function as much as possible.
        // ReSharper disable InconsistentNaming
        // ReSharper disable BuiltInTypeReferenceStyle

        private unsafe PduError PDURegisterEventCallback(UNUM32 hMod, UNUM32 hCLL, CALLBACKFNC EventCallbackFunction)
        {
            return ((delegate* unmanaged[Stdcall]<UNUM32, UNUM32, CALLBACKFNC, PduError>) AddressOfNativeMethod)(hMod,
                hCLL, EventCallbackFunction);
        }

        // ReSharper restore InconsistentNaming
        // ReSharper restore BuiltInTypeReferenceStyle
    }
}
