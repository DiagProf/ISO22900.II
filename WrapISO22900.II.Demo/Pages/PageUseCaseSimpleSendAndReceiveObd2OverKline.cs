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
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json.Serialization;
using Spectre.Console;

namespace ISO22900.II.Demo
{
    internal class PageUseCaseSimpleSendAndReceiveObd2OverKline : Page
    {
        public PageUseCaseSimpleSendAndReceiveObd2OverKline(AbstractPageControl program)
            : base("Use case simple send and receive OBD2 over K-Line", program)
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

            //for K-Line and OBD there are 3 options:
            //1. Fast wake up (is an impulse of 25 ms low and 25 ms high and then the message C1 33 F1 81 66 sent with fixed baudrate 10400.)
            //      e.g. (bytes are row trace! also shows what the D-PDU API encapsulates) 
            //      c1 33 f1 81 66
            //      83 f1 10 c1 e9 8f bd // Keybytes 8F E9 (but but consider!!! LSB first) from ECU 10 
            //      83 f1 18 c1 e9 8f c5 // Keybytes 8F E9 (but but consider!!! LSB first) from ECU 18 
            //      c2 33 f1 01 00 e7 // Mode 01 PID 00
            //      86 f1 10 41 00 be 3f e8 11 be
            //      86 f1 18 41 00 80 00 00 01 51
            //
            //     If the fast wake up was successful, only protocol ISO 14230-4 can arise, but never ISO 9141-2
            //
            //2. Wake up with 5 Baud (here the OBD address 0x33 is transmitted with 5 bits per second (therefore 5 baud wake up) on the K-line).
            //      An ECU that feels addressed responds with 8 alternating bits (that's why you always see the 0x55 in traces).
            //      The VCI calculates the baud rate from these alternating bits. So the baud rate is not fixed!
            //      e.g. (bytes are row trace! also shows what the D - PDU API encapsulates)
            //      33 // send with 5 Baud
            //      55 // syncByte 
            //      08 08 // Keybytes 08 08 (but but consider!!! LSB first)
            //      f7 // VCI sends the negated version of the last keybyte (~08) as ack
            //      cc // ECU sends the negated version of the 5 baud address (~33)
            //      68 6a f1 01 00 c4 // Mode 01 PID 00
            //      48 6b 10 41 00 be 1f b8 11 aa
            //   
            //      Keybytes: 0x0808 or 0x9494 mean protocol iso 9141-2
            //   
            //3. Option is like the option 2. but the Keybytes are different and therefore a different protocol is used.
            //      e.g. (bytes are row trace! also shows what the D - PDU API encapsulates)
            //      33 // send with 5 Baud
            //      55 // syncByte 
            //      e9 8f // Keybytes 8F E9 (but but consider!!! LSB first)
            //      70 // VCI sends the negated version of the last keybyte (~8f) as ack
            //      cc // ECU sends the negated version of the 5 baud address (~33)
            //      c2 33 f1 01 00 e7 // Mode 01 PID 00
            //      86 f1 10 41 00 be 3f e8 11 be
            //      
            //      Keybytes: 0x8FE9,0x8F6B,0x8F6D,0x8FEF mean protocol iso 9141-2
            // 
            //  What important to understand is that with option 2 and 3, the D-PDU API can simply switch the parameters in UniqueRespIdTable itself.
            //  but for the request parameters we have to do it


            using ( var api = DiagPduApiOneFactory.GetApi(
                       DiagPduApiHelper.FullLibraryPathFormApiShortName(AbstractPageControl.Preferences.GetSection("ApiVci:Api").Value),
                       AbstractPageControl.LoggerFactory) )
            {
                using ( var vci = api.ConnectVci(this.AbstractPageControl.Preferences.GetSection("ApiVci:Vci").Value) )
                {
                    //Define the protocol behavior
                    //These names (the strings) come from ODX or ISO 22900-2
                    var busTypeName = "ISO_9141_2_UART";
                    var protocolName = "ISO_OBD_on_K_Line";
                    var dlcPinData = new Dictionary<uint, string> { { 7, "K" }, { 15, "L" } };


                    using ( var link = vci.OpenComLogicalLink(busTypeName, protocolName, dlcPinData.ToList()) )
                    {
                        bool isOBDonKline = false;

                        #region FastWakeUp

                        //Try Fast Wakeup first. If Fast Wakeup succeeds, the result can only be ISO 14230-4 protocol
                        link.SetComParamValueViaGet("CP_Baudrate", 10400);
                        link.SetComParamValueViaGet("CP_InitializationSettings", 2); //2 fast init
                        link.SetComParamValueViaGet("CP_RequestAddrMode", 2);
                        link.SetComParamValueViaGet("CP_HeaderFormatKW", 4);
                        link.SetComParamValueViaGet("CP_FuncReqFormatPriorityType", 0xC0);
                        link.SetComParamValueViaGet("CP_FuncReqTargetAddr", 0x33);
                        link.SetComParamValueViaGet("CP_TesterSourceAddress", 0xF1);

                        //TesterPresent behavior
                        //0 = Send on periodic interval defined by CP_TesterPresentTime (periodically independent of other requests)
                        //1 = Send when bus has been idle for CP_TesterPresentTime(after the last request)
                        link.SetComParamValueViaGet("CP_TesterPresentSendType", 1); 
                    
                        link.SetComParamValueViaGet("CP_TesterPresentMessage", new byte[] { 0x01, 0x00 });
                        link.SetComParamValueViaGet("CP_TesterPresentReqRsp", 1); //0 = no response, 1 = response expected
                        link.SetComParamValueViaGet("CP_TesterPresentExpPosResp", new byte[] { 0x41, 0x00 });
                        link.SetComParamValueViaGet("CP_TesterPresentExpNegResp", new byte[] { 0x7F, 0x01 });
                        link.SetComParamValueViaGet("CP_TesterPresentTime", 2000000);

                        var ecuUniqueRespDatas = new List<PduEcuUniqueRespData>();
                        for ( uint i = 0; i < 256; i++ )
                        {
                            ecuUniqueRespDatas.Add(new PduEcuUniqueRespData(uniqueRespIdentifier: i, //<- this is the UniqueRespIdentifier
                                new List<PduComParam>
                                {
                                    DiagPduApiComParamFactory.Create("CP_EcuRespSourceAddress", i, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID),
                                    DiagPduApiComParamFactory.Create("CP_FuncRespFormatPriorityType", 0x80, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID),
                                    DiagPduApiComParamFactory.Create("CP_FuncRespTargetAddr", 0xF1, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID),
                                    DiagPduApiComParamFactory.Create("CP_PhysRespFormatPriorityType", 0xFF, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID),
                                }
                            ));
                        }

                        link.SetUniqueRespIdTable(ecuUniqueRespDatas);
                        link.Connect();

                        //Use StartComm to start tester present behavior
                        using ( var copStartComm = link.StartCop(PduCopt.PDU_COPT_STARTCOMM, 1, -2, new byte[] { 0x81 }) )
                        {
                            var result = copStartComm.WaitForCopResult();

                            var responseString = string.Empty;


                            if ( result.PduEventItemErrors().Count > 0 )
                            {
                                foreach ( var error in result.PduEventItemErrors() )
                                {
                                    responseString += $"{error.ErrorCodeId}" + $" ({error.ExtraErrorInfoId})";
                                }

                                responseString = "Error: " + responseString + " ISO 14230-4 with fast wake up is not working for OBD";
                            }
                            else
                            {
                                var listOfKeyBytes = GetListOfKeyBytes(result);
                                if ( IsListOfKeyBytesIso14230_4(listOfKeyBytes) )
                                {
                                    isOBDonKline = true;
                                    responseString = "ISO 14230-4 with fast wake up is working for OBD";
                                }
                                else
                                {
                                    isOBDonKline = false;
                                    responseString = "You should never come here. ISO 14230-4 with fast wake up is not working for OBD";
                                }
                            }

                            AnsiConsole.WriteLine($"{responseString}");
                        }

                        #endregion


                        #region 5BaudWakeUp

                        if ( isOBDonKline == false )
                        {
                            link.Disconnect();

                            //try with 5Baud wake up
                            //just change some settings, the rest can be taken over by fastinit
                            link.SetComParamValueViaGet("CP_5BaudAddressFunc", 0x33); //Set the functional request address if we use 11bit CAN ID
                            link.SetComParamValueViaGet("CP_5BaudMode", 0);
                            link.SetComParamValueViaGet("CP_InitializationSettings", 1); //1 5Baud init

                            link.Connect();

                            //Use StartComm to start tester present behavior
                            using ( var copStartComm = link.StartCop(PduCopt.PDU_COPT_STARTCOMM, 0, -2, new byte[] {}) )
                            {
                                var result = copStartComm.WaitForCopResult();

                                var responseString = string.Empty;

                                if ( result.PduEventItemErrors().Count > 0 )
                                {
                                    foreach ( var error in result.PduEventItemErrors() )
                                    {
                                        responseString += $"{error.ErrorCodeId}" + $" ({error.ExtraErrorInfoId})";
                                    }

                                    responseString = "Error: " + responseString + " ISO 14230-4 with 5Baud wake up is not working for OBD";
                                }
                                else
                                {
                                    var listOfKeyBytes = GetListOfKeyBytes(result);
                                    if ( IsListOfKeyBytesIso14230_4(listOfKeyBytes) )
                                    {
                                        isOBDonKline = true;
                                        responseString = "ISO 14230-4 with 5Baud wake up is working for OBD";
                                    }
                                    else if ( IsListOfKeyBytesIso9141_2(listOfKeyBytes) )
                                    {
                                        //using (var copRestore = link.StartCop(PduCopt.PDU_COPT_RESTORE_PARAM))
                                        //{
                                        //    copRestore.WaitForCopResult();
                                        //}

                                        //the keybytes say ISO9141-2 so we have to adjust a few request parameters.
                                        //The response parameters were automatically adjusted.This is because protocol string "ISO_OBD_on_K_Line" was used.
                                        link.SetComParamValueViaGet("CP_FuncReqFormatPriorityType", 0x68);
                                        link.SetComParamValueViaGet("CP_FuncReqTargetAddr", 0x6A);
                                        //we need PduCopt.PDU_COPT_UPDATEPARAM because we are changing an active link
                                        using ( var copUpdate = link.StartCop(PduCopt.PDU_COPT_UPDATEPARAM) )
                                        {
                                            copUpdate.WaitForCopResult();
                                        }

                                        isOBDonKline = true;
                                        responseString = "ISO 9141-2 with 5Baud wake up is working for OBD";
                                    }
                                    else
                                    {
                                        isOBDonKline = false;
                                        responseString = "No OBD on K-line";
                                    }
                                }

                                AnsiConsole.WriteLine($"{responseString}");
                            }
                        }

                        #endregion

                        if ( isOBDonKline == true )
                        {
                            //if you like you can see which baudrate is used. Because at 5 baud wakeup the ECUs baud rate is determined.
                            var resultBaudrate = ((PduComParamOfTypeUint)link.GetComParam("CP_Baudrate")).ComParamData;
                            AnsiConsole.WriteLine($"Used baudrate: {resultBaudrate}");


                            var request = new byte[] { 0x01, 0x00 };
                            for ( var i = 0x00; i < 0x20; i++ )
                            {
                                request[1] = (byte)i;

                                using ( var cop = link.StartCop(PduCopt.PDU_COPT_SENDRECV, 1, -2, request) )
                                {
                                    var result = cop.WaitForCopResult();

                                    //The following evaluation is okay for this use case, but it should be noted that the order may be lost.
                                    //e.g. the correct order might be first PduEventItemInfo and then DataMsg
                                    var responseString = string.Empty;
                                    uint responseTime = 0;


                                    foreach ( var evItemResult in result.PduEventItemResults() )
                                    {
                                        var uniqueRespIdentifier = evItemResult.ResultData.UniqueRespIdentifier;

                                        responseString += $" ECU: 0x{uniqueRespIdentifier:X02}  Data: ";
                                        responseString += BitConverter.ToString(evItemResult.ResultData.DataBytes);
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

        private List<ushort> GetListOfKeyBytes(ComPrimitiveResult result)
        {
            var listOfKeyBytes = new List<ushort>(); //List because more than one ECU can respond
            foreach ( var evItemResult in result.PduEventItemResults() )
            {
                AnsiConsole.WriteLine($"KeyBytes Raw:{BitConverter.ToString(evItemResult.ResultData.DataBytes)}");

                if ( evItemResult.ResultData.DataBytes.Length == 2 )
                {
                    listOfKeyBytes.Add((ushort)(evItemResult.ResultData.DataBytes[1] * 256 + evItemResult.ResultData.DataBytes[0]));
                    AnsiConsole.WriteLine(
                        $"KeyBytes right order:{(ushort)(evItemResult.ResultData.DataBytes[1] * 256 + evItemResult.ResultData.DataBytes[0]):X04}");
                }
            }

            return listOfKeyBytes;
        }

        private bool IsListOfKeyBytesIso14230_4(List<ushort> listOfKeyBytes)
        {
            var isListOfKeyBytesIso142304 = false;

            //If multiple ECUs in ONE vehicle support OBD they must use the same protocol.
            //that's why the test could look different here. But I used to do it like this.
            foreach ( var keyBytes in listOfKeyBytes )
            {
                switch ( keyBytes )
                {
                    case 0x8FE9:
                    case 0x8F6B:
                    case 0x8F6D:
                    case 0x8FEF:
                    {
                        isListOfKeyBytesIso142304 = true;
                    }
                        break;
                    default:
                        break;
                }
            }

            return isListOfKeyBytesIso142304;
        }

        private bool IsListOfKeyBytesIso9141_2(List<ushort> listOfKeyBytes)
        {
            var isListOfKeyBytesIso91412 = false;

            //If multiple ECUs in ONE vehicle support OBD they must use the same protocol.
            //that's why the test could look different here. But I used to do it like this.
            foreach ( var keyBytes in listOfKeyBytes )
            {
                switch ( keyBytes )
                {
                    case 0x0808:
                    case 0x9494:
                    {
                        isListOfKeyBytesIso91412 = true;
                    }
                        break;
                    default:
                        break;
                }
            }

            return isListOfKeyBytesIso91412;
        }
    }
}