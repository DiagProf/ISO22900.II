using System;

namespace ISO22900.II
{
    public class PduFlagDataCllCreateFlag : ICloneable
    {
        /// <summary>
        /// explanation why it is better to have the default 0x40 instead of 0x00
        /// 0x40 is the ChecksumMode flag
        /// The nature of the D-PDU API is to drive the vehicle protocols. That's why the D-PDU API inherently does the checksum.
        /// But if you e.g. want more info about the deep layers you can turn on the RawMode
        /// But now many are surprised that e.g. K-line protocols no longer work
        /// This is because the ChecksumMode flag is only evaluated when RawMode is true
        /// So from the user's point of view, you only want to turn on the RawMode for some protocols,
        /// but you forget that you also have to turn on ChecksumMode so that everything works as before.
        /// Therefore it is better that ChecksumMode is always set to true by default.
        /// </summary>
        public byte[] FlagData { get; private set; } = new byte[4] {0x40, 0x00, 0x00, 0x00};

        /// <summary>
        ///     Enables the ability to pass through entire received messages, unchanged, through the datalink (transmitted and
        ///     received)
        ///     This feature is protocol specific !!!
        ///     off -> indicates that the D-PDU API will strip the header bytes and checksums before returning (the TxFlag
        ///     ENABLE_EXTRA_INFO can be used to obtain additional message header/footer information)
        ///     on ->  indicates that the header bytes and checksums will be left in the Result Item that is returned
        ///     false = off (default)
        ///     true = on
        /// </summary>
        public bool RawMode
        {
            get => (FlagData[0] & 0x80) != 0;
            set
            {
                if (value)
                {
                    FlagData[0] |= 0x80;
                }
                else
                {
                    FlagData[0] &= 0x7F;
                }
            }
        }

        /// <summary>
        ///     D-PDU API create checksum to transmit messages
        ///     This flag is ignored if RawMode is set to off !!!
        ///     false = off (default)
        ///     true = on
        /// </summary>
        public bool ChecksumMode
        {
            get => (FlagData[0] & 0x40) != 0;
            set
            {
                if (value)
                {
                    FlagData[0] |= 0x40;
                }
                else
                {
                    FlagData[0] &= 0xBF;
                }
            }
        }

        /// <summary>
        ///     Only for Softing D-PDU-API valid !!! (not part of ISO22900-2)
        ///     creates the Monitor Link with a standard call of PDUCreateComLogicalLink and not a Logical Link
        ///     Only valid if protocol “ISO_11898_RAW” or “ISO_14230_3_on_ISO_14230_2” is used
        ///     And only if RawMode also set to true
        ///     false = off (default)
        ///     true = on
        /// </summary>
        public bool MonitorLink
        {
            get => (FlagData[3] & 0x01) != 0;
            set
            {
                if (value)
                {
                    FlagData[3] |= 0x01;
                }
                else
                {
                    FlagData[3] &= 0xFE;
                }
            }
        }

        public object Clone()
        {
            return new PduFlagDataCllCreateFlag()
            {
                FlagData = this.FlagData
            };
        }
    }
}