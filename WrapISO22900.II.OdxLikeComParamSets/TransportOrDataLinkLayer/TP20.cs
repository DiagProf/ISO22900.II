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
    /// <summary>
    ///     Params for TP2.0
    /// </summary>
    public partial class TP20 : AbstractTransportOrDataLinkLayer
    {
        private readonly PduComParamOfTypeUint _cpEnableConcatenation;
        private readonly PduComParamOfTypeUint _cpmVWTP_ApplicationType;
        private readonly PduComParamOfTypeUint _cpmVWTP_BlockSize;
        private readonly PduComParamOfTypeUint _cpmVWTP_BlockSizeOverrideHandling;
        private readonly PduComParamOfTypeUint _cpmVWTP_BlockSizeOverrideValue;
        private readonly PduComParamOfTypeUint _cpmVWTP_ConnectionTestMode;
        private readonly PduComParamOfTypeUint _cpmVWTP_DC_RequestTime;
        private readonly PduComParamOfTypeUint _cpmVWTP_DC_ResponseTimeout;
        private readonly PduComParamOfTypeUint _cpmVWTP_DestAddr;
        private readonly PduComParamOfTypeUint _cpmVWTP_EOMAckHandling;
        private readonly PduComParamOfTypeUint _cpmVWTP_InfoReceive;
        private readonly PduComParamOfTypeUint _cpmVWTP_InfoTransmit;
        private readonly PduComParamOfTypeUint _cpmVWTP_MNCT;
        private readonly PduComParamOfTypeUint _cpmVWTP_MNT;
        private readonly PduComParamOfTypeUint _cpmVWTP_MNTB;
        private readonly PduComParamOfTypeUint _cpmVWTP_MNTC;
        private readonly PduComParamOfTypeUint _cpmVWTP_SndTimeout;
        private readonly PduComParamOfTypeUint _cpmVWTP_TE;
        private readonly PduComParamOfTypeUint _cpmVWTP_TesterTEDelay;
        private readonly PduComParamOfTypeUint _cpmVWTP_TN;
        private readonly PduComParamOfTypeUint _cpmVWTP_T1;
        private readonly PduComParamOfTypeUint _cpmVWTP_TesterAckDelay;
        private readonly PduComParamOfTypeUint _cpmVWTP_T1EcuOverrideHandling;
        private readonly PduComParamOfTypeUint _cpmVWTP_T1EcuOverrideValue;
        private readonly PduComParamOfTypeUint _cpmVWTP_T3;
        private readonly PduComParamOfTypeUint _cpmVWTP_T3EcuOverrideHandling;
        private readonly PduComParamOfTypeUint _cpmVWTP_T3EcuOverrideValue;
        private readonly PduComParamOfTypeUint _cpmVWTP_T_CT;
        private readonly PduComParamOfTypeUint _cpmVWTP_T_Wait;
        private readonly PduComParamOfTypeUint _cpmVWTP_TesterAddr;

        protected sealed override CpTp20UniqueRespIdTables UniqueRespIdTables { get; }
        public ITp20CpUniqueRespIdTables CP_UniqueRespIdTable => UniqueRespIdTables;

        //these are comfort functions to be able to access Tp20CpUniqueRespIdTable[0] directly.This is the 90% use case
        public string CP_ECULayerShortName
        {
            get => UniqueRespIdTables[0].CP_ECULayerShortName;
            set => UniqueRespIdTables[0].CP_ECULayerShortName = value;
        }

        public uint CPM_VWTP_ComfortDiag
        {
            get => UniqueRespIdTables[0].CPM_VWTP_ComfortDiag;
            set => UniqueRespIdTables[0].CPM_VWTP_ComfortDiag = value;
        }

        public uint CPM_VWTP_PhysIdFormat
        {
            get => UniqueRespIdTables[0].CPM_VWTP_PhysIdFormat;
            set => UniqueRespIdTables[0].CPM_VWTP_PhysIdFormat = value;
        }

        public uint CPM_VWTP_PhysReqIdCon
        {
            get => UniqueRespIdTables[0].CPM_VWTP_PhysReqIdCon;
            set => UniqueRespIdTables[0].CPM_VWTP_PhysReqIdCon = value;
        }

        public uint CPM_VWTP_PhysReqIdSetup
        {
            get => UniqueRespIdTables[0].CPM_VWTP_PhysReqIdSetup;
            set => UniqueRespIdTables[0].CPM_VWTP_PhysReqIdSetup = value;
        }

        public uint CPM_VWTP_PhysRespIdCon
        {
            get => UniqueRespIdTables[0].CPM_VWTP_PhysRespIdCon;
            set => UniqueRespIdTables[0].CPM_VWTP_PhysRespIdCon = value;
        }

        public uint CPM_VWTP_PhysRespIdSetup
        {
            get => UniqueRespIdTables[0].CPM_VWTP_PhysRespIdSetup;
            set => UniqueRespIdTables[0].CPM_VWTP_PhysRespIdSetup = value;
        }

        //normal comParams

        public uint CP_EnableConcatenation
        {
            get => _cpEnableConcatenation.ComParamData;
            set => _cpEnableConcatenation.ComParamData = value;
        }

        public uint CPM_VWTP_ApplicationType
        {
            get => _cpmVWTP_ApplicationType.ComParamData;
            set => _cpmVWTP_ApplicationType.ComParamData = value;
        }

        public uint CPM_VWTP_BlockSize
        {
            get => _cpmVWTP_BlockSize.ComParamData;
            set => _cpmVWTP_BlockSize.ComParamData = value;
        }

        public uint CPM_VWTP_BlockSizeOverrideHandling
        {
            get => _cpmVWTP_BlockSizeOverrideHandling.ComParamData;
            set => _cpmVWTP_BlockSizeOverrideHandling.ComParamData = value;
        }

        public uint CPM_VWTP_BlockSizeOverrideValue
        {
            get => _cpmVWTP_BlockSizeOverrideValue.ComParamData;
            set => _cpmVWTP_BlockSizeOverrideValue.ComParamData = value;
        }

        public uint CPM_VWTP_ConnectionTestMode
        {
            get => _cpmVWTP_ConnectionTestMode.ComParamData;
            set => _cpmVWTP_ConnectionTestMode.ComParamData = value;
        }

        public uint CPM_VWTP_DC_RequestTime
        {
            get => _cpmVWTP_DC_RequestTime.ComParamData;
            set => _cpmVWTP_DC_RequestTime.ComParamData = value;
        }

        public uint CPM_VWTP_DC_ResponseTimeout
        {
            get => _cpmVWTP_DC_ResponseTimeout.ComParamData;
            set => _cpmVWTP_DC_ResponseTimeout.ComParamData = value;
        }

        public uint CPM_VWTP_DestAddr
        {
            get => _cpmVWTP_DestAddr.ComParamData;
            set => _cpmVWTP_DestAddr.ComParamData = value;
        }

        public uint CPM_VWTP_EOMAckHandling
        {
            get => _cpmVWTP_EOMAckHandling.ComParamData;
            set => _cpmVWTP_EOMAckHandling.ComParamData = value;
        }

        public uint CPM_VWTP_InfoReceive
        {
            get => _cpmVWTP_InfoReceive.ComParamData;
            set => _cpmVWTP_InfoReceive.ComParamData = value;
        }

        public uint CPM_VWTP_InfoTransmit
        {
            get => _cpmVWTP_InfoTransmit.ComParamData;
            set => _cpmVWTP_InfoTransmit.ComParamData = value;
        }

        public uint CPM_VWTP_MNCT
        {
            get => _cpmVWTP_MNCT.ComParamData;
            set => _cpmVWTP_MNCT.ComParamData = value;
        }

        public uint CPM_VWTP_MNT
        {
            get => _cpmVWTP_MNT.ComParamData;
            set => _cpmVWTP_MNT.ComParamData = value;
        }

        public uint CPM_VWTP_MNTB
        {
            get => _cpmVWTP_MNTB.ComParamData;
            set => _cpmVWTP_MNTB.ComParamData = value;
        }

        public uint CPM_VWTP_MNTC
        {
            get => _cpmVWTP_MNTC.ComParamData;
            set => _cpmVWTP_MNTC.ComParamData = value;
        }

        public uint CPM_VWTP_SndTimeout
        {
            get => _cpmVWTP_SndTimeout.ComParamData;
            set => _cpmVWTP_SndTimeout.ComParamData = value;
        }

        public uint CPM_VWTP_TE
        {
            get => _cpmVWTP_TE.ComParamData;
            set => _cpmVWTP_TE.ComParamData = value;
        }

        public uint CPM_VWTP_TesterTEDelay
        {
            get => _cpmVWTP_TesterTEDelay.ComParamData;
            set => _cpmVWTP_TesterTEDelay.ComParamData = value;
        }

        public uint CPM_VWTP_TN
        {
            get => _cpmVWTP_TN.ComParamData;
            set => _cpmVWTP_TN.ComParamData = value;
        }

        public uint CPM_VWTP_T1
        {
            get => _cpmVWTP_T1.ComParamData;
            set => _cpmVWTP_T1.ComParamData = value;
        }

        public uint CPM_VWTP_TesterAckDelay
        {
            get => _cpmVWTP_TesterAckDelay.ComParamData;
            set => _cpmVWTP_TesterAckDelay.ComParamData = value;
        }

        public uint CPM_VWTP_T1EcuOverrideHandling
        {
            get => _cpmVWTP_T1EcuOverrideHandling.ComParamData;
            set => _cpmVWTP_T1EcuOverrideHandling.ComParamData = value;
        }

        public uint CPM_VWTP_T1EcuOverrideValue
        {
            get => _cpmVWTP_T1EcuOverrideValue.ComParamData;
            set => _cpmVWTP_T1EcuOverrideValue.ComParamData = value;
        }

        public uint CPM_VWTP_T3
        {
            get => _cpmVWTP_T3.ComParamData;
            set => _cpmVWTP_T3.ComParamData = value;
        }

        public uint CPM_VWTP_T3EcuOverrideHandling
        {
            get => _cpmVWTP_T3EcuOverrideHandling.ComParamData;
            set => _cpmVWTP_T3EcuOverrideHandling.ComParamData = value;
        }

        public uint CPM_VWTP_T3EcuOverrideValue
        {
            get => _cpmVWTP_T3EcuOverrideValue.ComParamData;
            set => _cpmVWTP_T3EcuOverrideValue.ComParamData = value;
        }

        public uint CPM_VWTP_T_CT
        {
            get => _cpmVWTP_T_CT.ComParamData;
            set => _cpmVWTP_T_CT.ComParamData = value;
        }

        public uint CPM_VWTP_T_Wait
        {
            get => _cpmVWTP_T_Wait.ComParamData;
            set => _cpmVWTP_T_Wait.ComParamData = value;
        }

        public uint CPM_VWTP_TesterAddr
        {
            get => _cpmVWTP_TesterAddr.ComParamData;
            set => _cpmVWTP_TesterAddr.ComParamData = value;
        }

        public TP20(HashRuleUniqueRespIdentifierFromCpEcuLayerShortName hashAlgo)
        {
            HashAlgo = hashAlgo;
            UniqueRespIdTables = new CpTp20UniqueRespIdTables(HashAlgo);
            //init table one //We need at least one table
            UniqueRespIdTables.Add(new CpTp20UniqueRespIdTable("", HashAlgo));

            _cpEnableConcatenation = (PduComParamOfTypeUint)CreateCp("CP_EnableConcatenation", 1, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpmVWTP_ApplicationType = (PduComParamOfTypeUint)CreateCp("CPM_VWTP_ApplicationType", 1, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_INIT);
            _cpmVWTP_BlockSize = (PduComParamOfTypeUint)CreateCp("CPM_VWTP_BlockSize", 15, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpmVWTP_BlockSizeOverrideHandling = (PduComParamOfTypeUint)CreateCp("CPM_VWTP_BlockSizeOverrideHandling", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpmVWTP_BlockSizeOverrideValue = (PduComParamOfTypeUint)CreateCp("CPM_VWTP_BlockSizeOverrideValue", 15, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpmVWTP_ConnectionTestMode = (PduComParamOfTypeUint)CreateCp("CPM_VWTP_ConnectionTestMode", 1, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpmVWTP_DC_RequestTime = (PduComParamOfTypeUint)CreateCp("CPM_VWTP_DC_RequestTime", 50000, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TIMING);
            _cpmVWTP_DC_ResponseTimeout = (PduComParamOfTypeUint)CreateCp("CPM_VWTP_DC_ResponseTimeout", 50000, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TIMING);
            _cpmVWTP_DestAddr = (PduComParamOfTypeUint)CreateCp("CPM_VWTP_DestAddr", 1, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_INIT);
            _cpmVWTP_EOMAckHandling = (PduComParamOfTypeUint)CreateCp("CPM_VWTP_EOMAckHandling", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpmVWTP_InfoReceive = (PduComParamOfTypeUint)CreateCp("CPM_VWTP_InfoReceive", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_INIT);
            _cpmVWTP_InfoTransmit = (PduComParamOfTypeUint)CreateCp("CPM_VWTP_InfoTransmit", 1, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_INIT);
            _cpmVWTP_MNCT = (PduComParamOfTypeUint)CreateCp("CPM_VWTP_MNCT", 5, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpmVWTP_MNT = (PduComParamOfTypeUint)CreateCp("CPM_VWTP_MNT", 2, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpmVWTP_MNTB = (PduComParamOfTypeUint)CreateCp("CPM_VWTP_MNTB", 5, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpmVWTP_MNTC = (PduComParamOfTypeUint)CreateCp("CPM_VWTP_MNTC", 10, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpmVWTP_SndTimeout = (PduComParamOfTypeUint)CreateCp("CPM_VWTP_SndTimeout", 100000, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TIMING);
            _cpmVWTP_TE = (PduComParamOfTypeUint)CreateCp("CPM_VWTP_TE", 50000, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TIMING);
            _cpmVWTP_TesterTEDelay = (PduComParamOfTypeUint)CreateCp("CPM_VWTP_TesterTEDelay", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TIMING);
            _cpmVWTP_TN = (PduComParamOfTypeUint)CreateCp("CPM_VWTP_TN", 50000, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TIMING);
            _cpmVWTP_T1 = (PduComParamOfTypeUint)CreateCp("CPM_VWTP_T1", 100000, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TIMING);
            _cpmVWTP_TesterAckDelay = (PduComParamOfTypeUint)CreateCp("CPM_VWTP_TesterAckDelay", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TIMING);
            _cpmVWTP_T1EcuOverrideHandling = (PduComParamOfTypeUint)CreateCp("CPM_VWTP_T1EcuOverrideHandling", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TIMING);
            _cpmVWTP_T1EcuOverrideValue = (PduComParamOfTypeUint)CreateCp("CPM_VWTP_T1EcuOverrideValue", 50000, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TIMING);
            _cpmVWTP_T3 = (PduComParamOfTypeUint)CreateCp("CPM_VWTP_T3", 5000, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TIMING);
            _cpmVWTP_T3EcuOverrideHandling = (PduComParamOfTypeUint)CreateCp("CPM_VWTP_T3EcuOverrideHandling", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TIMING);
            _cpmVWTP_T3EcuOverrideValue = (PduComParamOfTypeUint)CreateCp("CPM_VWTP_T3EcuOverrideValue", 5000, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TIMING);
            _cpmVWTP_T_CT = (PduComParamOfTypeUint)CreateCp("CPM_VWTP_T_CT", 1000000, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TIMING);
            _cpmVWTP_T_Wait = (PduComParamOfTypeUint)CreateCp("CPM_VWTP_T_Wait", 100000, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TIMING);
            _cpmVWTP_TesterAddr = (PduComParamOfTypeUint)CreateCp("CPM_VWTP_TesterAddr", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_INIT);

            CpList = new List<PduComParam>
            {
                _cpEnableConcatenation, _cpmVWTP_ApplicationType, _cpmVWTP_BlockSize,
                _cpmVWTP_BlockSizeOverrideHandling, _cpmVWTP_BlockSizeOverrideValue, _cpmVWTP_ConnectionTestMode,
                _cpmVWTP_DC_RequestTime, _cpmVWTP_DC_ResponseTimeout, _cpmVWTP_DestAddr,
                _cpmVWTP_EOMAckHandling, _cpmVWTP_InfoReceive, _cpmVWTP_InfoTransmit,
                _cpmVWTP_MNCT, _cpmVWTP_MNT, _cpmVWTP_MNTB,
                _cpmVWTP_MNTC, _cpmVWTP_SndTimeout, _cpmVWTP_TE,
                _cpmVWTP_TesterTEDelay, _cpmVWTP_TN, _cpmVWTP_T1,
                _cpmVWTP_TesterAckDelay, _cpmVWTP_T1EcuOverrideHandling, _cpmVWTP_T1EcuOverrideValue,
                _cpmVWTP_T3, _cpmVWTP_T3EcuOverrideHandling, _cpmVWTP_T3EcuOverrideValue,
                _cpmVWTP_T_CT, _cpmVWTP_T_Wait, _cpmVWTP_TesterAddr
            };
        }
    }
}
