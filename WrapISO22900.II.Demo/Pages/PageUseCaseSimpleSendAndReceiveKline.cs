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
using System.Linq;
using ISO22900.II.OdxLikeComParamSets;
using Spectre.Console;

namespace ISO22900.II.Demo
{
    internal class PageUseCaseSimpleSendAndReceiveKline : Page
    {
        public PageUseCaseSimpleSendAndReceiveKline(AbstractPageControl program)
            : base("Use case simple send and receive use K-Line", program)
        {
        }

        public override void Display()
        {
            base.Display();
            var infoGrid = new Grid();
            var info = "Simple send and receive without task's for K-Line (e.g. 996 PCM)";
            infoGrid.AddColumn(new GridColumn().Centered());
            infoGrid.AddRow($"[yellow]{info}[/]");
            AnsiConsole.Write(infoGrid);

            var cllConfigPorscheKline = new LogicalLinkSettingZuffenhausenWithKline();

            using ( var api = DiagPduApiOneFactory.GetApi(
                       DiagPduApiHelper.FullLibraryPathFormApiShortName(AbstractPageControl.Preferences.GetSection("ApiVci:Api").Value),
                       AbstractPageControl.LoggerFactory) )
            {
                using ( var vci = api.ConnectVci(AbstractPageControl.Preferences.GetSection("ApiVci:Vci").Value) )
                {
                    //Define the protocol behavior
                    var dlcPinData = cllConfigPorscheKline.DlcPinData;
                    var busTypeName = cllConfigPorscheKline.BusTypeName;
                    //var busTypeName = "ISO_9141_2_UART";  //e.g. Softing
                    //var busTypeName = "ISO_14230_1_UART"; //e.g. Samtec  // Bosch
                    var protocolName = cllConfigPorscheKline.ProtocolName;

                    using ( var link = vci.OpenComLogicalLink(busTypeName, protocolName, dlcPinData.ToList()) )
                    {
                        cllConfigPorscheKline.LogicalLinkSettingPcm().SetUpLogicalLink(link);
                        link.Connect();
                        //Thread.Sleep(500);
                        var copStartcomm = link.StartCop(PduCopt.PDU_COPT_STARTCOMM, 1, 1, new byte[] { 0x81 });
                        var queueS = copStartcomm.WaitForCopResult();

                        var request = new byte[] { 0x1a, 0x90 };

                        for ( var i = 0x80; i <= 0xA0; i++ )
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

                        link.Disconnect();
                    }
                }
            }

            AnsiConsole.Console.ReadKey("Press [DodgerBlue1][[Enter]][/] to navigate home");
            AbstractPageControl.NavigateHome();
        }
    }
}
