using System;
using Microsoft.Extensions.Logging;

namespace ISO22900.II
{
    internal abstract class ApiCallPduDestruct : ApiCall
    {
        internal abstract void PduDestruct();
        
        protected ApiCallPduDestruct(IntPtr handleToLoadedNativeLibrary) : base(handleToLoadedNativeLibrary)
        {
        }

        protected sealed override string NativeMethodName => "PDUDestruct";
    }
}
