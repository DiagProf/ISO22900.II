using System;
using ISO22900.II;
using Microsoft.Extensions.Logging;

namespace ISO22900.II
{
    internal class ApiCallFactoryPduRegisterEventCallback : ApiCallFactory
    {
        internal ApiCallFactoryPduRegisterEventCallback(ApiModifications flag, IntPtr handleToLoadedNativeLibrary) : base(flag, handleToLoadedNativeLibrary)
        {
            
        }
        internal override ApiCallPduRegisterEventCallback GetApiCall()
        {
            if (Flag.HasFlag(ApiModifications.UNSAFE_API))
            {
                return new ApiCallPduRegisterEventCallbackUnsafe(HandleToLoadedNativeLibrary);
            }

            return null; //return new ApiCallPduRegisterEventCallbackSafe(handleToLoadedNativeLibrary);
        }
    }
}
