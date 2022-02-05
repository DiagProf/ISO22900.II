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
using System.Threading;
using Spectre.Console;

namespace ISO22900.II.Demo
{
    internal class PageUseCaseRecoverVciAfterVciLost : Page
    {
        public PageUseCaseRecoverVciAfterVciLost(AbstractPageControl program)
            : base("Use case recover VCI after VCI lost.", program)
        {
        }

        public override void Display()
        {
            base.Display();
            var infoGrid = new Grid();
            var info = "Read battery voltage and ignition state from VCI";
            infoGrid.AddColumn(new GridColumn().Centered());
            infoGrid.AddRow($"[yellow]{info}[/]");
            infoGrid.AddRow("Disconnect the VCI (e.g.power off or unplug the usb cable)");
            infoGrid.AddRow("Or break the loop with a keystroke");


            var tableBattIgn = new Table().AddColumns("[b]Battery voltage[/]", "[b]Ignition state[/]").LeftAligned();
            tableBattIgn.AddRow(string.Empty, string.Empty);

            infoGrid.AddEmptyRow();
            infoGrid.AddRow(tableBattIgn);
            infoGrid.AddEmptyRow();

           
            var tableInfo = new Table().Border(TableBorder.None).AddColumn(string.Empty).HideHeaders().LeftAligned()
                .AddRow(new FigletText("Running").LeftAligned().Color(Color.Green));
            infoGrid.AddRow(tableInfo);
     

            using ( var api = DiagPduApiOneFactory.GetApi(
                       DiagPduApiHelper.FullyQualifiedLibraryFileNameFormShortName(AbstractPageControl.Preferences.GetSection("ApiVci:Api").Value),
                       AbstractPageControl.LoggerFactory, ApiModifications.IGNITION_FIX | ApiModifications.VOLTAGE_FIX) )
            {
                using ( var vci = api.ConnectVci(AbstractPageControl.Preferences.GetSection("ApiVci:Vci").Value) )
                {
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
                                var batteryVoltage = vci.MeasureBatteryVoltage();
                                var isIgnitionOn = vci.IsIgnitionOn();

                                tableBattIgn.Rows.RemoveAt(0);
                                tableBattIgn.AddRow($"{batteryVoltage}", $"{isIgnitionOn}");
                                ctx.Refresh();
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
                                    tableInfo.AddRow("Press [Red][[Enter]][/] to start TryToRecover function or [Red][[any]][/] other key to exit");
                                    ctx.Refresh();

                                    if (Console.ReadKey(true).Key != ConsoleKey.Enter)
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
                                    if (!vci.TryToRecover(out var msg))
                                    {
                                        tableInfo.Rows.Clear();
                                        tableInfo.AddRow(new FigletText("Recovering failed").LeftAligned().Color(Color.Yellow));
                                        tableInfo.AddRow(new Text(msg));
                                        ctx.Refresh();

                                        //here a real app should return to a point where no VCI connection is needed
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
                }
            }

            AnsiConsole.Console.ReadKey("Press [DodgerBlue1][[Enter]][/] to navigate home");
            AbstractPageControl.NavigateHome();
        }
    }
}
