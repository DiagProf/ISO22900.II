using System;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace ISO22900.II
{
    internal class ApiCallPduConstructSafe : ApiCallPduConstruct
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private delegate PduError PDUConstruct(in string optionStr, in IntPtr pApiTag);
        private PDUConstruct _PDUConstruct;

        //Why are we not using IntPtr for the void* and use instead uint?
        //Why declaring all these abstract overloads for one Method,
        //when we could use default values? (would get rid of 3 Methods in this case) Example:
        internal override void PduConstruct(string optionStr = "", uint apiTag = 1)
        {
            //As my understanding of the Iso the apiTag should never be null, or am i wrong?
            var result = _PDUConstruct(optionStr, new IntPtr(apiTag));
            CheckResultThrowException(result);
        }

        internal override void PduConstruct(string optionStr)
        {
            PduConstruct(optionStr);
        }

        internal override void PduConstruct(uint apiTag)
        {
            PduConstruct(apiTag);
        }

        internal override void PduConstruct()
        {
            PduConstruct();
        }

        internal ApiCallPduConstructSafe(IntPtr handleToLoadedNativeLibrary) : base(handleToLoadedNativeLibrary)
        {
            _PDUConstruct = (PDUConstruct)Marshal.GetDelegateForFunctionPointer(AddressOfNativeMethod, typeof(PDUConstruct));
        }
    }
}
