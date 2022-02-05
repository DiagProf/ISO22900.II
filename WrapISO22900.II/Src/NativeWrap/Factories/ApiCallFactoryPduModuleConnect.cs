using System;
using ISO22900.II;
using Microsoft.Extensions.Logging;

namespace ISO22900.II
{
    internal class ApiCallFactoryPduModuleConnect : ApiCallFactory
    {
        internal ApiCallFactoryPduModuleConnect(ApiModifications flag, IntPtr handleToLoadedNativeLibrary) : base(flag, handleToLoadedNativeLibrary)
        {
            
        }
        internal override ApiCallPduModuleConnect GetApiCall()
        {
            if (Flag.HasFlag(ApiModifications.UNSAFE_API))
            {
                return new ApiCallPduModuleConnectUnsafe(HandleToLoadedNativeLibrary);
            }

            return null; //return new ApiCallPduModuleConnectSafe(handleToLoadedNativeLibrary);
        }
    }
}
