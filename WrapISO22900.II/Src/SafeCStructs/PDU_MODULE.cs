using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming
// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable IdentifierTypo

namespace ISO22900.II.SafeCStructs
{
    // Item for module identification (used by PDUGetModuleIds)
    [StructLayout(LayoutKind.Sequential)]
    internal struct PDU_MODULE_DATA
    {
        public uint ModuleTypeId;              /* MVCI Protocol ModuleTypeId */
        public uint hMod;                      /* handle of MVCI Protocol Module assigned by D-PDU API */
        public string pVendorModuleName;         /* Vendor specific information string for the unique module identification. * E.g. Module serial number or Module friendly name */
        public string pVendorAdditionalInfo;     /* Vendor specific additional information string */
        public PduStatus ModuleStatus;           /* Status of MVCI Protocol Module detected by D-PDU API session*/
    }
    
    [StructLayout(LayoutKind.Sequential)]
    internal struct PDU_MODULE_ITEM
    {
        public PduIt ItemType;                  /* value= PDU_IT_MODULE_ID */
        public uint NumEntries;                  /* number of entries written to the pModuleData array */
        public PDU_MODULE_DATA[] pModuleData;       /* pointer to array containing module types and module handles */
    };
}