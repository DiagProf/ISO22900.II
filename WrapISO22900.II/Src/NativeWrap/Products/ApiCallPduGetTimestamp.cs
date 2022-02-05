using System;
using Microsoft.Extensions.Logging;

namespace ISO22900.II
{
    internal abstract class ApiCallPduGetTimestamp : ApiCall
    {
        //timestamp in microseconds.
        internal abstract uint PduGetTimestamp(uint moduleHandle);
        
        protected ApiCallPduGetTimestamp(IntPtr handleToLoadedNativeLibrary) : base(handleToLoadedNativeLibrary)
        {
        }

        protected sealed override string NativeMethodName => "PDUGetTimestamp";
    }
}
