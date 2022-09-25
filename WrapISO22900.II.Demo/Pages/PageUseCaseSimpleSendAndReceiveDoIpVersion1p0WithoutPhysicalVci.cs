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
using System.Threading.Tasks;
using ISO22900.II.OdxLikeComParamSets;
using ISO22900.II.OdxLikeComParamSets.TransportOrDataLinkLayer;
using Spectre.Console;

namespace ISO22900.II.Demo
{
    internal class PageUseCaseSimpleSendAndReceiveDoIpVersion1p0WithoutPhysicalVci : Page
    {
        public PageUseCaseSimpleSendAndReceiveDoIpVersion1p0WithoutPhysicalVci(AbstractPageControl program)
            : base("Use case simple send and receive DoIP version 1.0 (obsolete) without physical VCI", program)
        {
        }

        public override void Display()
        {
            base.Display();
            var infoGrid = new Grid();
            var info = "Simple send and receive DoIP version 1.0 some D-PDU-APIs have not implemented this protocol!!";
            var ioCtlString = "D-PDU-API must support the special IoControl \"PDU_IOCTL_MS_VIR_V1\" so that you can switch to DoIP version 1 at RUNTIME!";
            var info2 = "I believe the Vector and Bosch D-PDU API support this.";
            var wire =
                "\"Without physical VCI\" means a simple OBD2 Ethernet Cable. With permanently installed (not switchable) activation resistor.";
            var lic =
                "Separate license for this is needed often!! Because \"without physical VCI\" imply that the VCI hardware can no longer serve as a license token.";
            infoGrid.AddColumn(new GridColumn().Centered());
            infoGrid.AddRow($"[yellow]{info}[/]");
            infoGrid.AddRow($"[red]{ioCtlString}[/]");
            infoGrid.AddRow($"[yellow]{info2}[/]");
            infoGrid.AddRow($"[yellow]{wire}[/]");
            infoGrid.AddRow($"[yellow]{lic}[/]");
            infoGrid.AddEmptyRow();
            infoGrid.AddRow("[white]Example is for a connection to the Mercedes Headunit A-Entry HUE213.[/]");
            infoGrid.AddRow("[white]E.g. Ethernet adapter should have IP address: 169.254.0.76 and subnet mask 255.255.0.0[/]");
            infoGrid.AddEmptyRow();
            AnsiConsole.Write(infoGrid);


            using ( var api = DiagPduApiOneFactory.GetApi(
                       DiagPduApiHelper.FullLibraryPathFormApiShortName(AbstractPageControl.Preferences.GetSection("ApiVci:Api").Value),
                       AbstractPageControl.LoggerFactory) )
            {
                //Bosch needs PduExCombinationMode.AllCombination
                var ioCtlVehicleIdRequestData = 
                    new PduIoCtlVehicleIdRequestData(PduExPreselectionMode.NoPreselection, "", PduExCombinationMode.AllCombination, 500);
                


                //This is a good example of the generic version of TryIoCtlVehicleIdRequest
                if ( api.TryIoCtlGeneral("PDU_IOCTL_MS_VIR_V1", ioCtlVehicleIdRequestData) )
                {
                    
                    var doipVCIs = api.PduModuleDataSets.FindAll(e => e.VendorModuleName.Contains("_DoIP_"));

                    AnsiConsole.WriteLine($"Module(s) found: {doipVCIs.Count}");
                    //List all with DoIP
                    foreach ( var pduModuleData in doipVCIs )
                    {
                        AnsiConsole.WriteLine($"VendorModuleName: {pduModuleData.VendorModuleName}");
                        AnsiConsole.WriteLine($"VendorAdditionalInfo: {pduModuleData.VendorAdditionalInfo}");
                        AnsiConsole.WriteLine();
                    }

                    //But for this ECU (which has some bugs in the DoIP stack) we are only interested in the Daimler variant.
                    //so we do that "MVCI_DAIMLER_DoIP" (an early DOIP implementation for Daimler)
                    doipVCIs = doipVCIs.FindAll(e => e.VendorModuleName.Contains("MVCI_DAIMLER_DoIP"));

                    if ( doipVCIs.Any() )
                    {
                        AnsiConsole.WriteLine();
                        AnsiConsole.WriteLine($"selected:");
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

                            //simple OBD2 Ethernet Cable pin out description! Note the pinout is different from API to API (i don't know why).
                            //But is a good example for GetResourceIds do figure out the right setting used with your VCI
                            //This is particularly helpful if an application is to use many different VCIs and D-PDU APIs.

                            //GetResourceIds with default values
                            List<uint> resourceIds = doIpVci.GetResourceIds(busTypeName, protocolName, dlcPinData.ToList());

                            if ( !resourceIds.Any() )
                            {
                                //Bosch needs protocol name only "ISO_13400" the right is "ISO_14229_5_on_ISO_13400_2" but only for DAIMLER_DOIP (an early DOIP implementation for Daimler)
                                protocolName = "ISO_13400";
                                //Bosch Pinout is  It's weird or I'm misunderstanding something
                                dlcPinData = new() { { 3, "TX" }, { 11, "LOW" }, { 12, "RX" }, { 13, "MINUS" } };
                                resourceIds = doIpVci.GetResourceIds(busTypeName, protocolName, dlcPinData.ToList());
                            }

                            if ( resourceIds.Any() )
                            {
                                //using (var link = doIpVci.OpenComLogicalLink(busTypeName, protocolName, dlcPinData.ToList()))
                                //or
                                using ( var link = doIpVci.OpenComLogicalLink(resourceIds.First()) )
                                {
                                    //With this protocol, Bosch has 5 entries in the UniqueRespIdTable but only for DAIMLER_DOIP (an early DOIP implementation for Daimler)
                                    //I don't understand exactly why
                                    //but that's why we set the ComParams separately from the ComParams in the UniqueRespIdTable
                                    cllSettingWithDoIp.LogicalLinkSettingHueMb().SetUpLogicalLinkComParamsOnly(link);
                                    link.SetUniqueRespIdTablePageOneUniqueRespIdentifier(0x4711);
                                    link.SetUniqueIdComParamValue(0x4711, "CP_LogicalAddressGateway", 0x2001);
                                    link.SetUniqueIdComParamValue(0x4711, "CP_LogicalSourceAddress", 0x0EF0); //new name is CP_DoIPLogicalTesterAddress
                                    link.SetUniqueIdComParamValue(0x4711, "CP_LogicalTargetAddress", 0x3000); //new name is CP_DoIPLogicalEcuAddress
                                    

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

                                        for ( var i = 0x90; i <= 0xa0; i++ )
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
