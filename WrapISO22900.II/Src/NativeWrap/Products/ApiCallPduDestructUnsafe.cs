using System;
using Microsoft.Extensions.Logging;

namespace ISO22900.II
{
    internal class ApiCallPduDestructUnsafe : ApiCallPduDestruct
    {
        internal override void PduDestruct()
        {
            CheckResultThrowException(PDUDestruct());
        }

        internal ApiCallPduDestructUnsafe(IntPtr handleToLoadedNativeLibrary) : base(handleToLoadedNativeLibrary)
        {
        }

        //should look like the C function as much as possible.
        private unsafe PduError PDUDestruct() => ((delegate* unmanaged[Stdcall]<PduError>)AddressOfNativeMethod)();
    }
}
