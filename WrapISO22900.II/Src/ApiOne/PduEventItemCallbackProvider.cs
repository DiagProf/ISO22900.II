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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ISO22900.II
{
    internal class PduEventItemCallbackProvider
    {
        private readonly Iso22900NativeWrapAccess _nativeAccess;
        private readonly ILogger _logger = ApiLibLogging.CreateLogger<PduEventItemCallbackProvider>();

        private readonly CancellationTokenSource _cts;
        private readonly CancellationToken _ct;
        private readonly ChannelWriter<PduEventItem> _channelWriter;
        private readonly ChannelReader<PduEventItem> _channelReader;

        
        protected readonly object LockerEventDataConsumer = new();
        protected readonly QueuedLock QueuedLockDataProducer = new();
        protected readonly QueuedLock QueuedLockDataLost = new();

        private readonly ConcurrentDictionary<ulong, KeyValuePair<Action<PduEventItem>, Action<CallbackEventArgs>>> _levelCallbacks;

        internal PduEventItemCallbackProvider(Iso22900NativeWrapAccess nativeAccess)
        {
            _nativeAccess = nativeAccess;
            _levelCallbacks = new ConcurrentDictionary<ulong, KeyValuePair<Action<PduEventItem>, Action<CallbackEventArgs>>>();

            _cts = new CancellationTokenSource();
            _ct = _cts.Token;
            
            var channel = Channel.CreateUnbounded<PduEventItem>(new UnboundedChannelOptions { SingleReader = true, SingleWriter = true });
            _channelWriter = channel.Writer;
            _channelReader = channel.Reader;
        }

        public void RegisterEventDataCallback(uint moduleHandle, uint comLogicalLinkHandle,
            Action<PduEventItem> callbackEventData, Action<CallbackEventArgs> callbackDataLost)
        {
            var callbackPair = new KeyValuePair<Action<PduEventItem>, Action<CallbackEventArgs>>(callbackEventData, callbackDataLost);
            var longKey = moduleHandle * 0x1_0000_0000ul + comLogicalLinkHandle;

            _levelCallbacks.TryAdd(longKey, callbackPair);
            _nativeAccess.PduRegisterEventCallback(moduleHandle, comLogicalLinkHandle, EventCallback());
        }

        public void UnRegisterEventDataCallback(uint moduleHandle, uint comLogicalLinkHandle)
        {
            ulong longKey = moduleHandle * 0x1_0000_0000ul + comLogicalLinkHandle;
            _levelCallbacks.Remove(longKey, out _);//remove it first... the next call often fails
            _nativeAccess.PduRegisterEventCallback(moduleHandle, comLogicalLinkHandle, null);
        }
        
        private Action<PduEvtData, uint, uint, uint, uint> EventCallback()
        {
            return (eventType, moduleHandle, comLogicalLinkHandle, comLogicalLinkTag, apiTag) =>
            {
                var callbackEventArgs = new CallbackEventArgs(eventType, moduleHandle, comLogicalLinkHandle,
                    comLogicalLinkTag, apiTag);
                
                var tcsCallback = new TaskCompletionSource<CallbackEventArgs>(TaskCreationOptions.RunContinuationsAsynchronously);
                var pseudoCallbackTask = tcsCallback.Task;
                
                pseudoCallbackTask.ContinueWith(async (cbTask) =>
                    {
                        if (_ct.IsCancellationRequested) return;

                        //Console.WriteLine("Task on thread {0} finished.", Thread.CurrentThread.ManagedThreadId);
                        if (cbTask.Result.EventType == PduEvtData.PDU_EVT_DATA_AVAILABLE)
                            try
                            {
                                await EventDataProducerTask(cbTask.Result);
                                await EventDataConsumerTask();
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("Continuation in progress... swallow all exception for now");
                                _logger.LogCritical(ex, "Attention an unhandled exception was thrown. This could be due to incorrect use.");
                            }
                        else
                            try
                            {
                                await DataLostTask(cbTask.Result);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("Continuation in progress... swallow all exception for now");
                                _logger.LogCritical(ex, "Attention an unhandled exception was thrown. This could be due to incorrect use.");
                            }
                    },_ct, TaskContinuationOptions.PreferFairness | TaskContinuationOptions.RunContinuationsAsynchronously, TaskScheduler.Default
                );
                
                tcsCallback.SetResult(callbackEventArgs);
            };
        }

        private Task DataLostTask(CallbackEventArgs args)
        {
            var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            try
            {
                QueuedLockDataLost.Enter();
                var longKey = args.ModuleHandle * 0x1_0000_0000ul + args.ComLogicalLinkHandle;
                if (_levelCallbacks.TryGetValue(longKey, out var callbackPair))
                    try
                    {
                        callbackPair.Value(args); //Value is like func call for Datalost
                        tcs.SetResult();
                    }
                    catch (Exception ex)
                    {
                        //you get there when exceptions are thrown in the consumer chain.
                        //at the moment i have no better solution
                        _logger.LogCritical(ex,
                            "Attention an unhandled exception was thrown in the forwarding of data lost callback from hMod: {ModuleHandle} hCll: {ComLogicalLinkHandle}",
                            args.ModuleHandle, args.ComLogicalLinkHandle);
                        tcs.SetException(ex);
                    }
            }
            finally
            {
                QueuedLockDataLost.Exit();
            }
            return tcs.Task;
        }

        private Task EventDataProducerTask(CallbackEventArgs args)
        {
            var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            try
            {
                // we have to make sure PduGetEventItem and TryWrite needs to be synchronized
                // Otherwise it could be that the events do not run in the correct order
                QueuedLockDataProducer.Enter();
                var isCanceled = false;

                //After this method call, new callback are possible
                //In other words, multiple events will not generate multiple callback even though each is a separate
                //event item in the Event Queue.
                //The application is responsible for reading ALL events from the ComLogicalLink’s Event Queue before another callback will be generated. 
                var queue = _nativeAccess.PduGetEventItem(args.ModuleHandle, args.ComLogicalLinkHandle);
                while (queue.TryDequeue(out var item))
                {
                    //Merge event item and callback EventArgs together
                    item.EventArgs = args;
                    
                    //add to producer queue
                    if (_channelWriter.TryWrite(item)) continue;

                    //This code should not be reached.
                    isCanceled = true;
                    break;
                }

                if (!isCanceled)
                {
                    tcs.SetResult();
                }
                else
                {
                    tcs.SetCanceled(CancellationToken.None);
                }
            }
            catch (Exception e)
            {
                //Exception if e.g. PduGetEventItem makes trouble
                tcs.SetException(e);
            }
            finally
            {
                QueuedLockDataProducer.Exit();
            }

            return tcs.Task;
        }

        private Task EventDataConsumerTask()
        {
            var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            //Here it is not important which thread continues after the lock.
            //The main thing is to keep turning the wheel.
            //The native event is only used to turning the wheel or push the wheel
            //The correct order is determined by the queue
            lock (LockerEventDataConsumer)
            {
                var isOkay = true;
                while (_channelReader.TryRead(out var item))
                {
                    var longKey = item.EventArgs.ModuleHandle * 0x1_0000_0000ul + item.EventArgs.ComLogicalLinkHandle;
                    if (_levelCallbacks.TryGetValue(longKey, out var callbackPair))
                    {
                        try
                        {
                            callbackPair.Key(item); //key is like func call for EventData
                        }
                        catch (Exception ex)
                        {
                            //you get there when exceptions are thrown in the consumer chain.
                            //at the moment i have no better solution
                            _logger.LogCritical(ex, "Attention an unhandled exception was thrown in the forwarding of callback from hMod: {ModuleHandle} hCll: {ComLogicalLinkHandle}",
                                item.EventArgs.ModuleHandle, item.EventArgs.ComLogicalLinkHandle);
                            if (isOkay)
                            {
                                tcs.SetException(ex);
                                isOkay = false;
                            }
                            //break; not yet run the hole while loop
                        }
                    }
                    else
                    {
                        // Actia Core XS VCIs shows this behavior
                        _logger.LogWarning("Unusual behavior... native API calls the callback. E.g. while calling PduModulConnect. Forwarding of callback from hMod: {ModuleHandle} hCll: {ComLogicalLinkHandle}", item.EventArgs.ModuleHandle, item.EventArgs.ComLogicalLinkHandle);
                    }
                }

                if (isOkay)
                {
                    tcs.SetResult();
                }
            }
            return tcs.Task;
        }

        #region DisposeBehavior

        public void ReleaseManagedResources()
        {
            _cts.Cancel(false);
            _channelWriter.TryComplete();
            _cts.Dispose();
        }

        #endregion
    }
}
