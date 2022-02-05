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

        internal PduEvtData EventType { get; }
        internal uint ModuleHandle { get; }
        internal uint ComLogicalLinkHandle { get; }
        internal uint ComLogicalLinkTag { get; }
        public uint ApiTag { get; }
    }
}