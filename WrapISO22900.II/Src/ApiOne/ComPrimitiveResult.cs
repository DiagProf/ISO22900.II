#region License

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

#endregion

using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace ISO22900.II
{
    /// <summary>
    ///     Class for convenient Cop-Result access
    /// </summary>
    public class ComPrimitiveResult
    {
        private readonly ILogger _logger = ApiLibLogging.CreateLogger<ComPrimitiveResult>();
        public Queue<PduEventItem> RawQueue { get; }

        public ComPrimitiveResult(Queue<PduEventItem> rawQueue)
        {
            RawQueue = rawQueue;
        }

        /// <summary>
        ///     under development :-)
        ///     don't build on it
        /// </summary>
        /// <returns></returns>
        public uint ResponseTime()
        {
            var all = RawQueue.Where(e => e.PduItemType == PduIt.PDU_IT_RESULT).Cast<PduEventItemResult>().ToList();
            if ( all.Count > 0 )
            {
                if ( all.First().ResultData.TimestampFlags.TxMsgDoneTimestampIndicator &
                     all.Last().ResultData.TimestampFlags.StartMsgTimestampIndicator )
                {
                    return all.Last().ResultData.StartMsgTimestamp - all.First().ResultData.TxMsgDoneTimestamp;
                }

                if ( all.First().ResultData.TimestampFlags.TxMsgDoneTimestampIndicator &
                     !all.Last().ResultData.TimestampFlags.StartMsgTimestampIndicator )
                {
                    return all.Last().Timestamp - all.First().ResultData.TxMsgDoneTimestamp;
                }
            }

            return 0;
        }


        public List<PduEventItemInfo> PduEventItemInfos()
        {
            return RawQueue.Where(e => e.PduItemType == PduIt.PDU_IT_INFO).Cast<PduEventItemInfo>().ToList();
        }

        public List<PduEventItemResult> PduEventItemResults()
        {
            return RawQueue.Where(e => e.PduItemType == PduIt.PDU_IT_RESULT).Cast<PduEventItemResult>().ToList();
        }

        public List<byte[]> DataMsgQueue()
        {
            List<byte[]> dataMsglist = new();
            var resultItems = RawQueue.Where(e => e.PduItemType == PduIt.PDU_IT_RESULT).Cast<PduEventItemResult>().ToList();
            if ( resultItems.Any() )
            {
                //List<PduEventItemResult> eventItemsResult = resultItems.Cast<PduEventItemResult>().ToList();
                foreach ( var result in resultItems )
                {
                    dataMsglist.Add(result.ResultData.DataBytes);
                }
            }

            return dataMsglist;
        }

        public List<PduEventItemError> PduEventItemErrors()
        {
            return RawQueue.Where(e => e.PduItemType == PduIt.PDU_IT_ERROR).Cast<PduEventItemError>().ToList();
        }

    }
}
