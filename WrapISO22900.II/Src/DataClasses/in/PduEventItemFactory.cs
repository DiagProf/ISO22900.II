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
    internal abstract class PduEventItemFactory
    {
        private protected uint CopHandle;
        protected uint CopTag;
        private protected uint Timestamp;
        
        protected PduEventItem PduEventItemFromItemType(PduIt pduIt)
        {
            return pduIt switch
            {
                PduIt.PDU_IT_STATUS => CreatePduEventItemStatus(),
                PduIt.PDU_IT_RESULT => CreatePduEventItemResult(),
                PduIt.PDU_IT_ERROR => CreatePduEventItemError(),
                PduIt.PDU_IT_INFO => CreatePduEventItemInfo(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        protected abstract PduEventItem CreatePduEventItemStatus();
        
        protected abstract PduEventItem CreatePduEventItemResult();

        protected abstract PduEventItem CreatePduEventItemError();
        
        protected abstract PduEventItem CreatePduEventItemInfo();

    }
}