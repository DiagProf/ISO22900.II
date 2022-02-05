using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ISO22900.II
{
    internal abstract class ApiCall
    {
        protected readonly IntPtr AddressOfNativeMethod;
        protected abstract string NativeMethodName { get; }

        protected ApiCall(IntPtr handleToLoadedNativeLibrary)
        {
            AddressOfNativeMethod = NativeLibrary.GetExport(handleToLoadedNativeLibrary, NativeMethodName);
        }

        protected virtual void CheckResultThrowException(PduError result, [CallerMemberName] string name = "")
        {
            if ( PduError.PDU_STATUS_NOERROR != result )
            {
                throw new Iso22900IIException("ApiCall: " + name + " returns: " + PduErrorCodeToStringLookUp.PduErrorToString[result], result);
            }
        }
    }
}
