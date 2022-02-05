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

namespace ISO22900.II.OdxLikeComParamSets.TransportOrDataLinkLayer
{
    public partial class ISO_15765_2
    {
        public class CpIso157652UniqueRespIdTable : CpUniqueRespIdTable, IIso157652UniqueComParams
        {
            private readonly PduComParamOfTypeUint _cpCanPhysReqExtAddr;
            private readonly PduComParamOfTypeUint _cpCanPhysReqFormat;
            private readonly PduComParamOfTypeUint _cpCanPhysReqId;
            private readonly PduComParamOfTypeUint _cpCanRespUSDTExtAddr;
            private readonly PduComParamOfTypeUint _cpCanRespUSDTFormat;
            private readonly PduComParamOfTypeUint _cpCanRespUSDTId;
            private readonly PduComParamOfTypeUint _cpCanRespUUDTExtAddr;
            private readonly PduComParamOfTypeUint _cpCanRespUUDTFormat;
            private readonly PduComParamOfTypeUint _cpCanRespUUDTId;


            public uint CP_CanPhysReqExtAddr
            {
                get => _cpCanPhysReqExtAddr.ComParamData;
                set => _cpCanPhysReqExtAddr.ComParamData = value;
            }

            public uint CP_CanPhysReqFormat
            {
                get => _cpCanPhysReqFormat.ComParamData;
                set => _cpCanPhysReqFormat.ComParamData = value;
            }

            public uint CP_CanPhysReqId
            {
                get => _cpCanPhysReqId.ComParamData;
                set => _cpCanPhysReqId.ComParamData = value;
            }

            public uint CP_CanRespUSDTExtAddr
            {
                get => _cpCanRespUSDTExtAddr.ComParamData;
                set => _cpCanRespUSDTExtAddr.ComParamData = value;
            }

            public uint CP_CanRespUSDTFormat
            {
                get => _cpCanRespUSDTFormat.ComParamData;
                set => _cpCanRespUSDTFormat.ComParamData = value;
            }

            public uint CP_CanRespUSDTId
            {
                get => _cpCanRespUSDTId.ComParamData;
                set => _cpCanRespUSDTId.ComParamData = value;
            }

            public uint CP_CanRespUUDTExtAddr
            {
                get => _cpCanRespUUDTExtAddr.ComParamData;
                set => _cpCanRespUUDTExtAddr.ComParamData = value;
            }

            public uint CP_CanRespUUDTFormat
            {
                get => _cpCanRespUUDTFormat.ComParamData;
                set => _cpCanRespUUDTFormat.ComParamData = value;
            }

            public uint CP_CanRespUUDTId
            {
                get => _cpCanRespUUDTId.ComParamData;
                set => _cpCanRespUUDTId.ComParamData = value;
            }

            protected internal CpIso157652UniqueRespIdTable(string cpEcuLayerShortName, HashRuleUniqueRespIdentifierFromCpEcuLayerShortName hashAlgo)
                : base(cpEcuLayerShortName, hashAlgo)
            {
                _cpCanPhysReqExtAddr = (PduComParamOfTypeUint)CreateCp("CP_CanPhysReqExtAddr", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID);
                _cpCanPhysReqFormat = (PduComParamOfTypeUint)CreateCp("CP_CanPhysReqFormat", 5, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID);
                _cpCanPhysReqId = (PduComParamOfTypeUint)CreateCp("CP_CanPhysReqId", 0x7E0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID);
                _cpCanRespUSDTExtAddr = (PduComParamOfTypeUint)CreateCp("CP_CanRespUSDTExtAddr", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID);
                _cpCanRespUSDTFormat = (PduComParamOfTypeUint)CreateCp("CP_CanRespUSDTFormat", 5, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID);
                _cpCanRespUSDTId = (PduComParamOfTypeUint)CreateCp("CP_CanRespUSDTId", 0x7E8, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID);
                _cpCanRespUUDTExtAddr = (PduComParamOfTypeUint)CreateCp("CP_CanRespUUDTExtAddr", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID);
                _cpCanRespUUDTFormat = (PduComParamOfTypeUint)CreateCp("CP_CanRespUUDTFormat", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID);
                _cpCanRespUUDTId = (PduComParamOfTypeUint)CreateCp("CP_CanRespUUDTId", 0xFFFFFFFF, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID);

                CpList = new List<PduComParam>
                {
                    _cpCanPhysReqExtAddr, _cpCanPhysReqFormat, _cpCanPhysReqId,
                    _cpCanRespUSDTExtAddr, _cpCanRespUSDTFormat, _cpCanRespUSDTId,
                    _cpCanRespUUDTExtAddr, _cpCanRespUUDTFormat, _cpCanRespUUDTId
                };
            }
        }
    }
}
