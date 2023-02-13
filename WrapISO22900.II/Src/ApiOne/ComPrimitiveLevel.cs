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
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ISO22900.II
{
    public class ComPrimitiveLevel : ManagedDisposable
    {
        private readonly ILogger _logger = ApiLibLogging.CreateLogger<ComPrimitiveLevel>();
        private readonly ChannelReader<PduEventItem> _channelReader;
        private readonly ComLogicalLinkLevel _cll;
        private bool _needsToBeCanceled;
        private bool _comPrimitiveLiveIsOver;
        protected internal uint ComPrimitiveHandle { get; }

        internal ComPrimitiveLevel(ComLogicalLinkLevel comLogicalLinkLevel, uint comPrimitiveHandle, ChannelReader<PduEventItem> channelReader, PduCopt pduCopType, byte[] copData, PduCopCtrlData copCtrlData, uint copTag)
        {
            _cll = comLogicalLinkLevel;
            ComPrimitiveHandle = comPrimitiveHandle;
            _channelReader= channelReader;

            if ((copCtrlData.NumSendCycles < 0) || copCtrlData.NumReceiveCycles == -1)
            {
                _needsToBeCanceled = true;
            }
        }

        public ComPrimitiveResult WaitForCopResult(CancellationToken ct)
        {
            try
            {
                return WaitForCopResultAsync(ct).Result;
            }
            catch ( AggregateException e )
            {
                foreach ( var ex in e.InnerExceptions )
                {
                    if ( ex is not OperationCanceledException )
                    {
                        throw ex;
                    }
                }
            }

            return new ComPrimitiveResult(new Queue<PduEventItem>());
        }

        public async Task<ComPrimitiveResult> WaitForCopResultAsync(CancellationToken ct)
        {
            Queue<PduEventItem> eventItemResults = new();
            var copResult = new ComPrimitiveResult(eventItemResults);
            try
            {
                while (await _channelReader.WaitToReadAsync(ct).ConfigureAwait(false))
                {
                    if (_channelReader.TryRead(out var item))
                    {
                        if ( item.PduItemType == PduIt.PDU_IT_STATUS )
                        {
                            //all status infos are not put into the queue.
                            if ( ((PduEventItemStatus)item).PduStatus == PduStatus.PDU_COPST_FINISHED ||
                                 ((PduEventItemStatus)item).PduStatus == PduStatus.PDU_COPST_CANCELLED )
                            {
                                _comPrimitiveLiveIsOver = true;
                                _needsToBeCanceled = false;
                                _cll.CopChannels.TryRemove(ComPrimitiveHandle, out var channel);

                                break;
                            }

                            continue;
                        }

                        eventItemResults.Enqueue(item);

                        //if the cop is set up in such a way that there can be an infinite number of results
                        //then each individual result is returned
                        if ( _needsToBeCanceled /*&& (item.PduItemType == PduIt.PDU_IT_RESULT || item.PduItemType == PduIt.PDU_IT_ERROR)*/ )
                        {
                            break;
                        }
                    }
                }
            }
            catch ( OperationCanceledException e)
            {
                _logger.LogError(e,"ComPrimitiveQueue reading was canceled.");
            }
            
            return copResult;
        }

        public PduExStatusData Status()
        {
            return _cll.Vci.SysLevel.Nwa.PduGetStatus(_cll.ModuleHandle, _cll.ComLogicalLinkHandle, ComPrimitiveHandle);
        }

        public void Cancel()
        {
            if ( _needsToBeCanceled )
            {
                _cll.Vci.SysLevel.Nwa.PduCancelComPrimitive(_cll.ModuleHandle, _cll.ComLogicalLinkHandle, ComPrimitiveHandle);
                _needsToBeCanceled = false;
                Cancel();
            }
            else
            {
                //We need this loop because it could be that nobody called WaitForCopResult..
                while ( _channelReader.TryRead(out var item) )
                {
                    if ( item.PduItemType == PduIt.PDU_IT_STATUS )
                    {
                        //all status infos are not put into the queue.
                        if ( ((PduEventItemStatus)item).PduStatus == PduStatus.PDU_COPST_FINISHED ||
                             ((PduEventItemStatus)item).PduStatus == PduStatus.PDU_COPST_CANCELLED )
                        {
                            _comPrimitiveLiveIsOver = true;
                            _cll.CopChannels.TryRemove(ComPrimitiveHandle, out var channel);
                        }
                    }
                }

                if ( !_comPrimitiveLiveIsOver )
                {
                    try
                    {
                        _cll.Vci.SysLevel.Nwa.PduCancelComPrimitive(_cll.ModuleHandle, _cll.ComLogicalLinkHandle, ComPrimitiveHandle);
                        Cancel();
                    }
                    catch ( Iso22900IIException )
                    {
                        //eat;
                    }
                }
            }
        }


        #region DisposeBehavior

        protected override void FreeUnmanagedResources()
        {
            Cancel();
        }

        protected override void FreeManagedResources()
        {
            _cll.Disposing -= Dispose;
        }

        /// <summary>
        ///     When the finalizer thread gets around to running, it runs all the destructors of the object.
        ///     Destructors will run in order from most derived to least derived.
        /// </summary>
        ~ComPrimitiveLevel()
        {
            try
            {
                Dispose(false);
            }
            catch ( Exception exception )
            {
                //This is bad.
                //At least attempt to get a log message out if this happens.
                try
                {
                    var builder = new StringBuilder();
                    builder.AppendLine($"{DateTime.Now} - Exception in type '{GetType()}'");
                    builder.Append(exception.StackTrace);
                    builder.Append(exception.Message);
                    var innerException = exception.InnerException;
                    while ( innerException != null )
                    {
                        builder.Append(innerException.Message);
                        innerException = innerException.InnerException;
                    }

                    File.AppendAllText(@"FinalizerException.txt", builder.ToString());
                }
                catch
                {
                } //Swallow any exceptions inside a finalizer
            }
        }

        #endregion
    }
}
