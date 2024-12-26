using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ISO22900.II;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace ISO22900.II.Test
{
    /// <summary>
    /// This is more of an integration test because a real D-PDU API with a real VCI is required
    /// </summary>
    public class IntegrationTestCheckTags
    {
        private Iso22900NativeWrapAccess _dPduApi;
        private uint _moduleOne = PduConst.PDU_HANDLE_UNDEF;
        private uint _cll = PduConst.PDU_HANDLE_UNDEF;
        private Dictionary<uint, string> copDescription;
        private Dictionary<uint, string> cllDescription;
        private Dictionary<uint, string> apiDescription;

        [SetUp]
        public void Setup()
        {
            //You can find your install path like this ...
            //1. look at the registry and find your root file path (I mean you as human being!! Function like "Registry.LocalMachine.OpenSubKey" e.g. abstracts 32 or 64bit registry access of course)
            //   If you have 64bit APP look under Computer\HKEY_LOCAL_MACHINE\SOFTWARE\D-PDU API -> Root File
            //   If you have 32bit APP look under Computer\HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\D-PDU API -> Root File
            //2. Open the root file and copy the entry under the tag <LIBRARY_FILE>. This is the path to the native dll
            //To me, Softing feels like the best horse in the race.

            var nativeLibraryPath = "";

            //Never ever do the following if-else to figure out it's 64bit or 32bit in a real application!!
            //DiagPduApiHelper does the job it's part of the behavior as 22900-2 describes
            if (Environment.Is64BitProcess)
            {
                //Attention, this path is only VALID for x86
                nativeLibraryPath = @"C:\Program Files\Softing\D-PDU API\11.30.058\VeCom\PDUAPI_SoftingAG_11.30.058.dll";
            }
            else
            {
                nativeLibraryPath = @"C:\Program Files (x86)\Softing\D-PDU API\11.30.058\VeCom\PDUAPI_SoftingAG_11.30.058.dll";
            }

            _dPduApi = new Iso22900NativeWrapAccess(nativeLibraryPath);


            apiDescription = new Dictionary<uint, string>()
                {{1, nameof(IntegrationTest_SetupAndTearDown)}, {2, "No name yet"}};

            cllDescription = new Dictionary<uint, string>()
                {{1, "DiagCom"}, {2, "CAN-Raw Trace"}};

            copDescription = new Dictionary<uint, string>()
                {{1, "DelayCop"}, {2, "ReadVin"}};


            _dPduApi.PduConstruct(apiDescription.First(x => x.Value == nameof(IntegrationTest_SetupAndTearDown)).Key);

            var modules = _dPduApi.PduGetModuleIds();
            foreach (var module in modules)
            {
                if (module.ModuleStatus != PduStatus.PDU_MODST_AVAIL || !module.VendorModuleName.Contains("EDIC"))
                    continue;

                _moduleOne = module.ModuleHandle;
                break;
            }

            Assert.That(_moduleOne, Is.Not.EqualTo(PduConst.PDU_HANDLE_UNDEF));

            _dPduApi.PduModuleConnect(_moduleOne);

            //All further tests depend partly on this protocol stack settings. So don't change it
            var busTypId = _dPduApi.PduGetObjectId(PduObjt.PDU_OBJT_BUSTYPE, "ISO_11898_2_DWCAN");
            var protocolTypId = _dPduApi.PduGetObjectId(PduObjt.PDU_OBJT_PROTOCOL, "ISO_15765_3_on_ISO_15765_2");
            var pinTypIdHi = _dPduApi.PduGetObjectId(PduObjt.PDU_OBJT_PINTYPE, "HI");
            var pinTypIdLow = _dPduApi.PduGetObjectId(PduObjt.PDU_OBJT_PINTYPE, "LOW");
            var pduRscData = new PduResourceData(busTypId, protocolTypId,
                new Dictionary<uint, uint> {{6, pinTypIdHi}, {14, pinTypIdLow}}.ToList());

            _cll = _dPduApi.PduCreateComLogicalLink(_moduleOne, pduRscData, PduConst.PDU_ID_UNDEF,
                cllDescription.First(x => x.Value == "DiagCom").Key,
                new PduFlagDataCllCreateFlag());

            Assert.That(_cll, Is.Not.EqualTo(PduConst.PDU_HANDLE_UNDEF));
        }


        [TearDown]
        public void TearDown()
        {
            //just to be sure that no callback handler is installed anymore
            _dPduApi.PduRegisterEventCallback(_moduleOne, _cll, null);

            _dPduApi.PduDestroyComLogicalLink(_moduleOne, _cll);

            //just to be sure that no callback handler is installed anymore
            _dPduApi.PduRegisterEventCallback(_moduleOne, PduConst.PDU_HANDLE_UNDEF, null);

            _dPduApi.PduModuleDisconnect(_moduleOne);

            //just to be sure that no callback handler is installed anymore
            _dPduApi.PduRegisterEventCallback(PduConst.PDU_HANDLE_UNDEF, PduConst.PDU_HANDLE_UNDEF, null);

            _dPduApi.PduDestruct();
            _dPduApi.Dispose();
        }
    


        [Test]
        public void TestPduStartComPrimitiveOfTypeDelay()
        {
            var autoResetEvent = new AutoResetEvent(false);
            var moduleHandleFromCallback = PduConst.PDU_HANDLE_UNDEF;
            var comLogicalLinkHandleFromCallback = PduConst.PDU_HANDLE_UNDEF;
            bool isTagApiOkay = false;
            bool isTagCllOkay = false;
            bool isTagCopOkay = false;
            try
            {
                var evHandler = new Action<PduEvtData, uint, uint, uint, uint>((PduEvtData eventType, uint moduleHandle,
                    uint comLogicalLinkHandle,
                    uint comLogicalLinkTag, uint apiTag) =>
                {
                    moduleHandleFromCallback = moduleHandle;
                    comLogicalLinkHandleFromCallback = comLogicalLinkHandle;
                    if (apiDescription[apiTag] == nameof(IntegrationTest_SetupAndTearDown))
                        isTagApiOkay = true;
                    if (cllDescription[comLogicalLinkTag] == "DiagCom")
                        isTagCllOkay = true;


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
                    copDescription.First(x => x.Value == "DelayCop").Key);

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
                            if (copDescription[item.CopTag] == "DelayCop")
                                isTagCopOkay = true;
                        }
                    }
                }

                Assert.That(isFinished, Is.True);

                _dPduApi.PduDisconnect(_moduleOne, _cll);
                Assert.That(autoResetEvent.WaitOne(TimeSpan.FromMilliseconds(200)), Is.True);
                Assert.That(isTagApiOkay && isTagCllOkay && isTagCopOkay, Is.True);
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
