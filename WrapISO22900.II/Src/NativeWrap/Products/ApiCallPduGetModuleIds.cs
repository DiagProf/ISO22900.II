using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace ISO22900.II
{
    internal abstract class ApiCallPduGetModuleIds : ApiCallPduDestroyItem
    {
        internal abstract List<PduModuleData> PduGetModuleIds();
        
        protected ApiCallPduGetModuleIds(IntPtr handleToLoadedNativeLibrary) : base(handleToLoadedNativeLibrary)
        {
        }

        protected sealed override string NativeMethodName => "PDUGetModuleIds";
    }
}
