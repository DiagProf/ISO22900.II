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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ISO22900.II.OdxLikeComParamSets;
using Spectre.Console;

namespace ISO22900.II.Demo
{
    internal class PageUseCaseSharingVciInstance : Page
    {
        public PageUseCaseSharingVciInstance(AbstractPageControl program)
            : base("Use case sharing user preference", program)
        {
        }

        public override void Display()
        {
            base.Display();
            var infoGrid = new Grid();
            var info = "Sharing Vci Instance to get a ComLogicalLink and VBatt and Igniton in another task";
            infoGrid.AddColumn(new GridColumn().Centered());
            infoGrid.AddRow($"[yellow]{info}[/]");
            AnsiConsole.Write(infoGrid);


            var app = Task.Run(async () =>
            {
                using ( var api =
                    DiagPduApiOneFactory.GetApi(
                        DiagPduApiHelper.FullLibraryPathFormApiShortName(AbstractPageControl.Preferences.GetSection("ApiVci:Api").Value)) )
                {
                    using ( var vci = api.ConnectVci(AbstractPageControl.Preferences.GetSection("ApiVci:Vci").Value) )
                    {
                        //prepare less important task
                        var cts = new CancellationTokenSource();
                        var ct = cts.Token;
                        var lessImportantTask = new Task(() =>
                        {
                            while ( !ct.IsCancellationRequested )
                            {
                                string ignitionState;
                                string vBat; //VBATT means Vehicle Battery Voltage
                                try
                                {
                                    vBat = $"VBATT: {(float) ( vci.MeasureBatteryVoltage() / 1000.0 ):00.00}";
                                    var temp = vci.IsIgnitionOn() ? "Yes" : "No";
                                    ignitionState = $"Ignition on: {temp}";
                                }
                                catch ( Iso22900IIException )
                                {
                                    // eat all Exceptions
                                    //vBat = "VBATT: ---";
                                    //ignitionState = "Ignition on: ---";
                                    break;
                                }

                                AnsiConsole.MarkupLine($"[Gray]{vBat}[/]");
                                AnsiConsole.MarkupLine($"[Gray]{ignitionState}[/]");
                                Thread.Sleep(100); //only for the show case never read this info so fast
                            }
                        }, ct, TaskCreationOptions.LongRunning | TaskCreationOptions.RunContinuationsAsynchronously);

                        //Define the protocol behavior
                        //These names (the strings) come from ODX or ISO 22900-2
                        var dlcPinData = new Dictionary<uint, string> { { 6, "HI" }, { 14, "LOW" } };
                        var busTypeName = "ISO_11898_2_DWCAN";
                        var protocolName = "ISO_15765_3_on_ISO_15765_2";


                        var ems = new VehicleInfoSpecForTestEcu();


                        using ( var link = vci.OpenComLogicalLink(busTypeName, protocolName, dlcPinData.ToList()) )
                        {
                            ems.SetUpLogicalLink(link); ;

                            //link.SetComParamValueViaGet("CP_P2Max", 900000);
                            //link.SetComParamValueViaGet("CP_RC21RequestTime", 200000);
                            //link.SetComParamValueViaGet("CP_P2Star", 4500000);
                            //link.SetComParamValueViaGet("CP_P3Phys", 50000);
                            //link.SetComParamValueViaGet("CP_TesterPresentMessage", new byte[] { 0x3E, 0x80 });
                            //link.SetComParam("CP_P2Max", 8000);

                            link.Connect();

                            lessImportantTask.Start();

                            var request = new byte[] { 0x22, 0xF1, 0x90 };

                            for ( var i = 0x80; i <= 0xA0; i++ )
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
                                        responseString = string.Join(",", result.DataMsgQueue().ConvertAll(bytes => { return BitConverter.ToString(bytes); }));
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

                            cts.Cancel(); //stop less important task

                            link.Disconnect();
                        }

                        var tasks = Task.WhenAll(lessImportantTask);
                        try
                        {
                            await tasks;
                        }
                        catch ( Exception ex )
                        {
                            AnsiConsole.WriteLine("Exception: " + ex.Message);
                            AnsiConsole.WriteLine("Task IsFaulted: " + tasks.IsFaulted);
                            foreach ( var inEx in tasks.Exception.InnerExceptions )
                            {
                                AnsiConsole.WriteLine("Task Inner Exception: " + inEx.Message);
                            }
                        }
                    }
                }
            });


            try
            {
                app.Wait(60000);
            }
            catch ( Exception ex )
            {
                AnsiConsole.WriteLine("Exception: " + ex.Message);
                AnsiConsole.WriteLine("Task IsFaulted: " + app.IsFaulted);
                foreach ( var inEx in app.Exception.InnerExceptions )
                {
                    AnsiConsole.WriteLine("Task Inner Exception: " + inEx.Message);
                }
            }


            AnsiConsole.Console.ReadKey("Press [DodgerBlue1][[Enter]][/] to navigate home");
            AbstractPageControl.NavigateHome();
        }
    }
}
