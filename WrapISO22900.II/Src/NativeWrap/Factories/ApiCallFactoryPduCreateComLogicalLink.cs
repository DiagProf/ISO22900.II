using System;
using ISO22900.II;
using Microsoft.Extensions.Logging;

namespace ISO22900.II
{
    internal class ApiCallFactoryPduCreateComLogicalLink : ApiCallFactory
    {
        internal ApiCallFactoryPduCreateComLogicalLink(ApiModifications flag, IntPtr handleToLoadedNativeLibrary) : base(flag, handleToLoadedNativeLibrary)
        {
            
        }
        internal override ApiCallPduCreateComLogicalLink GetApiCall()
        {
            if (Flag.HasFlag(ApiModifications.UNSAFE_API))
            {
                return new ApiCallPduCreateComLogicalLinkUnsafe(HandleToLoadedNativeLibrary);
            }

            return null; //return new ApiCallPduCreateComLogicalLinkSafe(handleToLoadedNativeLibrary);
        }
    }
}
