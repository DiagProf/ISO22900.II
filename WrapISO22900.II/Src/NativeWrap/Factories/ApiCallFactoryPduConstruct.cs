using System;
using ISO22900.II;
using Microsoft.Extensions.Logging;

namespace ISO22900.II
{
    internal class ApiCallFactoryPduConstruct : ApiCallFactory
    {
        internal ApiCallFactoryPduConstruct(ApiModifications flag, IntPtr handleToLoadedNativeLibrary) : base(flag, handleToLoadedNativeLibrary)
        {
            
        }
        internal override ApiCallPduConstruct GetApiCall()
        {
            if (Flag.HasFlag(ApiModifications.UNSAFE_API))
            {
                return new ApiCallPduConstructUnsafe(HandleToLoadedNativeLibrary);
            }

            return null; //return new ApiCallPduConstructSafe(handleToLoadedNativeLibrary);
        }
    }
}
