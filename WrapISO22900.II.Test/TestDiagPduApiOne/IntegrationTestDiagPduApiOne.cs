using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ISO22900.II;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace ISO22900.II.Test
{
    /// <summary>
    /// This is more of an integration test because a real D-PDU API with a real VCI is required
    /// </summary>
    public class IntegrationTestDiagPduApiOne
    {
        private string nativeLibraryPath;

        [SetUp]
        public void Setup()
        {
            //You can find your install path like this ...
            //1. look at the registry and find your root file path (I mean you as human being!! Function like "Registry.LocalMachine.OpenSubKey" e.g. abstracts 32 or 64bit registry access of course)
            //   If you have 64bit APP look under Computer\HKEY_LOCAL_MACHINE\SOFTWARE\D-PDU API -> Root File
            //   If you have 32bit APP look under Computer\HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\D-PDU API -> Root File
            //2. Open the root file and copy the entry under the tag <LIBRARY_FILE>. This is the path to the native dll
            
            //To me, Softing feels like the best horse in the race.

            nativeLibraryPath = "";

            //Never ever do the following if-else to figure out it's 64bit or 32bit in a real application!!
            //DiagPduApiHelper does the job it's part of the behavior as 22900-2 describes
            if (Environment.Is64BitProcess)
            {
                //Attention, this path is only VALID for x86
                nativeLibraryPath = @"C:\Program Files\Softing\D-PDU API\11.30.030\VeCom\PDUAPI_SoftingAG_11.30.030.dll";
            }
            else
            {
                nativeLibraryPath = @"C:\Program Files (x86)\Softing\D-PDU API\11.30.030\VeCom\PDUAPI_SoftingAG_11.30.030.dll";
            }

        }


        [TearDown]
        public void TearDown()
        {

        }

        [Test]
        public async Task UseSpecFuncForDispose()
        {
            //out side only for test
         
            Module vci = null;
            Module vci2 = null;
            ComLogicalLinkLevel cll;

            var task1 = Task.Run(() =>
            {
                vci = DiagPduApiOneFactory.GetVci(nativeLibraryPath);
                for(var i = 0; i < 1000; i++)
                {
                    if (vci.IsIgnitionOn(0) == false)
                    {
                        break;
                    }
                }

            });

            var task2 = Task.Run(() =>
            {
                vci2 = DiagPduApiOneFactory.GetVci(nativeLibraryPath);
                for (var i = 0; i < 1500; i++)
                {
                    var mvolt = vci2.MeasureBatteryVoltage();
                    if (mvolt != 12000)
                    {
                        mvolt = mvolt;
                        break;
                    }
                }

                
            });
            await Task.WhenAll(new Task[2] { task1, task2 });
            vci.Disconnect();
            vci2.Disconnect();
            //vci = DiagPduApiOneFactory.GetVci(vciName, nativeLibraryPath, NullLogger.Instance);
            //vci2 = DiagPduApiOneFactory.GetVci(vciName, nativeLibraryPath, NullLogger.Instance);
            //vci.Disconnect();
            //Assert.IsFalse(vci.IsDisposed); 
            //vci2.Status();
            Assert.IsTrue(vci2.IsDisposed);

            //vci2.Disconnect();

            //Assert.IsTrue(vci. .oneSysLevel.IsDisposed);
        }


        [Test]
        public async Task UseUsingForDispose()
        {
            
            //out side only for test
            DiagPduApiOneSysLevel oneSysLevel;
            Module vci;
            ComLogicalLink cll;

            using (oneSysLevel = DiagPduApiOneFactory.GetApi(nativeLibraryPath))
            using (vci = oneSysLevel.ConnectVci())
            using (cll = vci.OpenComLogicalLink("ISO_11898_2_DWCAN", "ISO_15765_3_on_ISO_15765_2",
                new Dictionary<uint, string> { { 6, "HI" }, { 14, "LOW" } }.ToList()))
            {
                cll.SetComParamValueViaGet("CP_P2Max", 900000);
                cll.Connect();
                var cop = cll.StartCop(PduCopt.PDU_COPT_SENDRECV, 1, 1, new byte[] { 0x22, 0xF1, 0x90 });
                var copResult = await cop.WaitForCopResultAsync(CancellationToken.None);
                var response = copResult.RawQueue.Where(e => e.PduItemType == PduIt.PDU_IT_RESULT);
                if (response.Any())
                {
                    //....
                }
                cll.Disconnect();
            }

            Assert.IsTrue(cll.IsDisposed);
            Assert.IsTrue(vci.IsDisposed);
            Assert.IsTrue(oneSysLevel.IsDisposed);
        }


        [Test]
        public void UseDiagPduApiFactoryInstanceForDispose()
        {
            //Alternate usage of the DiagPduApiOneFactory
            //Alternately, the DiagPduApiOneFactory can be instantiated as an instance, and when disposed, will dispose all children with it.
            //This negates the need for explicit using's'
            //except for the initial one for the DiagPduApiOneFactory.
            //NOTE:
            //The DiagPduApiOneFactory instance is only used to facilitate the disposal, and the instance does not need to be passed


            //out side run func only for test
            DiagPduApiOneSysLevel oneSysLevel;
            Module vci;
            ComLogicalLink cll;

            void Run()
            {
                oneSysLevel = DiagPduApiOneFactory.GetApi(nativeLibraryPath);
                vci = oneSysLevel.ConnectVci();
                cll = vci.OpenComLogicalLink("ISO_11898_2_DWCAN", "ISO_15765_3_on_ISO_15765_2",
                    new Dictionary<uint, string> { { 6, "HI" }, { 14, "LOW" } }.ToList());
            }

            //This is like main in a App
            using (var factory = new DiagPduApiOneFactory())
            {
                Run();
            }

            Assert.IsTrue(cll.IsDisposed);
            Assert.IsTrue(vci.IsDisposed);
            Assert.IsTrue(oneSysLevel.IsDisposed);
        }

    }


}
