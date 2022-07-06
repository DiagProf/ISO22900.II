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

namespace ISO22900.II.OdxLikeComParamSets.TransportOrDataLinkLayer
{
    public partial class ISO_15765_4 : AbstractTransportOrDataLinkLayer
    {
        private readonly PduComParamOfTypeUint _cpAr;
        private readonly PduComParamOfTypeUint _cpAs;
        private readonly PduComParamOfTypeUint _cpBlockSize;
        private readonly PduComParamOfTypeUint _cpBlockSizeOverride;
        private readonly PduComParamOfTypeUint _cpCANFDTxMaxDataLength;
        private readonly PduComParamOfTypeUint _cpMaxFirstFrameDataLength;
        private readonly PduComParamOfTypeUint _cpBr;
        private readonly PduComParamOfTypeUint _cpBs;
        private readonly PduComParamOfTypeUint _cpCanDataSizeOffset;
        private readonly PduComParamOfTypeUint _cpCanFillerByte;
        private readonly PduComParamOfTypeUint _cpCanFillerByteHandling;
        private readonly PduComParamOfTypeUint _cpCanFirstConsecutiveFrameValue;
        private readonly PduComParamOfTypeUint _cpCanFuncReqExtAddr;
        private readonly PduComParamOfTypeUint _cpCanFuncReqFormat;
        private readonly PduComParamOfTypeUint _cpCanFuncReqId;
        private readonly PduComParamOfTypeUint _cpCanMaxNumWaitFrames;
        private readonly PduComParamOfTypeUint _cpCr;
        private readonly PduComParamOfTypeUint _cpCs;
        private readonly PduComParamOfTypeUint _cpRepeatReqCountTrans;
        private readonly PduComParamOfTypeUint _cpRequestAddrMode;
        private readonly PduComParamOfTypeUint _cpSendRemoteFrame;
        private readonly PduComParamOfTypeUint _cpStMin;
        private readonly PduComParamOfTypeUint _cpStMinOverride;

        protected sealed override ISO_15765_4.CpIso157654UniqueRespIdTables UniqueRespIdTables { get; }
        public ISO_15765_4.IIso157654CpUniqueRespIdTables CP_UniqueRespIdTable => UniqueRespIdTables;

        //these are comfort functions to be able to access Iso157654CpUniqueRespIdTable[0] directly.This is the 90% use case
        public string CP_ECULayerShortName
        {
            get => UniqueRespIdTables[0].CP_ECULayerShortName;
            set => UniqueRespIdTables[0].CP_ECULayerShortName = value;
        }

        public uint CP_CanPhysReqExtAddr
        {
            get => UniqueRespIdTables[0].CP_CanPhysReqExtAddr;
            set => UniqueRespIdTables[0].CP_CanPhysReqExtAddr = value;
        }

        public uint CP_CanPhysReqFormat
        {
            get => UniqueRespIdTables[0].CP_CanPhysReqFormat;
            set => UniqueRespIdTables[0].CP_CanPhysReqFormat = value;
        }

        public uint CP_CanPhysReqId
        {
            get => UniqueRespIdTables[0].CP_CanPhysReqId;
            set => UniqueRespIdTables[0].CP_CanPhysReqId = value;
        }

        public uint CP_CanRespUSDTExtAddr
        {
            get => UniqueRespIdTables[0].CP_CanRespUSDTExtAddr;
            set => UniqueRespIdTables[0].CP_CanRespUSDTExtAddr = value;
        }

        public uint CP_CanRespUSDTFormat
        {
            get => UniqueRespIdTables[0].CP_CanRespUSDTFormat;
            set => UniqueRespIdTables[0].CP_CanRespUSDTFormat = value;
        }

        public uint CP_CanRespUSDTId
        {
            get => UniqueRespIdTables[0].CP_CanRespUSDTId;
            set => UniqueRespIdTables[0].CP_CanRespUSDTId = value;
        }

        public uint CP_CanRespUUDTExtAddr
        {
            get => UniqueRespIdTables[0].CP_CanRespUUDTExtAddr;
            set => UniqueRespIdTables[0].CP_CanRespUUDTExtAddr = value;
        }

        public uint CP_CanRespUUDTFormat
        {
            get => UniqueRespIdTables[0].CP_CanRespUUDTFormat;
            set => UniqueRespIdTables[0].CP_CanRespUUDTFormat = value;
        }

        public uint CP_CanRespUUDTId
        {
            get => UniqueRespIdTables[0].CP_CanRespUUDTId;
            set => UniqueRespIdTables[0].CP_CanRespUUDTId = value;
        }

        //normal comParams

        public uint CP_Ar
        {
            get => _cpAr.ComParamData;
            set => _cpAr.ComParamData = value;
        }

        public uint CP_As
        {
            get => _cpAs.ComParamData;
            set => _cpAs.ComParamData = value;
        }

        public uint CP_BlockSize
        {
            get => _cpBlockSize.ComParamData;
            set => _cpBlockSize.ComParamData = value;
        }

        public uint CP_BlockSizeOverride
        {
            get => _cpBlockSizeOverride.ComParamData;
            set => _cpBlockSizeOverride.ComParamData = value;
        }

        public uint CP_CANFDTxMaxDataLength
        {
            get => _cpCANFDTxMaxDataLength.ComParamData;
            set => _cpCANFDTxMaxDataLength.ComParamData = value;
        }

        public uint CP_MaxFirstFrameDataLength
        {
            get => _cpMaxFirstFrameDataLength.ComParamData;
            set => _cpMaxFirstFrameDataLength.ComParamData = value;
        }

        public uint CP_Br
        {
            get => _cpBr.ComParamData;
            set => _cpBr.ComParamData = value;
        }

        public uint CP_Bs
        {
            get => _cpBs.ComParamData;
            set => _cpBs.ComParamData = value;
        }

        public uint CP_CanDataSizeOffset
        {
            get => _cpCanDataSizeOffset.ComParamData;
            set => _cpCanDataSizeOffset.ComParamData = value;
        }
        
        public uint CP_CanFillerByte
        {
            get => _cpCanFillerByte.ComParamData;
            set => _cpCanFillerByte.ComParamData = value;
        }

        public uint CP_CanFillerByteHandling
        {
            get => _cpCanFillerByteHandling.ComParamData;
            set => _cpCanFillerByteHandling.ComParamData = value;
        }

        public uint CP_CanFirstConsecutiveFrameValue
        {
            get => _cpCanFirstConsecutiveFrameValue.ComParamData;
            set => _cpCanFirstConsecutiveFrameValue.ComParamData = value;
        }

        public uint CP_CanFuncReqExtAddr
        {
            get => _cpCanFuncReqExtAddr.ComParamData;
            set => _cpCanFuncReqExtAddr.ComParamData = value;
        }

        public uint CP_CanFuncReqFormat
        {
            get => _cpCanFuncReqFormat.ComParamData;
            set => _cpCanFuncReqFormat.ComParamData = value;
        }

        public uint CP_CanFuncReqId
        {
            get => _cpCanFuncReqId.ComParamData;
            set => _cpCanFuncReqId.ComParamData = value;
        }

        public uint CP_CanMaxNumWaitFrames
        {
            get => _cpCanMaxNumWaitFrames.ComParamData;
            set => _cpCanMaxNumWaitFrames.ComParamData = value;
        }

        public uint CP_Cr
        {
            get => _cpCr.ComParamData;
            set => _cpCr.ComParamData = value;
        }

        public uint CP_Cs
        {
            get => _cpCs.ComParamData;
            set => _cpCs.ComParamData = value;
        }

        public uint CP_RepeatReqCountTrans
        {
            get => _cpRepeatReqCountTrans.ComParamData;
            set => _cpRepeatReqCountTrans.ComParamData = value;
        }

        public uint CP_RequestAddrMode
        {
            get => _cpRequestAddrMode.ComParamData;
            set => _cpRequestAddrMode.ComParamData = value;
        }

        public uint CP_SendRemoteFrame
        {
            get => _cpSendRemoteFrame.ComParamData;
            set => _cpSendRemoteFrame.ComParamData = value;
        }

        public uint CP_StMin
        {
            get => _cpStMin.ComParamData;
            set => _cpStMin.ComParamData = value;
        }

        public uint CP_StMinOverride
        {
            get => _cpStMinOverride.ComParamData;
            set => _cpStMinOverride.ComParamData = value;
        }

        public ISO_15765_4(HashRuleUniqueRespIdentifierFromCpEcuLayerShortName hashAlgo)
        {
            HashAlgo = hashAlgo;
            UniqueRespIdTables = new ISO_15765_4.CpIso157654UniqueRespIdTables(HashAlgo);
            //init table one //We need at least one table
            UniqueRespIdTables.Add(new ISO_15765_4.CpIso157654UniqueRespIdTable("", HashAlgo));

            _cpAr = (PduComParamOfTypeUint)CreateCp("CP_Ar", 25000, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TIMING);
            _cpAs = (PduComParamOfTypeUint)CreateCp("CP_As", 25000, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TIMING);
            _cpBlockSize = (PduComParamOfTypeUint)CreateCp("CP_BlockSize", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpBlockSizeOverride = (PduComParamOfTypeUint)CreateCp("CP_BlockSizeOverride", 0xFFFF, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpCANFDTxMaxDataLength = (PduComParamOfTypeUint)CreateCp("CP_CANFDTxMaxDataLength", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpMaxFirstFrameDataLength = (PduComParamOfTypeUint)CreateCp("CP_MaxFirstFrameDataLength", 0xFFF, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpBr = (PduComParamOfTypeUint)CreateCp("CP_Br", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TIMING);
            _cpBs = (PduComParamOfTypeUint)CreateCp("CP_Bs", 75000, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TIMING);
            _cpCanDataSizeOffset = (PduComParamOfTypeUint)CreateCp("CP_CanDataSizeOffset", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpCanFillerByte = (PduComParamOfTypeUint)CreateCp("CP_CanFillerByte", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpCanFillerByteHandling = (PduComParamOfTypeUint)CreateCp("CP_CanFillerByteHandling", 1, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpCanFirstConsecutiveFrameValue = (PduComParamOfTypeUint)CreateCp("CP_CanFirstConsecutiveFrameValue", 1, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpCanFuncReqExtAddr = (PduComParamOfTypeUint)CreateCp("CP_CanFuncReqExtAddr", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpCanFuncReqFormat = (PduComParamOfTypeUint)CreateCp("CP_CanFuncReqFormat", 5, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpCanFuncReqId = (PduComParamOfTypeUint)CreateCp("CP_CanFuncReqId", 0x7DF, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpCanMaxNumWaitFrames = (PduComParamOfTypeUint)CreateCp("CP_CanMaxNumWaitFrames", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpCr = (PduComParamOfTypeUint)CreateCp("CP_Cr", 150000, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TIMING);
            _cpCs = (PduComParamOfTypeUint)CreateCp("CP_Cs", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TIMING);
            _cpRepeatReqCountTrans = (PduComParamOfTypeUint)CreateCp("CP_RepeatReqCountTrans", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_ERRHDL);
            _cpRequestAddrMode = (PduComParamOfTypeUint)CreateCp("CP_RequestAddrMode", 2, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpSendRemoteFrame = (PduComParamOfTypeUint)CreateCp("CP_SendRemoteFrame", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpStMin = (PduComParamOfTypeUint)CreateCp("CP_StMin", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TIMING);
            _cpStMinOverride = (PduComParamOfTypeUint)CreateCp("CP_StMinOverride", 0xFFFFFFFF, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TIMING);

            CpList = new List<PduComParam>
            {
                _cpAr, _cpAs, _cpBlockSize,
                _cpBlockSizeOverride, _cpCANFDTxMaxDataLength, _cpMaxFirstFrameDataLength,
                _cpBr, _cpBs, _cpCanDataSizeOffset, _cpCanFillerByte,
                _cpCanFillerByteHandling, _cpCanFirstConsecutiveFrameValue, _cpCanFuncReqExtAddr,
                _cpCanFuncReqFormat, _cpCanFuncReqId, _cpCanMaxNumWaitFrames,
                _cpCr, _cpCs, _cpRequestAddrMode, _cpRepeatReqCountTrans,
                _cpSendRemoteFrame, _cpStMin, _cpStMinOverride
            };
        }
    }
}
