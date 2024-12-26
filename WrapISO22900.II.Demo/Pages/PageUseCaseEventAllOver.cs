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
using System.Threading;
using ISO22900.II.OdxLikeComParamSets;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace ISO22900.II.Demo
{
    internal class PageUseCaseEventAllOver : Page
    {
        private readonly ILogger _logger;
        private Table tableEvent;

        public PageUseCaseEventAllOver(AbstractPageControl program)
            : base("Use case event on all API Levels", program)
        {
            _logger = program.LoggerFactory.CreateLogger<PageUseCaseEventAllOver>();
            tableEvent = new Table().AddColumns("[b]Class[/]", "[b]EventItemType[/]", "[b]Data1[/]", "[b]Data2[/]", "[b]CopTag[/]", "[b]Time(µs)[/]").LeftAligned();
            tableEvent.AddRow("System", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
            tableEvent.AddRow("Module", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
            tableEvent.AddRow("ComLink", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
        }

        public override void Display()
        {
            base.Display();
            var infoGrid = new Grid();
            var info = "Request -> Response ping pong in the loop. (Please look at the log file to see the events properly)";
            infoGrid.AddColumn(new GridColumn().Centered());
            infoGrid.AddRow($"[yellow]{info}[/]");
            infoGrid.AddRow("Disconnect the VCI (e.g.power off or unplug the usb cable)");
            infoGrid.AddRow("Or break the loop with a keystroke");


            var tableReqResp = new Table().AddColumns("[b]Request[/]", "[b]          Response          [/]", "[b]Time(µs)[/]").LeftAligned();
            tableReqResp.AddRow(string.Empty, string.Empty, string.Empty);

            infoGrid.AddEmptyRow();
            infoGrid.AddRow(tableReqResp);
            infoGrid.AddEmptyRow();


            var tableInfo = new Table().Border(TableBorder.None).AddColumn(string.Empty).HideHeaders().LeftAligned()
                .AddRow(new FigletText("Running").LeftJustified().Color(Color.Green));
            infoGrid.AddRow(tableInfo);
            infoGrid.AddEmptyRow();
            infoGrid.AddRow(tableEvent);

            using ( var api = DiagPduApiOneFactory.GetApi(
                       DiagPduApiHelper.FullLibraryPathFormApiShortName(AbstractPageControl.Preferences.GetSection("ApiVci:Api").Value),
                       AbstractPageControl.LoggerFactory, ApiModifications.IGNITION_FIX | ApiModifications.VOLTAGE_FIX) )
            {
                api.EventFired += EventHandlerOnSysLevel;

                using ( var vci = api.ConnectVci(AbstractPageControl.Preferences.GetSection("ApiVci:Vci").Value) )
                {
                    vci.EventFired += EventHandlerOnModulLevel;
                    //Define the protocol behavior
                    //These names (the strings) come from ODX or ISO 22900-2
                    var dlcPinData = new Dictionary<uint, string> { { 6, "HI" }, { 14, "LOW" } };
                    var busTypeName = "ISO_11898_2_DWCAN";
                    var protocolName = "ISO_15765_3_on_ISO_15765_2";

                    using ( var link = vci.OpenComLogicalLink(busTypeName, protocolName, dlcPinData.ToList()) )
                    {
                        link.EventFired += EventHandlerOnCllLevel;
                        var pcmCllConfig = new VehicleInfoSpecForTestEcu();
                        pcmCllConfig.SetUpLogicalLink(link);
                        link.Connect();

                        var request = new byte[] { 0x22, 0xF1, 0x92 };

                        //Use StartComm to start tester present behavior
                        using ( var copStartComm = link.StartCop(PduCopt.PDU_COPT_STARTCOMM) )
                        {
                            copStartComm.WaitForCopResult();
                        }

                        var i = request[2];
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
                                        request[2] = i;
                                        i++;
                                        if ( i == 0xB0 )
                                        {
                                            i = 0x80;
                                        }

                                        using ( var cop = link.StartCop(PduCopt.PDU_COPT_SENDRECV, 1, 1, request, 0815) )
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
                                            tableInfo.AddRow(new FigletText("Error VCI lost").LeftJustified().Color(Color.Red));
                                            tableInfo.AddRow("Once you fix the cause of vci lost...");
                                            tableInfo.AddRow(
                                                "Press [Red][[Enter]][/] to start TryToRecover function or [Red][[any]][/] other key to exit");
                                            ctx.Refresh();

                                            if ( Console.ReadKey(true).Key != ConsoleKey.Enter )
                                            {
                                                tableInfo.Rows.Clear();
                                                tableInfo.AddRow(new FigletText("Exit").LeftJustified().Color(Color.Yellow));
                                                ctx.Refresh();
                                                break;
                                            }

                                            tableInfo.Rows.Clear();
                                            tableInfo.AddRow(new FigletText("Start TryToRecover VCI").LeftJustified().Color(Color.Yellow));
                                            ctx.Refresh();

                                            //this sleep only makes sense if the user presses enter very quickly
                                            //Windows also needs some time to recognize a reconnected device
                                            //The time between "the mechanical connection is okay again" and "the operating system noticed it too" is difficult to determine
                                            Thread.Sleep(1000); //gives some time to e.g. reconnect the USB plug

                                            if ( !link.TryToRecover(out var msg) )
                                            {
                                                tableInfo.Rows.Clear();
                                                tableInfo.AddRow(new FigletText("Recovering failed").LeftJustified().Color(Color.Yellow));
                                                tableInfo.AddRow(new Text(msg));
                                                ctx.Refresh();

                                                //here a real app should return to a point where VCI connection is not needed
                                                //or can be restarted from there
                                                break;
                                            }

                                            tableInfo.Rows.Clear();
                                            tableInfo.AddRow(new FigletText("It's running again").LeftJustified().Color(Color.Green));
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

        void EventHandlerOnSysLevel(object sender, PduEventItem ev)
        {
            
            tableEvent.Rows.Update(0, 0, new Text(sender.ToString()));
            tableEvent.Rows.Update(0, 1, new Text(ev.PduItemType.ToString()));
            tableEvent.Rows.Update(0, 4, new Text(ev.CopTag.ToString()));
            tableEvent.Rows.Update(0, 5, new Text(ev.Timestamp.ToString()));
            switch (ev)
            {
                case PduEventItemError evE:
                    _logger.LogInformation("Sender: {sender} - EvType: {evType} ErrorCodeId: {ErrorCodeId} ExtraErrorInfoId: {ExtraErrorInfoId} Time: {time} CopTag: {CopTag}", sender.ToString(), ev.PduItemType.ToString(), evE.ErrorCodeId.ToString(), evE.ExtraErrorInfoId.ToString(), ev.Timestamp, ev.CopTag);
                    tableEvent.Rows.Update(0, 2, new Text(evE.ErrorCodeId.ToString()));
                    tableEvent.Rows.Update(0, 3, new Text(evE.ExtraErrorInfoId.ToString()));
                    break;
                case PduEventItemInfo evI:
                    _logger.LogInformation("Sender: {sender} - EvType: {evType} InfoCode: {InfoCode} ExtraInfoData: {ExtraInfoData} Time: {time} CopTag: {CopTag}", sender.ToString(), ev.PduItemType.ToString(), evI.InfoCode.ToString(), evI.ExtraInfoData.ToString(),  ev.Timestamp, ev.CopTag );
                    tableEvent.Rows.Update(0, 2, new Text(evI.InfoCode.ToString()));
                    tableEvent.Rows.Update(0, 3, new Text(evI.ExtraInfoData.ToString()));
                    break;
                case PduEventItemResult evR:
                    _logger.LogInformation("Sender: {sender} - EvType: {evType} ResultData.DataBytes: {DataBytes} ResultData.UniqueId: {UniqueId} Time: {time} CopTag: {CopTag}", sender.ToString(), ev.PduItemType.ToString(), BitConverter.ToString(evR.ResultData.DataBytes), evR.ResultData.UniqueRespIdentifier.ToString(), ev.Timestamp, ev.CopTag);
                    tableEvent.Rows.Update(0, 2, new Text(BitConverter.ToString(evR.ResultData.DataBytes)));
                    tableEvent.Rows.Update(0, 3, new Text(evR.ResultData.UniqueRespIdentifier.ToString()));
                    break;
                case PduEventItemStatus evS:
                    _logger.LogInformation("Sender: {sender} - EvType: {evType} PduStatus: {PduStatus} Time: {time} CopTag: {CopTag}", sender.ToString(), ev.PduItemType.ToString(), evS.PduStatus, ev.Timestamp, ev.CopTag);
                    tableEvent.Rows.Update(0, 2, new Text(evS.PduStatus.ToString()));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(ev));
            }
        }

        void EventHandlerOnModulLevel(object sender, PduEventItem ev)
        {
            tableEvent.Rows.Update(1, 0, new Text(sender.ToString()));
            tableEvent.Rows.Update(1, 1, new Text(ev.PduItemType.ToString()));
            tableEvent.Rows.Update(1, 4, new Text(ev.CopTag.ToString()));
            tableEvent.Rows.Update(1, 5, new Text(ev.Timestamp.ToString()));
            switch (ev)
            {
                case PduEventItemError evE:
                    _logger.LogInformation("Sender: {sender} - EvType: {evType} ErrorCodeId: {ErrorCodeId} ExtraErrorInfoId: {ExtraErrorInfoId} Time: {time} CopTag: {CopTag}", sender.ToString(), ev.PduItemType.ToString(), evE.ErrorCodeId.ToString(), evE.ExtraErrorInfoId.ToString(), ev.Timestamp, ev.CopTag);
                    tableEvent.Rows.Update(1, 2, new Text(evE.ErrorCodeId.ToString()));
                    tableEvent.Rows.Update(1, 3, new Text(evE.ExtraErrorInfoId.ToString()));
                    break;
                case PduEventItemInfo evI:
                    _logger.LogInformation("Sender: {sender} - EvType: {evType} InfoCode: {InfoCode} ExtraInfoData: {ExtraInfoData} Time: {time} CopTag: {CopTag}", sender.ToString(), ev.PduItemType.ToString(), evI.InfoCode.ToString(), evI.ExtraInfoData.ToString(), ev.Timestamp, ev.CopTag);
                    tableEvent.Rows.Update(1, 2, new Text(evI.InfoCode.ToString()));
                    tableEvent.Rows.Update(1, 3, new Text(evI.ExtraInfoData.ToString()));
                    break;
                case PduEventItemResult evR:
                    _logger.LogInformation("Sender: {sender} - EvType: {evType} ResultData.DataBytes: {DataBytes} ResultData.UniqueId: {UniqueId} Time: {time} CopTag: {CopTag}", sender.ToString(), ev.PduItemType.ToString(), BitConverter.ToString(evR.ResultData.DataBytes), evR.ResultData.UniqueRespIdentifier.ToString(), ev.Timestamp, ev.CopTag);
                    tableEvent.Rows.Update(1, 2, new Text(BitConverter.ToString(evR.ResultData.DataBytes)));
                    tableEvent.Rows.Update(1, 3, new Text(evR.ResultData.UniqueRespIdentifier.ToString()));
                    break;
                case PduEventItemStatus evS:
                    _logger.LogInformation("Sender: {sender} - EvType: {evType} PduStatus: {PduStatus} Time: {time} CopTag: {CopTag}", sender.ToString(), ev.PduItemType.ToString(), evS.PduStatus, ev.Timestamp, ev.CopTag);
                    tableEvent.Rows.Update(1, 2, new Text(evS.PduStatus.ToString()));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(ev));
            }
        }


        void EventHandlerOnCllLevel(object sender, PduEventItem ev)
        {
            tableEvent.Rows.Update(2, 0, new Text(sender.ToString()));
            tableEvent.Rows.Update(2, 1, new Text(ev.PduItemType.ToString()));
            tableEvent.Rows.Update(2, 4, new Text(ev.CopTag.ToString()));
            tableEvent.Rows.Update(2, 5, new Text(ev.Timestamp.ToString()));
            switch (ev)
            {
                case PduEventItemError evE:
                    _logger.LogInformation("Sender: {sender} - EvType: {evType} ErrorCodeId: {ErrorCodeId} ExtraErrorInfoId: {ExtraErrorInfoId} Time: {time} CopTag: {CopTag}", sender.ToString(), ev.PduItemType.ToString(), evE.ErrorCodeId.ToString(), evE.ExtraErrorInfoId.ToString(), ev.Timestamp, ev.CopTag);
                    tableEvent.Rows.Update(2, 2, new Text(evE.ErrorCodeId.ToString()));
                    tableEvent.Rows.Update(2, 3, new Text(evE.ExtraErrorInfoId.ToString()));
                    break;
                case PduEventItemInfo evI:
                    _logger.LogInformation("Sender: {sender} - EvType: {evType} InfoCode: {InfoCode} ExtraInfoData: {ExtraInfoData} Time: {time} CopTag: {CopTag}", sender.ToString(), ev.PduItemType.ToString(), evI.InfoCode.ToString(), evI.ExtraInfoData.ToString(), ev.Timestamp, ev.CopTag);
                    tableEvent.Rows.Update(2, 2, new Text(evI.InfoCode.ToString()));
                    tableEvent.Rows.Update(2, 3, new Text(evI.ExtraInfoData.ToString()));
                    break;
                case PduEventItemResult evR:
                    _logger.LogInformation("Sender: {sender} - EvType: {evType} ResultData.DataBytes: {DataBytes} ResultData.UniqueId: {UniqueId} Time: {time} CopTag: {CopTag}", sender.ToString(), ev.PduItemType.ToString(), BitConverter.ToString(evR.ResultData.DataBytes), evR.ResultData.UniqueRespIdentifier.ToString(), ev.Timestamp, ev.CopTag);
                    tableEvent.Rows.Update(2, 2, new Text(BitConverter.ToString(evR.ResultData.DataBytes)));
                    tableEvent.Rows.Update(2, 3, new Text(evR.ResultData.UniqueRespIdentifier.ToString()));
                    break;
                case PduEventItemStatus evS:
                    _logger.LogInformation("Sender: {sender} - EvType: {evType} PduStatus: {PduStatus} Time: {time} CopTag: {CopTag}", sender.ToString(), ev.PduItemType.ToString(), evS.PduStatus, ev.Timestamp, ev.CopTag);
                    tableEvent.Rows.Update(2, 2, new Text(evS.PduStatus.ToString()));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(ev));
            }
        }

    }

}
