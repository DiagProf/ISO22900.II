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
    internal class PageUseCaseSimpleSendAndReceiveTP20 : Page
    {
        
        public PageUseCaseSimpleSendAndReceiveTP20(AbstractPageControl program)
            : base("Use case simple send and receive TP2.0 (SAE J2819)", program)
        {
        }

        public override void Display()
        {
            base.Display();
            var infoGrid = new Grid();
            var info = "Simple send and receive TP2.0 (SAE J2819) some D-PDU-APIs have not implemented this protocol!!";
            infoGrid.AddColumn(new GridColumn().Centered());
            infoGrid.AddRow($"[yellow]{info}[/]");
            AnsiConsole.Write(infoGrid);

           

            using ( var api = DiagPduApiOneFactory.GetApi(DiagPduApiHelper.FullyQualifiedLibraryFileNameFormShortName(AbstractPageControl.Preferences.GetSection("ApiVci:Api").Value), AbstractPageControl.LoggerFactory))
            {
                using ( var vci = api.ConnectVci(this.AbstractPageControl.Preferences.GetSection("ApiVci:Vci").Value) )
                {
                    //Define the protocol behavior
                    //These names (the strings) come from ODX or ISO 22900-2
                    var cllSettingVagWithTp20 = new LogicalLinkSettingVagWithTp20();
                    var dlcPinData = cllSettingVagWithTp20.DlcPinData;
                    var busTypeName = cllSettingVagWithTp20.BusTypeName;
                    var protocolName = cllSettingVagWithTp20.ProtocolName;


                    using ( var link = vci.OpenComLogicalLink(busTypeName, protocolName, dlcPinData.ToList()) )
                    {

                        cllSettingVagWithTp20.LogicalLinkSettingPcm().SetUpLogicalLink(link);
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
                        if ( errorString.Equals(string.Empty) )
                        {
                            var request = new byte[] { 0x1A, 0x9B };

                            for ( var i = 0x80; i <= 0xa0; i++ )
                            {
                                request[1] = (byte)i;

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
                        }

                        link.Disconnect();
                    }
                }
            }

            AnsiConsole.Console.ReadKey("Press [DodgerBlue1][[Enter]][/] to navigate home");
            AbstractPageControl.NavigateHome();
        }
    }
}
