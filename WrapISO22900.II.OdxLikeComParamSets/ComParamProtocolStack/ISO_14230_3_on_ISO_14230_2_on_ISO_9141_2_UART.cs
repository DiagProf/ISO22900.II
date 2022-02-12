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
    public abstract class ISO_14230_3_on_ISO_14230_2_on_ISO_9141_2_UART : ProtocolStack
    {
        protected sealed override ISO_9141_2_UART Bus { get; } = new();
        protected sealed override ISO_14230_2 Tpl { get; }
        protected sealed override ISO_14230_3_Kline App { get; } = new();

        protected ISO_14230_3_on_ISO_14230_2_on_ISO_9141_2_UART(HashRuleUniqueRespIdentifierFromCpEcuLayerShortName hashAlgo = null)
        {
            Tpl = new ISO_14230_2(hashAlgo);
            InitializeAllComParams();
        }

        protected void InitializeAllComParams()
        {
            // Hardware Uart Config
            Bus.CP_Baudrate = 10400;    //Unit bps (bits per second)
            Bus.CP_K_LinePullup = 1;    //0 = No pull‐up (or hard wired), 1 = 12V, 2 = 24V
            Bus.CP_K_L_LineInit = 0;    //0 = Use L‐line and K‐line for initialization address, 1 = Use K‐line only for initialization address, 2 = Use L‐Line only for initialization address
            Bus.CP_UartConfig = 6;      //0-17  (Default = 06) Configure the parity, data bit size and stop bits of a Uart protocol  
                                        //0 = 7N1, 1 = 7O1, 2 = 7E1, 3 = 7N2, 4 = 7O2, 5 = 7E2, 6 = 8N1, 7 = 8O1, 8 = 8E1
                                        //9 = 8N2, 10 = 8O2, 11 = 8E2, 12 = 9N1, 13 = 9O1, 14 = 9E1, 15 = 9N2, 16 = 9O2, 17 = 9E2

            
            // Application Layer and Transport/DataLink Layer
            
            // Used send type for Requests and Tester Present
            Tpl.CP_RequestAddrMode = 1;         //1 = physical, 2 = functional  //for the standard(normal) request 
            App.CP_TesterPresentAddrMode = 0;   //0 = physical, 1 = functional  //for the Tester Present message

            // Protocol-header definitions
            Tpl.CP_HeaderFormatKW = 0;          //0-8   Header Byte configuration for K‐Line protocols.
                                                //This setting is used to properly construct the message header bytes to complete the PDU.
                                                //Header bytes are constructed following the rules of the protocol specification.
                                                //Attention !!! -> This ComParam can be used to override any keybyte values received from the ECU during initialization
                                                //(shit ECUs need this especially if the Keybytes allow different settings of the format. Some ECUs cannot handle all formats.)
                                                //0 = Use the header byte format specified by the ECU key bytes
                                                //7 = OEM‐9141 Header Format (ms nibble of first byte = byte count)
                                                //8 = No header bytes
            Tpl.CP_EcuRespSourceAddress = 0x10;         //ECU Source Address response
            Tpl.CP_FuncRespTargetAddr = 0xF1;           //Second Header Byte of a message received from the ECU for functional addressing.
            Tpl.CP_FuncRespFormatPriorityType = 0xC0;   //First Header Byte of a message received from the ECU for functional addressing.
            Tpl.CP_PhysRespFormatPriorityType = 0x80;   //First Header Byte of a message received from the ECU for physical addressing. 

            Tpl.CP_TesterSourceAddress = 0xF1;          //Tester Source address of transmitted message
            Tpl.CP_PhysReqTargetAddr = 0x10;            //Physical Target Addressing (ECU-address) Information used for correct Message Header Construction.
            Tpl.CP_FuncReqTargetAddr = 0x33;            //Second Header Byte of a message for a functional address transmit.
            Tpl.CP_PhysReqFormatPriorityType = 0x80;    //First Header Byte of a message for a physical address transmit.
            Tpl.CP_FuncReqFormatPriorityType = 0xC0;    //First Header Byte of a message for a functional address transmit.
            


            // Wake up (Initialization) settings
            Tpl.CP_InitializationSettings = 2;  //Set Initialization method. 1 = 5 Baud Init sequence, 2 = Fast Init sequence, 3 = No Init sequence
            Tpl.CP_TIdle = 300_000;	//0-10000000us Minimum bus idle time before tester starts the address byte sequence or the fast init sequence.
            //fast
            Tpl.CP_TInil = 25_000;	//0-250000us Sets the duration for the low pulse in a fast initialization sequence.
            Tpl.CP_TWup = 50_000;    //0-250000us Sets total duration of the wakeup pulse (TWUP-TINIL)=high pulse before start communication message.
            //5Baud
            Tpl.CP_5BaudAddressFunc = 0x33; //hex 0-FF Value of 5 baud address in case of functionaladdressed communication.
            Tpl.CP_5BaudAddressPhys = 0x01; //hex 0-FF Value of 5 baud address in case of physicaladdressed communication.
            Tpl.CP_5BaudMode = 0;           //Type of 5 Baud initialization.
                                            //This ComParam allows either ISO 9141 initialization sequence, ISO 9141-2/ISO 14230 initialization sequence, or hybrid versions, which include only one of the extra bytes defined for ISO 9141-2 and ISO 14230.
                                            //(Initialization for ISO 9141-2 and ISO 14230 include the init sequence as defined in ISO 9141 plus inverted key byte 2 sent from the interface to the ECU and the inverted address sent from the ECU to the interface.)

            // Timing for Transportlayer
            Tpl.CP_P1Max = 20_000;   //0-250000us Maximum inter‐byte time for ECU Responses. Interface shall be capable of handling a P1_MIN time of 0 ms.
            Tpl.CP_P4Min = 5_000;    //0-250000us Minimum inter-byte time for tester transmits.
            Tpl.CP_P4Max = 20_000;   //0-250000us Maximum inter-byte time for tester transmits.


            Tpl.CP_W1Max = 300_000;  //0-1000000us Maximum time from the end of address byte to start of the synchronization pattern from the ECU.
            Tpl.CP_W2Max = 20_000;	//0-1000000us Maximum time from the end of the synchronization pattern to the start of key byte 1.
            Tpl.CP_W3Max = 20_000;	//0-1000000us Maximum time between key byte 1 and key byte 2.
            Tpl.CP_W4Max = 50_000;	//0-1000000us Maximum time between receiving key byte 2 from the vehicle and the inversion being returned by the interface. Same is true for the inverted key byte 2 sent by the tester and the received inverted address from the vehicle.
            Tpl.CP_W4Min = 25_000;   //0-250000us Minimum time between receiving key byte 2 from the vehicle and the inversion being returned by the interface. Same is true for the inverted key byte 2 sent by the tester and the received inverted address from the vehicle.

            Tpl.CP_ExtendedTiming = new PduParamStructAccessTiming[]{};    //This ComParam is used to define extended timing values for K‐Line protocols.
                                                                           //The values are used after the key bytes are received from the ECU during the initialization sequence.

            // Message formatting
            Tpl.CP_EnableConcatenation = 0; //0 = Disabled, 1 = Enabled
                                            //This ComParam instructs the application layer to automatically detect multiple responses from a single ECU and construct a single ECU response to the client application.
            Tpl.CP_FillerByteHandling = 0;  //0 = Padding Disabled, 1 = Padding Enabled
            Tpl.CP_FillerByte = 0x00;       //0-FF Padding data byte to be used to pad all.
            Tpl.CP_FillerByteLength = 0;    //dez 0-255 Length to pad the data portion of the message.
                                            //e.g. Request  Req=0x21 01 and CP_FillerByteLenght = 5. Result Request to ECU is  => 0x21 01 00 00 00


            // Timing for Application
            App.CP_P2Min = 25_000;       //0-250000us This sets the minimum time between tester request and ECU responses or two ECU responses.
            App.CP_P2Max = 100_000;      //0-125000000us Timeout in receiving an expected frame after a successful transmit complete. Also used for multiple ECU responses.
            App.CP_P2Star = 5_000_000;    //0-655350000us TimeOut Request -> Response nach neg.Response 78
            App.CP_P3Min = 100_000;      //0-250000us Minimum time between end of non-negative ECU responses and start of new request. The interface will accept all responses up to CP_P3Min time. The interface will allow transmission of a request any time after CP_P3Min
            App.CP_ModifyTiming = 0;    //0 = off, 1 = on  This parameter signals the D-PDU API to automatically modify timing parameters based on a response from the ECU.
            Tpl.CP_AccessTimingOverride = new PduParamStructAccessTiming[] {}; //This ComParam along with CP_ModifyTiming ComParam signals the D-PDU API to override the response from any ECUs to an Access Timing request. see 22900-2


            // TesterPresent Handling for Application
            App.CP_TesterPresentHandling = 1;   //(0 = off, 1 = on)
            App.CP_TesterPresentSendType = 1;   //1 = Send when bus has been idle for CP_TesterPresentTime (after the last request) (for K-line 1 is the only choice) 
            App.CP_TesterPresentMessage = new byte[] { 0x3E };
            App.CP_TesterPresentReqRsp = 1; //0 = no response, 1 = response expected
            App.CP_TesterPresentExpPosResp = new byte[] { 0x7E };
            App.CP_TesterPresentExpNegResp = new byte[] { 0x7F, 0x3E };
            App.CP_TesterPresentTime = 1000000; //0-30000000us Time interval

            // NegativeResponse Handling:
            App.CP_RCByteOffset = 0x3;   //0-FFFFFFFF Position of the error type in negResp, 1 = 1st byte, FFFFFFFF = last byte
            App.CP_RepeatReqCountApp = 3;       //0-127500  Repetition if there is no response, number of repetitions!
            App.CP_RC78Handling = 1;            //Switches Neg.Resp78 Handling (0 = off, 1 = until TimeOut, 2 = infinite)
            App.CP_RC78CompletionTimeout = 25000000; //0-999999999us Time to cancellation, not number of repetitions
            App.CP_RC21Handling = 1;                 //Switches Neg.Resp21 Repetition mode (0 = off, 1 = until TimeOut, 2 = infinite)
            App.CP_RC21CompletionTimeout = 15000000; //0-999999999us Time to cancellation, not number of repetitions
            App.CP_RC21RequestTime = 200000;         //0-100000000us negative response 21: repetition period
            App.CP_RC23Handling = 1;                 //Switches Neg.Resp23 Repetition mode (0 = off, 1 = until TimeOut, 2 = infinite)
            App.CP_RC23CompletionTimeout = 15000000; //0-999999999us Time to cancellation, not number of repetitions
            App.CP_RC23RequestTime = 200000;        //0-100000000us nnegative response 23: repetition period


            // Baudrate switch (mostly only used for flashing)
            App.CP_ChangeSpeedCtrl = 0;			        //0=disabled, 1=enabled   global on off switch for functionality
            App.CP_ChangeSpeedResCtrl = 0;              //This ComParam is used in conjunction with CP_ChangeSpeedCtrl.
            //This ComParam is used to control automatic loading or unloading of the physical resource resistor when a change speed message has been transmitted or received.
            //Most of the time the VCIs do not support this
            App.CP_ChangeSpeedTxDelay = 0;              //0-4294967295us Minimum amount of time to wait before allowing the next transmit message on the Vehicle Bus after the successful transmission of a baud rate change message.
            App.CP_ChangeSpeedMessage = new byte[] { };   //Switch Speed Message.
            //The message is monitored for transmit and receive. 
            //As a rule, ECUs send a positive response before the baud rate is switched.
            //That is why in most cases the positive response is entered here.
            App.CP_ChangeSpeedRate = 0;                 //Unit bps (bits per second)


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
