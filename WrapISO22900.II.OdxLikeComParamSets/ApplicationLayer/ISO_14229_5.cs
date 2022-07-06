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

using System.Collections.Generic;

namespace ISO22900.II.OdxLikeComParamSets.ApplicationLayer
{
    public class ISO_14229_5 : AbstractApplicationLayer
    {
        private readonly PduComParamOfTypeUint _cpCyclicRespTimeout;
        private readonly PduComParamOfTypeUint _cpEnablePerformanceTest;
        private readonly PduComParamOfTypeUint _cpModifyTiming;
        private readonly PduComParamOfTypeUint _cpP2Min;
        private readonly PduComParamOfTypeUint _cpP3Func;
        private readonly PduComParamOfTypeUint _cpP3Phys;
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
        private readonly PduComParamOfTypeStructField _cpSessionTimingOverride;
        private readonly PduComParamOfTypeUint _cpSuspendQueueOnError;
        private readonly PduComParamOfTypeUint _cpTesterPresentHandling;
        private readonly PduComParamOfTypeUint _cpTesterPresentReqRsp;
        private readonly PduComParamOfTypeUint _cpTesterPresentSendType;
        private readonly PduComParamOfTypeUint _cpTesterPresentAddrMode;
        private readonly PduComParamOfTypeUint _cpTesterPresentTime;
        private readonly PduComParamOfTypeByteField _cpTesterPresentMessage;
        private readonly PduComParamOfTypeByteField _cpTesterPresentExpPosResp;
        private readonly PduComParamOfTypeByteField _cpTesterPresentExpNegResp;
        private readonly PduComParamOfTypeUint _cpTransmitIndEnable;
        private readonly PduComParamOfTypeUint _cpDoIPTcpErrHandling;
        private readonly PduComParamOfTypeUint _cpLoopback;
        private readonly PduComParamOfTypeUint _cpNetworkTransmissionTime;
        private readonly PduComParamOfTypeUint _cpP6Max;
        private readonly PduComParamOfTypeUint _cpP6Star;


        public ISO_14229_5()
        {
            _cpCyclicRespTimeout = (PduComParamOfTypeUint)CreateCp("CP_CyclicRespTimeout", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TIMING);
            _cpEnablePerformanceTest = (PduComParamOfTypeUint)CreateCp("CP_EnablePerformanceTest", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpModifyTiming = (PduComParamOfTypeUint)CreateCp("CP_ModifyTiming", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TIMING);
            _cpP2Min = (PduComParamOfTypeUint)CreateCp("CP_P2Min", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TIMING);
            _cpP3Func = (PduComParamOfTypeUint)CreateCp("CP_P3Func", 50000, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TIMING);
            _cpP3Phys = (PduComParamOfTypeUint)CreateCp("CP_P3Phys", 50000, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TIMING);
            _cpRC21CompletionTimeout = (PduComParamOfTypeUint)CreateCp("CP_RC21CompletionTimeout", 1300000, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_ERRHDL);
            _cpRC21Handling = (PduComParamOfTypeUint)CreateCp("CP_RC21Handling", 1, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_ERRHDL);
            _cpRC21RequestTime = (PduComParamOfTypeUint)CreateCp("CP_RC21RequestTime", 200000, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_ERRHDL);
            _cpRC23CompletionTimeout = (PduComParamOfTypeUint)CreateCp("CP_RC23CompletionTimeout", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_ERRHDL);
            _cpRC23Handling = (PduComParamOfTypeUint)CreateCp("CP_RC23Handling", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_ERRHDL);
            _cpRC23RequestTime = (PduComParamOfTypeUint)CreateCp("CP_RC23RequestTime", 200000, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_ERRHDL);
            _cpRC78CompletionTimeout = (PduComParamOfTypeUint)CreateCp("CP_RC78CompletionTimeout", 25000000, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_ERRHDL);
            _cpRC78Handling = (PduComParamOfTypeUint)CreateCp("CP_RC78Handling", 2, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_ERRHDL);
            _cpRCByteOffset = (PduComParamOfTypeUint)CreateCp("CP_RCByteOffset", 0xFFFFFFFF, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_ERRHDL);
            _cpRepeatReqCountApp = (PduComParamOfTypeUint)CreateCp("CP_RepeatReqCountApp", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_ERRHDL);
            _cpSessionTimingOverride = (PduComParamOfTypeStructField)CreateCp("CP_SessionTimingOverride", new PduParamStructFieldData(new PduParamStructSessionTiming[] { }, 255), PduPt.PDU_PT_STRUCTFIELD, PduPc.PDU_PC_TIMING);
            _cpSuspendQueueOnError = (PduComParamOfTypeUint)CreateCp("CP_SuspendQueueOnError", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_ERRHDL);
            _cpTesterPresentHandling = (PduComParamOfTypeUint)CreateCp("CP_TesterPresentHandling", 1, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TESTER_PRESENT);
            _cpTesterPresentReqRsp = (PduComParamOfTypeUint)CreateCp("CP_TesterPresentReqRsp", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TESTER_PRESENT);
            _cpTesterPresentSendType = (PduComParamOfTypeUint)CreateCp("CP_TesterPresentSendType", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TESTER_PRESENT);
            _cpTesterPresentAddrMode = (PduComParamOfTypeUint)CreateCp("CP_TesterPresentAddrMode", 1, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TESTER_PRESENT);
            _cpTesterPresentTime = (PduComParamOfTypeUint)CreateCp("CP_TesterPresentTime", 2_000_000, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TESTER_PRESENT);
            _cpTesterPresentMessage = (PduComParamOfTypeByteField)CreateCp("CP_TesterPresentMessage", new byte[] { 0x3e, 0x80 }, PduPt.PDU_PT_BYTEFIELD, PduPc.PDU_PC_TESTER_PRESENT);
            _cpTesterPresentExpPosResp = (PduComParamOfTypeByteField)CreateCp("CP_TesterPresentExpPosResp", new byte[] { }, PduPt.PDU_PT_BYTEFIELD, PduPc.PDU_PC_TESTER_PRESENT);
            _cpTesterPresentExpNegResp = (PduComParamOfTypeByteField)CreateCp("CP_TesterPresentExpNegResp", new byte[] { }, PduPt.PDU_PT_BYTEFIELD, PduPc.PDU_PC_TESTER_PRESENT);
            _cpTransmitIndEnable = (PduComParamOfTypeUint)CreateCp("CP_TransmitIndEnable", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpDoIPTcpErrHandling = (PduComParamOfTypeUint)CreateCp("CP_DoIPTcpErrHandling", 1, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_ERRHDL);
            _cpLoopback = (PduComParamOfTypeUint)CreateCp("CP_Loopback", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpNetworkTransmissionTime = (PduComParamOfTypeUint)CreateCp("CP_NetworkTransmissionTime", 100_000, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TIMING);
            _cpP6Max = (PduComParamOfTypeUint)CreateCp("CP_P6Max", 1_000_000, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TIMING);
            _cpP6Star = (PduComParamOfTypeUint)CreateCp("CP_P6Star", 10_000_000, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TIMING);



            CpList = new List<PduComParam>
            {
                _cpCyclicRespTimeout,_cpEnablePerformanceTest,_cpModifyTiming,
                _cpP2Min,_cpP3Func,_cpP3Phys,
                _cpRC21CompletionTimeout,_cpRC21Handling,_cpRC21RequestTime,
                _cpRC23CompletionTimeout,_cpRC23Handling,_cpRC23RequestTime,
                _cpRC78CompletionTimeout,_cpRC78Handling,_cpRCByteOffset,
                _cpRepeatReqCountApp,_cpSessionTimingOverride,_cpSuspendQueueOnError,
                _cpTesterPresentHandling,_cpTesterPresentReqRsp,_cpTesterPresentSendType,
                _cpTesterPresentAddrMode,_cpTesterPresentTime,_cpTesterPresentMessage,
                _cpTesterPresentExpPosResp,_cpTesterPresentExpNegResp,_cpTransmitIndEnable,
                _cpDoIPTcpErrHandling,_cpLoopback,_cpNetworkTransmissionTime,
                _cpP6Max,_cpP6Star

            };
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
        public uint CP_ModifyTiming
        {
            get => _cpModifyTiming.ComParamData;
            set => _cpModifyTiming.ComParamData = value;
        }
        public uint CP_P2Min
        {
            get => _cpP2Min.ComParamData;
            set => _cpP2Min.ComParamData = value;
        }
        public uint CP_P3Func
        {
            get => _cpP3Func.ComParamData;
            set => _cpP3Func.ComParamData = value;
        }
        public uint CP_P3Phys
        {
            get => _cpP3Phys.ComParamData;
            set => _cpP3Phys.ComParamData = value;
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

        public PduParamStructSessionTiming[] CP_SessionTimingOverride
        {
            get => (PduParamStructSessionTiming[])_cpSessionTimingOverride.ComParamData.StructArray;
            set
            {
                for (var i = 0; i < value.Length; i++)
                    if (i < _cpSessionTimingOverride.ComParamData.ParamMaxEntries)
                        _cpSessionTimingOverride.ComParamData.StructArray[i] = value[i];
            }
        }
        public uint CP_SuspendQueueOnError
        {
            get => _cpSuspendQueueOnError.ComParamData;
            set => _cpSuspendQueueOnError.ComParamData = value;
        }
        public uint CP_TesterPresentHandling
        {
            get => _cpTesterPresentHandling.ComParamData;
            set => _cpTesterPresentHandling.ComParamData = value;
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
        public uint CP_TesterPresentAddrMode
        {
            get => _cpTesterPresentAddrMode.ComParamData;
            set => _cpTesterPresentAddrMode.ComParamData = value;
        }
        public uint CP_TesterPresentTime
        {
            get => _cpTesterPresentTime.ComParamData;
            set => _cpTesterPresentTime.ComParamData = value;
        }
        public byte[] CP_TesterPresentMessage
        {
            get => _cpTesterPresentMessage.ComParamData.DataArray;
            set => _cpTesterPresentMessage.ComParamData.DataArray = value;
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
        public uint CP_TransmitIndEnable
        {
            get => _cpTransmitIndEnable.ComParamData;
            set => _cpTransmitIndEnable.ComParamData = value;
        }
        public uint CP_DoIPTcpErrHandling
        {
            get => _cpDoIPTcpErrHandling.ComParamData;
            set => _cpDoIPTcpErrHandling.ComParamData = value;
        }
        public uint CP_Loopback
        {
            get => _cpLoopback.ComParamData;
            set => _cpLoopback.ComParamData = value;
        }
        public uint CP_NetworkTransmissionTime
        {
            get => _cpNetworkTransmissionTime.ComParamData;
            set => _cpNetworkTransmissionTime.ComParamData = value;
        }
        public uint CP_P6Max
        {
            get => _cpP6Max.ComParamData;
            set => _cpP6Max.ComParamData = value;
        }
        public uint CP_P6Star
        {
            get => _cpP6Star.ComParamData;
            set => _cpP6Star.ComParamData = value;
        }
    }
}
