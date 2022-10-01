namespace ISO22900.II
{
    /// <summary>
    /// I am not a structure from ISO 22900.
    /// That is why my name start with PduEx (Ex -> Extension).
    /// I am an encapsulation of values that are returned individually in C as call-by-reference.
    /// </summary>
    public class PduExStatusData
    {
        /// <summary>
        ///     Status of MVCI Protocol Module or ComLogicalLinkLayer or ComPrimitiveLevel detected by D-PDU API session
        /// </summary>
        public PduStatus Status { get; }

        /// <summary>
        ///     timestamp in microseconds.
        /// </summary>
        public uint Timestamp { get; }

        /// <summary>
        ///     For ComPrimitives, ExtraInfo is 0.
        ///     For MVCI protocol modules and ComLogicalLinks, ExtraInfo contains additional information which is defined by the
        ///     MVCI protocol module supplier. Use MDF file to get a text from code.
        ///     If no information is available, it shall be 0.
        /// </summary>
        public uint ExtraInfo { get; }

        internal PduExStatusData(PduStatus status, uint timestamp, uint extraInfo)
        {
            Status = status;
            Timestamp = timestamp;
            ExtraInfo = extraInfo;
        }
    }
}