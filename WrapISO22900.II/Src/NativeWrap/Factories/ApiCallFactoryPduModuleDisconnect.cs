using System;
using ISO22900.II;
using Microsoft.Extensions.Logging;

namespace ISO22900.II
{
    internal class ApiCallFactoryPduModuleDisconnect : ApiCallFactory
    {
        internal ApiCallFactoryPduModuleDisconnect(ApiModifications flag, IntPtr handleToLoadedNativeLibrary) : base(flag, handleToLoadedNativeLibrary)
        {
            
        }
        internal override ApiCallPduModuleDisconnect GetApiCall()
        {
            if (Flag.HasFlag(ApiModifications.UNSAFE_API))
            {
                return new ApiCallPduModuleDisconnectUnsafe(HandleToLoadedNativeLibrary);
            }

            return null; //return new ApiCallPduModuleDisconnectSafe(handleToLoadedNativeLibrary);
        }
    }
}
