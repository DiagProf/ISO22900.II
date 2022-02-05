using System;
using ISO22900.II;
using Microsoft.Extensions.Logging;

namespace ISO22900.II
{
    internal class ApiCallFactoryPduGetStatus : ApiCallFactory
    {
        internal ApiCallFactoryPduGetStatus(ApiModifications flag, IntPtr handleToLoadedNativeLibrary) : base(flag, handleToLoadedNativeLibrary)
        {
            
        }
        internal override ApiCallPduGetStatus GetApiCall()
        {
            if (Flag.HasFlag(ApiModifications.UNSAFE_API))
            {
                return new ApiCallPduGetStatusUnsafe(HandleToLoadedNativeLibrary);
            }

            return null; //return new ApiCallPduGetStatusSafe(handleToLoadedNativeLibrary);
        }
    }
}
