using System;
using Microsoft.Extensions.Logging;

namespace ISO22900.II
{
    internal abstract class ApiCallPduDestroyComLogicalLink : ApiCall
    {
        internal abstract void PduDestroyComLogicalLink(uint moduleHandle, uint comLogicalLinkHandle);
        
        protected ApiCallPduDestroyComLogicalLink(IntPtr handleToLoadedNativeLibrary) : base(handleToLoadedNativeLibrary)
        {
        }

        protected sealed override string NativeMethodName => "PDUDestroyComLogicalLink";
    }
}
