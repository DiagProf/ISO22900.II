#region License

// // MIT License
// //
// // Copyright (c) 2022 Joerg Frank
// // http://www.diagprof.com/
// //
// // Permission is hereby granted, free of charge, to any person obtaining a copy
// // of this software and associated documentation files (the "Software"), to deal
// // in the Software without restriction, including without limitation the rights
// // to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// // copies of the Software, and to permit persons to whom the Software is
// // furnished to do so, subject to the following conditions:
// //
// // The above copyright notice and this permission notice shall be included in all
// // copies or substantial portions of the Software.
// //
// // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// // IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// // FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// // AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// // LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// // OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// // SOFTWARE.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using ISO22900.II.OdxLikeComParamSets;
using Spectre.Console;

namespace ISO22900.II.Demo
{
    internal class PageUseCaseSimpleSendAndReceiveDoIpWithPhysicalVci : Page
    {
        
        public PageUseCaseSimpleSendAndReceiveDoIpWithPhysicalVci(AbstractPageControl program)
            : base("Use case simple send and receive DoIP [[does not work yet!!]]", program)
        {
        }

        public override void Display()
        {
            base.Display();
            var infoGrid = new Grid();
            var info = "Simple send and receive DoIP some D-PDU-APIs have not implemented this protocol!!";
            infoGrid.AddColumn(new GridColumn().Centered());
            infoGrid.AddRow($"[yellow]{info}[/]");
            AnsiConsole.Write(infoGrid);

           

            using ( var api = DiagPduApiOneFactory.GetApi(DiagPduApiHelper.FullLibraryPathFormApiShortName(AbstractPageControl.Preferences.GetSection("ApiVci:Api").Value), AbstractPageControl.LoggerFactory))
            {
                using ( var vci = api.ConnectVci(this.AbstractPageControl.Preferences.GetSection("ApiVci:Vci").Value) )
                {



                    //DoIP Setup Pin8->activation Pull-Up mostly mechanically hardwired
                    if (!vci.TryIoCtlSetEthSwitchState(PduExEthernetActivationPin.On))
                    {
                        AnsiConsole.WriteLine("DoIP Activation Line not possible to switch on.");
                    }



                    ////Test Dummy
                    //var test = new PduIoCtlVehicleIdRequestData(0, "", 3, 5000,
                    //    new[]
                    //    {
                    //            new PduIoCtlVehicleIdRequestIpAddrInfoData(4, new byte[] { 255,255,255,255 }),
                    //    }
                    //);

                    //Test Dummy
                    //var test = new PduIoCtlVehicleIdRequestData(0, "", 3, 5000,
                    //    new[]
                    //    {
                    //        new PduIoCtlVehicleIdRequestIpAddrInfoData(4, new byte[] { 255,255,255,255 }),
                    //    }
                    //);
                    //PDU_IOCTL_MS_VIR_WITH_
                            ////if (!api.TryIoCtlGeneral("PDU_IOCTL_MS_VIR_V1", test))
                            ////{
                            ////    AnsiConsole.WriteLine("TryIoCtlVehicleIdRequest on API not possible.");
                            ////}
                            //if (!api.TryIoCtlVehicleIdRequest( test))
                            //{
                            //    AnsiConsole.WriteLine("TryIoCtlVehicleIdRequest on API not possible.");
                            //}


                            //if (!vci.TryIoCtlVehicleIdRequest(test))
                            //{
                            //    AnsiConsole.WriteLine("TryIoCtlVehicleIdRequest on VCI not possible.");

                            //    if (!api.TryIoCtlVehicleIdRequest(test))
                            //    {
                            //        AnsiConsole.WriteLine("TryIoCtlVehicleIdRequest on API not possible.");
                            //    }
                            //}


                            //List<PduModuleData> doipVCIs = (List<PduModuleData>)api.PduModuleDataSets.FindAll(e => e.VendorModuleName.Contains("MVCI_ISO_13400_DoIP"));

                            //if (doipVCIs.Any())
                            //{
                            //AnsiConsole.WriteLine($"VendorModuleName: {doipVCIs.First().VendorModuleName}");
                            //AnsiConsole.WriteLine($"VendorAdditionalInfo: {doipVCIs.First().VendorAdditionalInfo}");

                            //using (var doIpVci = api.ConnectVci(doipVCIs.First().VendorModuleName))
                            //{
                            //vci = api.ConnectVci(b.VendorModuleName);

                            //Define the protocol behavior
                            //These names (the strings) come from ODX or ISO 22900-2
                            var cllSettingXxWithDoIp = new LogicalLinkSettingXXWithDoIp();
                            var dlcPinData = cllSettingXxWithDoIp.DlcPinData;
                            var busTypeName = cllSettingXxWithDoIp.BusTypeName;
                            var protocolName = cllSettingXxWithDoIp.ProtocolName;

                            //that's big shit for you with DoIP is... You have to make settings on the D-PDU-API where the values don't come from ODX
                            //One way to see if DoIP is requested to look at the busTypeName
                            //if ( busTypeName.Equals("IEEE_802_3") )
                            //{
                            //    if (vci.TryIoCtlGetEthPinOption(out var option))
                            //    {
                            //        if ( option == 0 )
                            //        {
                            //            AnsiConsole.WriteLine("DoIP maybe not possible");
                            //        }
                            //    }






                            //    if ( !vci.TryIoCtlVehicleIdRequest(test) )
                            //    {
                            //        AnsiConsole.WriteLine("TryIoCtlVehicleIdRequest not possible.");
                            //    }

                            //}
                            
                            Dictionary<uint, string> DlcPinDataDefault = new() { { 1, "TX" }, { 3, "RX" } };
                            
                            //Dictionary<uint, string> DlcPinDataDefault = new() { { 3, "RX" }, { 12, "TX" } };

                            using (var link = vci.OpenComLogicalLink(busTypeName, protocolName, dlcPinData.ToList()))
                            {

                                cllSettingXxWithDoIp.LogicalLinkSettingGateway().SetUpLogicalLink(link);
                                link.Connect();


                                var errorString = string.Empty;
                                //Use StartComm to start tester present behavior (is a must for the TP2.0)
                                using (var copStartComm = link.StartCop(PduCopt.PDU_COPT_STARTCOMM))
                                {
                                    var result = copStartComm.WaitForCopResult();

                                    if (result.PduEventItemErrors().Count > 0)
                                    {
                                        foreach (var error in result.PduEventItemErrors())
                                        {
                                            errorString += $"{error.ErrorCodeId}" + $" ({error.ExtraErrorInfoId})";
                                        }
                                        errorString = "Error: " + errorString;
                                        AnsiConsole.WriteLine($"{errorString}");
                                    }
                                }

                                //if there is an error (e.g. Error: PDU_ERR_EVT_INIT_ERROR) at PDU_COPT_STARTCOMM, nothing goes on with TP2.0
                                if (errorString.Equals(string.Empty))
                                {
                                    var request = new byte[] { 0x22, 0xF1, 0x90 };

                                    for (var i = 0x80; i <= 0xa0; i++)
                                    {
                                        request[2] = (byte)i;

                                        using (var cop = link.StartCop(PduCopt.PDU_COPT_SENDRECV, 1, 1, request))
                                        {
                                            var result = cop.WaitForCopResult();

                                            //The following evaluation is okay for this use case, but it should be noted that the order may be lost.
                                            //e.g. the correct order might be first PduEventItemInfo and then DataMsg
                                            var responseString = string.Empty;
                                            uint responseTime = 0;
                                            if (result.DataMsgQueue().Count > 0)
                                            {
                                                responseString = string.Join(",",
                                                    result.DataMsgQueue().ConvertAll(bytes => { return BitConverter.ToString(bytes); }));
                                                responseTime = result.ResponseTime();
                                            }

                                            if (result.PduEventItemErrors().Count > 0)
                                            {
                                                foreach (var error in result.PduEventItemErrors())
                                                {
                                                    responseString += $"{error.ErrorCodeId}" + $" ({error.ExtraErrorInfoId})";
                                                }

                                                responseString = "Error: " + responseString;
                                            }

                                            if (result.PduEventItemInfos().Count > 0)
                                            {
                                                foreach (var error in result.PduEventItemInfos())
                                                {
                                                    responseString += $"{error.InfoCode}" + $" ({error.ExtraInfoData})";
                                                }

                                                responseString = "Info: " + responseString;
                                            }

                                            AnsiConsole.WriteLine($"{BitConverter.ToString(request)} | {responseString}  | {responseTime}µs");
                                        }
                                    }


                                    using (var copStopComm = link.StartCop(PduCopt.PDU_COPT_STOPCOMM))
                                    {
                                        var result = copStopComm.WaitForCopResult();

                                        if (result.PduEventItemErrors().Count > 0)
                                        {
                                            foreach (var error in result.PduEventItemErrors())
                                            {
                                                errorString += $"{error.ErrorCodeId}" + $" ({error.ExtraErrorInfoId})";
                                            }
                                            errorString = "Error: " + errorString;
                                            AnsiConsole.WriteLine($"{errorString}");
                                        }
                                    }
                                }

                                link.Disconnect();
                            }
                    //    }
                    //}
                }
            }

            AnsiConsole.Console.ReadKey("Press [DodgerBlue1][[Enter]][/] to navigate home");
            AbstractPageControl.NavigateHome();
        }
    }
}
