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
using ISO22900.II.OdxLikeComParamSets.TransportOrDataLinkLayer;

namespace ISO22900.II.OdxLikeComParamSets
{
    public class LogicalLinkSettingZuffenhausenWithKline : ISO_14230_3_on_ISO_14230_2_on_ISO_9141_2_UART
    {
        private const string BusTypeNameDefault = "ISO_9141_2_UART";
        private const string ProtocolNameDefault = "ISO_14230_3_on_ISO_14230_2";
        private static readonly Dictionary<uint, string> DlcPinDataDefault = new() { { 7, "K" } /*, { 15, "L" }*/ };
        public string BusTypeName { get; private set; } = BusTypeNameDefault;
        public string ProtocolName { get; private set; } = ProtocolNameDefault;
        public Dictionary<uint, string> DlcPinData { get; private set; } = DlcPinDataDefault;


        public LogicalLinkSettingZuffenhausenWithKline(HashRuleUniqueRespIdentifierFromCpEcuLayerShortName hashAlgo = null) : base(HashAlgo)
        {
            BusTypeName = BusTypeNameDefault;
            ProtocolName = ProtocolNameDefault;
            InitializeAllComParams();
        }

        private new void InitializeAllComParams()
        {
            base.InitializeAllComParams();
            DlcPinData = DlcPinDataDefault;
            //settings specific to this manufacturer for all ECUs
            //e.g. TesterPresent behavior or TesterAddress
            //or the functional addresses
        }

        public LogicalLinkSettingZuffenhausenWithKline LogicalLinkSettingPcm()
        {
            InitializeAllComParams();

            //ECU specific
            Tpl.CP_EcuRespSourceAddress = 0x00000011;
            Tpl.CP_PhysReqTargetAddr = 0x00000011;
            Tpl.CP_ECULayerShortName = "PowertrainControlModule";

            return this;
        }

        public LogicalLinkSettingZuffenhausenWithKline LogicalLinkSettingTcm()
        {
            InitializeAllComParams();

            //ECU specific
            Tpl.CP_EcuRespSourceAddress = 0x00000018;
            Tpl.CP_PhysReqTargetAddr = 0x00000018;
            Tpl.CP_ECULayerShortName = "TransmissionControlModule";

            return this;
        }

        public LogicalLinkSettingZuffenhausenWithKline LogicalLinkSettingABS()
        {
            InitializeAllComParams();

            //ECU specific
            Tpl.CP_EcuRespSourceAddress = 0x0000001F;
            Tpl.CP_PhysReqTargetAddr = 0x0000001F;
            Tpl.CP_ECULayerShortName = "AntilockBrakingSystem";

            DlcPinData = new Dictionary<uint, string> { { 3, "K" }};

            return this;
        }

        public LogicalLinkSettingZuffenhausenWithKline LogicalLinkSettingIPC()
        {
            InitializeAllComParams();

            //ECU specific
            Tpl.CP_EcuRespSourceAddress = 0x00000064;
            Tpl.CP_PhysReqTargetAddr = 0x00000064;
            Tpl.CP_ECULayerShortName = "InstrumentPanelControl";

            DlcPinData = new Dictionary<uint, string> { { 3, "K" } };

            return this;
        }

        public LogicalLinkSettingZuffenhausenWithKline LogicalLinkSettingAirbag()
        {
            InitializeAllComParams();

            //ECU specific
            Tpl.CP_EcuRespSourceAddress = 0x00000057;
            Tpl.CP_PhysReqTargetAddr = 0x00000057;
            Tpl.CP_ECULayerShortName = "Airbag";

            DlcPinData = new Dictionary<uint, string> { { 3, "K" } };

            return this;
        }

        public LogicalLinkSettingZuffenhausenWithKline LogicalLinkSettingAC()
        {
            InitializeAllComParams();

            //ECU specific
            Tpl.CP_EcuRespSourceAddress = 0x00000098;
            Tpl.CP_PhysReqTargetAddr = 0x00000098;
            Tpl.CP_ECULayerShortName = "AirConditioning";

            DlcPinData = new Dictionary<uint, string> { { 3, "K" } };

            return this;
        }

        //ToDo an implementation that shows the real idea
        private static uint HashAlgo(string cpeculayershortname)
        {
            return 1;
        }
    }
}
