using System;
using ISO22900.II;
using Microsoft.Extensions.Logging;

namespace ISO22900.II
{
    internal class ApiCallFactoryPduDestruct : ApiCallFactory
    {
        internal ApiCallFactoryPduDestruct(ApiModifications flag, IntPtr handleToLoadedNativeLibrary) : base(flag, handleToLoadedNativeLibrary)
        {
            
        }
        internal override ApiCallPduDestruct GetApiCall()
        {
            if (Flag.HasFlag(ApiModifications.UNSAFE_API))
            {
                return new ApiCallPduDestructUnsafe(HandleToLoadedNativeLibrary);
            }

            return null; //return new ApiCallPduDestructSafe(handleToLoadedNativeLibrary);
        }
    }
}
