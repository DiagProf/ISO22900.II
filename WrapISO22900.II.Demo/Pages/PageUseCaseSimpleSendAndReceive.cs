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
using System.Threading.Tasks;
using ISO22900.II;
using ISO22900.II.OdxLikeComParamSets;
using Microsoft.Extensions.Configuration;
using Spectre.Console;

namespace ISO22900.II.Demo
{
    internal class PageUseCaseSimpleSendAndReceive : Page
    {
        
        public PageUseCaseSimpleSendAndReceive(AbstractPageControl program)
            : base("Use case simple send and receive", program)
        {
        }

        public override void Display()
        {
            base.Display();
            var infoGrid = new Grid();
            var info = "Simple send and receive without task's";
            infoGrid.AddColumn(new GridColumn().Centered());
            infoGrid.AddRow($"[yellow]{info}[/]");
            AnsiConsole.Write(infoGrid);

            using ( var api = DiagPduApiOneFactory.GetApi(DiagPduApiHelper.FullyQualifiedLibraryFileNameFormShortName(AbstractPageControl.Preferences.GetSection("ApiVci:Api").Value), AbstractPageControl.LoggerFactory))
            {
                using ( var vci = api.ConnectVci(this.AbstractPageControl.Preferences.GetSection("ApiVci:Vci").Value) )
                {
                    //Define the protocol behavior
                    //These names (the strings) come from ODX or ISO 22900-2
                    var dlcPinData = new Dictionary<uint, string> { { 6, "HI" }, { 14, "LOW" } };
                    var busTypeName = "ISO_11898_2_DWCAN";
                    var protocolName = "ISO_15765_3_on_ISO_15765_2";

                    using ( var link = vci.OpenComLogicalLink(busTypeName, protocolName, dlcPinData.ToList()) )
                    {
                        var pcmCllConfig = new VehicleInfoSpecForTestEcu();
                        pcmCllConfig.SetUpLogicalLink(link);
                        link.Connect();

                        var request = new byte[] { 0x22, 0xF1, 0x92 };

                        //Use StartComm to start tester present behavior
                        using (var copStartComm = link.StartCop(PduCopt.PDU_COPT_STARTCOMM))
                        {
                            copStartComm.WaitForCopResult();
                        }

                        for ( var i = 0x80; i <= 0xFF; i++ )
                        {
                            request[2] = (byte)i;

                            //there's no reason for the copDelay here... just for demo
                            //it just makes communication pointlessly slow
                            //but in reality there are sometimes ECUs that need time between 2 requests with response (e.g. 2 consecutive 0x2E-Services)
                            //and CP_P3Phys is only active between 2 requests !without! response (not to be confused with CP_P3min for K-Line)
                            //in these very rare cases you can use a Pdu Copt.PDU COPT DELAY or make Thread.Sleep(xx) in the application
                            var copDelay = link.StartCop(PduCopt.PDU_COPT_DELAY, TimeSpan.FromMilliseconds(5));
                            

                            using (var cop = link.StartCop(PduCopt.PDU_COPT_SENDRECV,1,1,  request) )
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
                            copDelay.Dispose();
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
