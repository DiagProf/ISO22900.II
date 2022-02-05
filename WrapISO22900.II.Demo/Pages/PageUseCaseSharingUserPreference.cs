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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Spectre.Console;

namespace ISO22900.II.Demo
{
    internal class PageUseCaseSharingUserPreference : Page
    {

        public PageUseCaseSharingUserPreference(AbstractPageControl program)
            : base("Use case sharing user preference", program)
        {
        }

        public override void Display()
        {
            base.Display();
            var infoGrid = new Grid();
            var info =
                "Sharing User Preference to get a Vci or a ComChannel can be used if your application has the user preference already in the hand and errors in some Tasks don't play a roll.";
            infoGrid.AddColumn(new GridColumn().Centered());
            infoGrid.AddRow($"[yellow]{info}[/]");
            AnsiConsole.Write(infoGrid);

            var app = Task.Run(async () =>
            {
                var cts = new CancellationTokenSource();
                var ct = cts.Token;
               
                var lessImportantTask = new Task(  () =>
                {
                    var vci = DiagPduApiOneFactory.GetVci(
                        DiagPduApiHelper.FullyQualifiedLibraryFileNameFormShortName(AbstractPageControl.Preferences.GetSection("ApiVci:Api").Value), AbstractPageControl.Preferences.GetSection("ApiVci:Vci").Value);
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


                //The main use
                using ( var cll = DiagPduApiOneFactory.GetCll(
                    DiagPduApiHelper.FullyQualifiedLibraryFileNameFormShortName(AbstractPageControl.Preferences.GetSection("ApiVci:Api").Value),
                    AbstractPageControl.LoggerFactory, "", AbstractPageControl.Preferences.GetSection("ApiVci:Vci").Value) )
                {
                    cll.Connect();

                    lessImportantTask.Start();

                    var request = new byte[] { 0x22, 0xF1, 0x90 };

                    for ( var i = 0x80; i <= 0xA0; i++ )
                    {
                        request[2] = (byte)i;
                        using ( var cop = cll.StartCop(PduCopt.PDU_COPT_SENDRECV, 1, 1, request) )
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

                    cll.Disconnect();
                }

                cts.Cancel(); //stop less important task


                try
                {
                    if ( await Task.WhenAny(lessImportantTask, Task.Delay(2000)) == lessImportantTask )
                    {
                        // task completed within timeout

                    }
                    else
                    {
                        // timeout logic
                    }
                }
                catch ( Exception ex )
                {
                    AnsiConsole.WriteLine("Exception: " + ex.Message);
                    AnsiConsole.WriteLine("Task IsFaulted: " + lessImportantTask.IsFaulted);
                    foreach ( var inEx in lessImportantTask.Exception.InnerExceptions )
                    {
                        AnsiConsole.WriteLine("Task Inner Exception: " + inEx.Message);
                    }
                }
                finally
                {
                    cts.Dispose();
                    lessImportantTask.Dispose();
                }

            });

            try
            {
                app.Wait(60000); //timeout
               
            }
            catch ( AggregateException e )
            {
                AnsiConsole.WriteException(e.InnerException ?? e);
            }
            catch (Exception e)
            {
                AnsiConsole.WriteException(e);
            }
            finally
            {
                //important in the end, the Sys and VCI must be cleared away.
                new DiagPduApiOneFactory().Dispose();
                app.Dispose();
            }
            AnsiConsole.Console.ReadKey("Press [DodgerBlue1][[Enter]][/] to navigate home");
            AbstractPageControl.NavigateHome();
        }
    }
}
