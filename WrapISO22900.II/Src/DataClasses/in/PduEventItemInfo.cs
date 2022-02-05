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
    /// Asynchronous event information notification for PDU_IT_INFO Item
    /// </summary>
    public class PduEventItemInfo : PduEventItem
    {
        /// <summary>
        /// Information code. See D.15 - Information event values 
        /// </summary>
        public PduInfo InfoCode { get; }

        /// <summary>
        /// Optional additional information code. Text translation via MDF file.
        /// I think...not sure 0 indicates no extra information data.
        /// </summary>
        public uint ExtraInfoData { get; }

        internal PduEventItemInfo(uint copHandle, uint copTag, uint timestamp, PduInfo infoCode,
            uint extraInfoData) : base(PduIt.PDU_IT_INFO, copHandle, copTag, timestamp)
        {
            InfoCode = infoCode;
            ExtraInfoData = extraInfoData;
        }
    }
}