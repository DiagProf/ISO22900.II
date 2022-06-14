using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming
// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable IdentifierTypo

namespace ISO22900.II.SafeCStructs
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct PDU_RSC_DATA
    {
        public uint BusTypeId;                 /* Bus Type Id (IN parameter) */
        public uint ProtocolId;                /* Protocol Id (IN parameter) */
        public uint NumPinData;                /* Number of items in the following array */
        public PDU_PIN_DATA[] pDLCPinData;         /* Pointer to array of PDU_PIN_DATA structures*/
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct PDU_PIN_DATA
    {
        public uint DLCPinNumber;              /* Pin number on DLC */
        public uint DLCPinTypeId;              /* Pin ID */
    }
}