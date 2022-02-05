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
    public partial class TP20
    {
        public class CpTp20UniqueRespIdTable : CpUniqueRespIdTable, ITp20UniqueComParams
        {
            private readonly PduComParamOfTypeUint _cpmVWTP_ComfortDiag;
            private readonly PduComParamOfTypeUint _cpmVWTP_PhysIdFormat;
            private readonly PduComParamOfTypeUint _cpmVWTP_PhysReqIdCon;
            private readonly PduComParamOfTypeUint _cpmVWTP_PhysReqIdSetup;
            private readonly PduComParamOfTypeUint _cpmVWTP_PhysRespIdCon;
            private readonly PduComParamOfTypeUint _cpmVWTP_PhysRespIdSetup;


            public uint CPM_VWTP_ComfortDiag
            {
                get => _cpmVWTP_ComfortDiag.ComParamData;
                set => _cpmVWTP_ComfortDiag.ComParamData = value;
            }

            public uint CPM_VWTP_PhysIdFormat
            {
                get => _cpmVWTP_PhysIdFormat.ComParamData;
                set => _cpmVWTP_PhysIdFormat.ComParamData = value;
            }

            public uint CPM_VWTP_PhysReqIdCon
            {
                get => _cpmVWTP_PhysReqIdCon.ComParamData;
                set => _cpmVWTP_PhysReqIdCon.ComParamData = value;
            }

            public uint CPM_VWTP_PhysReqIdSetup
            {
                get => _cpmVWTP_PhysReqIdSetup.ComParamData;
                set => _cpmVWTP_PhysReqIdSetup.ComParamData = value;
            }

            public uint CPM_VWTP_PhysRespIdCon
            {
                get => _cpmVWTP_PhysRespIdCon.ComParamData;
                set => _cpmVWTP_PhysRespIdCon.ComParamData = value;
            }

            public uint CPM_VWTP_PhysRespIdSetup
            {
                get => _cpmVWTP_PhysRespIdSetup.ComParamData;
                set => _cpmVWTP_PhysRespIdSetup.ComParamData = value;
            }

            protected internal CpTp20UniqueRespIdTable(string cpEcuLayerShortName, HashRuleUniqueRespIdentifierFromCpEcuLayerShortName hashAlgo) :
                base(cpEcuLayerShortName, hashAlgo)
            {
                _cpmVWTP_ComfortDiag = (PduComParamOfTypeUint)CreateCp("CPM_VWTP_ComfortDiag", 1, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID);
                _cpmVWTP_PhysIdFormat = (PduComParamOfTypeUint)CreateCp("CPM_VWTP_PhysIdFormat", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID);
                _cpmVWTP_PhysReqIdCon = (PduComParamOfTypeUint)CreateCp("CPM_VWTP_PhysReqIdCon", 0x740, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID);
                _cpmVWTP_PhysReqIdSetup =
                    (PduComParamOfTypeUint)CreateCp("CPM_VWTP_PhysReqIdSetup", 0x200, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID);
                _cpmVWTP_PhysRespIdCon =
                    (PduComParamOfTypeUint)CreateCp("CPM_VWTP_PhysRespIdCon", 0x300, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID);
                _cpmVWTP_PhysRespIdSetup =
                    (PduComParamOfTypeUint)CreateCp("CPM_VWTP_PhysRespIdSetup", 0x201, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_UNIQUE_ID);

                CpList = new List<PduComParam>
                {
                    _cpmVWTP_ComfortDiag, _cpmVWTP_PhysIdFormat, _cpmVWTP_PhysReqIdCon,
                    _cpmVWTP_PhysReqIdSetup, _cpmVWTP_PhysRespIdCon, _cpmVWTP_PhysRespIdSetup
                };
            }
        }
    }
}
