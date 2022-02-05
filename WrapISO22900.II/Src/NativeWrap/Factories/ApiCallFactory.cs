using System;
using Microsoft.Extensions.Logging;

namespace ISO22900.II
{
    internal abstract class ApiCallFactory
    {
        protected readonly ApiModifications Flag;
        protected readonly IntPtr HandleToLoadedNativeLibrary;

        protected ApiCallFactory(ApiModifications flag, IntPtr handleToLoadedNativeLibrary)
        {
            Flag = flag;
            HandleToLoadedNativeLibrary = handleToLoadedNativeLibrary;
        }

        internal abstract ApiCall GetApiCall();
    }
}
