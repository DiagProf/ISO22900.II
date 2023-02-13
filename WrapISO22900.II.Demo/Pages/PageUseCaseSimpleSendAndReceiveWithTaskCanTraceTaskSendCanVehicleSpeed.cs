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
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using ISO22900.II.OdxLikeComParamSets;
using Spectre.Console;

namespace ISO22900.II.Demo
{
    internal class PageUseCaseSimpleSendAndReceiveWithTaskCanTraceTaskSendCanVehicleSpeed : Page
    {

        public PageUseCaseSimpleSendAndReceiveWithTaskCanTraceTaskSendCanVehicleSpeed(AbstractPageControl program)
            : base("Use case simple send and receive with Task for CAN Trace and Task for send CAN Vehicle Speed and a Link for ignition on", program)
        {
        }

        public override void Display()
        {
            base.Display();
            var infoGrid = new Grid();
            var info = "Simple send and receive with Task for CAN Trace and Task for send CAN Vehicle Speed and a Link for ignition on";
            var info2 = "I'm afraid not all D-PDU-API's can do that. I tested it with Softing(my best horse) and Actia for VW.";
            var info3 =
                "A real application you should be able to switch off the trace. Think of Bluetooth, it can quickly overflow when there is a lot of activity on the CAN-Bus.";
            infoGrid.AddColumn(new GridColumn().Centered());
            infoGrid.AddRow($"[yellow]{info}[/]");
            infoGrid.AddRow($"[yellow]{info2}[/]");
            infoGrid.AddRow($"[red]{info3}[/]");
            AnsiConsole.Write(infoGrid);

            using ( var api = DiagPduApiOneFactory.GetApi(
                       DiagPduApiHelper.FullLibraryPathFormApiShortName(AbstractPageControl.Preferences.GetSection("ApiVci:Api").Value),
                       AbstractPageControl.LoggerFactory) )
            {
                using ( var vci = api.ConnectVci(this.AbstractPageControl.Preferences.GetSection("ApiVci:Vci").Value) )
                {
                    //Define the protocol behavior
                    //These names (the strings) come from ODX or ISO 22900-2
                    var dlcPinData = new Dictionary<uint, string> { { 6, "HI" }, { 14, "LOW" } };
                    var busTypeName = "ISO_11898_2_DWCAN";
                    var protocolName = "ISO_15765_3_on_ISO_15765_2";


                    //It is important that the link you want to work with is opened first and then the trace links.
                    //Because only the first link has full access to the physical bus parameters.
                    //If you want to change these parameters later (e.g. baud rate switching) there are problems if you are not the master
                    //link -> is the worker
                    using ( var link = vci.OpenComLogicalLink(busTypeName, protocolName, dlcPinData.ToList()) )
                    using ( var ctsCanTraceTask = new CancellationTokenSource() )
                    {
                        //setUp the link (the worker)
                        var pcmCllConfig = new VehicleInfoSpecForTestEcu().LogicalLinkSettingForIpc();
                        pcmCllConfig.SetUpLogicalLink(link);

                        link.Connect();

                        //Trace init
                        var canTraceTask = new Task(() => CanTraceTaskFunc(vci, dlcPinData, ctsCanTraceTask.Token)
                            , ctsCanTraceTask.Token, TaskCreationOptions.LongRunning | TaskCreationOptions.RunContinuationsAsynchronously);
                        canTraceTask.Start();

                        //SendIgnitionOnEndless init
                        var sendIgnitionOnEndlessTask = new Task(() => SendIgnitionOnEndlessTaskFunc(vci, dlcPinData, ctsCanTraceTask.Token)
                            , ctsCanTraceTask.Token);
                        sendIgnitionOnEndlessTask.Start();

                        //SendChangeableCanMessagesTaskFunc
                        var sendChangeableCanMessagesTask = new Task(() => SendChangeableCanMessagesTaskFunc(vci, dlcPinData, ctsCanTraceTask.Token)
                            , ctsCanTraceTask.Token);
                        sendChangeableCanMessagesTask.Start();

                        var request = new byte[] { 0x22, 0xF1, 0x90 };

                        //Use StartComm to start tester present behavior
                        using ( var copStartComm = link.StartCop(PduCopt.PDU_COPT_STARTCOMM) )
                        {
                            copStartComm.WaitForCopResult();
                        }

                        for ( var i = 0x00; i <= 0xFF; i++ )
                        {
                            //request[2] = (byte)i;

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

                        link.Disconnect();

                        ctsCanTraceTask.Cancel();
                        Task.WaitAll(new []{ canTraceTask, sendIgnitionOnEndlessTask, sendChangeableCanMessagesTask });
                    }
                }
            }

            AnsiConsole.Console.ReadKey("Press [DodgerBlue1][[Enter]][/] to navigate home");
            AbstractPageControl.NavigateHome();
        }


        private void CanTraceTaskFunc(Module vci, Dictionary<uint, string> dlcPinData, CancellationToken ct)
        {

            var cllCreateFlag = new PduFlagDataCllCreateFlag()
            {
                RawMode = true, //this causes the ComLogicalLink to be opened in RawMode
                ChecksumMode = false,
                MonitorLink = true, //This is a special flag for the Softing D-PDU-API If you don't set it, you don't see what you are sending.

            };

            //with cllCreateFlag we open the ComLogicalLink in RawMode now the CAN IDs are part of the actual data
            //linkCanRawTrace -> is the tracer
            using ( var linkCanRawTrace = vci.OpenComLogicalLink("ISO_11898_2_DWCAN", "ISO_11898_RAW", dlcPinData.ToList(), cllCreateFlag) )
            {
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
                        DiagPduApiComParamFactory.Create("CP_CanRespUUDTFormat", 0xFF, PduPt.PDU_PT_UNUM32,
                            PduPc.PDU_PC_UNIQUE_ID), //0x00 -> normal, unsegmented, 11‐bit, without FC
                        DiagPduApiComParamFactory.Create("CP_CanRespUUDTId", 0xFF_FF_FF_FF, PduPt.PDU_PT_UNUM32,
                            PduPc.PDU_PC_UNIQUE_ID), //we don't want to send anything
                        DiagPduApiComParamFactory.Create("CP_CanRespUUDTExtAddr", 0xFF, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID),
                    })
                };

                //setup the linkCanRawTrace (the tracer)
                linkCanRawTrace.SetUniqueRespIdTable(traceUniqueResponseDatas);
                linkCanRawTrace.Connect();

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

                using ( var comPrimTrace = linkCanRawTrace.StartCop(PduCopt.PDU_COPT_SENDRECV, Array.Empty<byte>(), copCtrlDataForTrace) )
                {
                    while ( !ct.IsCancellationRequested )
                    {
                        try
                        {
                            var result = comPrimTrace.WaitForCopResult(ct);

                            foreach ( var e in result.RawQueue )
                            {
                                if ( e.PduItemType == PduIt.PDU_IT_RESULT )
                                {
                                    var resultData = ((PduEventItemResult)e).ResultData;
                                    var responseString = string.Empty;
                                    if ( resultData.DataBytes.Length >= 4 )
                                    {
                                        //For ISO 15765, SAE J1939 and ISO 11898, the first 4 bytes of the pCopData shall be the CAN ID (11-bit or 
                                        //29 - bit). If extended addressing is enabled(see D.2.1), then the byte following the CAN ID contains the extended address byte
                                        //we are in raw mode so canIds are in the data
                                        uint canId = BitConverter.ToUInt32(resultData.DataBytes, 0);

                                        if ( BitConverter.IsLittleEndian )
                                        {
                                            canId = BinaryPrimitives.ReverseEndianness(canId);
                                        }


                                        if (resultData.RxFlag.Can29BitId)
                                        {
                                            //responseString = $"29Bit CanId: 0x{canId:X8}  Data: ";
                                            responseString = $"CanId: 0x{canId:X8}  Data: ";
                                        }
                                        else
                                        {
                                            //responseString = $"11Bit CanId: 0x{canId:X3}  Data: ";
                                            responseString = $"CanId: 0x{canId:X3}  Data: ";
                                        }


                                        if ( resultData.DataBytes.Length > 4 )
                                            responseString += BitConverter.ToString(resultData.DataBytes, 4);

                                        AnsiConsole.MarkupLine($"[green]{responseString}[/]");
                                    }
                                }

                                //Yes, PduIt.PDU_IT_ERROR can also happen with just tracing, e.g. if you swapped CAN high-low lines or forgot the terminating resistor for CAN.
                                //You can display something or just ignore it
                                if ( e.PduItemType == PduIt.PDU_IT_ERROR )
                                {
                                    var resultError = ((PduEventItemError)e);
                                    var responseString = string.Empty;
                                    responseString += $"{resultError.ErrorCodeId}" + $" ({resultError.ExtraErrorInfoId})";
                                    AnsiConsole.MarkupLine($"[red]{responseString}[/]");
                                }
                            }
                        }
                        catch ( Iso22900IIException )
                        {
                            break;
                        }
                    }
                }
            }
        }

        private void SendIgnitionOnEndlessTaskFunc(Module vci, Dictionary<uint, string> dlcPinData, CancellationToken ct)
        {
            //in raw mode we can later include the CANIDs in the data to send
            var cllCreateFlag = new PduFlagDataCllCreateFlag()
            {
                RawMode = true, //this causes the ComLogicalLink to be opened in RawMode
                ChecksumMode = false,

            };

            //with cllCreateFlag we open the ComLogicalLink in RawMode now the CAN IDs are part of the actual data
            //linkCanRawTrace -> is the tracer
            using ( var canRawSendEndless = vci.OpenComLogicalLink("ISO_11898_2_DWCAN", "ISO_11898_RAW", dlcPinData.ToList(), cllCreateFlag) )
            {
                var offRespDatasUniqueResponseDatas = new List<PduEcuUniqueRespData>
                {
                    new PduEcuUniqueRespData(PduConst.PDU_ID_UNDEF, new List<PduComParam>
                    {
                        //The next 3 should only be initialized and if possible so that the link cannot send anything.
                        DiagPduApiComParamFactory.Create("CP_CanPhysReqFormat", 0xFF, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID),
                        DiagPduApiComParamFactory.Create("CP_CanPhysReqId", 0xFFFFFFFF, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID),
                        DiagPduApiComParamFactory.Create("CP_CanPhysReqExtAddr", 0xFF, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID),

                        //now let's set the receiving side
                        DiagPduApiComParamFactory.Create("CP_CanRespUUDTFormat", 0xFF, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID), //0x00 -> normal, unsegmented, 11‐bit, without FC
                        DiagPduApiComParamFactory.Create("CP_CanRespUUDTId", 0xFF_FF_FF_FF, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID), //we don't want to send anything
                        DiagPduApiComParamFactory.Create("CP_CanRespUUDTExtAddr", 0xFF, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID),
                    })
                };

                canRawSendEndless.SetUniqueRespIdTable(offRespDatasUniqueResponseDatas);
                canRawSendEndless.Connect();

                //for sending only we need a ComPrimitive with certain properties
                var copCtrlDataForTrace = new PduCopCtrlData
                {
                    //NumSendCycles = -1, and NumReceiveCycles = 0  -> Continuously send messages and ignore any responses.
                    NumSendCycles = -1,
                    NumReceiveCycles = 0,
                    Time = 100, // important here we say that the Msg from the D-PDU-API should be sent automatically every 100ms
                    TxFlag = new PduFlagDataTxFlag()
                    {
                        Can29Bit = false, 
                        Iso15765AddrType = false,
                        Iso15765FramePad = false,
                    },
                    PduExpectedResponseDatas = new[]
                    {
                        new PduExpectedResponseData(PduExResponseType.PositiveResponse, 1,
                            new MaskAndPatternBytes(Array.Empty<byte>(), Array.Empty<byte>()),
                            new UniqueRespIds(Array.Empty<uint>()))
                    }
                };

                //CAN-Messages
                var msg1 = new byte[]
                {
                    0x00, 0x00, 0x02, 0x9B, //CAN Id (we use 11Bit) 
                    0x00, 0x70, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  // 8 Data bytes 
                };

                var msg2 = new byte[]
                {
                    0x00, 0x00, 0x04, 0xD7, //CAN Id (we use 11Bit)   
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  // 8 Data bytes 
                };

                //Within this using statement there is nothing that the tracing setup needs.
                using ( var IgnitionOnMsg1 = canRawSendEndless.StartCop(PduCopt.PDU_COPT_SENDRECV, msg1, copCtrlDataForTrace) )
                using ( var IgnitionOnMsg2 = canRawSendEndless.StartCop(PduCopt.PDU_COPT_SENDRECV, msg2, copCtrlDataForTrace) )
                {
                    //nothing to do here the D - PDU API does the job. As long as the link is open
                    while ( !ct.IsCancellationRequested )
                    {
                       Thread.Sleep(1000);     
                    }
                    //dispose cleans everything up
                }
            }
        }


        private void SendChangeableCanMessagesTaskFunc(Module vci, Dictionary<uint, string> dlcPinData, CancellationToken ct)
        {
            //in raw mode we can later include the CANIDs in the data to send
            var cllCreateFlag = new PduFlagDataCllCreateFlag()
            {
                RawMode = true, //this causes the ComLogicalLink to be opened in RawMode
                ChecksumMode = false,

            };

            //with cllCreateFlag we open the ComLogicalLink in RawMode now the CAN IDs are part of the actual data
            //linkCanRawTrace -> is the tracer
            using (var canRawSendlink = vci.OpenComLogicalLink("ISO_11898_2_DWCAN", "ISO_11898_RAW", dlcPinData.ToList(), cllCreateFlag))
            {
                var offRespDatasUniqueResponseDatas = new List<PduEcuUniqueRespData>
                {
                    new PduEcuUniqueRespData(PduConst.PDU_ID_UNDEF, new List<PduComParam>
                    {
                        //The next 3 should only be initialized and if possible so that the link cannot send anything.
                        DiagPduApiComParamFactory.Create("CP_CanPhysReqFormat", 0xFF, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID),
                        DiagPduApiComParamFactory.Create("CP_CanPhysReqId", 0xFFFFFFFF, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID),
                        DiagPduApiComParamFactory.Create("CP_CanPhysReqExtAddr", 0xFF, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID),

                        //now let's set the receiving side
                        DiagPduApiComParamFactory.Create("CP_CanRespUUDTFormat", 0xFF, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID), //0x00 -> normal, unsegmented, 11‐bit, without FC
                        DiagPduApiComParamFactory.Create("CP_CanRespUUDTId", 0xFF_FF_FF_FF, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID), //we don't want to send anything
                        DiagPduApiComParamFactory.Create("CP_CanRespUUDTExtAddr", 0xFF, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID),
                    })
                };

                canRawSendlink.SetUniqueRespIdTable(offRespDatasUniqueResponseDatas);
                canRawSendlink.Connect();

                //for sending only we need a ComPrimitive with certain properties
                var copCtrlData = new PduCopCtrlData
                {
                    //NumSendCycles = 1, and NumReceiveCycles = 0  -> send messages 
                    NumSendCycles = 1,
                    NumReceiveCycles = 0,
                    TxFlag = new PduFlagDataTxFlag()
                    {
                        Can29Bit = false,
                        Iso15765AddrType = false,
                        Iso15765FramePad = false,
                    },
                    PduExpectedResponseDatas = new[]
                    {
                        new PduExpectedResponseData(PduExResponseType.PositiveResponse, 1,
                            new MaskAndPatternBytes(Array.Empty<byte>(), Array.Empty<byte>()),
                            new UniqueRespIds(Array.Empty<uint>()))
                    }
                };

                while ( !ct.IsCancellationRequested )
                {
                   
                    //CAN-Messages
                    var msg = new byte[]
                    {
                        0x00, 0x00, 0x02, 0x95, //CAN Id (we use 11Bit) 
                        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x16, 0x63, // 8 Data bytes 
                    };



                    //Within this using statement there is nothing that the tracing setup needs.
                    using ( var Msg = canRawSendlink.StartCop(PduCopt.PDU_COPT_SENDRECV, msg, copCtrlData) )
                    {
                        Msg.WaitForCopResult(ct);

                        //ComPrimitive of type delay doesn't seem to work everywhere (for every protocol and every API)
                        //that's why we use Thread.Sleep
                        Thread.Sleep(100);

                        //using (var delay = canRawSendlink.StartCop(PduCopt.PDU_COPT_DELAY, TimeSpan.FromMilliseconds(100)))
                        //{
                        //    delay.WaitForCopResult(ct);
                        //}

                    }
                }
            }
        }

    }
}
