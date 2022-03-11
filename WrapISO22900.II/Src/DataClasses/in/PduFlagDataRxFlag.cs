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
    public class PduFlagDataRxFlag
    {
        public byte[] FlagData { get; } = new byte[4] { 0x00, 0x00, 0x00, 0x00 };

        /// <summary>
        ///     CAN remote frame detected. No data bytes are received.The first byte of the D-DPU will contain the data length code
        ///     false = No Remote Frame Received
        ///     true = Received a Remote Frame
        /// </summary>
        public bool RemoteFrame => (FlagData[0] & 0x80) != 0;

        /// <summary>
        ///     Indicates that the serial bus has transitioned to a new speed.
        ///     false = No Event
        ///     true = Transitioned to new speed rate
        /// </summary>
        public bool SpeedChangeEvent => (FlagData[1] & 0x04) != 0;

        /// <summary>
        ///     The timing ComParams values have been modified for the Com Logical Link.
        ///     This flag will only be set if the CP_ModifyTiming ComParam is set to Enable !!
        ///     false = No Timing Change
        ///     true = Timing ComParams have been modified
        /// </summary>
        public bool EcuTimingChange => (FlagData[1] & 0x02) != 0;

        /// <summary>
        ///     Indicates that the Single Wire CAN message received was a High-Voltage Message
        ///     false = Normal Message
        ///     true = High-Voltage Message
        /// </summary>
        public bool SingleWireCanHvRx => (FlagData[1] & 0x01) != 0;

        /// <summary>
        ///     RAW_MODE ONLY (is only effective in conjunction with RawMode true inside PduFlagDataCllCreateFlag)
        ///     CAN ID type for ISO 11898, SAE J1939, andISO 15765
        ///     CAN ID is contained in the first 4 bytes of thePDU Data
        ///     false = 11-bit
        ///     true = 29-bit
        /// </summary>
        public bool Can29BitId => (FlagData[2] & 0x01) != 0;

        /// <summary>
        ///     RAW_MODE ONLY (is only effective in conjunction with RawMode true inside PduFlagDataCllCreateFlag)
        ///     ISO 15765-2 Addressing Method CAN Extended Address byte is contained in the bytes following the CAN ID in the PDU Data.
        ///     false = no extended address
        ///     true = extended addressing is used
        /// </summary>
        public bool Iso15765AddrType => (FlagData[3] & 0x80) != 0;

        /// <summary>
        ///     RAW_MODE ONLY (is only effective in conjunction with RawMode true inside PduFlagDataCllCreateFlag)
        ///     ISO 15765-2 Can Segmentation handling Received message was either handled as a segmented or unsegmented message.
        ///     (If Segmented, then the segment information was removed from the PDU Data.)
        ///     false = no segmentation
        ///     true = segmented
        /// </summary>
        public bool CanSegmentation => (FlagData[3] & 0x40) != 0;

        /// <summary>
        ///     RAW_MODE ONLY (is only effective in conjunction with RawMode true inside PduFlagDataCllCreateFlag)
        ///     For Protocol ISO 15765, a CAN frame was received with less than 8 data bytes.
        ///     false = No Error
        ///     true = Padding Error
        /// </summary>
        public bool Iso15765PaddingError => (FlagData[3] & 0x10) != 0;

        /// <summary>
        ///     TxDone indication
        ///     false = No TxDone
        ///     true = TxDone
        /// </summary>
        public bool TxIndication => (FlagData[3] & 0x08) != 0;

        /// <summary>
        ///     SAE J2610 and SAE J1850 VPW onlyBreak indication received
        ///     false = No break received
        ///     true = Break received
        /// </summary>
        public bool RxBreak => (FlagData[3] & 0x04) != 0;

        /// <summary>
        ///     Indicates the reception of the first byte of an ISO 9141 or ISO 14230 message, or first frame of an ISO 15765
        ///     multiframe message.
        ///     false = Not a start of message indication
        ///     true = First byte or frame received
        /// </summary>
        public bool StartOfMessage => (FlagData[3] & 0x02) != 0;

        /// <summary>
        ///     Receive Indication/TransmitLoopback
        ///     false = received (i.e. this message was transmitted on the bus by anothernode)
        ///     true = transmitted (i.e. this is the echo of themessage transmitted by the device)
        /// </summary>
        public bool TxMsgType => (FlagData[3] & 0x01) != 0;

        internal PduFlagDataRxFlag(byte[] flagData)
        {
            if (flagData.Length >= 4)
            {
                FlagData = flagData;
            }
        }
    }
}