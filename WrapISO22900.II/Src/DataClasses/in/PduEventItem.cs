using System;

namespace ISO22900.II
{
    public abstract class PduEventItem : EventArgs
    {
        /// <summary>
        /// 10.1.4.11 Item for event notification 
        /// </summary>
        /// <param name="pduItemType">allowed item types for the PduEventItem are PduIt.PDU_IT_RESULT or PDU_IT_STATUS or PDU_IT_ERROR or PDU_IT_INFO</param>
        /// <param name="copHandle"></param>
        /// <param name="copTag"></param>
        /// <param name="timestamp"></param>
        protected PduEventItem(PduIt pduItemType, uint copHandle, uint copTag, uint timestamp)
        {
            PduItemType = pduItemType;
            CopHandle = copHandle;
            CopTag = copTag;
            Timestamp = timestamp;
        }

        /// <summary>
        /// PDU_IT_RESULT or PDU_IT_STATUS or PDU_IT_ERROR or PDU_IT_INFO
        /// </summary>
        public PduIt PduItemType { get; }

        /// <summary>
        ///     If item is from a ComPrimitiveLevel then the hCop contains the valid ComPrimitiveLevel handle, else it contains
        ///     PDU_HANDLE_UNDEF
        /// </summary>
        internal uint CopHandle { get; }

        /// <summary>
        /// ComPrimitiveLevel Tag. Should be ignored if hCop = PDU_HANDLE_UNDEF
        /// </summary>
        public uint CopTag { get; }
        
        /// <summary>
        /// Timestamp in microseconds
        /// </summary>
        public uint Timestamp { get; }

        /// <summary>
        /// According to spec, EventArgs it is not an element of this structure
        /// </summary>
        internal CallbackEventArgs EventArgs { get; set; }
    }
}