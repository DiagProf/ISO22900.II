using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace ISO22900.II
{
    internal abstract class ApiCallPduGetEventItem : ApiCallPduDestroyItem
    {
        internal abstract Queue<PduEventItem> PduGetEventItem(uint moduleHandle, uint comLogicalLinkHandle);

        protected ApiCallPduGetEventItem(IntPtr handleToLoadedNativeLibrary) : base(handleToLoadedNativeLibrary)
        {
        }

        protected sealed override string NativeMethodName => "PDUGetEventItem";

        protected override void CheckResultThrowException(PduError result, [CallerMemberName] string name = "")
        {
            if (!(PduError.PDU_STATUS_NOERROR == result || PduError.PDU_ERR_EVENT_QUEUE_EMPTY == result))
            {
                base.CheckResultThrowException(result, name);
            }
        }
    }
}
