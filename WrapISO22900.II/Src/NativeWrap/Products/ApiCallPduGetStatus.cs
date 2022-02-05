using System;
using Microsoft.Extensions.Logging;

namespace ISO22900.II
{
    internal abstract class ApiCallPduGetStatus : ApiCall
    {
        internal abstract PduExStatusData PduGetStatus(uint moduleHandle, uint comLogicalLinkHandle, uint comPrimitiveHandle);
        
        protected ApiCallPduGetStatus(IntPtr handleToLoadedNativeLibrary) : base(handleToLoadedNativeLibrary)
        {
        }

        protected sealed override string NativeMethodName => "PDUGetStatus";
    }
}
