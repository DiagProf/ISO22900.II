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

using ISO22900.II.OdxLikeComParamSets.ApplicationLayer;
using ISO22900.II.OdxLikeComParamSets.PhysicalLayer;
using ISO22900.II.OdxLikeComParamSets.TransportOrDataLinkLayer;

namespace ISO22900.II.OdxLikeComParamSets
{
    public abstract class MSP_VW2000LP_CAN_on_TP20_on_ISO_11898_2_DWCAN : ProtocolStack
    {
        protected sealed override ISO_11898_2_DWCAN Bus { get; } = new();
        protected sealed override TP20 Tpl { get; }
        protected sealed override MSP_VW2000LP_CAN App { get; } = new();

        protected MSP_VW2000LP_CAN_on_TP20_on_ISO_11898_2_DWCAN(HashRuleUniqueRespIdentifierFromCpEcuLayerShortName hashAlgo)
        {
            Tpl = new TP20(hashAlgo);
            InitializeAllComParams();
        }
        protected void InitializeAllComParams()
        {
            // Hardware-Canbus-Timing
            Bus.CP_Baudrate = 500000;       //Unit bps (bits per second)
            Bus.CP_BitSamplePoint = 80;     //Unit % of Bit-Time (0-100)
            Bus.CP_SamplesPerBit = 0;       //0 = 1 Sample point, 1 = 3 Sample point
            Bus.CP_SyncJumpWidth = 15;      //SJW % of Bit-Time (0-100)
            Bus.CP_TerminationType = 3;     //0 = No termination, 1 = AC termination, 2 = 60 Ohm termination, 3 = 120 Ohm termination, 4 = SWCAN termination

            Tpl.CPM_VWTP_ApplicationType = 0x01;
            Tpl.CPM_VWTP_BlockSize = 15;
            Tpl.CPM_VWTP_BlockSizeOverrideHandling = 0; //0 = Disabled, 1 = Enabled
            Tpl.CPM_VWTP_BlockSizeOverrideValue = 15;
            Tpl.CPM_VWTP_ConnectionTestMode = 1;     //0 = Nome, 1 = Active, 2 = Passive

            Tpl.CPM_VWTP_ComfortDiag = 1; //0 = Disabled, 1 = Enabled , 2 = Use the ECU Channel-ID to build the CAN-ResponseID
            Tpl.CPM_VWTP_PhysIdFormat = 0; //0 = 11 Bit,  2 = 29 Bit
            Tpl.CPM_VWTP_PhysReqIdCon = 0x740;
            Tpl.CPM_VWTP_PhysReqIdSetup = 0x200;
            Tpl.CPM_VWTP_PhysRespIdCon = 0x300;
            Tpl.CPM_VWTP_PhysRespIdSetup = 0x201;

            Tpl.CPM_VWTP_DestAddr = 0x01; 

            Tpl.CPM_VWTP_DC_RequestTime = 50000;
            Tpl.CPM_VWTP_DC_ResponseTimeout = 50000;

            Tpl.CPM_VWTP_SndTimeout = 100000;

            Tpl.CPM_VWTP_T_CT = 1000000;

            Tpl.CPM_VWTP_T_Wait = 100000;
            Tpl.CPM_VWTP_T1 = 100000;

            Tpl.CPM_VWTP_T1EcuOverrideHandling = 0; //0 = Disabled, 1 = Enabled
            Tpl.CPM_VWTP_T1EcuOverrideValue = 50000;

            Tpl.CPM_VWTP_T3 = 5000;
            Tpl.CPM_VWTP_T3EcuOverrideHandling = 0; //0 = Disabled, 1 = Enabled
            Tpl.CPM_VWTP_T3EcuOverrideValue = 5000;


            Tpl.CPM_VWTP_TE = 50000;

            Tpl.CPM_VWTP_TesterAckDelay = 0;
            Tpl.CPM_VWTP_TesterAddr = 0;
            Tpl.CPM_VWTP_TesterTEDelay = 0;
            Tpl.CPM_VWTP_TN = 50000;


            Tpl.CPM_VWTP_EOMAckHandling = 0;  //0 = EOM Data Block with AR Flag=0, 1 = EOM Data Block with AR Flag=1

            Tpl.CPM_VWTP_InfoReceive = 0;  // 0 = Info RX-Flag set to 0, 1 = Info RX-Flag set to 1
            Tpl.CPM_VWTP_InfoTransmit = 1; // 0 = Info TX-Flag set to 0, 1 = Info TX-Flag set to 1

            Tpl.CPM_VWTP_MNCT = 5;
            Tpl.CPM_VWTP_MNT = 2;
            Tpl.CPM_VWTP_MNTB = 5;
            Tpl.CPM_VWTP_MNTC = 10; //2

            // Message formatting
            Tpl.CP_EnableConcatenation = 1; //0 = Disabled, 1 = Enabled
                                            //This ComParam instructs the application layer to automatically detect multiple responses from a single ECU and construct a single ECU response to the client application.


            // Timing for Application
            App.CP_P2Max = 600000;      //0-125000000us Timeout in receiving an expected frame after a successful transmit complete. Also used for multiple ECU responses.
            App.CP_P2Star = 9000000;    //0-655350000us TimeOut Request -> Response nach neg.Response 78
            App.CP_P3Min = 0;       //0-250000us Minimum time between end of non-negative ECU responses and start of new request. The interface will accept all responses up to CP_P3Min time. The interface will allow transmission of a request any time after CP_P3Min
            
            
            
            
            // TesterPresent Handling for Application
            //App.CP_TesterPresentSendType = 1;   //0 = Send on periodic interval defined by CP_TesterPresentTime (periodically independent of other requests),  1 = Send when bus has been idle for CP_TesterPresentTime(after the last request)
            //App.CP_TesterPresentHandling = 1;   //(0 = off, 1 = on)
            //App.CP_TesterPresentMessage = new byte[] { 0x3E, 0x00 };
            //App.CP_TesterPresentReqRsp = 1; //0 = no response, 1 = response expected
            //App.CP_TesterPresentExpPosResp = new byte[] { 0x7E, 0x00 };
            //App.CP_TesterPresentExpNegResp = new byte[] { 0x7F, 0x3E };
            //App.CP_TesterPresentTime = 2000000; //0-30000000us Time interval

            // ***** NegativeResponse Handling:
            App.CP_RCByteOffset = 2; //0-FFFFFFFF Position of the error type in negResp, 1 = 1st byte first byte after!! the ServiceId byte (0x7F),
                                     //0xFFFFFFFF is a workaround for shit ECUs
            App.CP_RepeatReqCountApp = 2;       //0-127500  Repetition if there is no response, number of repetitions!
            App.CP_RC78Handling = 1;            //Switches Neg.Resp78 Handling (0 = off, 1 = until TimeOut, 2 = infinite)
            App.CP_RC78CompletionTimeout = 30000000; //0-999999999us Time to cancellation, not number of repetitions
            App.CP_RC21Handling = 1;                 //Switches Neg.Resp21 Repetition mode (0 = off, 1 = until TimeOut, 2 = infinite)
            App.CP_RC21CompletionTimeout = 30000000; //0-999999999us Time to cancellation, not number of repetitions
            App.CP_RC21RequestTime = 800000;         //0-100000000us negative response 21: repetition period
            App.CP_RC23Handling = 1;                 //Switches Neg.Resp23 Repetition mode (0 = off, 1 = until TimeOut, 2 = infinite)
            App.CP_RC23CompletionTimeout = 30000000; //0-999999999us Time to cancellation, not number of repetitions
            App.CP_RC23RequestTime = 500000;         //0-100000000us negative response 23: repetition period

            //Application Layer and Transport/DataLink Layer less useful or never seen for this protocol
            App.CP_CyclicRespTimeout = 0;   //0-4294967295us This ComParam is used for ComPrimitives that have a NumRecvCycles set to IS-CYCLIC (-1, infinite). The timer is enabled after the first positive response is received from an ECU.
                                            //If CP_CyclicRespTimeout = 0, there is no receive timing enabled for the infinite receive ComPrimitiveLevel.        
            App.CP_StartMsgIndEnable = 0;   //0 = Disabled, 1 = Enabled. Start Message Indication Enable. Upon receiving a first byte of a UART message, an indication will be set in the RX result item. No data bytes will accompany the result item.
            App.CP_TransmitIndEnable = 0;   //0 = Disabled, 1 = Enabled. Transmit Indication Enable. On completion of a transmit message by the protocol, an indication will be set in the RX_FLAG result item. No data bytes will accompany the result item.
            App.CP_Loopback = 0;            //0 = Disabled, 1 = Enabled. Echo Transmitted messages in the receive queue, including periodic messages. Loopback messages shall only be sent after successful transmission of a message. Loopback frames are not subject to message filtering.
            App.CP_SuspendQueueOnError = 0;
        }
    }
}
