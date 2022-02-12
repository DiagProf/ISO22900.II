#region License

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

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ISO22900.II.OdxLikeComParamSets;
using Spectre.Console;

namespace ISO22900.II.Demo
{
    internal class PageUseCaseRecoverComLogicalLinkAfterVciLostKline : Page
    {
        public PageUseCaseRecoverComLogicalLinkAfterVciLostKline(AbstractPageControl program)
            : base("Use case recover ComLogicalLink after VCI lost K-Line", program)
        {
        }

        public override void Display()
        {
            base.Display();
            var infoGrid = new Grid();
            var info = "Request -> Response ping pong in the loop";
            infoGrid.AddColumn(new GridColumn().Centered());
            infoGrid.AddRow($"[yellow]{info}[/]");
            infoGrid.AddRow("Disconnect the VCI (e.g.power off or unplug the usb cable)");
            infoGrid.AddRow("Or break the loop with a keystroke");


            var tableReqResp = new Table().AddColumns("[b]Request[/]", "[b]Response[/]", "[b]Time(µs)[/]").LeftAligned();
            tableReqResp.AddRow(string.Empty, string.Empty);

            infoGrid.AddEmptyRow();
            infoGrid.AddRow(tableReqResp);
            infoGrid.AddEmptyRow();


            var tableInfo = new Table().Border(TableBorder.None).AddColumn(string.Empty).HideHeaders().LeftAligned()
                .AddRow(new FigletText("Running").LeftAligned().Color(Color.Green));
            infoGrid.AddRow(tableInfo);

            var cllConfigPorscheKline = new LogicalLinkSettingZuffenhausenWithKline();

            using ( var api = DiagPduApiOneFactory.GetApi(
                       DiagPduApiHelper.FullyQualifiedLibraryFileNameFormShortName(AbstractPageControl.Preferences.GetSection("ApiVci:Api").Value),
                       AbstractPageControl.LoggerFactory, ApiModifications.IGNITION_FIX | ApiModifications.VOLTAGE_FIX) )
            {
                using ( var vci = api.ConnectVci(AbstractPageControl.Preferences.GetSection("ApiVci:Vci").Value) )
                {
                    //Define the protocol behavior
                    var dlcPinData = cllConfigPorscheKline.DlcPinData;
                    var busTypeName = cllConfigPorscheKline.BusTypeName;
                    //var busTypeName = "ISO_9141_2_UART";  //e.g. Softing
                    //var busTypeName = "ISO_14230_1_UART"; //e.g. Samtec
                    var protocolName = cllConfigPorscheKline.ProtocolName;

                    using ( var link = vci.OpenComLogicalLink(busTypeName, protocolName, dlcPinData.ToList()) )
                    {
                        cllConfigPorscheKline.LogicalLinkSettingPcm().SetUpLogicalLink(link);
                        link.Connect();

                        var request = new byte[] { 0x1A, 0x80 };

                        //Use StartComm to start tester present behavior
                        using ( var copStartComm = link.StartCop(PduCopt.PDU_COPT_STARTCOMM,1, 1, new byte[] { 0x81 }) )
                        {
                            copStartComm.WaitForCopResult();
                        }

                        var i = request[1];
                        AnsiConsole.Live(infoGrid)
                            .Start(ctx =>
                            {
                                while ( true )
                                {
                                    if ( Console.KeyAvailable )
                                    {
                                        Console.ReadKey(true);
                                        break;
                                    }

                                    try
                                    {
                                        request[1] = i;
                                        i++;
                                        if ( i == 0xA0 )
                                        {
                                            i = 0x80;
                                        }

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


                                            tableReqResp.Rows.RemoveAt(0);
                                            tableReqResp.AddRow($"{BitConverter.ToString(request)}", $"{responseString}", $"{responseTime}");
                                        }


                                        ctx.Refresh();
                                        Thread.Sleep(500); //only that user can read the output
                                    }
                                    catch ( Iso22900IIException e )
                                    {
                                        if ( e.PduError == PduError.PDU_ERR_MODULE_NOT_CONNECTED ||
                                             e.PduError == PduError.PDU_ERR_COMM_PC_TO_VCI_FAILED ||
                                             e.PduError == PduError.PDU_ERR_FCT_FAILED )
                                        {
                                            tableInfo.Rows.Clear();
                                            tableInfo.AddRow(new FigletText("Error VCI lost").LeftAligned().Color(Color.Red));
                                            tableInfo.AddRow("Once you fix the cause of vci lost...");
                                            tableInfo.AddRow(
                                                "Press [Red][[Enter]][/] to start TryToRecover function or [Red][[any]][/] other key to exit");
                                            ctx.Refresh();

                                            if ( Console.ReadKey(true).Key != ConsoleKey.Enter )
                                            {
                                                tableInfo.Rows.Clear();
                                                tableInfo.AddRow(new FigletText("Exit").LeftAligned().Color(Color.Yellow));
                                                ctx.Refresh();
                                                break;
                                            }

                                            tableInfo.Rows.Clear();
                                            tableInfo.AddRow(new FigletText("Start TryToRecover VCI").LeftAligned().Color(Color.Yellow));
                                            ctx.Refresh();

                                            //this sleep only makes sense if the user presses enter very quickly
                                            //Windows also needs some time to recognize a reconnected device
                                            //The time between "the mechanical connection is okay again" and "the operating system noticed it too" is difficult to determine
                                            Thread.Sleep(1000); //gives some time to e.g. reconnect the USB plug

                                            if ( !link.TryToRecover(out var msg) )
                                            {
                                                tableInfo.Rows.Clear();
                                                tableInfo.AddRow(new FigletText("Recovering failed").LeftAligned().Color(Color.Yellow));
                                                tableInfo.AddRow(new Text(msg));
                                                ctx.Refresh();

                                                //here a real app should return to a point where VCI connection is not needed
                                                //or can be restarted from there
                                                break;
                                            }

                                            tableInfo.Rows.Clear();
                                            tableInfo.AddRow(new FigletText("It's running again").LeftAligned().Color(Color.Green));
                                            ctx.Refresh();
                                        }
                                    }
                                }
                            });
                        link.Disconnect();
                    }
                }
            }

            AnsiConsole.Console.ReadKey("Press [DodgerBlue1][[Enter]][/] to navigate home");
            AbstractPageControl.NavigateHome();
        }
    }
}
