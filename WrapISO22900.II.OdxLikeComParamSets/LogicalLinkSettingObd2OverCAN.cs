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

using ISO22900.II.OdxLikeComParamSets.TransportOrDataLinkLayer;
using System.Collections.Generic;

namespace ISO22900.II.OdxLikeComParamSets
{
    public class LogicalLinkSettingObd2OverCAN : ISO_OBD_on_ISO_15765_4_on_ISO_11898_2_DWCAN
    {
        private const string BusTypeNameDefault = "ISO_11898_2_DWCAN";
        private const string ProtocolNameDefault = "ISO_OBD_on_ISO_15765_4";
        private static readonly Dictionary<uint, string> DlcPinDataDefault = new() { { 6, "HI" }, { 14, "LOW" } };
        public string BusTypeName { get; private set; } = BusTypeNameDefault;
        public string ProtocolName { get; private set; } = ProtocolNameDefault;
        public Dictionary<uint, string> DlcPinData { get; private set; } = DlcPinDataDefault;
        Dictionary<uint,string> HashToEcuDomainName = new Dictionary<uint, string>();




        public LogicalLinkSettingObd2OverCAN(HashRuleUniqueRespIdentifierFromCpEcuLayerShortName hashAlgo = null) :base(HashAlgo)
        {
            InitializeAllComParams();
        }

        private new void InitializeAllComParams()
        {
            base.InitializeAllComParams();
        }


        public LogicalLinkSettingObd2OverCAN LogicalLinkSettingForOBD()
        {
            InitializeAllComParams();
            Tpl.CP_UniqueRespIdTable[this["PowertrainControlModule"]].CP_CanPhysReqId = 0x7E0;
            Tpl.CP_UniqueRespIdTable[this["PowertrainControlModule"]].CP_CanRespUSDTId = 0x7E8;
            Tpl.CP_UniqueRespIdTable[this["TransmissionControlModule"]].CP_CanPhysReqId = 0x7E1;
            Tpl.CP_UniqueRespIdTable[this["TransmissionControlModule"]].CP_CanRespUSDTId = 0x7E9;
            return this;
        }

        private string this[string name]
        {
            get { HashToEcuDomainName.TryAdd(HashAlgo(name), name); return name; }
        }

        //ToDo an implementation that shows the real idea
        private static uint HashAlgo(string cpeculayershortname)
        {
            return (uint)cpeculayershortname.GetHashCode(System.StringComparison.InvariantCulture);
        }
    }
}
