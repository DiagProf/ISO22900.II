using System;
using System.Threading;
using ISO22900.II;
using NUnit.Framework;

namespace ISO22900.II.Test
{
    public partial class IntegrationTest_SetupAndTearDown
    {
        [Test]
        public void TestPduStartAndCancelComPrimitive()
        {
            var autoResetEvent = new AutoResetEvent(false);
            var moduleHandleFromCallback = PduConst.PDU_HANDLE_UNDEF;
            var comLogicalLinkHandleFromCallback = PduConst.PDU_HANDLE_UNDEF;
            try
            {
                var evHandler = new Action<PduEvtData, uint, uint, uint, uint>((PduEvtData eventType, uint moduleHandle, uint comLogicalLinkHandle,
                    uint comLogicalLinkTag, uint apiTag) =>
                {
                    moduleHandleFromCallback = moduleHandle;
                    comLogicalLinkHandleFromCallback = comLogicalLinkHandle;

                    autoResetEvent.Set();
                    
                });
                _dPduApi.PduRegisterEventCallback(_moduleOne, _cll, evHandler);
                
                _dPduApi.PduConnect(_moduleOne, _cll);
                Assert.IsTrue(autoResetEvent.WaitOne(TimeSpan.FromMilliseconds(200)));
                //Remove all Items from event queue to enable a new callback
                var queue = _dPduApi.PduGetEventItem(moduleHandleFromCallback, comLogicalLinkHandleFromCallback);
                
                Assert.IsTrue(queue.TryDequeue(out var item));
                if (item.PduItemType == PduIt.PDU_IT_STATUS)
                {
                    Assert.AreEqual(((PduEventItemStatus)item).PduStatus, PduStatus.PDU_CLLST_ONLINE);
                }    


                var maskAndPatternBytes = new MaskAndPatternBytes(new byte[] { }, new byte[] { });

                var uniqueRespIds = new UniqueRespIds(new uint[] { });

                var exResp = new PduExpectedResponseData(0, 1, maskAndPatternBytes, uniqueRespIds);

                var copControl = new PduCopCtrlData
                {
                    NumSendCycles = 0,  //no Request
                    NumReceiveCycles = -1,  //more than one Response
                    TempParamUpdate = 0,
                    Time = 0,
                    TxFlag = new PduFlagDataTxFlag(),
                    PduExpectedResponseDatas = new[] {exResp}
                };

                var request = new byte[] {};

                var hCoP = _dPduApi.PduStartComPrimitive(_moduleOne, _cll, PduCopt.PDU_COPT_SENDRECV, request,
                    copControl, 0);
                    
                Assert.IsTrue(autoResetEvent.WaitOne(TimeSpan.FromMilliseconds(200)));
               
                //Remove all Items from event queue to enable a new callback
                queue = _dPduApi.PduGetEventItem(moduleHandleFromCallback, comLogicalLinkHandleFromCallback);
                bool isExecuting = false;
                while (queue.TryDequeue(out item))
                {
                    if (item.PduItemType == PduIt.PDU_IT_STATUS)
                    {
                        if (((PduEventItemStatus)item).PduStatus == PduStatus.PDU_COPST_EXECUTING)
                        {
                            isExecuting = true;
                        }
                    }
                }
                Assert.IsTrue(isExecuting);

                //ISO_22900_2_2017 -> If the ComPrimitiveLevel is already in the PDU_COPST_FINISHED status, this call will return success!!
                //But e.g. Softing D-PDU-API unfortunately returns a negative answer here (PDU_ERR_INVALID_HANDLE)
                //That's because there's a gap in the documentation...
                //On the one hand, the ComPrimitive should clean itself after it has reached the state "PDU_COPST_FINISHED"
                //But that would also mean that the handle is lost. 
                _dPduApi.PduCancelComPrimitive(_moduleOne, _cll, hCoP);

                Assert.IsTrue(autoResetEvent.WaitOne(TimeSpan.FromMilliseconds(200)));
                //Remove all Items from event queue to enable a new callback
                queue = _dPduApi.PduGetEventItem(moduleHandleFromCallback, comLogicalLinkHandleFromCallback);
                bool isCanceled = false;
                while (queue.TryDequeue(out item))
                {
                    if (item.PduItemType == PduIt.PDU_IT_STATUS)
                    {
                        if (((PduEventItemStatus) item).PduStatus == PduStatus.PDU_COPST_CANCELLED)
                        {
                            isCanceled = true;
                        }
                    }
                }
                Assert.IsTrue(isCanceled);
                _dPduApi.PduDisconnect(_moduleOne, _cll);
                Assert.IsTrue(autoResetEvent.WaitOne(TimeSpan.FromMilliseconds(200)));
            }
            catch (Exception)
            {
                Assert.Fail();
            }
            finally
            {
                _dPduApi.PduRegisterEventCallback(_moduleOne, _cll, null);
                autoResetEvent.Dispose();
            }
        }

        [Test]
        public void TestPduStartComPrimitiveOfTypeDelay()
        {
            var autoResetEvent = new AutoResetEvent(false);
            var moduleHandleFromCallback = PduConst.PDU_HANDLE_UNDEF;
            var comLogicalLinkHandleFromCallback = PduConst.PDU_HANDLE_UNDEF;
            try
            {
                var evHandler = new Action<PduEvtData, uint, uint, uint, uint>((PduEvtData eventType, uint moduleHandle,
                    uint comLogicalLinkHandle,
                    uint comLogicalLinkTag, uint apiTag) =>
                {
                    moduleHandleFromCallback = moduleHandle;
                    comLogicalLinkHandleFromCallback = comLogicalLinkHandle;

                    autoResetEvent.Set();

                });
                _dPduApi.PduRegisterEventCallback(_moduleOne, _cll, evHandler);
                
                _dPduApi.PduConnect(_moduleOne, _cll);
                Assert.IsTrue(autoResetEvent.WaitOne(TimeSpan.FromMilliseconds(200)));
                //Remove all Items from event queue to enable a new callback
                var queue = _dPduApi.PduGetEventItem(moduleHandleFromCallback, comLogicalLinkHandleFromCallback);

                Assert.IsTrue(queue.TryDequeue(out var item));
                if (item.PduItemType == PduIt.PDU_IT_STATUS)
                {
                    Assert.AreEqual(((PduEventItemStatus) item).PduStatus, PduStatus.PDU_CLLST_ONLINE);
                }

                uint delay = 500;

                var copControlDelay = new PduCopCtrlData(delay);

                var hCoP = _dPduApi.PduStartComPrimitive(_moduleOne, _cll, PduCopt.PDU_COPT_DELAY, new byte[] { },
                    copControlDelay,
                    0);

                Assert.IsTrue(autoResetEvent.WaitOne(TimeSpan.FromMilliseconds(delay + 200)));
                //Remove all Items from event queue to enable a new callback
                queue = _dPduApi.PduGetEventItem(moduleHandleFromCallback, comLogicalLinkHandleFromCallback);
                bool isFinished = false;
                while (queue.TryDequeue(out item))
                {
                    if (item.PduItemType == PduIt.PDU_IT_STATUS)
                    {
                        if (((PduEventItemStatus) item).PduStatus == PduStatus.PDU_COPST_FINISHED)
                        {
                            isFinished = true;
                        }
                    }
                }

                Assert.IsTrue(isFinished);

                _dPduApi.PduDisconnect(_moduleOne, _cll);
                Assert.IsTrue(autoResetEvent.WaitOne(TimeSpan.FromMilliseconds(200)));

            }
            catch
            {
                Assert.Fail();
            }
            finally
            {
                _dPduApi.PduRegisterEventCallback(_moduleOne, _cll, null);
                autoResetEvent.Dispose();
            }
        }
    }
}