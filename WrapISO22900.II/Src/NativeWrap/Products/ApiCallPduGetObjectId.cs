using System;
using Microsoft.Extensions.Logging;

namespace ISO22900.II
{
    internal abstract class ApiCallPduGetObjectId : ApiCall
    {
        internal abstract uint PduGetObjectId(PduObjt pduObjectType, string shortName);


        protected ApiCallPduGetObjectId(IntPtr handleToLoadedNativeLibrary) : base(handleToLoadedNativeLibrary)
        {
        }

        protected sealed override string NativeMethodName => "PDUGetObjectId";
    }
}
