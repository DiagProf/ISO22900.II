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
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Intrinsics.X86;
using System.Text;
using ISO22900.II.OdxLikeComParamSets;
using Spectre.Console;

namespace ISO22900.II.Demo
{
    internal class PageUseCaseSimpleSendAndReceiveWithEventDrivenCanTrace : Page
    {
        
        public PageUseCaseSimpleSendAndReceiveWithEventDrivenCanTrace(AbstractPageControl program)
            : base("Use case simple send and receive with event driven CAN Trace", program)
        {
        }

        public override void Display()
        {
            base.Display();
            var infoGrid = new Grid();
            var info = "Simple send and receive with event driven CAN Trace";
            var info2 = "I'm afraid not all D-PDU-API's can do that. I tested it with Softing(my best horse) and Actia for VW.";
            var info3 = "A real application you should be able to switch off the trace. Think of Bluetooth, it can quickly overflow when there is a lot of activity on the CAN-Bus.";
            infoGrid.AddColumn(new GridColumn().Centered());
            infoGrid.AddRow($"[yellow]{info}[/]");
            infoGrid.AddRow($"[yellow]{info2}[/]");
            infoGrid.AddRow($"[red]{info3}[/]");
            AnsiConsole.Write(infoGrid);

            using ( var api = DiagPduApiOneFactory.GetApi(DiagPduApiHelper.FullLibraryPathFormApiShortName(AbstractPageControl.Preferences.GetSection("ApiVci:Api").Value), AbstractPageControl.LoggerFactory))
            {
                using ( var vci = api.ConnectVci(this.AbstractPageControl.Preferences.GetSection("ApiVci:Vci").Value) )
                {
                    //Define the protocol behavior
                    //These names (the strings) come from ODX or ISO 22900-2
                    var dlcPinData = new Dictionary<uint, string> { { 6, "HI" }, { 14, "LOW" } };
                    var busTypeName = "ISO_11898_2_DWCAN";
                    var protocolName = "ISO_15765_3_on_ISO_15765_2";
                    var protocolNameForCanTrace = "ISO_11898_RAW";

                    //It is important that the link you want to work with is opened first and then the trace links.
                    //Because only the first link has full access to the physical bus parameters.
                    //If you want to change these parameters later (e.g. baud rate switching) there are problems if you are not the master
                    //link -> is the worker
                    using ( var link = vci.OpenComLogicalLink(busTypeName, protocolName, dlcPinData.ToList()) )
                    {
                        
                        
                        var cllCreateFlag = new PduFlagDataCllCreateFlag()
                        {
                            RawMode = true, //this causes the ComLogicalLink to be opened in RawMode
                            ChecksumMode = false,
                            MonitorLink =true,  //This is a special flag for the Softing D-PDU-API If you don't set it, you don't see what you are sending.
                                                //for this example that means I would't see the CAN-ID 0x7E0
                        };

                        //with cllCreateFlag we open the ComLogicalLink in RawMode now the CAN IDs are part of the actual data
                        //linkCanRawTrace -> is the tracer
                        using ( var linkCanRawTrace = vci.OpenComLogicalLink(busTypeName, protocolNameForCanTrace, dlcPinData.ToList(), cllCreateFlag ))
                        {
                            //setUp the link (the worker)
                            var pcmCllConfig = new VehicleInfoSpecForTestEcu();
                            pcmCllConfig.SetUpLogicalLink(link);

                            //The next 3 commands are only needed if I want to build a filter that pass only the diagnostic Can-Ids from the link (the worker).
                            link.SetUniqueRespIdTablePageOneUniqueRespIdentifier(0x4711); //this is a hack because the next function needs an id. I think i need something like "get UniqueRespIdentifier from first table" //ToDo for APIOne extension
                            //here we pick up the request and response address and use it as two response addresses for tracing
                            var canPhysReqId = link.GetUniqueIdComParamValue(0x4711, "CP_CanPhysReqId");
                            var canRespUSDTId = link.GetUniqueIdComParamValue(0x4711, "CP_CanRespUSDTId");


                            var traceUniqueResponseDatas = new List<PduEcuUniqueRespData>
                            {
                                //that lets all CAN-IDs pass
                                new PduEcuUniqueRespData(PduConst.PDU_ID_UNDEF, new List<PduComParam>
                                {
                                        //The next 3 should only be initialized and if possible so that the link cannot send anything.
                                        DiagPduApiComParamFactory.Create("CP_CanPhysReqFormat", 0xFF, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID),
                                        DiagPduApiComParamFactory.Create("CP_CanPhysReqId", 0xFF_FF_FF_FF, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID),
                                        DiagPduApiComParamFactory.Create("CP_CanPhysReqExtAddr", 0xFF, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID),

                                        //now let's set the receiving side
                                        DiagPduApiComParamFactory.Create("CP_CanRespUUDTFormat", 0xFF, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID), //0x00 -> normal, unsegmented, 11‐bit, without FC
                                        DiagPduApiComParamFactory.Create("CP_CanRespUUDTId", 0xFF_FF_FF_FF, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID), //we don't want to send anything
                                        DiagPduApiComParamFactory.Create("CP_CanRespUUDTExtAddr", 0xFF, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID),
                                })

                                /*
                                    //that only lets pass the CAN IDs from the from the link (the worker)

                                    //that looks at the request messages
                                    new PduEcuUniqueRespData('>', new List<PduComParam>
                                    {
                                        //The next 3 should only be initialized and if possible so that the link cannot send anything.
                                        DiagPduApiComParamFactory.Create("CP_CanPhysReqFormat", 0x00, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID),
                                        DiagPduApiComParamFactory.Create("CP_CanPhysReqId", 0xFF_FF_FF_FF, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID),
                                        DiagPduApiComParamFactory.Create("CP_CanPhysReqExtAddr", 0x00, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID),

                                        //now let's set the receiving side
                                        DiagPduApiComParamFactory.Create("CP_CanRespUUDTFormat", 0x00, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID), //0x00 -> normal, unsegmented, 11‐bit, without FC
                                        DiagPduApiComParamFactory.Create("CP_CanRespUUDTId", canPhysReqId, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID), //we don't want to send anything
                                        DiagPduApiComParamFactory.Create("CP_CanRespUUDTExtAddr", 0x00, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID),
                                    }),
                                    //that looks at the response messages
                                    new PduEcuUniqueRespData('<', new List<PduComParam>
                                    {
                                        //The next 3 should only be initialized and if possible so that the link cannot send anything.
                                        DiagPduApiComParamFactory.Create("CP_CanPhysReqFormat", 0x00, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID),
                                        DiagPduApiComParamFactory.Create("CP_CanPhysReqId", 0xFF_FF_FF_FF, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID),
                                        DiagPduApiComParamFactory.Create("CP_CanPhysReqExtAddr", 0x00, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID),

                                        //now let's set the receiving side
                                        DiagPduApiComParamFactory.Create("CP_CanRespUUDTFormat", 0x00, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID), //0x00 -> normal, unsegmented, 11‐bit, without FC
                                        DiagPduApiComParamFactory.Create("CP_CanRespUUDTId",  canRespUSDTId, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID), //we don't want to send anything
                                        DiagPduApiComParamFactory.Create("CP_CanRespUUDTExtAddr", 0x00, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID),
                                    })
                                */
                            };

                            //setup the linkCanRawTrace (the tracer)
                            linkCanRawTrace.SetUniqueRespIdTable(traceUniqueResponseDatas);
                            linkCanRawTrace.EventFired += LinkCanRawTrace_EventFired;
                            linkCanRawTrace.Connect();

                            //Connect link (the worker)
                            link.Connect();


                            //for tracing we need a ComPrimitive with certain properties
                            var copCtrlDataForTrace = new PduCopCtrlData 
                            {
                                //NumSendCycles = 0, and NumReceiveCycles = -1  -> Continuously look for received messages.
                                NumSendCycles = 0,
                                NumReceiveCycles = -1, 
                                TxFlag = new PduFlagDataTxFlag(),
                                PduExpectedResponseDatas = new[]
                                {
                                    new PduExpectedResponseData(PduExResponseType.PositiveResponse, 1,
                                        new MaskAndPatternBytes(Array.Empty<byte>(), Array.Empty<byte>()),
                                        new UniqueRespIds(Array.Empty<uint>()))
                                }
                            };

                            //Within this using statement there is nothing that the tracing setup needs.
                            using ( var comPrimTrace = linkCanRawTrace.StartCop(PduCopt.PDU_COPT_SENDRECV, Array.Empty<byte>(), copCtrlDataForTrace))
                            {


                                var request = new byte[] { 0x22, 0xF1, 0x92 };

                                //Use StartComm to start tester present behavior
                                using ( var copStartComm = link.StartCop(PduCopt.PDU_COPT_STARTCOMM) )
                                {
                                    copStartComm.WaitForCopResult();
                                }

                                for ( var i = 0x80; i <= 0xFF; i++ )
                                {
                                    request[2] = (byte)i;

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

                                        AnsiConsole.MarkupLine($"[white]{BitConverter.ToString(request)} | {responseString}  | {responseTime}µs[/]");
                                    }

                                }

                            }
                            linkCanRawTrace.EventFired -= LinkCanRawTrace_EventFired;
                            linkCanRawTrace.Disconnect(); //Stop Tracer
                        } //this (using end) is very important here because a continuously running ComPrimivtive must necessarily be disposed


                        link.Disconnect();
                    }
                }
            }

            AnsiConsole.Console.ReadKey("Press [DodgerBlue1][[Enter]][/] to navigate home");
            AbstractPageControl.NavigateHome();
        }

        private void LinkCanRawTrace_EventFired(object sender, PduEventItem e)
        {
            if (e.PduItemType == PduIt.PDU_IT_RESULT)
            {
                var result = ((PduEventItemResult)e).ResultData;
                string responseString;
                if (result.DataBytes.Length >= 4) 
                {
                    //we are in raw mode so canIds are in the date
                    uint canId = BitConverter.ToUInt32(result.DataBytes,0);
                   
                    if (BitConverter.IsLittleEndian)
                    {
                        canId = BinaryPrimitives.ReverseEndianness(canId);
                    }

                    if ( canId > 0x80_00_00_00 )
                    {
                        canId &= 0x1F_FF_FF_FF;
                        //responseString = $"29Bit CanId: 0x{canId:X8}  Data: ";
                        responseString = $"CanId: 0x{canId:X8}  Data: ";
                    }
                    else
                    {
                        //responseString = $"11Bit CanId: 0x{canId:X3}  Data: ";
                        responseString = $"CanId: 0x{canId:X3}  Data: ";
                    }


                    if (result.DataBytes.Length > 4)
                        responseString += BitConverter.ToString(result.DataBytes, 4);

                    AnsiConsole.MarkupLine($"[green]{responseString}[/]");
                }
            }
        }


    }
}
