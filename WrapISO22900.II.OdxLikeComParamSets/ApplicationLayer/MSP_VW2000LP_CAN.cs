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
    public class MSP_VW2000LP_CAN : AbstractApplicationLayer
    {
        private readonly PduComParamOfTypeUint _cpCyclicRespTimeout;
        private readonly PduComParamOfTypeUint _cpLoopback;
        private readonly PduComParamOfTypeUint _cpP2Max;
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
        private readonly PduComParamOfTypeUint _cpTransmitIndEnable;

        public MSP_VW2000LP_CAN()
        {
            _cpCyclicRespTimeout = (PduComParamOfTypeUint)CreateCp("CP_CyclicRespTimeout", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TIMING);
            _cpLoopback = (PduComParamOfTypeUint)CreateCp("CP_Loopback", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpP2Max = (PduComParamOfTypeUint)CreateCp("CP_P2Max", 50000, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TIMING);
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
            _cpRCByteOffset = (PduComParamOfTypeUint)CreateCp("CP_RCByteOffset", 0xFFFFFFFF, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_ERRHDL);
            _cpRepeatReqCountApp = (PduComParamOfTypeUint)CreateCp("CP_RepeatReqCountApp", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_ERRHDL);
            _cpStartMsgIndEnable = (PduComParamOfTypeUint)CreateCp("CP_StartMsgIndEnable", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpSuspendQueueOnError = (PduComParamOfTypeUint)CreateCp("CP_SuspendQueueOnError", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_ERRHDL);
            _cpTransmitIndEnable = (PduComParamOfTypeUint)CreateCp("CP_TransmitIndEnable", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);

            CpList = new List<PduComParam>
            {
                _cpCyclicRespTimeout,
                _cpLoopback,
                _cpP2Max, _cpP2Star, _cpP3Min,
                _cpRC21CompletionTimeout, _cpRC21Handling, _cpRC21RequestTime,
                _cpRC23CompletionTimeout, _cpRC23Handling, _cpRC23RequestTime,
                _cpRC78CompletionTimeout, _cpRC78Handling, _cpRCByteOffset,
                _cpRepeatReqCountApp, _cpStartMsgIndEnable, _cpSuspendQueueOnError,
                _cpTransmitIndEnable,
            };
        }

        public uint CP_Loopback
        {
            get => _cpLoopback.ComParamData;
            set => _cpLoopback.ComParamData = value;
        }
        public uint CP_P2Max
        {
            get => _cpP2Max.ComParamData;
            set => _cpP2Max.ComParamData = value;
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
        public uint CP_StartMsgIndEnable
        {
            get => _cpStartMsgIndEnable.ComParamData;
            set => _cpStartMsgIndEnable.ComParamData = value;
        }
        public uint CP_CyclicRespTimeout
        {
            get => _cpCyclicRespTimeout.ComParamData;
            set => _cpCyclicRespTimeout.ComParamData = value;
        }
        public uint CP_RepeatReqCountApp
        {
            get => _cpRepeatReqCountApp.ComParamData;
            set => _cpRepeatReqCountApp.ComParamData = value;
        }
        public uint CP_TransmitIndEnable
        {
            get => _cpTransmitIndEnable.ComParamData;
            set => _cpTransmitIndEnable.ComParamData = value;
        }
        public uint CP_SuspendQueueOnError
        {
            get => _cpSuspendQueueOnError.ComParamData;
            set => _cpSuspendQueueOnError.ComParamData = value;
        }
    }
}
