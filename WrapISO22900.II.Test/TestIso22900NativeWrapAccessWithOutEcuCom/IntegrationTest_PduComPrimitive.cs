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
                Assert.That(autoResetEvent.WaitOne(TimeSpan.FromMilliseconds(200)), Is.True);
                //Remove all Items from event queue to enable a new callback
                var queue = _dPduApi.PduGetEventItem(moduleHandleFromCallback, comLogicalLinkHandleFromCallback);

                Assert.That(queue.TryDequeue(out var item), Is.True);
                if (item?.PduItemType == PduIt.PDU_IT_STATUS)
                {
                    Assert.That(((PduEventItemStatus)item).PduStatus, Is.EqualTo(PduStatus.PDU_CLLST_ONLINE));
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

                Assert.That(autoResetEvent.WaitOne(TimeSpan.FromMilliseconds(200)), Is.True);
               
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
                Assert.That(isExecuting, Is.True);

                //ISO_22900_2_2017 -> If the ComPrimitiveLevel is already in the PDU_COPST_FINISHED status, this call will return success!!
                //But e.g. Softing D-PDU-API unfortunately returns a negative answer here (PDU_ERR_INVALID_HANDLE)
                //That's because there's a gap in the documentation...
                //On the one hand, the ComPrimitive should clean itself after it has reached the state "PDU_COPST_FINISHED"
                //But that would also mean that the handle is lost. 
                _dPduApi.PduCancelComPrimitive(_moduleOne, _cll, hCoP);

                Assert.That(autoResetEvent.WaitOne(TimeSpan.FromMilliseconds(200)), Is.True);
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
                Assert.That(isCanceled, Is.True);
                _dPduApi.PduDisconnect(_moduleOne, _cll);
                Assert.That(autoResetEvent.WaitOne(TimeSpan.FromMilliseconds(200)), Is.True);
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
                Assert.That(autoResetEvent.WaitOne(TimeSpan.FromMilliseconds(200)), Is.True);
                //Remove all Items from event queue to enable a new callback
                var queue = _dPduApi.PduGetEventItem(moduleHandleFromCallback, comLogicalLinkHandleFromCallback);

                Assert.That(queue.TryDequeue(out var item), Is.True);
                if (item?.PduItemType == PduIt.PDU_IT_STATUS)
                {
                    Assert.That(((PduEventItemStatus)item).PduStatus, Is.EqualTo(PduStatus.PDU_CLLST_ONLINE));
                }

                uint delay = 500;

                var copControlDelay = new PduCopCtrlData(delay);

                var hCoP = _dPduApi.PduStartComPrimitive(_moduleOne, _cll, PduCopt.PDU_COPT_DELAY, new byte[] { },
                    copControlDelay,
                    0);

                Assert.That(autoResetEvent.WaitOne(TimeSpan.FromMilliseconds(delay + 200)), Is.True);
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

                Assert.That(isFinished, Is.True);

                _dPduApi.PduDisconnect(_moduleOne, _cll);
                Assert.That(autoResetEvent.WaitOne(TimeSpan.FromMilliseconds(200)), Is.True);

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