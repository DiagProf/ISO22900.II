#region License

// /*
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
// */

#endregion

using ISO22900.II.OdxLikeComParamSets.TransportOrDataLinkLayer;

namespace ISO22900.II.OdxLikeComParamSets
{
    public class VehicleInfoSpecForTestEcu : ISO_14229_3_on_ISO_15765_2_on_ISO_11898_2_DWCAN
    {
        public VehicleInfoSpecForTestEcu(HashRuleUniqueRespIdentifierFromCpEcuLayerShortName hashAlgo = null) :base(HashAlgo)
        {
            InitializeAllComParams();
        }

        private new void InitializeAllComParams()
        {
            base.InitializeAllComParams();
            //settings specific to this manufacturer for all ECUs
            //e.g. TesterPresent behavior or TesterAddress
            //or the functional addresses
            Tpl.CP_CanPhysReqId = 0x7E0;
            Tpl.CP_CanRespUSDTId = 0x7E8;
            Tpl.CP_ECULayerShortName = "None";
        }

        /// <summary>
        ///     Changes the ComParams for the simulation.
        ///     Attention only for an entry in the CP_UniqueRespIdTable
        /// </summary>
        /// <returns></returns>
        public VehicleInfoSpecForTestEcu ChangeToSimulation()
        {
            //swap via deconstruction
            (Tpl.CP_CanPhysReqId, Tpl.CP_CanRespUSDTId) = (Tpl.CP_CanRespUSDTId, Tpl.CP_CanPhysReqId);
            return this;
        }

        public VehicleInfoSpecForTestEcu LogicalLinkSettingForPcm()
        {
            InitializeAllComParams();
            Tpl.CP_CanPhysReqId = 0x7E0;
            Tpl.CP_CanRespUSDTId = 0x7E8;
            return this;
        }

        public VehicleInfoSpecForTestEcu LogicalLinkSettingForTcm()
        {
            InitializeAllComParams();
            Tpl.CP_CanPhysReqId = 0x7E1;
            Tpl.CP_CanRespUSDTId = 0x7E9;
            return this;
        }

        public VehicleInfoSpecForTestEcu LogicalLinkSettingForOBD()
        {
            InitializeAllComParams();
            Tpl.CP_UniqueRespIdTable["PowertrainControlModule"].CP_CanPhysReqId = 0x7E0;
            Tpl.CP_UniqueRespIdTable["PowertrainControlModule"].CP_CanRespUSDTId = 0x7E8;
            Tpl.CP_UniqueRespIdTable["TransmissionControlModule"].CP_CanPhysReqId = 0x7E1;
            Tpl.CP_UniqueRespIdTable["TransmissionControlModule"].CP_CanRespUSDTId = 0x7E9;
            return this;
        }

        //ToDo an implementation that shows the real idea
        private static uint HashAlgo(string cpeculayershortname)
        {
            return 1;
        }
    }
}
