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
    public partial class ISO_14230_2
    {
        public class CpIso142302UniqueRespIdTable : CpUniqueRespIdTable, IIso142302UniqueComParams
        {
            private readonly PduComParamOfTypeUint _cpEcuRespSourceAddress;
            private readonly PduComParamOfTypeUint _cpFuncRespFormatPriorityType;
            private readonly PduComParamOfTypeUint _cpFuncRespTargetAddr;
            private readonly PduComParamOfTypeUint _cpPhysRespFormatPriorityType;


            public uint CP_EcuRespSourceAddress
            {
                get => _cpEcuRespSourceAddress.ComParamData;
                set => _cpEcuRespSourceAddress.ComParamData = value;
            }

            public uint CP_FuncRespFormatPriorityType
            {
                get => _cpFuncRespFormatPriorityType.ComParamData;
                set => _cpFuncRespFormatPriorityType.ComParamData = value;
            }

            public uint CP_FuncRespTargetAddr
            {
                get => _cpFuncRespTargetAddr.ComParamData;
                set => _cpFuncRespTargetAddr.ComParamData = value;
            }

            public uint CP_PhysRespFormatPriorityType
            {
                get => _cpPhysRespFormatPriorityType.ComParamData;
                set => _cpPhysRespFormatPriorityType.ComParamData = value;
            }


            protected internal CpIso142302UniqueRespIdTable(string cpEcuLayerShortName, HashRuleUniqueRespIdentifierFromCpEcuLayerShortName hashAlgo)
                : base(cpEcuLayerShortName, hashAlgo)
            {
                _cpEcuRespSourceAddress = (PduComParamOfTypeUint)CreateCp("CP_EcuRespSourceAddress", 0x10, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID);
                _cpFuncRespFormatPriorityType = (PduComParamOfTypeUint)CreateCp("CP_FuncRespFormatPriorityType", 0xC0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID);
                _cpFuncRespTargetAddr = (PduComParamOfTypeUint)CreateCp("CP_FuncRespTargetAddr", 0xF1, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID);
                _cpPhysRespFormatPriorityType = (PduComParamOfTypeUint)CreateCp("CP_PhysRespFormatPriorityType", 0x80, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID);

                CpList = new List<PduComParam>
                {
                    _cpEcuRespSourceAddress, _cpFuncRespFormatPriorityType, _cpFuncRespTargetAddr,
                    _cpPhysRespFormatPriorityType
                };
            }
        }
    }
}
