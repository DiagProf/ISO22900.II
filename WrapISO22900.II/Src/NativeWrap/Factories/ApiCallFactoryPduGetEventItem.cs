using System;
using ISO22900.II;
using Microsoft.Extensions.Logging;

namespace ISO22900.II
{
    internal class ApiCallFactoryPduGetEventItem : ApiCallFactory
    {
        internal ApiCallFactoryPduGetEventItem(ApiModifications flag, IntPtr handleToLoadedNativeLibrary) : base(flag, handleToLoadedNativeLibrary)
        {
            
        }
        internal override ApiCallPduGetEventItem GetApiCall()
        {
            if (Flag.HasFlag(ApiModifications.UNSAFE_API))
            {
                return new ApiCallPduGetEventItemUnsafe(HandleToLoadedNativeLibrary);
            }

            return null; //return new ApiCallPduGetEventItemSafe(handleToLoadedNativeLibrary);
        }
    }
}
