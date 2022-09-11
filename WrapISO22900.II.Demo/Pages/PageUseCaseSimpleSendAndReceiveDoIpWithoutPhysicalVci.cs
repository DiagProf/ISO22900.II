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
using System.Xml.XPath;
using ISO22900.II.OdxLikeComParamSets;
using Spectre.Console;

namespace ISO22900.II.Demo
{
    internal class PageUseCaseSimpleSendAndReceiveDoIpWithoutPhysicalVci : Page
    {
        public PageUseCaseSimpleSendAndReceiveDoIpWithoutPhysicalVci(AbstractPageControl program)
            : base("Use case simple send and receive DoIP without physical VCI", program)
        {
        }

        public override void Display()
        {
            base.Display();
            var infoGrid = new Grid();
            var info = "Simple send and receive DoIP some D-PDU-APIs have not implemented this protocol!!";
            var wire =
                "\"Without physical VCI\" means a simple OBD2 Ethernet Cable. With permanently installed (not switchable) activation resistor.";
            var lic =
                "Separate license for this is needed often!! Because \"without physical VCI\" imply that the VCI hardware can no longer serve as a license token.";
            infoGrid.AddColumn(new GridColumn().Centered());
            infoGrid.AddRow($"[yellow]{info}[/]");
            infoGrid.AddRow($"[yellow]{wire}[/]");
            infoGrid.AddRow($"[yellow]{lic}[/]");
            infoGrid.AddEmptyRow();
            infoGrid.AddRow("[white]Example is for a connection to the Golf 8 gateway.[/]");
            infoGrid.AddRow("[white]E.g. Ethernet adapter should have IP address: 169.254.123.1 and subnet mask 255.255.0.0[/]");
            infoGrid.AddEmptyRow();
            AnsiConsole.Write(infoGrid);


            using ( var api = DiagPduApiOneFactory.GetApi(
                       DiagPduApiHelper.FullLibraryPathFormApiShortName(AbstractPageControl.Preferences.GetSection("ApiVci:Api").Value),
                       AbstractPageControl.LoggerFactory) )
            {
                var ioCtlVehicleIdRequestData =
                    new PduIoCtlVehicleIdRequestData(PduExPreselectionMode.NoPreselection, "", PduExCombinationMode.VinCombination, 5000);

                if ( api.TryIoCtlVehicleIdRequest(ioCtlVehicleIdRequestData) )
                {
                    var doipVCIs = api.PduModuleDataSets.FindAll(e => e.VendorModuleName.Contains("MVCI_ISO_13400_DoIP"));

                    if ( doipVCIs.Any() )
                    {
                        AnsiConsole.WriteLine($"VendorModuleName: {doipVCIs.First().VendorModuleName}");
                        AnsiConsole.WriteLine($"VendorAdditionalInfo: {doipVCIs.First().VendorAdditionalInfo}");
                        AnsiConsole.WriteLine();

                        using ( var doIpVci = api.ConnectVci(doipVCIs.First().VendorModuleName) )
                        {
                            //Define the protocol behavior
                            //These names (the strings) come from ODX or ISO 22900-2
                            var cllSettingWithDoIp = new LogicalLinkSettingXXWithDoIp();
                            var dlcPinData = cllSettingWithDoIp.DlcPinData;
                            var busTypeName = cllSettingWithDoIp.BusTypeName;
                            var protocolName = cllSettingWithDoIp.ProtocolName;

                            //simple OBD2 Ethernet Cable pin out description! (not the pin out) is different from API to API (i don't know why).
                            //But is a good example for GetResourceIds do figure out the right setting used with your VCI
                            //This is particularly helpful if an application is to use many different VCIs and D-PDU APIs.

                            //GetResourceIds with default values
                            List<uint> resourceIds = doIpVci.GetResourceIds(busTypeName, protocolName, dlcPinData.ToList());

                            if ( !resourceIds.Any() )
                            {
                                //GetResourceIds with values for softing
                                dlcPinData = new() { { 1, "TX" }, { 3, "RX" } };
                                resourceIds = doIpVci.GetResourceIds(busTypeName, protocolName, dlcPinData.ToList());

                                if ( !resourceIds.Any() )
                                {
                                    //GetResourceIds with values for porscheActia (PT3G-VCI (v2))
                                    dlcPinData = new() { { 3, "RX" }, { 12, "TX" } };
                                    resourceIds = doIpVci.GetResourceIds(busTypeName, protocolName, dlcPinData.ToList());

                                    if (!resourceIds.Any())
                                    {
                                        //GetResourceIds with values for vw actia (VAS 6154 (A))
                                        // like porscheActia  dlcPinData = new() { { 3, "RX" }, { 12, "TX" } };
                                        busTypeName = "XDE_5024_UART"; //i don't know what vw is doing here. but that is at VW the synonym for "IEEE_802_3"
                                        resourceIds = doIpVci.GetResourceIds(busTypeName, protocolName, dlcPinData.ToList());
                                    }
                                }
  
                            }

                            if ( resourceIds.Any() )
                            {
                                //using (var link = doIpVci.OpenComLogicalLink(busTypeName, protocolName, dlcPinData.ToList()))
                                //or
                                using ( var link = doIpVci.OpenComLogicalLink(resourceIds.First()) )
                                {
                                    cllSettingWithDoIp.LogicalLinkSettingGateway().SetUpLogicalLink(link);
                                    link.Connect();


                                    var errorString = string.Empty;

                                    using ( var copStartComm = link.StartCop(PduCopt.PDU_COPT_STARTCOMM) )
                                    {
                                        var result = copStartComm.WaitForCopResult();

                                        if ( result.PduEventItemErrors().Count > 0 )
                                        {
                                            foreach ( var error in result.PduEventItemErrors() )
                                            {
                                                errorString += $"{error.ErrorCodeId}" + $" ({error.ExtraErrorInfoId})";
                                            }

                                            errorString = "Error: " + errorString;
                                            AnsiConsole.WriteLine($"{errorString}");
                                        }
                                    }


                                    if ( errorString.Equals(string.Empty) )
                                    {
                                        var request = new byte[] { 0x22, 0xF1, 0x90 };

                                        for ( var i = 0x80; i <= 0xa0; i++ )
                                        {
                                            request[2] = (byte)i;

                                            using ( var cop = link.StartCop(PduCopt.PDU_COPT_SENDRECV, 1, 1, request) )
                                            {
                                                var result = cop.WaitForCopResult();

                                                //The following evaluation is okay for this use case, but it should be noted that the order may be lost.
                                                //e.g. the correct order might be first PduEventItemInfo and then DataMsg
                                                var responseString = string.Empty;
                                                uint responseTime = 0;
                                                if ( result.DataMsgQueue().Count > 0 )
                                                {
                                                    responseString = string.Join(",",
                                                        result.DataMsgQueue().ConvertAll(bytes => { return BitConverter.ToString(bytes); }));
                                                    responseTime = result.ResponseTime();
                                                }

                                                if ( result.PduEventItemErrors().Count > 0 )
                                                {
                                                    foreach ( var error in result.PduEventItemErrors() )
                                                    {
                                                        responseString += $"{error.ErrorCodeId}" + $" ({error.ExtraErrorInfoId})";
                                                    }

                                                    responseString = "Error: " + responseString;
                                                }

                                                if ( result.PduEventItemInfos().Count > 0 )
                                                {
                                                    foreach ( var error in result.PduEventItemInfos() )
                                                    {
                                                        responseString += $"{error.InfoCode}" + $" ({error.ExtraInfoData})";
                                                    }

                                                    responseString = "Info: " + responseString;
                                                }

                                                AnsiConsole.WriteLine($"{BitConverter.ToString(request)} | {responseString}  | {responseTime}µs");
                                            }
                                        }


                                        using ( var copStopComm = link.StartCop(PduCopt.PDU_COPT_STOPCOMM) )
                                        {
                                            var result = copStopComm.WaitForCopResult();

                                            if ( result.PduEventItemErrors().Count > 0 )
                                            {
                                                foreach ( var error in result.PduEventItemErrors() )
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
                            }
                            else
                            {
                                AnsiConsole.WriteLine("Can't find the right settings for ComLogicalLink.");
                            }
                        }
                    }
                    else
                    {
                        AnsiConsole.WriteLine("No DoIP VCI found.");
                    }
                }
                else
                {
                    AnsiConsole.WriteLine("TryIoCtlVehicleIdRequest on API not possible.");
                }
            }


            AnsiConsole.Console.ReadKey("Press [DodgerBlue1][[Enter]][/] to navigate home");
            AbstractPageControl.NavigateHome();
        }
    }
}
