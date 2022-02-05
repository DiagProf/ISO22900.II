using System;
using Microsoft.Extensions.Logging;

namespace ISO22900.II
{
    internal abstract class ApiCallPduModuleDisconnect : ApiCall
    {
        internal abstract void PduModuleDisconnect(uint moduleHandle);
        
        protected ApiCallPduModuleDisconnect(IntPtr handleToLoadedNativeLibrary) : base(handleToLoadedNativeLibrary)
        {
        }

        protected sealed override string NativeMethodName => "PDUModuleDisconnect";
    }
}
