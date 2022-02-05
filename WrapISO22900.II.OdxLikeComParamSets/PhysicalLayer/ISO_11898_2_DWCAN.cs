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

namespace ISO22900.II.OdxLikeComParamSets.PhysicalLayer
{
    public class ISO_11898_2_DWCAN : AbstractPhysicalLayer
    {
        private readonly PduComParamOfTypeUint _cpBaudrate;
        private readonly PduComParamOfTypeUint _cpCANFDBaudrate;
        private readonly PduComParamOfTypeUint _cpCANFDBitSamplePoint;
        private readonly PduComParamOfTypeUint _cpCANFDSyncJumpWidth;
        private readonly PduComParamOfTypeUint _cpBitSamplePoint;
        private readonly PduComParamOfTypeUint _cpListenOnly;
        private readonly PduComParamOfTypeUint _cpSamplesPerBit;
        private readonly PduComParamOfTypeUint _cpSyncJumpWidth;
        private readonly PduComParamOfTypeUint _cpTerminationType;
        private readonly PduComParamOfTypeUintField _cpCanBaudrateRecord;


        public uint CP_Baudrate
        {
            get => _cpBaudrate.ComParamData;
            set => _cpBaudrate.ComParamData = value;
        }

        public uint CP_CANFDBaudrate
        {
            get => _cpCANFDBaudrate.ComParamData;
            set => _cpCANFDBaudrate.ComParamData = value;
        }

        public uint CP_CANFDBitSamplePoint
        {
            get => _cpCANFDBitSamplePoint.ComParamData;
            set => _cpCANFDBitSamplePoint.ComParamData = value;
        }

        public uint CP_CANFDSyncJumpWidth
        {
            get => _cpCANFDSyncJumpWidth.ComParamData;
            set => _cpCANFDSyncJumpWidth.ComParamData = value;
        }

        public uint CP_BitSamplePoint
        {
            get => _cpBitSamplePoint.ComParamData;
            set => _cpBitSamplePoint.ComParamData = value;
        }

        public uint[] CP_CanBaudrateRecord
        {
            get => _cpCanBaudrateRecord.ComParamData.DataArray;
            set => _cpCanBaudrateRecord.ComParamData.DataArray = value;
        }

        public uint CP_ListenOnly
        {
            get => _cpListenOnly.ComParamData;
            set => _cpListenOnly.ComParamData = value;
        }

        public uint CP_SamplesPerBit
        {
            get => _cpSamplesPerBit.ComParamData;
            set => _cpSamplesPerBit.ComParamData = value;
        }

        public uint CP_SyncJumpWidth
        {
            get => _cpSyncJumpWidth.ComParamData;
            set => _cpSyncJumpWidth.ComParamData = value;
        }

        public uint CP_TerminationType
        {
            get => _cpTerminationType.ComParamData;
            set => _cpTerminationType.ComParamData = value;
        }

        public ISO_11898_2_DWCAN()
        {
            _cpBaudrate = (PduComParamOfTypeUint)CreateCp("CP_Baudrate", 500000, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_BUSTYPE);
            _cpCANFDBaudrate = (PduComParamOfTypeUint)CreateCp("CP_CANFDBaudrate", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_BUSTYPE);
            _cpCANFDBitSamplePoint = (PduComParamOfTypeUint)CreateCp("CP_CANFDBitSamplePoint", 80, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_BUSTYPE);
            _cpCANFDSyncJumpWidth = (PduComParamOfTypeUint)CreateCp("CP_CANFDSyncJumpWidth", 15, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_BUSTYPE);
            _cpBitSamplePoint = (PduComParamOfTypeUint)CreateCp("CP_BitSamplePoint", 80, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_BUSTYPE);
            _cpCanBaudrateRecord = (PduComParamOfTypeUintField)CreateCp("CP_CanBaudrateRecord", new uint[]{}, PduPt.PDU_PT_LONGFIELD, PduPc.PDU_PC_BUSTYPE);
            _cpListenOnly = (PduComParamOfTypeUint)CreateCp("CP_ListenOnly", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_BUSTYPE);
            _cpSamplesPerBit = (PduComParamOfTypeUint)CreateCp("CP_SamplesPerBit", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_BUSTYPE);
            _cpSyncJumpWidth = (PduComParamOfTypeUint)CreateCp("CP_SyncJumpWidth", 15, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_BUSTYPE);
            _cpTerminationType = (PduComParamOfTypeUint)CreateCp("CP_TerminationType", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_BUSTYPE);
            
            CpList = new List<PduComParam>
            {
                _cpBaudrate, _cpCANFDBaudrate, _cpCANFDBitSamplePoint,
                _cpCANFDSyncJumpWidth, _cpBitSamplePoint, _cpCanBaudrateRecord,
                _cpListenOnly, _cpSamplesPerBit, _cpSyncJumpWidth,
                _cpTerminationType
            };
        }

    }
}
