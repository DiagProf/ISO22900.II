using System;
using System.Diagnostics;
using System.Threading;
using ISO22900.II;
using NUnit.Framework;

namespace ISO22900.II.Test
{
    public partial class IntegrationTest_SetupAndTearDown
    {
        [Test]
        public void TestPduConnectDisconnectWithComLogicalLinkLevelEvent()
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
                var firstCheckPoint = autoResetEvent.WaitOne(TimeSpan.FromMilliseconds(200));

                //Remove all Items from event queue to enable a new callback
                var queue = _dPduApi.PduGetEventItem(moduleHandleFromCallback, comLogicalLinkHandleFromCallback);

                Assert.IsTrue(queue.TryDequeue(out var item));
                if (item.PduItemType == PduIt.PDU_IT_STATUS)
                {
                    Assert.AreEqual(((PduEventItemStatus)item).PduStatus, PduStatus.PDU_CLLST_ONLINE);
                }

                _dPduApi.PduDisconnect(_moduleOne, _cll);
                var secondCheckPoint = autoResetEvent.WaitOne(TimeSpan.FromMilliseconds(200));

                //Remove all Items from event queue to enable a new callback
                queue = _dPduApi.PduGetEventItem(moduleHandleFromCallback, comLogicalLinkHandleFromCallback);

                Assert.IsTrue(queue.TryDequeue(out item));
                if (item.PduItemType == PduIt.PDU_IT_STATUS)
                {
                    Assert.AreEqual(((PduEventItemStatus)item).PduStatus, PduStatus.PDU_CLLST_OFFLINE);
                }
                

                Assert.IsTrue(firstCheckPoint && secondCheckPoint);
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

        [Test]
        public void TestPduConnectDisconnectWithModuleLevelEvent()
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

                _dPduApi.PduRegisterEventCallback(_moduleOne, PduConst.PDU_HANDLE_UNDEF, evHandler);

                _dPduApi.PduConnect(_moduleOne, _cll);
                var firstCheckPoint = autoResetEvent.WaitOne(TimeSpan.FromMilliseconds(200));

                //Remove all Items from event queue to enable a new callback
                _dPduApi.PduGetEventItem(moduleHandleFromCallback, comLogicalLinkHandleFromCallback);

                _dPduApi.PduDisconnect(_moduleOne, _cll);
                var secondCheckPoint = autoResetEvent.WaitOne(TimeSpan.FromMilliseconds(200));

                Assert.IsTrue(firstCheckPoint && secondCheckPoint);
            }
            catch
            {
                Assert.Fail();
            }
            finally
            {
                _dPduApi.PduRegisterEventCallback(_moduleOne, PduConst.PDU_HANDLE_UNDEF, null);
                autoResetEvent.Dispose();
            }
        }


        [Test]
        public void TestPduConnectDisconnectWithSysLevelEvent()
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

                _dPduApi.PduRegisterEventCallback(PduConst.PDU_HANDLE_UNDEF, PduConst.PDU_HANDLE_UNDEF, evHandler);

                _dPduApi.PduConnect(_moduleOne, _cll);
                var firstCheckPoint = autoResetEvent.WaitOne(TimeSpan.FromMilliseconds(200));

                //Remove all Items from event queue to enable a new callback
                _dPduApi.PduGetEventItem(moduleHandleFromCallback, comLogicalLinkHandleFromCallback);

                _dPduApi.PduDisconnect(_moduleOne, _cll);
                var secondCheckPoint = autoResetEvent.WaitOne(TimeSpan.FromMilliseconds(200));

                Assert.IsTrue(firstCheckPoint && secondCheckPoint);
            }
            catch
            {
                Assert.Fail();
            }
            finally
            {
                _dPduApi.PduRegisterEventCallback(PduConst.PDU_HANDLE_UNDEF, PduConst.PDU_HANDLE_UNDEF, null);
                autoResetEvent.Dispose();
            }
        }
    }
}