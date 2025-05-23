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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ISO22900.II;
using NUnit.Framework;
using Microsoft.Extensions.Logging.Abstractions;

namespace ISO22900.II.Test
{
    /// <summary>
    /// This is more of an integration test because a real D-PDU API with a real VCI is required
    /// </summary>
    public partial class IntegrationTest_SetupAndTearDown
    {
        private Iso22900NativeWrapAccess _dPduApi;
        private uint _moduleOne = PduConst.PDU_HANDLE_UNDEF;
        private uint _cll = PduConst.PDU_HANDLE_UNDEF;

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
            _dPduApi.PduConstruct();

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
            var pduRscData = new PduResourceData(busTypId, protocolTypId, new Dictionary<uint, uint> {{6, pinTypIdHi}, {14, pinTypIdLow}}.ToList());

            _cll = _dPduApi.PduCreateComLogicalLink(_moduleOne, pduRscData, PduConst.PDU_ID_UNDEF, 0,
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
    }
}
