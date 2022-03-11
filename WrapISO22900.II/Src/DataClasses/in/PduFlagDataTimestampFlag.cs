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
    public class PduFlagDataTimestampFlag
    {
        public byte[] FlagData { get; } = new byte[4] { 0x00, 0x00, 0x00, 0x00 };

        /// <summary>
        ///     Transmit Done Timestamp Indicator. Indication that the Transmit Done Timestamp value in the PDU_RESULT_DATA
        ///     structure is valid.
        ///     false = Not Valid
        ///     true = Valid
        /// </summary>
        public bool TxMsgDoneTimestampIndicator => (FlagData[0] & 0x80) != 0;

        /// <summary>
        ///     Start Message Timestamp Indicator. Indication. Indication that the Start Message Timestamp value in the
        ///     PDU_RESULT_DATA structure is valid.
        ///     false = Not Valid
        ///     true = Valid
        /// </summary>
        public bool StartMsgTimestampIndicator => (FlagData[0] & 0x40) != 0;

        internal PduFlagDataTimestampFlag(byte[] flagData)
        {
            //Old Samtec D-PDU-API has length == 0
            //in this case we use initial value
            //to avoid later out of bounds exceptions
            if ( flagData.Length < 1 )
            {
                return;
            }

            FlagData = flagData;
        }
    }
}