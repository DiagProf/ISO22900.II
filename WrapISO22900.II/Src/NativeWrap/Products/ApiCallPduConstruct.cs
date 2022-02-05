using System;
using Microsoft.Extensions.Logging;

namespace ISO22900.II
{
    internal abstract class ApiCallPduConstruct : ApiCall
    {
        internal abstract void PduConstruct(string optionStr, uint apiTag);

        internal abstract void PduConstruct(string optionStr);

        internal abstract void PduConstruct(uint apiTag);

        internal abstract void PduConstruct();
        
        protected ApiCallPduConstruct(IntPtr handleToLoadedNativeLibrary) : base(handleToLoadedNativeLibrary)
        {
        }

        protected sealed override string NativeMethodName => "PDUConstruct";
    }
}
