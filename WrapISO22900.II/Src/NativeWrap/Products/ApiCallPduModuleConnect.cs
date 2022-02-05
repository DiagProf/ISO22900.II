using System;
using Microsoft.Extensions.Logging;

namespace ISO22900.II
{
    internal abstract class ApiCallPduModuleConnect : ApiCall
    {
        internal abstract void PduModuleConnect(uint moduleHandle);
        
        protected ApiCallPduModuleConnect(IntPtr handleToLoadedNativeLibrary) : base(handleToLoadedNativeLibrary)
        {
        }

        protected sealed override string NativeMethodName => "PDUModuleConnect";
    }
}
