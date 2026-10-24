using System;
using System.Collections.Generic;

namespace ISO22900.II
{ 
    internal class PduResourceData
    {
        public UInt32 BusTypeId;                 /* Bus Type Id (IN parameter) */
        public UInt32 ProtocolId;                /* Protocol Id (IN parameter) */
        public List<KeyValuePair<UInt32, UInt32>> DlcPinData;         /* Array of PDU_PIN_DATA pairs*/

        internal PduResourceData(uint busTypeId, uint protocolId, List<KeyValuePair<uint, uint>> dlcPinData)
        {
            BusTypeId = busTypeId;
            ProtocolId = protocolId;
            DlcPinData = dlcPinData;
        }
    }
}