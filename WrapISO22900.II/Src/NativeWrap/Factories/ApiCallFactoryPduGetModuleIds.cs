using System;
using ISO22900.II;
using Microsoft.Extensions.Logging;

namespace ISO22900.II
{
    internal class ApiCallFactoryPduGetModuleIds : ApiCallFactory
    {
        internal ApiCallFactoryPduGetModuleIds(ApiModifications flag, IntPtr handleToLoadedNativeLibrary) : base(flag, handleToLoadedNativeLibrary)
        {
            
        }
        internal override ApiCallPduGetModuleIds GetApiCall()
        {
            if (Flag.HasFlag(ApiModifications.UNSAFE_API))
            {
                return new ApiCallPduGetModuleIdsUnsafe(HandleToLoadedNativeLibrary);
            }

            return null; //return new ApiCallPduGetModuleIdsSafe(handleToLoadedNativeLibrary);
        }
    }
}
