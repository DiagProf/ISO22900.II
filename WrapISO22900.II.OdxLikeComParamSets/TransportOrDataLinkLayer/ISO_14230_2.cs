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
    public partial class ISO_14230_2 : AbstractTransportOrDataLinkLayer
    {
        private readonly PduComParamOfTypeUint _cp5BaudAddressFunc;
        private readonly PduComParamOfTypeUint _cp5BaudAddressPhys;
        private readonly PduComParamOfTypeUint _cp5BaudMode;
        private readonly PduComParamOfTypeStructField _cpAccessTimingOverride;
        private readonly PduComParamOfTypeStructField _cpExtendedTiming;
        private readonly PduComParamOfTypeUint _cpFillerByte;
        private readonly PduComParamOfTypeUint _cpFillerByteHandling;
        private readonly PduComParamOfTypeUint _cpFillerByteLength;
        private readonly PduComParamOfTypeUint _cpEnableConcatenation;
        private readonly PduComParamOfTypeUint _cpFuncReqFormatPriorityType;
        private readonly PduComParamOfTypeUint _cpFuncReqTargetAddr;
        private readonly PduComParamOfTypeUint _cpHeaderFormatKW;
        private readonly PduComParamOfTypeUint _cpInitializationSettings;
        private readonly PduComParamOfTypeUint _cpP1Max;
        private readonly PduComParamOfTypeUint _cpP4Max;
        private readonly PduComParamOfTypeUint _cpP4Min;
        private readonly PduComParamOfTypeUint _cpPhysReqFormatPriorityType;
        private readonly PduComParamOfTypeUint _cpPhysReqTargetAddr;
        private readonly PduComParamOfTypeUint _cpRequestAddrMode;
        private readonly PduComParamOfTypeUint _cpTesterSourceAddress;
        private readonly PduComParamOfTypeUint _cpTIdle;
        private readonly PduComParamOfTypeUint _cpTInil;
        private readonly PduComParamOfTypeUint _cpTWup;
        private readonly PduComParamOfTypeUint _cpW1Max;
        private readonly PduComParamOfTypeUint _cpW2Max;
        private readonly PduComParamOfTypeUint _cpW3Max;
        private readonly PduComParamOfTypeUint _cpW4Max;
        private readonly PduComParamOfTypeUint _cpW4Min;
        //private readonly PduComParamOfTypeUint _cpmVWTP_5BaudAddressParity;
        //private readonly PduComParamOfTypeUint _cpmVW2000_5BaudFlashProgInit;

        protected sealed override CpIso142302UniqueRespIdTables UniqueRespIdTables { get; }
        public IIso142302CpUniqueRespIdTables CP_UniqueRespIdTable => UniqueRespIdTables;

        //these are comfort functions to be able to access Iso142302CpUniqueRespIdTable[0] directly.This is the 90% use case
        public string CP_ECULayerShortName
        {
            get => UniqueRespIdTables[0].CP_ECULayerShortName;
            set => UniqueRespIdTables[0].CP_ECULayerShortName = value;
        }

        public uint CP_EcuRespSourceAddress
        {
            get => UniqueRespIdTables[0].CP_EcuRespSourceAddress;
            set => UniqueRespIdTables[0].CP_EcuRespSourceAddress = value;
        }

        public uint CP_FuncRespFormatPriorityType
        {
            get => UniqueRespIdTables[0].CP_FuncRespFormatPriorityType;
            set => UniqueRespIdTables[0].CP_FuncRespFormatPriorityType = value;
        }

        public uint CP_FuncRespTargetAddr
        {
            get => UniqueRespIdTables[0].CP_FuncRespTargetAddr;
            set => UniqueRespIdTables[0].CP_FuncRespTargetAddr = value;
        }

        public uint CP_PhysRespFormatPriorityType
        {
            get => UniqueRespIdTables[0].CP_PhysRespFormatPriorityType;
            set => UniqueRespIdTables[0].CP_PhysRespFormatPriorityType = value;
        }

        //normal comParams

        public uint CP_5BaudAddressFunc
        {
            get => _cp5BaudAddressFunc.ComParamData;
            set => _cp5BaudAddressFunc.ComParamData = value;
        }

        public uint CP_5BaudAddressPhys
        {
            get => _cp5BaudAddressPhys.ComParamData;
            set => _cp5BaudAddressPhys.ComParamData = value;
        }

        public uint CP_5BaudMode
        {
            get => _cp5BaudMode.ComParamData;
            set => _cp5BaudMode.ComParamData = value;
        }

        public PduParamStructAccessTiming[] CP_AccessTimingOverride
        {
            get => (PduParamStructAccessTiming[])_cpAccessTimingOverride.ComParamData.StructArray;
            set
            {
                for ( var i = 0; i < value.Length; i++ )
                {
                    if ( i < _cpAccessTimingOverride.ComParamData.ParamMaxEntries )
                    {
                        _cpAccessTimingOverride.ComParamData.StructArray[i] = value[i];
                    }
                }
            }
        }

        public PduParamStructAccessTiming[] CP_ExtendedTiming
        {
            get => (PduParamStructAccessTiming[])_cpExtendedTiming.ComParamData.StructArray;
            set
            {
                for ( var i = 0; i < value.Length; i++ )
                {
                    if ( i < _cpExtendedTiming.ComParamData.ParamMaxEntries )
                    {
                        _cpExtendedTiming.ComParamData.StructArray[i] = value[i];
                    }
                }
            }
        }

        public uint CP_FillerByte
        {
            get => _cpFillerByte.ComParamData;
            set => _cpFillerByte.ComParamData = value;
        }

        public uint CP_FillerByteHandling
        {
            get => _cpFillerByteHandling.ComParamData;
            set => _cpFillerByteHandling.ComParamData = value;
        }

        public uint CP_FillerByteLength
        {
            get => _cpFillerByteLength.ComParamData;
            set => _cpFillerByteLength.ComParamData = value;
        }

        public uint CP_EnableConcatenation
        {
            get => _cpEnableConcatenation.ComParamData;
            set => _cpEnableConcatenation.ComParamData = value;
        }

        public uint CP_FuncReqFormatPriorityType
        {
            get => _cpFuncReqFormatPriorityType.ComParamData;
            set => _cpFuncReqFormatPriorityType.ComParamData = value;
        }

        public uint CP_FuncReqTargetAddr
        {
            get => _cpFuncReqTargetAddr.ComParamData;
            set => _cpFuncReqTargetAddr.ComParamData = value;
        }

        public uint CP_HeaderFormatKW
        {
            get => _cpHeaderFormatKW.ComParamData;
            set => _cpHeaderFormatKW.ComParamData = value;
        }

        public uint CP_InitializationSettings
        {
            get => _cpInitializationSettings.ComParamData;
            set => _cpInitializationSettings.ComParamData = value;
        }

        public uint CP_P1Max
        {
            get => _cpP1Max.ComParamData;
            set => _cpP1Max.ComParamData = value;
        }

        public uint CP_P4Max
        {
            get => _cpP4Max.ComParamData;
            set => _cpP4Max.ComParamData = value;
        }

        public uint CP_P4Min
        {
            get => _cpP4Min.ComParamData;
            set => _cpP4Min.ComParamData = value;
        }

        public uint CP_PhysReqFormatPriorityType
        {
            get => _cpPhysReqFormatPriorityType.ComParamData;
            set => _cpPhysReqFormatPriorityType.ComParamData = value;
        }

        public uint CP_PhysReqTargetAddr
        {
            get => _cpPhysReqTargetAddr.ComParamData;
            set => _cpPhysReqTargetAddr.ComParamData = value;
        }

        public uint CP_RequestAddrMode
        {
            get => _cpRequestAddrMode.ComParamData;
            set => _cpRequestAddrMode.ComParamData = value;
        }

        public uint CP_TesterSourceAddress
        {
            get => _cpTesterSourceAddress.ComParamData;
            set => _cpTesterSourceAddress.ComParamData = value;
        }

        public uint CP_TIdle
        {
            get => _cpTIdle.ComParamData;
            set => _cpTIdle.ComParamData = value;
        }

        public uint CP_TInil
        {
            get => _cpTInil.ComParamData;
            set => _cpTInil.ComParamData = value;
        }

        public uint CP_TWup
        {
            get => _cpTWup.ComParamData;
            set => _cpTWup.ComParamData = value;
        }

        public uint CP_W1Max
        {
            get => _cpW1Max.ComParamData;
            set => _cpW1Max.ComParamData = value;
        }

        public uint CP_W2Max
        {
            get => _cpW2Max.ComParamData;
            set => _cpW2Max.ComParamData = value;
        }

        public uint CP_W3Max
        {
            get => _cpW3Max.ComParamData;
            set => _cpW3Max.ComParamData = value;
        }

        public uint CP_W4Max
        {
            get => _cpW4Max.ComParamData;
            set => _cpW4Max.ComParamData = value;
        }

        public uint CP_W4Min
        {
            get => _cpW4Min.ComParamData;
            set => _cpW4Min.ComParamData = value;
        }

        //public uint CPM_VWTP_5BaudAddressParity
        //{
        //    get => _cpmVWTP_5BaudAddressParity.ComParamData;
        //    set => _cpmVWTP_5BaudAddressParity.ComParamData = value;
        //}

        //public uint CPM_VW2000_5BaudFlashProgInit
        //{
        //    get => _cpmVW2000_5BaudFlashProgInit.ComParamData;
        //    set => _cpmVW2000_5BaudFlashProgInit.ComParamData = value;
        //}

        public ISO_14230_2(HashRuleUniqueRespIdentifierFromCpEcuLayerShortName hashAlgo)
        {
            HashAlgo = hashAlgo;
            UniqueRespIdTables = new CpIso142302UniqueRespIdTables(HashAlgo);
            //init table one //We need at least one table
            UniqueRespIdTables.Add(new CpIso142302UniqueRespIdTable("", HashAlgo));

            _cp5BaudAddressFunc = (PduComParamOfTypeUint)CreateCp("CP_5BaudAddressFunc", 0x33, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cp5BaudAddressPhys = (PduComParamOfTypeUint)CreateCp("CP_5BaudAddressPhys", 1, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cp5BaudMode = (PduComParamOfTypeUint)CreateCp("CP_5BaudMode", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_INIT);
            _cpAccessTimingOverride = (PduComParamOfTypeStructField)CreateCp("CP_AccessTimingOverride",
                new PduParamStructFieldData(PduCpSt.PDU_CPST_ACCESS_TIMING, new PduParamStructData[] {}, 8), PduPt.PDU_PT_STRUCTFIELD,
                PduPc.PDU_PC_TIMING);
            _cpExtendedTiming = (PduComParamOfTypeStructField)CreateCp("CP_ExtendedTiming",
                new PduParamStructFieldData(PduCpSt.PDU_CPST_ACCESS_TIMING, new PduParamStructData[] {}, 1), PduPt.PDU_PT_STRUCTFIELD,
                PduPc.PDU_PC_TIMING);
            _cpFillerByte = (PduComParamOfTypeUint)CreateCp("CP_FillerByte", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpFillerByteHandling = (PduComParamOfTypeUint)CreateCp("CP_FillerByteHandling", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpFillerByteLength = (PduComParamOfTypeUint)CreateCp("CP_FillerByteLength", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpEnableConcatenation = (PduComParamOfTypeUint)CreateCp("CP_EnableConcatenation", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpFuncReqFormatPriorityType =
                (PduComParamOfTypeUint)CreateCp("CP_FuncReqFormatPriorityType", 0xC0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpFuncReqTargetAddr = (PduComParamOfTypeUint)CreateCp("CP_FuncReqTargetAddr", 51, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpHeaderFormatKW = (PduComParamOfTypeUint)CreateCp("CP_HeaderFormatKW", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpInitializationSettings = (PduComParamOfTypeUint)CreateCp("CP_InitializationSettings", 2, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_INIT);
            _cpP1Max = (PduComParamOfTypeUint)CreateCp("CP_P1Max", 20000, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TIMING);
            _cpP4Max = (PduComParamOfTypeUint)CreateCp("CP_P4Max", 20000, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TIMING);
            _cpP4Min = (PduComParamOfTypeUint)CreateCp("CP_P4Min", 5000, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TIMING);
            _cpPhysReqFormatPriorityType =
                (PduComParamOfTypeUint)CreateCp("CP_PhysReqFormatPriorityType", 0x80, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpPhysReqTargetAddr = (PduComParamOfTypeUint)CreateCp("CP_PhysReqTargetAddr", 0x10, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpRequestAddrMode = (PduComParamOfTypeUint)CreateCp("CP_RequestAddrMode", 1, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpTesterSourceAddress = (PduComParamOfTypeUint)CreateCp("CP_TesterSourceAddress", 0xF1, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpTIdle = (PduComParamOfTypeUint)CreateCp("CP_TIdle", 300000, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_INIT);
            _cpTInil = (PduComParamOfTypeUint)CreateCp("CP_TInil", 25000, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_INIT);
            _cpTWup = (PduComParamOfTypeUint)CreateCp("CP_TWup", 50000, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_INIT);
            _cpW1Max = (PduComParamOfTypeUint)CreateCp("CP_W1Max", 300000, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_INIT);
            _cpW2Max = (PduComParamOfTypeUint)CreateCp("CP_W2Max", 20000, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_INIT);
            _cpW3Max = (PduComParamOfTypeUint)CreateCp("CP_W3Max", 20000, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_INIT);
            _cpW4Max = (PduComParamOfTypeUint)CreateCp("CP_W4Max", 50000, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_INIT);
            _cpW4Min = (PduComParamOfTypeUint)CreateCp("CP_W4Min", 25000, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_INIT);
            //_cpmVWTP_5BaudAddressParity = (PduComParamOfTypeUint)CreateCp("CPM_VWTP_5BaudAddressParity", 1, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_INIT);
           // _cpmVW2000_5BaudFlashProgInit = (PduComParamOfTypeUint)CreateCp("CPM_VW2000_5BaudFlashProgInit", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);


            CpList = new List<PduComParam>
            {
                _cp5BaudAddressFunc, _cp5BaudAddressPhys, _cp5BaudMode,
                _cpAccessTimingOverride, _cpExtendedTiming, _cpFillerByte,
                _cpFillerByteHandling, _cpFillerByteLength, _cpEnableConcatenation,
                _cpFuncReqFormatPriorityType, _cpFuncReqTargetAddr, _cpHeaderFormatKW,
                _cpInitializationSettings, _cpP1Max, _cpP4Max, _cpP4Min,
                _cpPhysReqFormatPriorityType, _cpPhysReqTargetAddr, _cpRequestAddrMode,
                _cpTesterSourceAddress, _cpTIdle, _cpTInil,
                _cpTWup, _cpW1Max, _cpW2Max,
                _cpW3Max, _cpW4Max, _cpW4Min,
                //_cpmVWTP_5BaudAddressParity, _cpmVW2000_5BaudFlashProgInit
            };
        }
    }
}
