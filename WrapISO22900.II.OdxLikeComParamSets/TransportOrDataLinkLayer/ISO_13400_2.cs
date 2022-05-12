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
    public partial class ISO_13400_2 : AbstractTransportOrDataLinkLayer
    {
        private readonly PduComParamOfTypeUint _cpRepeatReqCountTrans;
        private readonly PduComParamOfTypeUint _cpRequestAddrMode;
        private readonly PduComParamOfTypeUint _cpDoIPLogicalGatewayAddress;
        private readonly PduComParamOfTypeUint _cpDoIPLogicalTesterAddress;
        private readonly PduComParamOfTypeUint _cpDoIPLogicalFunctionalAddress;
        private readonly PduComParamOfTypeUint _cpDoIPNumberOfRetries;
        private readonly PduComParamOfTypeUint _cpDoIPDiagnosticAckTimeout;
        private readonly PduComParamOfTypeUint _cpDoIPRetryPeriod;
        private readonly PduComParamOfTypeUint _cpDoIPProcessingTime;
        private readonly PduComParamOfTypeUint _cpDoIPRoutingActivationType;
        private readonly PduComParamOfTypeUint _cpDoIPRoutingActivationTimeout;


        protected sealed override CpIso134002UniqueRespIdTables UniqueRespIdTables { get; }
        public IIso134002CpUniqueRespIdTables CP_UniqueRespIdTable => UniqueRespIdTables;

        //these are comfort functions to be able to access Iso157652CpUniqueRespIdTable[0] directly.This is the 90% use case
        public string CP_ECULayerShortName
        {
            get => UniqueRespIdTables[0].CP_ECULayerShortName;
            set => UniqueRespIdTables[0].CP_ECULayerShortName = value;
        }

        public uint CP_DoIPLogicalEcuAddress
        {
            get => UniqueRespIdTables[0].CP_DoIPLogicalEcuAddress;
            set => UniqueRespIdTables[0].CP_DoIPLogicalEcuAddress = value;
        }

        public uint CP_DoIPSecondaryLogicalECUResponseAddress
        {
            get => UniqueRespIdTables[0].CP_DoIPSecondaryLogicalECUResponseAddress;
            set => UniqueRespIdTables[0].CP_DoIPSecondaryLogicalECUResponseAddress = value;
        }


        //normal comParams

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
        public uint CP_DoIPLogicalGatewayAddress
        {
            get => _cpDoIPLogicalGatewayAddress.ComParamData;
            set => _cpDoIPLogicalGatewayAddress.ComParamData = value;
        }
        public uint CP_DoIPLogicalTesterAddress
        {
            get => _cpDoIPLogicalTesterAddress.ComParamData;
            set => _cpDoIPLogicalTesterAddress.ComParamData = value;
        }
        public uint CP_DoIPLogicalFunctionalAddress
        {
            get => _cpDoIPLogicalFunctionalAddress.ComParamData;
            set => _cpDoIPLogicalFunctionalAddress.ComParamData = value;
        }
        public uint CP_DoIPNumberOfRetries
        {
            get => _cpDoIPNumberOfRetries.ComParamData;
            set => _cpDoIPNumberOfRetries.ComParamData = value;
        }
        public uint CP_DoIPDiagnosticAckTimeout
        {
            get => _cpDoIPDiagnosticAckTimeout.ComParamData;
            set => _cpDoIPDiagnosticAckTimeout.ComParamData = value;
        }
        public uint CP_DoIPRetryPeriod
        {
            get => _cpDoIPRetryPeriod.ComParamData;
            set => _cpDoIPRetryPeriod.ComParamData = value;
        }
        public uint CP_DoIPProcessingTime
        {
            get => _cpDoIPProcessingTime.ComParamData;
            set => _cpDoIPProcessingTime.ComParamData = value;
        }
        public uint CP_DoIPRoutingActivationType
        {
            get => _cpDoIPRoutingActivationType.ComParamData;
            set => _cpDoIPRoutingActivationType.ComParamData = value;
        }
        public uint CP_DoIPRoutingActivationTimeout
        {
            get => _cpDoIPRoutingActivationTimeout.ComParamData;
            set => _cpDoIPRoutingActivationTimeout.ComParamData = value;
        }

        public ISO_13400_2(HashRuleUniqueRespIdentifierFromCpEcuLayerShortName hashAlgo)
        {
            HashAlgo = hashAlgo;
            UniqueRespIdTables = new CpIso134002UniqueRespIdTables(HashAlgo);
            //init table one //We need at least one table
            UniqueRespIdTables.Add(new CpIso134002UniqueRespIdTable("", HashAlgo));

            _cpRepeatReqCountTrans = (PduComParamOfTypeUint)CreateCp("CP_RepeatReqCountTrans", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_ERRHDL);
            _cpRequestAddrMode = (PduComParamOfTypeUint)CreateCp("CP_RequestAddrMode", 1, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpDoIPLogicalGatewayAddress = (PduComParamOfTypeUint)CreateCp("CP_DoIPLogicalGatewayAddress", 0x1, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpDoIPLogicalTesterAddress = (PduComParamOfTypeUint)CreateCp("CP_DoIPLogicalTesterAddress", 0xE00, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpDoIPLogicalFunctionalAddress = (PduComParamOfTypeUint)CreateCp("CP_DoIPLogicalFunctionalAddress", 0xE400, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpDoIPNumberOfRetries = (PduComParamOfTypeUint)CreateCp("CP_DoIPNumberOfRetries", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpDoIPDiagnosticAckTimeout = (PduComParamOfTypeUint)CreateCp("CP_DoIPDiagnosticAckTimeout", 2000000, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TIMING);
            _cpDoIPRetryPeriod = (PduComParamOfTypeUint)CreateCp("CP_DoIPRetryPeriod", 1000000, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TIMING);
            _cpDoIPProcessingTime = (PduComParamOfTypeUint)CreateCp("CP_DoIPProcessingTime", 2000000, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TIMING);
            _cpDoIPRoutingActivationType = (PduComParamOfTypeUint)CreateCp("CP_DoIPRoutingActivationType", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_COM);
            _cpDoIPRoutingActivationTimeout = (PduComParamOfTypeUint)CreateCp("CP_DoIPRoutingActivationTimeout", 1000000, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_TIMING);
           

            CpList = new List<PduComParam>
            {
                _cpDoIPDiagnosticAckTimeout, _cpDoIPLogicalFunctionalAddress, _cpDoIPLogicalGatewayAddress,
                _cpDoIPLogicalTesterAddress, _cpDoIPNumberOfRetries, _cpDoIPProcessingTime,
                _cpDoIPRetryPeriod, _cpDoIPRoutingActivationTimeout, _cpDoIPRoutingActivationType, 
                _cpRepeatReqCountTrans, _cpRequestAddrMode
            };
        }
    }
}
