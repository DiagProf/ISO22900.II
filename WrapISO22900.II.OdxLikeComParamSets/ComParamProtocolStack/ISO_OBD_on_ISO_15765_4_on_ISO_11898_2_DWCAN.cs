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

using ISO22900.II.OdxLikeComParamSets.ApplicationLayer;
using ISO22900.II.OdxLikeComParamSets.PhysicalLayer;
using ISO22900.II.OdxLikeComParamSets.TransportOrDataLinkLayer;

namespace ISO22900.II.OdxLikeComParamSets
{
    public abstract class ISO_OBD_on_ISO_15765_4_on_ISO_11898_2_DWCAN : ProtocolStack
    {
        protected sealed override ISO_11898_2_DWCAN Bus { get; } = new();
        protected sealed override ISO_15765_4 Tpl { get; }// = new();
        protected sealed override ISO_OBD App { get; } = new();

        protected ISO_OBD_on_ISO_15765_4_on_ISO_11898_2_DWCAN(HashRuleUniqueRespIdentifierFromCpEcuLayerShortName hashAlgo)
        {
            Tpl = new ISO_15765_4(hashAlgo);
            InitializeAllComParams();
        }
        protected void InitializeAllComParams()
        {
            // Hardware-Canbus-Timing
            Bus.CP_Baudrate = 500000; //Unit bps (bits per second)
            Bus.CP_BitSamplePoint = 80; //Unit % of Bit-Time (0-100)
            Bus.CP_SamplesPerBit = 0; //0 = 1 Sample point, 1 = 3 Sample point
            Bus.CP_SyncJumpWidth = 15; //SJW % of Bit-Time (0-100)
            Bus.CP_ListenOnly = 0;
            Bus.CP_TerminationType = 3; //0 = No termination, 1 = AC termination, 2 = 60 Ohm termination,
                                        //3 = 120 Ohm termination, 4 = SWCAN termination
            Bus.CP_CanBaudrateRecord = new uint[] { 500000, 250000 };

            //For CANFD
            Bus.CP_CANFDBaudrate = 0;
            Bus.CP_CANFDBitSamplePoint = 80;
            Bus.CP_CANFDSyncJumpWidth = 15;


            // Used send type for Requests and Tester Present
            Tpl.CP_RequestAddrMode = 2; //1 = physical, 2 = functional  //for the request 
            App.CP_TesterPresentAddrMode = 1; //0 = physical, 1 = functional  //for the Tester Present message

            // Formatbytes: NormalAdr 11bit = #05, NormalAdr 29bit = #07, ExtendAdr 11bit = #0D, ExtendAdr 29bit = #0F, etc.
            Tpl.CP_CanFuncReqFormat = 0x05; //Functional Requests: defines ID type and address mode
            Tpl.CP_CanFuncReqId = 0x7DF; //0-1FFFFFFF  Functional Request-ID
            Tpl.CP_CanFuncReqExtAddr = 0x00; //0-FF Extended address

            Tpl.CP_CanPhysReqFormat = 0x05; //Physical Requests: defines ID type and address mode
            Tpl.CP_CanPhysReqId = 0x7E0; //0-1FFFFFFF Functional Request-ID          
            Tpl.CP_CanPhysReqExtAddr = 0x00; //0-FF Extended address

            Tpl.CP_CanRespUSDTFormat = 0x05; //0-3F Response Filter1: defines ID type and address mode
            Tpl.CP_CanRespUSDTId = 0x7E8; //0-1FFFFFFF Functional Request-ID          
            Tpl.CP_CanRespUSDTExtAddr = 0x00; //0-FF Extended address

            Tpl.CP_CanRespUUDTFormat = 0x00; //0-3F Response Filter2: defines ID type and address mode
            Tpl.CP_CanRespUUDTId = 0xFFFFFFFF; //0-1FFFFFFF Functional Request-ID
            Tpl.CP_CanRespUUDTExtAddr = 0x00; //0-FF Extended address


            // Timing and ECU Bugfixing(Workarounds) for Transportlayer
            Tpl.CP_As = 25000; //0-20000000us 
            Tpl.CP_Bs = 75000; //0-20000000us
            Tpl.CP_Ar = 25000; //0-20000000us
            Tpl.CP_Cr = 150000; //0-20000000us
            Tpl.CP_Br = 0; //0-20000000us
            Tpl.CP_Cs = 0; //0-20000000us
            Tpl.CP_BlockSize = 0; //0-FF (BS) for sent FlowControlFrame (FC)
            Tpl.CP_BlockSizeOverride = 0xFFFF; //0-FFFF,  0 – 0xFFFE = fixed Block Size, 0xFFFF = Use the value reported by the vehicle
            Tpl.CP_StMin = 0x00; //0-FF for sent FlowControlFrame (FC)
            Tpl.CP_StMinOverride = 0xFFFFFFFF; //0-FFFFFFFF, 0 – 0xFFFFFFFEus fixed STmin in us independent by FC, 0xFFFFFFFF = Use FC-STmin
            Tpl.CP_CanMaxNumWaitFrames = 0; //0-1027 Maximum Number of waitframes
            Tpl.CP_CanFirstConsecutiveFrameValue = 1; //0-F First SequenceNo in sent consecutive frames (default = 1) (shit ECUs need 0)
            Tpl.CP_CanFillerByteHandling = 1; //Padding on / off (0 = off, 1 = on) Attention, the format byte can overwrite e.g. CP_CanFuncReqFormat
            Tpl.CP_CanFillerByte = 0x00; // 0-FF Padding byte
            Tpl.CP_SendRemoteFrame = 0;
            Tpl.CP_CanDataSizeOffset = 0;
            Tpl.CP_CANFDTxMaxDataLength = 0;
           


            // Timing for Application
            App.CP_P2Min = 0; // After the request, the interface shall be capable of handling an immediate response
            App.CP_P2Max = 50_000;   //150000     //0-125000000us TimeOut Request -> Response
            App.CP_P2Star = 5_000_000; //0-655350000us TimeOut Request -> Response after negative response 78
            App.CP_P3Func = 150_000; //0-125000000us Minimum waiting time between two functional requests 
            App.CP_P3Phys = 150_000; //0-125000000us Minimum waiting time between two physical requests!!
            //(but not between a response and the next request)

            // TesterPresent Handling for Application
            App.CP_TesterPresentHandling = 0; //(0 = off, 1 = on)
            App.CP_TesterPresentSendType = 0; //0 = Send on periodic interval defined by CP_TesterPresentTime (periodically independent of other requests)
            //1 = Send when bus has been idle for CP_TesterPresentTime(after the last request)
            App.CP_TesterPresentMessage = new byte[] { 0x01, 0x00 };
            App.CP_TesterPresentReqRsp = 0; //0 = no response, 1 = response expected
            App.CP_TesterPresentExpPosResp = new byte[] { 0x41, 0x00 };
            App.CP_TesterPresentExpNegResp = new byte[] { };
            App.CP_TesterPresentTime = 2000000; //0-30000000us Time interval

            // NegativeResponse Handling:
            App.CP_RCByteOffset = 0xFFFFFFFF; //0-FFFFFFFF Position of the error type in negResp, 1 = 1st byte first byte after!! the ServiceId byte (0x7F),
                                              //0xFFFFFFFF = last byte   //0xFFFFFFFF is a workaround for shit ECUs
            App.CP_RepeatReqCountApp = 0; //0-127500  Repetition if there is no response, number of repetitions!
            Tpl.CP_RepeatReqCountTrans = 0;  //This ComParam contains a counter to enable a re‐transmission of the last request when either a transmit, a receive error or transport layer timeout is detected.
                                             //This applies to the transport layer only.
            
            App.CP_RC78Handling = 1;//or2?//Switches Neg.Resp78 Handling (0 = off, 1 = until TimeOut, 2 = infinite)
            App.CP_RC78CompletionTimeout = 30000000; //0-999999999us Time to cancellation, not number of repetitions
            App.CP_RC21Handling = 1; //or2?//Switches Neg.Resp21 Repetition mode (0 = off, 1 = until TimeOut, 2 = infinite)
            App.CP_RC21CompletionTimeout = 1300000; //0-999999999us Time to cancellation, not number of repetitions
            App.CP_RC21RequestTime = 200000; //0-100000000us negative response 21: repetition period
            App.CP_RC23Handling = 0; //Switches Neg.Resp23 Repetition mode (0 = off, 1 = until TimeOut, 2 = infinite)
            App.CP_RC23CompletionTimeout = 0; //0-999999999us Time to cancellation, not number of repetitions
            App.CP_RC23RequestTime = 200000; //0-100000000us nnegative response 23: repetition period

            //Application Layer and Transport/DataLink Layer less useful or never seen for this protocol
            App.CP_CyclicRespTimeout = 0;   //0-4294967295us This ComParam is used for ComPrimitives that have a NumRecvCycles set to IS-CYCLIC (-1, infinite). The timer is enabled after the first positive response is received from an ECU.
            //If CP_CyclicRespTimeout = 0, there is no receive timing enabled for the infinite receive ComPrimitiveLevel.        
            App.CP_StartMsgIndEnable = 0;   //0 = Disabled, 1 = Enabled. Start Message Indication Enable. Upon receiving a first byte of a UART message, an indication will be set in the RX result item. No data bytes will accompany the result item.
            App.CP_TransmitIndEnable = 0;   //0 = Disabled, 1 = Enabled. Transmit Indication Enable. On completion of a transmit message by the protocol, an indication will be set in the RX_FLAG result item. No data bytes will accompany the result item.
            App.CP_CanTransmissionTime = 0;
            App.CP_Loopback = 0;            //0 = Disabled, 1 = Enabled. Echo Transmitted messages in the receive queue, including periodic messages. Loopback messages shall only be sent after successful transmission of a message. Loopback frames are not subject to message filtering.
            App.CP_EnablePerformanceTest = 0;
            App.CP_SwCan_HighVoltage = 0;
            App.CP_ModifyTiming = 0;
            App.CP_SuspendQueueOnError = 0;



            //Change Speed
            App.CP_ChangeSpeedCtrl = 0;
            App.CP_ChangeSpeedMessage = new byte[] {};
            App.CP_ChangeSpeedRate = 0;
            App.CP_ChangeSpeedResCtrl = 0;
            App.CP_ChangeSpeedTxDelay = 0;
        }
    }
}
