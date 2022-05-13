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
    internal class PageUseCaseRecoverComPrimitiveAfterVciLost : Page
    {
        public PageUseCaseRecoverComPrimitiveAfterVciLost(AbstractPageControl program)
            : base("Use case recover ComPrimitive after VCI lost.", program)
        {
        }

        public override void Display()
        {
            base.Display();
            var infoGrid = new Grid();
            var info = "Recover on ComPrimitive is more for long running Cop's... so this is again an ECU simulator";
            infoGrid.AddColumn(new GridColumn().Centered());
            infoGrid.AddRow($"[yellow]{info}[/]");
            infoGrid.AddRow("Disconnect the VCI (e.g.power off or unplug the usb cable)");
            infoGrid.AddRow("Or break the loop with a keystroke");


            var tableInfo = new Table().Border(TableBorder.None).AddColumn(string.Empty).HideHeaders().LeftAligned();
            infoGrid.AddRow(tableInfo);

            AnsiConsole.Write(infoGrid);

            using ( var api = DiagPduApiOneFactory.GetApi(
                       DiagPduApiHelper.FullLibraryPathFormApiShortName(AbstractPageControl.Preferences.GetSection("ApiVci:Api").Value),
                       AbstractPageControl.LoggerFactory, ApiModifications.IGNITION_FIX | ApiModifications.VOLTAGE_FIX) )
            {
                using ( var vci = api.ConnectVci(AbstractPageControl.Preferences.GetSection("ApiVci:Vci").Value) )
                {
                    AnsiConsole.Write(new FigletText("Running").LeftAligned().Color(Color.Green));
                    //Define the protocol behavior
                    //These names (the strings) come from ODX or ISO 22900-2
                    var dlcPinData = new Dictionary<uint, string> { { 6, "HI" }, { 14, "LOW" } };
                    var busTypeName = "ISO_11898_2_DWCAN";
                    var protocolName = "ISO_15765_3_on_ISO_15765_2";

                    using ( var link = vci.OpenComLogicalLink(busTypeName, protocolName, dlcPinData.ToList()) )
                    {
                        // Set ComParams and UniqueRespIdTable
                        AnsiConsole.WriteLine("MainThread:    Set ComParams and UniqueRespIdTable ...");
                        var pcmCllConfig = new VehicleInfoSpecForTestEcu();

                        pcmCllConfig.LogicalLinkSettingForTcm().ChangeToSimulation().SetUpLogicalLink(link);


                        // Connect Link
                        AnsiConsole.WriteLine("MainThread:    Connect Link...");
                        link.Connect();

                        var cts = new CancellationTokenSource();

                        // Start send and receive thread
                        AnsiConsole.WriteLine("MainThread:    Start receive thread.");
                        var receiveThread = new Thread(() => ReceiveThreadFunction(link, cts));
                        receiveThread.Start();

                        AnsiConsole.WriteLine("MainThread:    Start key press thread.");
                        var keyPressThread = new Thread(() => KeyPressThreadFunction(cts));
                        keyPressThread.Start();

                        // Wait until keyPressThread has finished
                        keyPressThread.Join();
                        cts.Cancel();

                        // Wait until receiveThread has finished
                        receiveThread.Join();
                        AnsiConsole.WriteLine("MainThread:    Done receive thread.");
                        ;
                        link.Disconnect();

                        // Close all open resources
                        AnsiConsole.WriteLine("MainThread:    Close all open resources...");
                    }
                }
            }

            AnsiConsole.Console.ReadKey("Press [DodgerBlue1][[Enter]][/] to navigate home");
            AbstractPageControl.NavigateHome();
        }

        public static void KeyPressThreadFunction(CancellationTokenSource cts)
        {
            AnsiConsole.MarkupLine("KeyPressThread: Press [red][[Enter]][/] to cancel...");

            while ( true )
            {
                if ( cts.IsCancellationRequested )
                {
                    break;
                }

                if ( Console.KeyAvailable )
                {
                    if ( Console.ReadKey(true).Key == ConsoleKey.Enter )
                    {
                        break;
                    }

                    AnsiConsole.MarkupLine("KeyPressThread: Press [red][[Enter]][/] to cancel...");
                }

                Thread.Sleep(500);
            }

            AnsiConsole.WriteLine("KeyPressThread: Done");
        }

        public static void ReceiveThreadFunction(ComLogicalLink link, CancellationTokenSource cts)
        {
            // Start receiving ComPrimitive...
            AnsiConsole.WriteLine("ReceiveThread: Start receiving ComPrimitive.");

            using ( var receiveCop = link.StartCop(PduCopt.PDU_COPT_SENDRECV, 0, -1, new byte[] {}) )
            {
                AnsiConsole.WriteLine("ReceiveThread: Receiving ComPrimitive started.");

                // Poll for responses
                AnsiConsole.WriteLine("ReceiveThread: Wait for request...");

                while ( !cts.Token.IsCancellationRequested )
                {
                    // Thread.Sleep(1000);
                    var result = receiveCop.WaitForCopResultAsync(cts.Token).Result;

                    if ( result.DataMsgQueue().Count > 0 )
                    {
                        var request = string.Join(",", result.DataMsgQueue().ConvertAll(bytes => { return BitConverter.ToString(bytes); }));
                        AnsiConsole.WriteLine($"Req: {request}");

                        byte[] response;
                        switch ( request )
                        {
                            case "22-F1-90":
                                response = new byte[]
                                {
                                    0x62, 0xF1, 0x90, 0x4c, 0x6f, 0x6f, 0x6b, 0x69, 0x6e, 0x67, 0x46, 0x6f, 0x72, 0x53, 0x65, 0x63, 0x72, 0x65, 0x74,
                                    0x3f
                                };
                                break;
                            default:
                                response = new byte[] { 0x7F, result.DataMsgQueue()[0][0], 0x11 };
                                break;
                        }

                        AnsiConsole.WriteLine($"Response: {BitConverter.ToString(response)}");
                        using ( var responseCop = link.StartCop(PduCopt.PDU_COPT_SENDRECV, 1, 0, response) )
                        {
                            var resultResponse = responseCop.WaitForCopResultAsync(cts.Token).Result;

                            //for the information quite good... but breaks the order of how the events were fired
                            resultResponse.PduEventItemResults().ForEach(result =>
                            {
                                AnsiConsole.WriteLine($"{BitConverter.ToString(result.ResultData.DataBytes)}");
                            });
                            resultResponse.PduEventItemErrors().ForEach(error => { AnsiConsole.WriteLine($"{error.ErrorCodeId}"); });
                            resultResponse.PduEventItemInfos().ForEach(info => { AnsiConsole.WriteLine($"{info.InfoCode}"); });
                        }
                    }
                    else if ( !cts.Token.IsCancellationRequested )
                    {
                        //an empty content could be an error
                        try
                        {
                            //we ask for the status to see if the cop is still working
                            receiveCop.Status();
                        }
                        catch ( Iso22900IIException )
                        {
                            AnsiConsole.Write(new FigletText("Error VCI lost").LeftAligned().Color(Color.Red));
                            AnsiConsole.WriteLine("Fix the cause of vci lost...");
                            AnsiConsole.MarkupLine("Reboot in 10 seconds");

                            AnsiConsole.Status()
                                .AutoRefresh(true)
                                .Spinner(Spinner.Known.Default)
                                .Start("[yellow]10 second[/]", ctx =>
                                {
                                    for ( var i = 9; i >= 0; i-- )
                                    {
                                        Thread.Sleep(1000);
                                        ctx.Spinner(Spinner.Known.Star);
                                        var time = $"{i} second";
                                        ctx.Status($"[red]{time}[/]");
                                    }
                                });

                            AnsiConsole.Write(new FigletText("Start TryToRecover VCI").LeftAligned().Color(Color.Yellow));
                            if ( !receiveCop.TryToRecover(out var msg) )
                            {
                                //here a real app should return to a point where VCI connection is not needed
                                //or can be restarted from there
                                AnsiConsole.Write(new FigletText("Recovering failed").LeftAligned().Color(Color.Yellow));
                                AnsiConsole.WriteLine(msg);
                                cts.Cancel();
                                break;
                            }

                            AnsiConsole.Write(new FigletText("It's running again").LeftAligned().Color(Color.Green));
                        }
                    }
                }

                AnsiConsole.WriteLine("ReceiveThread: Stop receiving ComPrimitive...");
            }
        }
    }
}
