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
    internal class VisitorPduIoCtlToUnmanagedMemoryUnsafe : IVisitorPduIoCtl
    {
        private unsafe PDU_DATA_ITEM* _pointerGeneralData;
        private unsafe void* _pointerSpecialData;

        internal unsafe void* PointerForIoCtlData
        {
            set
            {
                _pointerGeneralData = (PDU_DATA_ITEM*)value;
                _pointerSpecialData = _pointerGeneralData + 1;
            }
        }

        #region UsedInsidePduIoCtl

        public unsafe void VisitConcretePduIoCtlOfTypeUint(PduIoCtlOfTypeUint cd)
        {
            PduCopCtrlDataGeneralDataUnmanagedMemory(cd);
            (*(uint*)_pointerSpecialData) = cd.Value;
        }

        public unsafe void VisitConcretePduIoCtlOfTypeByteField(PduIoCtlOfTypeByteField cd)
        {
            PduCopCtrlDataGeneralDataUnmanagedMemory(cd);
            var pByteArrayData = (PDU_IO_BYTEARRAY_DATA*)_pointerSpecialData;
            _pointerSpecialData = (byte*)_pointerSpecialData + sizeof(PDU_IO_BYTEARRAY_DATA);
            pByteArrayData->DataSize = (uint)cd.Value.Length;
            pByteArrayData->pData = (byte*)_pointerSpecialData;

            for (var index = 0; index < cd.Value.Length; index++)
                pByteArrayData->pData[index] = cd.Value[index];

            //_pointerSpecialData = (byte*)_pointerSpecialData + sizeof(byte) * cd.Value.Length;
        }

        public unsafe void VisitConcretePduIoCtlOfTypeProgVoltage(PduIoCtlOfTypeProgVoltage cd)
        {
            PduCopCtrlDataGeneralDataUnmanagedMemory(cd);
            var pProgVoltageData = (PDU_IO_PROG_VOLTAGE_DATA*)_pointerSpecialData;
            _pointerSpecialData = (byte*)_pointerSpecialData + sizeof(PDU_IO_PROG_VOLTAGE_DATA);
            pProgVoltageData->PinOnDLC = cd.PinOnDlc;
            pProgVoltageData->ProgVoltage_mv = cd.ProgVoltageMv;
        }

        public unsafe void VisitConcretePduIoCtlOfTypeFilterList(PduIoCtlOfTypeFilterList cd)
        {
            PduCopCtrlDataGeneralDataUnmanagedMemory(cd);
            cd.Accept(this);
        }

        public unsafe void VisitConcretePduIoCtlFilterListData(PduIoCtlFilterListData cd)
        {
            var pduIoFilterList = (PDU_IO_FILTER_LIST*)_pointerSpecialData;
            _pointerSpecialData = (byte*)_pointerSpecialData + sizeof(PDU_IO_FILTER_LIST);
            pduIoFilterList->NumFilterEntries = (uint)cd.FilterDataArray.Length;
            pduIoFilterList->pFilterData = (PDU_IO_FILTER_DATA*)_pointerSpecialData;

            for ( var index = 0; index < cd.FilterDataArray.Length; index++ )
            {
                cd.Accept(this);
            }
        }

        public unsafe void VisitConcretePduIoCtlFilterData(PduIoCtlFilterData cd)
        {
            var pFilterData = (PDU_IO_FILTER_DATA*)_pointerSpecialData;

            pFilterData->FilterType = cd.FilterType;
            pFilterData->FilterNumber = cd.FilterNumber;
            pFilterData->FilterCompareSize = cd.FilterCompareSize;

            for ( var i = 0; i < cd.FilterMaskMessage.Length; i++ )
            {
                pFilterData->FilterMaskMessage[i] = cd.FilterMaskMessage[i];
            }

            for (var i = 0; i < cd.FilterPatternMessage.Length; i++)
            {
                pFilterData->FilterPatternMessage[i] = cd.FilterPatternMessage[i];
            }

            _pointerSpecialData = (PDU_IO_FILTER_DATA*)_pointerSpecialData + sizeof(PDU_IO_FILTER_DATA);

        }

        public void VisitConcretePduIoCtlOfTypeVehicleIdRequest(PduIoCtlOfTypeVehicleIdRequest cd)
        {
            PduCopCtrlDataGeneralDataUnmanagedMemory(cd);
            cd.VehicleIdRequest.Accept(this);
        }

        public unsafe void VisitConcretePduIoCtlVehicleIdRequestData(PduIoCtlVehicleIdRequestData cd)
        {
            var pIoVehicleIdRequest = (PDU_IO_VEHICLE_ID_REQUEST*)_pointerSpecialData;
            _pointerSpecialData = (byte*)_pointerSpecialData + sizeof(PDU_IO_VEHICLE_ID_REQUEST);
            pIoVehicleIdRequest->PreselectionMode = (uint)cd.PreselectionMode;
           
            pIoVehicleIdRequest->CombinationMode = (uint)cd.CombinationMode;
            pIoVehicleIdRequest->VehicleDiscoveryTime = cd.VehicleDiscoveryTime;


            if (cd.DestinationAddresses.Length == 0)
            {
                pIoVehicleIdRequest->NumDestinationAddresses = 0;
                pIoVehicleIdRequest->pDestinationAddresses = null;
            }
            else
            {
                pIoVehicleIdRequest->NumDestinationAddresses = (uint)cd.DestinationAddresses.Length;
                pIoVehicleIdRequest->pDestinationAddresses = (PDU_IP_ADDR_INFO*)_pointerSpecialData;
                foreach (var ipAddrInfo in cd.DestinationAddresses)
                {
                    ipAddrInfo.Accept(this);
                }
            }

            if (string.IsNullOrWhiteSpace(cd.PreselectionValue))
            {
                pIoVehicleIdRequest->PreselectionValue = null;
            }
            else
            {
                pIoVehicleIdRequest->PreselectionValue = (byte*)_pointerSpecialData;

                var asciiString = Encoding.ASCII.GetBytes(cd.PreselectionValue + char.MinValue /*Add null terminator*/);
                for (var i = 0; i < asciiString.Length; i++)
                {
                    pIoVehicleIdRequest->PreselectionValue[i] = asciiString[i];
                }
                _pointerSpecialData = (byte*)_pointerSpecialData + asciiString.Length;
            }
        }

        public unsafe void VisitConcretePduIoCtlVehicleIdRequestIpAddrInfoData(IpAddressInfo cd)
        {
            var pIpAddrInfo = (PDU_IP_ADDR_INFO*)_pointerSpecialData;
            _pointerSpecialData = (byte*)_pointerSpecialData + sizeof(PDU_IP_ADDR_INFO);
            pIpAddrInfo->IpVersion = (uint)cd.IpVersion;
            pIpAddrInfo->pAddress = (byte*)_pointerSpecialData;
            var addressBytesIp = cd.GetAddressBytes();
            for (var i = 0; i < addressBytesIp.Length; i++)
            {
                pIpAddrInfo->pAddress[i] = addressBytesIp[i];
            }

            _pointerSpecialData = (byte*)_pointerSpecialData + addressBytesIp.Length;
        }

        public unsafe void VisitConcretePduIoCtlOfTypeEthSwitchState(PduIoCtlOfTypeEthSwitchState cd)
        {
            PduCopCtrlDataGeneralDataUnmanagedMemory(cd);
            var pIoEthSwitchState = (PDU_IO_ETH_SWITCH_STATE*)_pointerSpecialData;
            _pointerSpecialData = (byte*)_pointerSpecialData + sizeof(PDU_IO_ETH_SWITCH_STATE);
            pIoEthSwitchState->EthernetActivationPin = cd.EthernetActivationPin;
            pIoEthSwitchState->EthernetActPinNumber = cd.EthernetActDlcPinNumber;
        }

        public unsafe void VisitConcretePduIoCtlOfTypeEntityAddress(PduIoCtlOfTypeEntityAddress cd)
        {
            PduCopCtrlDataGeneralDataUnmanagedMemory(cd);
            var pduIoEntityAddressData = (PDU_IO_ENTITY_ADDRESS_DATA*)_pointerSpecialData;
            _pointerSpecialData = (byte*)_pointerSpecialData + sizeof(PDU_IO_ENTITY_ADDRESS_DATA);
            pduIoEntityAddressData->LogicalAddress = cd.LogicalAddress;
            pduIoEntityAddressData->DoIPCtrlTimeout = cd.DoIpCtrlTimeout;
        }


        protected unsafe void PduCopCtrlDataGeneralDataUnmanagedMemory(PduIoCtl cd)
        {
            var pPduDataItem = _pointerGeneralData;
            pPduDataItem->ItemType = cd.PduItemType;
            pPduDataItem->pData = _pointerSpecialData;
        }
        #endregion
    }
}