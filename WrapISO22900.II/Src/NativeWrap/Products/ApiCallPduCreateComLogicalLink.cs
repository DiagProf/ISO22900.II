using System;
using Microsoft.Extensions.Logging;

namespace ISO22900.II
{
    internal abstract class ApiCallPduCreateComLogicalLink : ApiCall
    {
        internal abstract uint PduCreateComLogicalLink(uint moduleHandle, PduResourceData pduResourceData, uint resourceId, uint cllTag, PduFlagDataCllCreateFlag pduFlagDataCllCreateFlag);
        
        protected ApiCallPduCreateComLogicalLink(IntPtr handleToLoadedNativeLibrary) : base(handleToLoadedNativeLibrary)
        {
        }

        protected sealed override string NativeMethodName => "PDUCreateComLogicalLink";
    }
}
