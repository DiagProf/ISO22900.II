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

using System;

namespace ISO22900.II
{
    public class PduFlagDataTxFlag : ICloneable
    {
        public byte[] FlagData { get; init; } = new byte[4] {0x00, 0x00, 0x00, 0x00};

        /// <summary>
        /// ISO 15765-3/ISO14229-3 Suppress Positive Response
        /// This is also a part of ODX
        /// false = Not Enabled (default)
        /// true = Enabled
        /// </summary>
        public bool SuppressPositiveResponse
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
        ///     Enable adding header and footer information into the result data(see 10.1.4.11.4).
        ///     Extra information can be used for ECU response debugging
        ///     false = Not Enabled (default)
        ///     true = Enabled
        /// </summary>
        public bool EnableExtraInfo
        {
            get => (FlagData[0] & 0x20) != 0;
            set
            {
                if (value)
                {
                    FlagData[0] |= 0x20;
                }
                else
                {
                    FlagData[0] &= 0xDF;
                }
            }
        }

        /// <summary>
        ///     is only effective in conjunction with RawMode true inside PduFlagDataCllCreateFlag
        ///     false = Interface message timing as specified in ISO 14230 (default)
        ///     true = After a response is received for a physical request, the wait time shall be reduced to P3_MIN.
        ///     Modified message timing for ISO 14230.
        ///     Used to decrease programming time if application knows only one response will be received.
        ///     Does not affect timing on responses to functional requests.
        /// </summary>
        public bool WaitP3MinOnly
        {
            get => (FlagData[2] & 0x02) != 0;
            set
            {
                if (value)
                {
                    FlagData[2] |= 0x02;
                }
                else
                {
                    FlagData[2] &= 0xFD;
                }
            }
        }

        /// <summary>
        ///     is only effective in conjunction with RawMode true inside PduFlagDataCllCreateFlag
        ///     false = 11-bit (default)
        ///     true = 29-bit
        ///     CAN ID type for ISO 11898, SAE J1939 and ISO 15765.
        ///     CAN ID is contained in the first 4 bytes of the PDU Data.
        /// </summary>
        public bool Can29Bit
        {
            get => (FlagData[2] & 0x01) != 0;
            set
            {
                if (value)
                {
                    FlagData[2] |= 0x01;
                }
                else
                {
                    FlagData[2] &= 0xFE;
                }
            }
        }

        /// <summary>
        ///     is only effective in conjunction with RawMode true inside PduFlagDataCllCreateFlag
        ///     false = no extended address (default)
        ///     true = extended addressing is used
        ///     ISO 15765-2 Addressing Method CAN Extended Address is contained in the byte following the CAN ID in the PDU Data.
        /// </summary>
        public bool Iso15765AddrType
        {
            get => (FlagData[3] & 0x80) != 0;
            set
            {
                if (value)
                {
                    FlagData[3] |= 0x80;
                }
                else
                {
                    FlagData[3] &= 0x7F;
                }
            }
        }

        /// <summary>
        ///     is only effective in conjunction with RawMode true inside PduFlagDataCllCreateFlag
        ///     false = no padding (default)
        ///     true = pad all messages to a full CAN frame using the value in the ComParam CP_CanFillerByte
        ///     ISO 15765-2 Frame Padding
        /// </summary>
        public bool Iso15765FramePad
        {
            get => (FlagData[3] & 0x40) != 0;
            set
            {
                if ( value )
                {
                    FlagData[3] |= 0x40;
                }
                else
                {
                    FlagData[3] &= 0xBF;
                }
            }
        }

        public PduFlagDataTxFlag()
        {

        }

        public PduFlagDataTxFlag(byte[] flagData)
        {
            if ( flagData.Length >= 4 )
            {
                FlagData = flagData;
            }
        }


        public object Clone()
        {
            return new PduFlagDataTxFlag(FlagData);
        }
    }
}