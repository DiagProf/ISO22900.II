using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace ISO22900.II
{
    internal abstract class ApiCallPduRegisterEventCallback : ApiCall
    {
        internal abstract void PduRegisterEventCallback(uint moduleHandle, uint comLogicalLinkHandle,
            Action<PduEvtData, uint, uint, uint, uint> eventHandler);

        // should look like the C function as much as possible.
        // ReSharper disable InconsistentNaming
        // ReSharper disable BuiltInTypeReferenceStyle
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate void CALLBACKFNC(PduEvtData eventType, UInt32 hMod, UInt32 hCLL, IntPtr pCllTag,
            IntPtr pAPITag);

        protected ConcurrentDictionary<ulong, CALLBACKFNC> _callbackfncs = new ConcurrentDictionary<ulong, CALLBACKFNC>();
        
        protected ApiCallPduRegisterEventCallback(IntPtr handleToLoadedNativeLibrary) : base(handleToLoadedNativeLibrary)
        {
        }

        protected sealed override string NativeMethodName => "PDURegisterEventCallback";

    }
}
