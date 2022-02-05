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


using System.Collections.Generic;

namespace ISO22900.II.OdxLikeComParamSets.ApplicationLayer
{
    public class ISO_14230_3_Kline : AbstractApplicationLayer
    {
        private readonly PduComParamOfTypeUint _cpChangeSpeedCtrl;
        private readonly PduComParamOfTypeByteField _cpChangeSpeedMessage;
        private readonly PduComParamOfTypeUint _cpChangeSpeedRate;
        private readonly PduComParamOfTypeUint _cpChangeSpeedResCtrl;
        private readonly PduComParamOfTypeUint _cpChangeSpeedTxDelay;
        private readonly PduComParamOfTypeUint _cpCyclicRespTimeout;
        private readonly PduComParamOfTypeUint _cpEnablePerformanceTest;
        private readonly PduComParamOfTypeUint _cpLoopback;
        private readonly PduComParamOfTypeUint _cpModifyTiming;
        private readonly PduComParamOfTypeUint _cpP2Max;
        private readonly PduComParamOfTypeUint _cpP2Min;
        private readonly PduComParamOfTypeUint _cpP2Star;
        private readonly PduComParamOfTypeUint _cpP3Min;
        private readonly PduComParamOfTypeUint _cpRC21CompletionTimeout;
        private readonly PduComParamOfTypeUint _cpRC21Handling;
        private readonly PduComParamOfTypeUint _cpRC21RequestTime;
        private readonly PduComParamOfTypeUint _cpRC23CompletionTimeout;
        private readonly PduComParamOfTypeUint _cpRC23Handling;
        private readonly PduComParamOfTypeUint _cpRC23RequestTime;
        private readonly PduComParamOfTypeUint _cpRC78CompletionTimeout;
        private readonly PduComParamOfTypeUint _cpRC78Handling;
        private readonly PduComParamOfTypeUint _cpRCByteOffset;
        private readonly PduComParamOfTypeUint _cpRepeatReqCountApp;
        private readonly PduComParamOfTypeUint _cpStartMsgIndEnable;
        private readonly PduComParamOfTypeUint _cpSuspendQueueOnError;
        //private readonly PduComParamOfTypeUint _cpSwCan_HighVoltage;
        private readonly PduComParamOfTypeUint _cpTesterPresentAddrMode;
        private readonly PduComParamOfTypeByteField _cpTesterPresentExpPosResp;
        private readonly PduComParamOfTypeByteField _cpTesterPresentExpNegResp;
        private readonly PduComParamOfTypeUint _cpTesterPresentHandling;
        private readonly PduComParamOfTypeByteField _cpTesterPresentMessage;
        private readonly PduComParamOfTypeUint _cpTesterPresentReqRsp;
        private readonly PduComParamOfTypeUint _cpTesterPresentSendType;
        private readonly PduComParamOfTypeUint _cpTesterPresentTime;
        private readonly PduComParamOfTypeUint _cpTransmitIndEnable;

        public ISO_14230_3_Kline()
        {
            _cpChangeSpeedCtrl = (PduComParamOfTypeUint)CreateCp("CP_ChangeSpeedCtrl", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpChangeSpeedMessage = (PduComParamOfTypeByteField)CreateCp("CP_ChangeSpeedMessage", new byte[] {}, PduPt.PDU_PT_BYTEFIELD, PduPc.PDU_PC_COM);
            _cpChangeSpeedRate = (PduComParamOfTypeUint)CreateCp("CP_ChangeSpeedRate", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpChangeSpeedResCtrl = (PduComParamOfTypeUint)CreateCp("CP_ChangeSpeedResCtrl", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpChangeSpeedTxDelay = (PduComParamOfTypeUint)CreateCp("CP_ChangeSpeedTxDelay", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TIMING);
            _cpCyclicRespTimeout = (PduComParamOfTypeUint)CreateCp("CP_CyclicRespTimeout", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TIMING);
            _cpEnablePerformanceTest = (PduComParamOfTypeUint)CreateCp("CP_EnablePerformanceTest", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpLoopback = (PduComParamOfTypeUint)CreateCp("CP_Loopback", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpModifyTiming = (PduComParamOfTypeUint)CreateCp("CP_ModifyTiming", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TIMING);
            _cpP2Max = (PduComParamOfTypeUint)CreateCp("CP_P2Max", 50000, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TIMING);
            _cpP2Min = (PduComParamOfTypeUint)CreateCp("CP_P2Min", 25000, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TIMING);
            _cpP2Star = (PduComParamOfTypeUint)CreateCp("CP_P2Star", 5000000, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TIMING);
            _cpP3Min = (PduComParamOfTypeUint)CreateCp("CP_P3Min", 55000, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TIMING);
            _cpRC21CompletionTimeout = (PduComParamOfTypeUint)CreateCp("CP_RC21CompletionTimeout", 1300000, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_ERRHDL);
            _cpRC21Handling = (PduComParamOfTypeUint)CreateCp("CP_RC21Handling", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_ERRHDL);
            _cpRC21RequestTime = (PduComParamOfTypeUint)CreateCp("CP_RC21RequestTime", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_ERRHDL);
            _cpRC23CompletionTimeout = (PduComParamOfTypeUint)CreateCp("CP_RC23CompletionTimeout", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_ERRHDL);
            _cpRC23Handling = (PduComParamOfTypeUint)CreateCp("CP_RC23Handling", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_ERRHDL);
            _cpRC23RequestTime = (PduComParamOfTypeUint)CreateCp("CP_RC23RequestTime", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_ERRHDL);
            _cpRC78CompletionTimeout = (PduComParamOfTypeUint)CreateCp("CP_RC78CompletionTimeout", 25000000, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_ERRHDL);
            _cpRC78Handling = (PduComParamOfTypeUint)CreateCp("CP_RC78Handling", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_ERRHDL);
            _cpRCByteOffset = (PduComParamOfTypeUint)CreateCp("CP_RCByteOffset", 4294967295, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_ERRHDL);
            _cpRepeatReqCountApp = (PduComParamOfTypeUint)CreateCp("CP_RepeatReqCountApp", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_ERRHDL);
            _cpStartMsgIndEnable = (PduComParamOfTypeUint)CreateCp("CP_StartMsgIndEnable", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpSuspendQueueOnError = (PduComParamOfTypeUint)CreateCp("CP_SuspendQueueOnError", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_ERRHDL);
            //_cpSwCan_HighVoltage = (PduComParamOfTypeUint)CreateCp("CP_SwCan_HighVoltage", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpTesterPresentAddrMode = (PduComParamOfTypeUint)CreateCp("CP_TesterPresentAddrMode", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TESTER_PRESENT);
            _cpTesterPresentExpPosResp = (PduComParamOfTypeByteField)CreateCp("CP_TesterPresentExpPosResp", new byte[] { 0x7E }, PduPt.PDU_PT_BYTEFIELD, PduPc.PDU_PC_TESTER_PRESENT);
            _cpTesterPresentExpNegResp = (PduComParamOfTypeByteField)CreateCp("CP_TesterPresentExpNegResp", new byte[] {0x7F, 0x3E }, PduPt.PDU_PT_BYTEFIELD, PduPc.PDU_PC_TESTER_PRESENT);
            _cpTesterPresentHandling = (PduComParamOfTypeUint)CreateCp("CP_TesterPresentHandling", 1, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TESTER_PRESENT);
            _cpTesterPresentMessage = (PduComParamOfTypeByteField)CreateCp("CP_TesterPresentMessage", new byte[] { 0x3E }, PduPt.PDU_PT_BYTEFIELD, PduPc.PDU_PC_TESTER_PRESENT);
            _cpTesterPresentReqRsp = (PduComParamOfTypeUint)CreateCp("CP_TesterPresentReqRsp", 1, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TESTER_PRESENT);
            _cpTesterPresentSendType = (PduComParamOfTypeUint)CreateCp("CP_TesterPresentSendType", 1, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TESTER_PRESENT);
            _cpTesterPresentTime = (PduComParamOfTypeUint)CreateCp("CP_TesterPresentTime", 3000000, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TESTER_PRESENT);
            _cpTransmitIndEnable = (PduComParamOfTypeUint)CreateCp("CP_TransmitIndEnable", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);

            CpList = new List<PduComParam>
            {
                _cpChangeSpeedCtrl, _cpChangeSpeedMessage, _cpChangeSpeedRate,
                _cpChangeSpeedResCtrl, _cpChangeSpeedTxDelay, _cpCyclicRespTimeout,
                _cpEnablePerformanceTest, _cpLoopback, _cpModifyTiming,
                _cpP2Max, _cpP2Min, _cpP2Star, _cpP3Min, 
                _cpRC21CompletionTimeout, _cpRC21Handling, _cpRC21RequestTime, 
                _cpRC23CompletionTimeout, _cpRC23Handling, _cpRC23RequestTime,
                _cpRC78CompletionTimeout, _cpRC78Handling, _cpRCByteOffset, 
                _cpRepeatReqCountApp, _cpStartMsgIndEnable, _cpSuspendQueueOnError,
                /*_cpSwCan_HighVoltage,*/ _cpTesterPresentAddrMode, _cpTesterPresentExpPosResp,
                _cpTesterPresentExpNegResp, _cpTesterPresentHandling, _cpTesterPresentMessage,
                _cpTesterPresentReqRsp, _cpTesterPresentSendType, _cpTesterPresentTime,
                _cpTransmitIndEnable,
            };
        }

        public uint CP_ChangeSpeedCtrl
        {
            get => _cpChangeSpeedCtrl.ComParamData;
            set => _cpChangeSpeedCtrl.ComParamData = value;
        }
        public byte[] CP_ChangeSpeedMessage
        {
            get => _cpChangeSpeedMessage.ComParamData.DataArray;
            set => _cpChangeSpeedMessage.ComParamData.DataArray = value;
        }
        public uint CP_ChangeSpeedRate
        {
            get => _cpChangeSpeedRate.ComParamData;
            set => _cpChangeSpeedRate.ComParamData = value;
        }
        public uint CP_ChangeSpeedResCtrl
        {
            get => _cpChangeSpeedResCtrl.ComParamData;
            set => _cpChangeSpeedResCtrl.ComParamData = value;
        }
        public uint CP_ChangeSpeedTxDelay
        {
            get => _cpChangeSpeedTxDelay.ComParamData;
            set => _cpChangeSpeedTxDelay.ComParamData = value;
        }
        public uint CP_CyclicRespTimeout
        {
            get => _cpCyclicRespTimeout.ComParamData;
            set => _cpCyclicRespTimeout.ComParamData = value;
        }
        public uint CP_EnablePerformanceTest
        {
            get => _cpEnablePerformanceTest.ComParamData;
            set => _cpEnablePerformanceTest.ComParamData = value;
        }
        public uint CP_Loopback
        {
            get => _cpLoopback.ComParamData;
            set => _cpLoopback.ComParamData = value;
        }
        public uint CP_ModifyTiming
        {
            get => _cpModifyTiming.ComParamData;
            set => _cpModifyTiming.ComParamData = value;
        }
        public uint CP_P2Max
        {
            get => _cpP2Max.ComParamData;
            set => _cpP2Max.ComParamData = value;
        }
        public uint CP_P2Min
        {
            get => _cpP2Min.ComParamData;
            set => _cpP2Min.ComParamData = value;
        }
        public uint CP_P2Star
        {
            get => _cpP2Star.ComParamData;
            set => _cpP2Star.ComParamData = value;
        }
        public uint CP_P3Min
        {
            get => _cpP3Min.ComParamData;
            set => _cpP3Min.ComParamData = value;
        }
        public uint CP_RC21CompletionTimeout
        {
            get => _cpRC21CompletionTimeout.ComParamData;
            set => _cpRC21CompletionTimeout.ComParamData = value;
        }
        public uint CP_RC21Handling
        {
            get => _cpRC21Handling.ComParamData;
            set => _cpRC21Handling.ComParamData = value;
        }
        public uint CP_RC21RequestTime
        {
            get => _cpRC21RequestTime.ComParamData;
            set => _cpRC21RequestTime.ComParamData = value;
        }
        public uint CP_RC23CompletionTimeout
        {
            get => _cpRC23CompletionTimeout.ComParamData;
            set => _cpRC23CompletionTimeout.ComParamData = value;
        }
        public uint CP_RC23Handling
        {
            get => _cpRC23Handling.ComParamData;
            set => _cpRC23Handling.ComParamData = value;
        }
        public uint CP_RC23RequestTime
        {
            get => _cpRC23RequestTime.ComParamData;
            set => _cpRC23RequestTime.ComParamData = value;
        }
        public uint CP_RC78CompletionTimeout
        {
            get => _cpRC78CompletionTimeout.ComParamData;
            set => _cpRC78CompletionTimeout.ComParamData = value;
        }
        public uint CP_RC78Handling
        {
            get => _cpRC78Handling.ComParamData;
            set => _cpRC78Handling.ComParamData = value;
        }
        public uint CP_RCByteOffset
        {
            get => _cpRCByteOffset.ComParamData;
            set => _cpRCByteOffset.ComParamData = value;
        }
        public uint CP_RepeatReqCountApp
        {
            get => _cpRepeatReqCountApp.ComParamData;
            set => _cpRepeatReqCountApp.ComParamData = value;
        }
        public uint CP_StartMsgIndEnable
        {
            get => _cpStartMsgIndEnable.ComParamData;
            set => _cpStartMsgIndEnable.ComParamData = value;
        }
        public uint CP_SuspendQueueOnError
        {
            get => _cpSuspendQueueOnError.ComParamData;
            set => _cpSuspendQueueOnError.ComParamData = value;
        }
        //public uint CP_SwCan_HighVoltage
        //{
        //    get => _cpSwCan_HighVoltage.ComParamData;
        //    set => _cpSwCan_HighVoltage.ComParamData = value;
        //}
        public uint CP_TesterPresentAddrMode
        {
            get => _cpTesterPresentAddrMode.ComParamData;
            set => _cpTesterPresentAddrMode.ComParamData = value;
        }
        public byte[] CP_TesterPresentExpPosResp
        {
            get => _cpTesterPresentExpPosResp.ComParamData.DataArray;
            set => _cpTesterPresentExpPosResp.ComParamData.DataArray = value;
        }
        public byte[] CP_TesterPresentExpNegResp
        {
            get => _cpTesterPresentExpNegResp.ComParamData.DataArray;
            set => _cpTesterPresentExpNegResp.ComParamData.DataArray = value;
        }
        public uint CP_TesterPresentHandling
        {
            get => _cpTesterPresentHandling.ComParamData;
            set => _cpTesterPresentHandling.ComParamData = value;
        }
        public byte[] CP_TesterPresentMessage
        {
            get => _cpTesterPresentMessage.ComParamData.DataArray;
            set => _cpTesterPresentMessage.ComParamData.DataArray = value;
        }
        public uint CP_TesterPresentReqRsp
        {
            get => _cpTesterPresentReqRsp.ComParamData;
            set => _cpTesterPresentReqRsp.ComParamData = value;
        }
        public uint CP_TesterPresentSendType
        {
            get => _cpTesterPresentSendType.ComParamData;
            set => _cpTesterPresentSendType.ComParamData = value;
        }
        public uint CP_TesterPresentTime
        {
            get => _cpTesterPresentTime.ComParamData;
            set => _cpTesterPresentTime.ComParamData = value;
        }
        public uint CP_TransmitIndEnable
        {
            get => _cpTransmitIndEnable.ComParamData;
            set => _cpTransmitIndEnable.ComParamData = value;
        }
    }
}
