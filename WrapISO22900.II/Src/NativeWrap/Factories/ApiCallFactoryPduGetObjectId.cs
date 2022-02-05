using System;
using ISO22900.II;
using Microsoft.Extensions.Logging;

namespace ISO22900.II
{
    internal class ApiCallFactoryPduGetObjectId : ApiCallFactory
    {
        internal ApiCallFactoryPduGetObjectId(ApiModifications flag, IntPtr handleToLoadedNativeLibrary) : base(flag, handleToLoadedNativeLibrary)
        {
            
        }
        internal override ApiCallPduGetObjectId GetApiCall()
        {
            if (Flag.HasFlag(ApiModifications.UNSAFE_API))
            {
                return new ApiCallPduGetObjectIdUnsafe(HandleToLoadedNativeLibrary);
            }

            return null; //return new ApiCallPduGetObjectIdSafe(handleToLoadedNativeLibrary);
        }
    }
}
