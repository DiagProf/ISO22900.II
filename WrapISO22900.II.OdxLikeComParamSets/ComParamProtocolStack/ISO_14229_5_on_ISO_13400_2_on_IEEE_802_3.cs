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
    public abstract class ISO_14229_5_on_ISO_13400_2_on_IEEE_802_3 : ProtocolStack
    {
        protected sealed override IEEE_802_3 Bus { get; } = new();
        protected sealed override ISO_13400_2 Tpl { get; }// = new();
        protected sealed override ISO_14229_5 App { get; } = new();

        protected ISO_14229_5_on_ISO_13400_2_on_IEEE_802_3(HashRuleUniqueRespIdentifierFromCpEcuLayerShortName hashAlgo)
        {
            Tpl = new ISO_13400_2(hashAlgo);
            InitializeAllComParams();
        }
        protected void InitializeAllComParams()
        {
            // Used send type for Requests and Tester Present
            Tpl.CP_RequestAddrMode = 1; //1 = physical, 2 = functional  //for the request 
            Tpl.CP_DoIPLogicalGatewayAddress = 0x0001; //The logical address of the DoIP gateway or the DoIP node
            Tpl.CP_DoIPLogicalTesterAddress = 0x0E00; //The logical source address of the Tester.
            Tpl.CP_DoIPLogicalFunctionalAddress = 0xE400; //The logical functional target  address to address multiple ECUs behind a DoIP gateway
            Tpl.CP_DoIPNumberOfRetries = 0; //The number of retries to be performed when a certain NACK condition is encountered
            Tpl.CP_DoIPDiagnosticAckTimeout = 2000000; //This timeout specifies the maximum time that the test equipment waits for a confirmation ACK or NACK from the DoIP entity after the last byte of a DoIP Diagnostic request message has been sent.
            Tpl.CP_DoIPRetryPeriod = 1000000; //The period between retries, performed when a certain NACK condition is encountered
            Tpl.CP_DoIPRoutingActivationType = 0; //Specifies the Activation Type sent in the DoIP Routing Activation request.
            Tpl.CP_DoIPRoutingActivationTimeout = 1000000; //This ComParam is used to configure the timeout value for a DoIP Routing Activation request.
            Tpl.CP_RepeatReqCountTrans = 0; //This ComParam contains a counter to enable a retransmission of the last request when either a transmit, a receive error or transport layer timeout is detected. This applies to the transport layer only.

            Tpl.CP_DoIPLogicalEcuAddress = 0x0001; //The logical target address of the ECU to communicate with
            Tpl.CP_DoIPSecondaryLogicalECUResponseAddress = 0; //Secondary logical ECU address delivered with ECU responses corresponding to CAN UUDT addressed responses.


            // Timing for Application
            App.CP_P2Min = 25_000;       //0-250000us This sets the minimum time between tester request and ECU responses or two ECU responses.
            App.CP_ModifyTiming = 0;    //0 = off, 1 = on  This parameter signals the D-PDU API to automatically modify timing parameters based on a response from the ECU.
            App.CP_P3Func = 50_000;  //0-125000000us Minimum waiting time between two functional requests 
            App.CP_P3Phys = 50_000;  //0-125000000us Minimum waiting time between two physical requests!! (but not between a response and the next request)
            App.CP_P6Max  =  1_000_000; //0-125000000us Timeout for the client to wait  after the successful transmission of a request message for the complete reception of thecorresponding response message
            App.CP_P6Star = 10_000_000; //0-655350000us Enhanced timeout for the client to wait after the reception of a negative response message with negative response code 0x78
            App.CP_NetworkTransmissionTime = 100000;

            // TesterPresent Handling for Application
            App.CP_TesterPresentAddrMode = 1; //0 = physical, 1 = functional  //for the Tester Present message
            App.CP_TesterPresentHandling = 1; //(0 = off, 1 = on)
            App.CP_TesterPresentSendType = 0; //0 = Send on periodic interval defined by CP_TesterPresentTime (periodically independent of other requests)
            //1 = Send when bus has been idle for CP_TesterPresentTime(after the last request)
            App.CP_TesterPresentMessage = new byte[] { 0x3E, 0x80 };
            App.CP_TesterPresentReqRsp = 0; //0 = no response, 1 = response expected
            App.CP_TesterPresentExpPosResp = new byte[] { };
            App.CP_TesterPresentExpNegResp = new byte[] { };
            App.CP_TesterPresentTime = 2000000; //0-30000000us Time interval

            // NegativeResponse Handling:
            App.CP_RCByteOffset = 2; //0-FFFFFFFF Position of the error type in negResp, 1 = 1st byte first byte after!! the ServiceId byte (0x7F),
            //0xFFFFFFFF = last byte   //0xFFFFFFFF is a workaround for shit ECUs
            App.CP_RepeatReqCountApp = 0; //0-127500  Repetition if there is no response, number of repetitions!
            App.CP_RC78Handling = 1; //Switches Neg.Resp78 Handling (0 = off, 1 = until TimeOut, 2 = infinite)
            App.CP_RC78CompletionTimeout = 30_000_000; //0-999999999us Time to cancellation, not number of repetitions
            App.CP_RC21Handling = 1; //Switches Neg.Resp21 Repetition mode (0 = off, 1 = until TimeOut, 2 = infinite)
            App.CP_RC21CompletionTimeout = 10_000_000; //0-999999999us Time to cancellation, not number of repetitions
            App.CP_RC21RequestTime = 800000; //0-100000000us negative response 21: repetition period
            App.CP_RC23Handling = 1; //Switches Neg.Resp23 Repetition mode (0 = off, 1 = until TimeOut, 2 = infinite)
            App.CP_RC23CompletionTimeout = 10_000_000; //0-999999999us Time to cancellation, not number of repetitions
            App.CP_RC23RequestTime = 500000; //0-100000000us negative response 23: repetition period

            //Application Layer and Transport/DataLink Layer less useful or never seen for this protocol
            App.CP_CyclicRespTimeout = 0;   //0-4294967295us This ComParam is used for ComPrimitives that have a NumRecvCycles set to IS-CYCLIC (-1, infinite). The timer is enabled after the first positive response is received from an ECU.
                                            //If CP_CyclicRespTimeout = 0, there is no receive timing enabled for the infinite receive ComPrimitiveLevel.        
            App.CP_TransmitIndEnable = 0;   //0 = Disabled, 1 = Enabled. Transmit Indication Enable. On completion of a transmit message by the protocol, an indication will be set in the RX_FLAG result item. No data bytes will accompany the result item.
            App.CP_Loopback = 0;            //0 = Disabled, 1 = Enabled. Echo Transmitted messages in the receive queue, including periodic messages. Loopback messages shall only be sent after successful transmission of a message. Loopback frames are not subject to message filtering.

            //???
            //CP_DoIPConnectionCloseDelay
            //CP_NetworkTransmissionTime
        }
    }
}
