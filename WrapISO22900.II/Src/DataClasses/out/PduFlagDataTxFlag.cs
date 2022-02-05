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
    public class PduFlagDataTxFlag
    {
        public byte[] FlagData { get; init; } = new byte[4] {0x00, 0x00, 0x00, 0x00};

        public void SuppressPositiveResponse(bool flag)
        {
            if (flag)
                FlagData[0] |= 0x40;
            else
                FlagData[0] &= 0xBF;
        }

        public void EnableExtraInfo(bool flag)
        {
            if (flag)
                FlagData[0] |= 0x20;
            else
                FlagData[0] &= 0xDF;
        }

        public void WaitP3MinOnly(bool flag)
        {
            if (flag)
                FlagData[2] |= 0x02;
            else
                FlagData[2] &= 0xFD;
        }

        public void Can29Bit(bool flag)
        {
            if (flag)
                FlagData[2] |= 0x01;
            else
                FlagData[2] &= 0xFE;
        }

        public void Iso15765AddrType(bool flag)
        {
            if (flag)
                FlagData[3] |= 0x80;
            else
                FlagData[3] &= 0x7F;
        }

        public void Iso15765FramePad(bool flag)
        {
            if (flag)
                FlagData[3] |= 0x40;
            else
                FlagData[3] &= 0xBF;
        }

        //public bool Iso15765FramePads
        //{
        //    get => (FlagData[3] & 0xBF) == 0x40;
        //    set
        //    {
        //        if (value)
        //            FlagData[3] |= 0x40;
        //        else
        //            FlagData[3] &= 0xBF;
        //    }
        //} 
    }
}