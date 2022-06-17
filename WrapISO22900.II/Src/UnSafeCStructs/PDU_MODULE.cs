using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming
// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable IdentifierTypo

namespace ISO22900.II.UnSafeCStructs
{
    // Basic data types used in ISO22900-2
    // C# using alias directive     //C typedef...
    //typedef unsigned char UNUM8;      // Unsigned numeric 8 bits
    //typedef signed char SNUM8;        // Signed numeric 8 bits
    //typedef unsigned short UNUM16;    // Unsigned numeric 16 bits
    //typedef signed short SNUM16;      // Signed numeric 16 bits
    using UNUM32 = System.UInt32;   //typedef unsigned long UNUM32;     // Unsigned numeric 32 bits
    //typedef signed long SNUM32;       // Signed numeric 32 bits
    using CHAR8 = System.Byte;      //typedef char CHAR8;               // ASCII-coded 8-bit character value (ISO8859-1 (Latin 1))

    // Item for module identification (used by PDUGetModuleIds)
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct PDU_MODULE_DATA
    {
        public UNUM32 ModuleTypeId;              /* MVCI Protocol ModuleTypeId */
        public UNUM32 hMod;                      /* handle of MVCI Protocol Module assigned by D-PDU API */
        public CHAR8* pVendorModuleName;         /* Vendor specific information string for the unique module identification. * E.g. Module serial number or Module friendly name */
        public CHAR8* pVendorAdditionalInfo;     /* Vendor specific additional information string */
        public PduStatus ModuleStatus;           /* Status of MVCI Protocol Module detected by D-PDU API session*/
    }
    
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct PDU_MODULE_ITEM
    {
        public PduIt ItemType;                  /* value= PDU_IT_MODULE_ID */
        public UNUM32 NumEntries;                  /* number of entries written to the pModuleData array */
        public PDU_MODULE_DATA* pModuleData;       /* pointer to array containing module types and module handles */
    };
}