using System;
using ISO22900.II;
using Microsoft.Extensions.Logging;

namespace ISO22900.II
{
    internal class ApiCallFactoryPduDestroyComLogicalLink : ApiCallFactory
    {
        internal ApiCallFactoryPduDestroyComLogicalLink(ApiModifications flag, IntPtr handleToLoadedNativeLibrary) : base(flag, handleToLoadedNativeLibrary)
        {
            
        }
        internal override ApiCallPduDestroyComLogicalLink GetApiCall()
        {
            if (Flag.HasFlag(ApiModifications.UNSAFE_API))
            {
                return new ApiCallPduDestroyComLogicalLinkUnsafe(HandleToLoadedNativeLibrary);
            }

            return null; //return new ApiCallPduDestroyComLogicalLinkSafe(handleToLoadedNativeLibrary);
        }
    }
}
