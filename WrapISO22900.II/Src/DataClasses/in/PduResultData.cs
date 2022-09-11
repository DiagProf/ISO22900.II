#region License

// /*
// MIT License
// 
// Copyright (c) 2022 Joerg Frank
// 
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// */

#endregion

namespace ISO22900.II
{
    /// <summary>
    /// Asynchronous result notification structure (received data) for the PDU_IT_RESULT Item.
    /// </summary>
    public class PduResultData
    {
        /// <summary>
        /// Receive message status. See RxFlag definition
        /// </summary>
        public PduFlagDataRxFlag RxFlag { get; }

        /// <summary>
        /// ECU response unique identifier
        /// </summary>
        public uint UniqueRespIdentifier { get; }

        /// <summary>
        /// Acceptance Id value from ComPrimitiveLevel Expected Response Structure.
        /// If multiple expected response entries match the response payload data,
        /// then the first matching expected response id found in the array of expected
        /// responses is used. (I.e. acceptance filtering is carried out in the sequence
        /// of the expected responses as they appear in the array of expected responses.
        /// Thus, an expected response with the lowest array index has the highest priority.)
        /// </summary>
        public uint AcceptanceId { get; }

        /// <summary>
        /// Bitoriented Timestamp Indicator flag (See sections Structure for flag data and TimestampFlag definition)
        /// If the flag data is 0, then the following timestamp information is not valid.
        /// </summary>
        public PduFlagDataTimestampFlag TimestampFlags { get; }

        /// <summary>
        /// Transmit Message DONE! Timestamp in microseconds
        /// Timestamp after the Tester request was transmitted.
        /// </summary>
        public uint TxMsgDoneTimestamp { get; }

        /// <summary>
        /// Start Message Timestamp in microseconds
        /// "Start Message" -> is the start of ECU Response
        /// </summary>
        public uint StartMsgTimestamp { get; }

        /// <summary>
        /// if RawMode then the data includes header bytes, checksum, message data bytes, and extra data, if any.
        /// In RawMode -> For ISO 15765, ISO11898 and SAE J1939, the first 4 bytes are the CAN ID (11 bit or 29 bit) followed by a possible extended address byte
        /// </summary>
        public byte[] DataBytes { get; }


        /// <summary>
        /// If false, no extra information is attached to the response structure.
        /// This feature is enabled by setting the ENABLE_EXTRA_INFO bit in the TxFlag for
        /// the ComPrimitiveLevel (See TxFlag definition section)
        /// </summary>
        public bool IsExtraInfo { get => ExtraInfoHeaderBytes.Length != 0 | ExtraInfoFooterBytes.Length != 0; }

        //The next 2 parameters actually come from the c structure PDU_EXTRA_INFO
        //this structure is omitted on the c # side

        /// <summary>
        /// Response PDU Header bytes
        /// </summary>
        public byte[] ExtraInfoHeaderBytes { get; }

        /// <summary>
        /// Response PDU Footer bytes
        /// (SAE J1850 PWM) Start position of extra data in received message (for example, IFR) or ISO14230 checksum.)
        /// When no extra data bytes are present in the message, FooterBytes has zero size
        /// </summary>
        public byte[] ExtraInfoFooterBytes { get; }

        internal PduResultData(PduFlagDataRxFlag rxFlag, uint uniqueRespIdentifier, uint acceptanceId, PduFlagDataTimestampFlag timestampFlags, uint txMsgDoneTimestamp, uint startMsgTimestamp, byte[] dataBytes, byte[] extraInfoHeaderBytes, byte[] extraInfoFooterBytes)
        {

            RxFlag = rxFlag;
            UniqueRespIdentifier = uniqueRespIdentifier;
            AcceptanceId = acceptanceId;
            TimestampFlags = timestampFlags;
            TxMsgDoneTimestamp = txMsgDoneTimestamp;
            StartMsgTimestamp = startMsgTimestamp;
            DataBytes = dataBytes;
            ExtraInfoHeaderBytes = extraInfoHeaderBytes;
            ExtraInfoFooterBytes = extraInfoFooterBytes;
        }

        internal PduResultData(PduFlagDataRxFlag rxFlag, uint uniqueRespIdentifier, uint acceptanceId, PduFlagDataTimestampFlag timestampFlags, uint txMsgDoneTimestamp, uint startMsgTimestamp, byte[] dataBytes)
        {

            RxFlag = rxFlag;
            UniqueRespIdentifier = uniqueRespIdentifier;
            AcceptanceId = acceptanceId;
            TimestampFlags = timestampFlags;
            TxMsgDoneTimestamp = txMsgDoneTimestamp;
            StartMsgTimestamp = startMsgTimestamp;
            DataBytes = dataBytes;
            ExtraInfoHeaderBytes = System.Array.Empty<byte>();
            ExtraInfoFooterBytes = System.Array.Empty<byte>();
        }
    }
}
