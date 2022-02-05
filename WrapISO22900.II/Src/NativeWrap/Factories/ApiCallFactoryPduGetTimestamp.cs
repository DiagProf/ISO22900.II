using System;
using ISO22900.II;
using Microsoft.Extensions.Logging;

namespace ISO22900.II
{
    internal class ApiCallFactoryPduGetTimestamp : ApiCallFactory
    {
        internal ApiCallFactoryPduGetTimestamp(ApiModifications flag, IntPtr handleToLoadedNativeLibrary) : base(flag, handleToLoadedNativeLibrary)
        {
            
        }
        internal override ApiCallPduGetTimestamp GetApiCall()
        {
            if (Flag.HasFlag(ApiModifications.UNSAFE_API))
            {
                return new ApiCallPduGetTimestampUnsafe(HandleToLoadedNativeLibrary);
            }

            return null; //return new ApiCallPduGetTimestampSafe(handleToLoadedNativeLibrary);
        }
    }
}
