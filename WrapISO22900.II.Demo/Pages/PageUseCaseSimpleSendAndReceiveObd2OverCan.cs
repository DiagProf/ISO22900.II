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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using ISO22900.II.OdxLikeComParamSets;
using ISO22900.II.OdxLikeComParamSets.TransportOrDataLinkLayer;
using Spectre.Console;

namespace ISO22900.II.Demo
{
    internal class PageUseCaseSimpleSendAndReceiveObd2OverCan : Page
    {
        
        public PageUseCaseSimpleSendAndReceiveObd2OverCan(AbstractPageControl program)
            : base("Use case simple send and receive OBD2 over CAN", program)
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




            using ( var api = DiagPduApiOneFactory.GetApi(DiagPduApiHelper.FullLibraryPathFormApiShortName(AbstractPageControl.Preferences.GetSection("ApiVci:Api").Value), AbstractPageControl.LoggerFactory))
            {
                using ( var vci = api.ConnectVci(this.AbstractPageControl.Preferences.GetSection("ApiVci:Vci").Value) )
                {
                    //Define the protocol behavior
                    //These names (the strings) come from ODX or ISO 22900-2
                    var busTypeName = "ISO_11898_2_DWCAN";
                    var protocolName = "ISO_OBD_on_ISO_15765_4"; //"ISO_15031_5"
                    var dlcPinData = new Dictionary<uint, string> { { 6, "HI" }, { 14, "LOW" } };

            

                    using (var link = vci.OpenComLogicalLink(busTypeName, protocolName, dlcPinData.ToList()))
                    {
                        bool isOBDonCAN = false;
                        

                        //Here we build the UniqueRespIdTable (is a List of PduEcuUniqueRespData) for OBD on CAN with 11–bit CAN Id size
                        //In case of functional addressing. The UniqueRespIdentifier becomes more interesting than usual in physical addressing
                        //because you can use it (the UniqueRespIdentifier) to find out the ECUs that have responded to the functional request

                        var ecuUniqueRespDatas = new List<PduEcuUniqueRespData>();
                        //for CAN OBD 11bit
                        for (byte i = 0; i < 8; i++)
                        {
                            ecuUniqueRespDatas.Add(new PduEcuUniqueRespData(uniqueRespIdentifier:  i+1u,   //<- this is the UniqueRespIdentifier
                            new List<PduComParam>
                            {
                                DiagPduApiComParamFactory.Create("CP_CanPhysReqExtAddr", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID),
                                DiagPduApiComParamFactory.Create("CP_CanPhysReqFormat", 5, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID),
                                DiagPduApiComParamFactory.Create("CP_CanPhysReqId", 0x7E0 + i, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID),
                                DiagPduApiComParamFactory.Create("CP_CanRespUSDTExtAddr", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID),
                                DiagPduApiComParamFactory.Create("CP_CanRespUSDTFormat", 5, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID),
                                DiagPduApiComParamFactory.Create("CP_CanRespUSDTId", 0x7E8 + i, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID),
                                DiagPduApiComParamFactory.Create("CP_CanRespUUDTExtAddr", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID),
                                DiagPduApiComParamFactory.Create("CP_CanRespUUDTFormat", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID),
                                DiagPduApiComParamFactory.Create("CP_CanRespUUDTId", 0xFFFFFFFF, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID)
                            }
                            ));
                        }



                        link.SetComParamValueViaGet("CP_CanFuncReqId", 0x7DF); //Set the functional request address if we use 11bit CAN ID
                        link.SetComParamValueViaGet("CP_Baudrate", 0); //0 -> 0 activates the testing of the baudrates specified in the CP_CanBaudrateRecord
                        link.SetComParamValueViaGet("CP_CanBaudrateRecord", new uint[] { 500000, 250000 });

                        link.SetUniqueRespIdTable(ecuUniqueRespDatas);
                        link.Connect();

                        //something like a ping to see if 11bit CAN ID are used for OBD.
                        using (var copPing = link.StartCop(PduCopt.PDU_COPT_SENDRECV, 1, -2, new byte[] { 0x01, 0x00 }))
                        {
                            var result = copPing.WaitForCopResult();

                            var responseString = string.Empty;


                            if (result.PduEventItemErrors().Count > 0)
                            {
                                foreach (var error in result.PduEventItemErrors())
                                {
                                    responseString += $"{error.ErrorCodeId}" + $" ({error.ExtraErrorInfoId})";
                                }
                                responseString = "Error: " + responseString + " 11Bit CAN is not working for OBD";
                            }
                            else
                            {
                                isOBDonCAN = true;
                                responseString = "11Bit CAN is working for OBD";
                            }
                            AnsiConsole.WriteLine($"{responseString}");
                        }

                        if (isOBDonCAN == false)
                        {
                            link.Disconnect();
                            ecuUniqueRespDatas.Clear();
                            //try with 29bit CAN id;
                            //for OBD on CAN with 29–bit CAN Id size

                            //Here we build the UniqueRespIdTable (is a List of PduEcuUniqueRespData) for OBD on CAN with 29–bit CAN Id size
                            //In case of functional addressing. The UniqueRespIdentifier becomes more interesting than usual in physical addressing
                            //because you can use it (the UniqueRespIdentifier) to find out the ECUs that have responded to the functional request
                            for (byte i = 0; i < 255; i++)
                            {
                                ecuUniqueRespDatas.Add(new PduEcuUniqueRespData(uniqueRespIdentifier: i+1u,   //<- this is the UniqueRespIdentifier
                                new List<PduComParam>
                                {
                                    DiagPduApiComParamFactory.Create("CP_CanPhysReqExtAddr", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID),
                                    DiagPduApiComParamFactory.Create("CP_CanPhysReqFormat", 7, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID),
                                    DiagPduApiComParamFactory.Create("CP_CanPhysReqId", 0x18DA0000 + ((i * 256) + 0xF1), PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID),
                                    DiagPduApiComParamFactory.Create("CP_CanRespUSDTExtAddr", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID),
                                    DiagPduApiComParamFactory.Create("CP_CanRespUSDTFormat", 7, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID),
                                    DiagPduApiComParamFactory.Create("CP_CanRespUSDTId",  0x18DAF100 + i, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID),
                                    DiagPduApiComParamFactory.Create("CP_CanRespUUDTExtAddr", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID),
                                    DiagPduApiComParamFactory.Create("CP_CanRespUUDTFormat", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID),
                                    DiagPduApiComParamFactory.Create("CP_CanRespUUDTId", 0xFFFFFFFF, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID)
                                }
                                ));
                            }

                            link.SetComParamValueViaGet("CP_CanFuncReqId", 0x18DB33F1); //Set the functional request address if we use 29bit CAN ID
                            link.SetComParamValueViaGet("CP_Baudrate", 0); //0 -> 0 activates the testing of the baudrates specified in the CP_CanBaudrateRecord
                            link.SetComParamValueViaGet("CP_CanBaudrateRecord", new uint[] { 500000, 250000 });
                            link.SetUniqueRespIdTable(ecuUniqueRespDatas);
                            link.Connect();

                            //Short ping to see if 29 bit CAN id work
                            using (var copPing = link.StartCop(PduCopt.PDU_COPT_SENDRECV, 1, -2, new byte[] { 0x01, 0x00 }))
                            {
                                var result = copPing.WaitForCopResult();

                                var responseString = string.Empty;


                                if (result.PduEventItemErrors().Count > 0)
                                {
                                    foreach (var error in result.PduEventItemErrors())
                                    {
                                        responseString += $"{error.ErrorCodeId}" + $" ({error.ExtraErrorInfoId})";
                                    }
                                    responseString = "Error: " + responseString + " 29Bit CAN is not working for OBD";
                                  
                                }
                                else
                                {
                                    isOBDonCAN = true;
                                    responseString = "29Bit CAN is working for OBD";
                                }
                                AnsiConsole.WriteLine($"{responseString}");

                            }

                        }






                        if (isOBDonCAN == true)
                        {
                            //if you like you can see which baudrate from CP_CanBaudrateRecord is used.
                            var resultBaudrate = ((PduComParamOfTypeUint)link.GetComParam("CP_Baudrate")).ComParamData;
                            AnsiConsole.WriteLine($"Used baudrate: {resultBaudrate}");


                            //Use StartComm to start tester present behavior
                            using (var copStartComm = link.StartCop(PduCopt.PDU_COPT_STARTCOMM, 1, -2, new byte[] { 0x01, 0x00 }))
                            {
                                copStartComm.WaitForCopResult();
                            }


                            var request = new byte[] { 0x01, 0x0C };
                            for (var i = 0x0C; i <= 0x20; i++)
                            {
                                request[1] = (byte)i;

                                using (var cop = link.StartCop(PduCopt.PDU_COPT_SENDRECV, 1, -2, request))
                                {
                                    var result = cop.WaitForCopResult();

                                    //The following evaluation is okay for this use case, but it should be noted that the order may be lost.
                                    //e.g. the correct order might be first PduEventItemInfo and then DataMsg
                                    var responseString = string.Empty;
                                    uint responseTime = 0;


                                    foreach (var evItemResult in result.PduEventItemResults())
                                    {
                                        var uniqueRespIdentifier = evItemResult.ResultData.UniqueRespIdentifier;

                                        responseString = $"ECU: {uniqueRespIdentifier}  Data: ";
                                        responseString += BitConverter.ToString(evItemResult.ResultData.DataBytes);
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
