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
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ISO22900.II;
using NUnit.Framework;

namespace ISO22900.II.Test
{
    public partial class IntegrationTestWithEcuCom
    {
        [Test]
        public async Task TestPduStartAndCancelComPrimitive()
        {
            CancellationTokenSource _cts;
            CancellationToken _ct;
            _cts = new CancellationTokenSource();
            _ct = _cts.Token;
            var tcsResult = new TaskCompletionSource<byte[]>(TaskCreationOptions.RunContinuationsAsynchronously);

            try
            {
                var evHandler = new Action<PduEvtData, uint, uint, uint, uint>(
                    (eventType, moduleHandle, comLogicalLinkHandle, comLogicalLinkTag, apiTag) =>
                    {
                        var tcsCallback =
                            new TaskCompletionSource<KeyValuePair<uint, uint>>(TaskCreationOptions
                                .RunContinuationsAsynchronously);
                        var pseudoCallbackTask = tcsCallback.Task;

                        pseudoCallbackTask.ContinueWith(async cbTask =>
                            {
                                if (_ct.IsCancellationRequested) return;

                                //Remove all Items from event queue to enable a new callback
                                var queue = _dPduApi.PduGetEventItem(cbTask.Result.Key, cbTask.Result.Value);
                                while (queue.TryDequeue(out var item))
                                {
                                    if (item.PduItemType == PduIt.PDU_IT_RESULT)
                                        tcsResult.SetResult(((PduEventItemResult) item).ResultData.DataBytes);

                                    if (item.PduItemType == PduIt.PDU_IT_ERROR) tcsResult.SetCanceled();
                                }
                            }, _ct,
                            TaskContinuationOptions.PreferFairness |
                            TaskContinuationOptions.RunContinuationsAsynchronously,
                            TaskScheduler.Default
                        );

                        tcsCallback.SetResult(new KeyValuePair<uint, uint>(comLogicalLinkHandle, comLogicalLinkHandle));
                    });

                _dPduApi.PduRegisterEventCallback(_moduleOne, _cll, evHandler);

                _dPduApi.PduConnect(_moduleOne, _cll);


                var maskAndPatternBytes = new MaskAndPatternBytes(new byte[] { }, new byte[] { });

                var uniqueRespIds = new UniqueRespIds(new uint[] { });

                var exResp = new PduExpectedResponseData(0, 1, maskAndPatternBytes, uniqueRespIds);

                var copControl = new PduCopCtrlData(new[] {exResp});

                var request = new byte[] {0x22, 0xF1, 0x90};


                _dPduApi.PduStartComPrimitive(_moduleOne, _cll, PduCopt.PDU_COPT_SENDRECV, request,
                    copControl, 0);
                var result = await tcsResult.Task;

                Assert.GreaterOrEqual(result.Length,20);

                _dPduApi.PduDisconnect(_moduleOne, _cll);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
            finally
            {
                _dPduApi.PduRegisterEventCallback(_moduleOne, _cll, null);
                _cts.Dispose();
            }
        }
    }
}