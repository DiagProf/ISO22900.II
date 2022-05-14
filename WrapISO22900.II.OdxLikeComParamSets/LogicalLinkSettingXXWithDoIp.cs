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
using ISO22900.II.OdxLikeComParamSets.TransportOrDataLinkLayer;

namespace ISO22900.II.OdxLikeComParamSets
{
    public class LogicalLinkSettingXXWithDoIp : ISO_14229_5_on_ISO_13400_2_on_IEEE_802_3
    {
        private const string BusTypeNameDefault = "IEEE_802_3";
        private const string ProtocolNameDefault = "ISO_14229_5_on_ISO_13400_2";
        private static readonly Dictionary<uint, string> DlcPinDataDefault = new() { { 3, "RX" }, { 11, "MINUS" }, { 12, "TX" }, { 13, "LOW" } };
        public string BusTypeName { get; private set; } = BusTypeNameDefault;
        public string ProtocolName { get; private set; } = ProtocolNameDefault;
        public Dictionary<uint, string> DlcPinData { get; private set; } = DlcPinDataDefault;


        public LogicalLinkSettingXXWithDoIp(HashRuleUniqueRespIdentifierFromCpEcuLayerShortName hashAlgo = null) : base(HashAlgo)
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

        public LogicalLinkSettingXXWithDoIp LogicalLinkSettingPcm()
        {
            InitializeAllComParams();
            //ECU specific
           // Tpl.CPM_VWTP_DestAddr = 0x01;
            Tpl.CP_ECULayerShortName = "PowertrainControlModule";
            return this;
        }


        //ToDo an implementation that shows the real idea
        private static uint HashAlgo(string cpeculayershortname)
        {
            return 1;
        }
    }
}
