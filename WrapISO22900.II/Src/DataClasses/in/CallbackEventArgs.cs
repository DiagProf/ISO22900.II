using System;
using System.Threading.Tasks;

namespace ISO22900.II
{
    //public delegate void CallbackDelegate(object sender, CallbackEventArgs e);

    public class CallbackEventArgs
    {
        internal CallbackEventArgs(PduEvtData eventType, uint moduleHandle, uint comLogicalLinkHandle,
            uint comLogicalLinkTag, uint apiTag)
        {
            EventType = eventType;
            ModuleHandle = moduleHandle;
            ComLogicalLinkHandle = comLogicalLinkHandle;
            ComLogicalLinkTag = comLogicalLinkTag;
            ApiTag = apiTag;
        }

        /// <summary>
        /// Creates event args without native handles, e.g. for raising <c>DataLost</c> in own interface implementations (simulators, test doubles).
        /// </summary>
        public CallbackEventArgs(PduEvtData eventType, uint apiTag = 0)
            : this(eventType, PduConst.PDU_HANDLE_UNDEF, PduConst.PDU_HANDLE_UNDEF, 0, apiTag)
        {
        }

        internal PduEvtData EventType { get; }
        internal uint ModuleHandle { get; }
        internal uint ComLogicalLinkHandle { get; }
        internal uint ComLogicalLinkTag { get; }
        public uint ApiTag { get; }
    }
}