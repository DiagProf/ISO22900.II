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
        public byte[] FlagData { get; internal set; } = new byte[4] {0x00, 0x00, 0x00, 0x00};

        public bool RemoteFrame()
        {
            return (FlagData[0] & 0x80) != 0;
        }

        public bool SpeedChangeEvent()
        {
            return (FlagData[1] & 0x04) != 0;
        }

        public bool EcuTimingChange()
        {
            return (FlagData[1] & 0x02) != 0;
        }

        public bool SingleWireCanHvRx()
        {
            return(FlagData[1] & 0x01) != 0;
        }

        public bool Can29BitId()
        {
            return (FlagData[2] & 0x01) != 0;
        }

        public bool Iso15765AddrType()
        {
            return (FlagData[3] & 0x80) != 0;
        }

        public bool CanSegmentation()
        {
            return (FlagData[3] & 0x40) != 0;
        }

        public bool Iso15765PaddingError()
        {
            return (FlagData[3] & 0x10) != 0;
        }

        public bool TxIndication()
        {
            return (FlagData[3] & 0x08) != 0;
        }

        public bool RxBreak()
        {
            return (FlagData[3] & 0x04) != 0;
        }

        public bool StartOfMessage()
        {
            return (FlagData[3] & 0x02) != 0;
        }

        public bool TxMsgType()
        {
            return (FlagData[3] & 0x01) != 0;
        }

        internal PduFlagDataRxFlag(byte[] flagData)
        {
            FlagData = flagData;
        }
    }
}