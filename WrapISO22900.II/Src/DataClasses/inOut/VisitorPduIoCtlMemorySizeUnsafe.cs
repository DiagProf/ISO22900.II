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

using System.Text;
using ISO22900.II.UnSafeCStructs;

namespace ISO22900.II
{
    internal class VisitorPduIoCtlMemorySizeUnsafe : IVisitorPduIoCtl
    {
        internal int MemorySize { get; set; }

        #region UsedInsidePduIoCtl

        public void VisitConcretePduIoCtlOfTypeUint(PduIoCtlOfTypeUint cd)
        {
            MemorySize += CalculateSizeOfPduIoCtlDataBase() + sizeof(uint);
        }

        public unsafe void VisitConcretePduIoCtlOfTypeByteField(PduIoCtlOfTypeByteField cd)
        {
            MemorySize += CalculateSizeOfPduIoCtlDataBase() + sizeof(PDU_IO_BYTEARRAY_DATA) + cd.Value.Length;
        }

        public unsafe void VisitConcretePduIoCtlOfTypeProgVoltage(PduIoCtlOfTypeProgVoltage cd)
        {
            MemorySize += CalculateSizeOfPduIoCtlDataBase() + sizeof(PDU_IO_PROG_VOLTAGE_DATA);
        }

        public unsafe void VisitConcretePduIoCtlOfTypeFilterList(PduIoCtlOfTypeFilterList cd)
        {
            MemorySize += CalculateSizeOfPduIoCtlDataBase();
            cd.Accept(this);
        }

        public unsafe void VisitConcretePduIoCtlFilterListData(PduIoCtlFilterListData cd)
        {
            MemorySize += sizeof(PDU_IO_FILTER_LIST);
            foreach ( var data in cd.FilterDataArray )
            {
                data.Accept(this);
            }
        }

        public unsafe void VisitConcretePduIoCtlFilterData(PduIoCtlFilterData pduIoCtlFilterData)
        {
            MemorySize += sizeof(PDU_IO_FILTER_DATA);
        }

        public unsafe void VisitConcretePduIoCtlOfTypeVehicleIdRequest(PduIoCtlOfTypeVehicleIdRequest cd)
        {
            MemorySize += CalculateSizeOfPduIoCtlDataBase();
            cd.VehicleIdRequest.Accept(this);
        }

        public unsafe void VisitConcretePduIoCtlVehicleIdRequestData(PduIoCtlVehicleIdRequestData cd)
        {
            MemorySize += sizeof(PDU_IO_VEHICLE_ID_REQUEST);
            MemorySize += Encoding.ASCII.GetBytes(cd.PreselectionValue + char.MinValue /*Add null terminator*/).Length;
            foreach (var ipAddrInfo in cd.DestinationAddresses)
            {
                ipAddrInfo.Accept(this);
            }
        }

        public unsafe void VisitConcretePduIoCtlVehicleIdRequestIpAddrInfoData(IpAddressInfo cd)
        {
            MemorySize += sizeof(PDU_IP_ADDR_INFO) + cd.GetAddressBytes().Length;
        }

        public unsafe void VisitConcretePduIoCtlOfTypeEthSwitchState(PduIoCtlOfTypeEthSwitchState cd)
        {
            MemorySize += CalculateSizeOfPduIoCtlDataBase() + sizeof(PDU_IO_ETH_SWITCH_STATE);
        }

        public unsafe void VisitConcretePduIoCtlOfTypeEntityAddress(PduIoCtlOfTypeEntityAddress cd)
        {
            MemorySize += CalculateSizeOfPduIoCtlDataBase() + sizeof(PDU_IO_ENTITY_ADDRESS_DATA);
        }


        #endregion

        protected static unsafe int CalculateSizeOfPduIoCtlDataBase()
        {
            return sizeof(PDU_DATA_ITEM);
        }
    }
}