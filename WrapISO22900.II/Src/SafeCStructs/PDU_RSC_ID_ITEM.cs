using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming
// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable IdentifierTypo

namespace ISO22900.II.SafeCStructs
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct PDU_RSC_ID_ITEM
    {
        internal PduIt ItemType;                  /* value= PDU_IT_RSC_ID */
        internal uint NumModules;               /* number of entries in pResourceIdDataArray.*/
        internal PDU_RSC_ID_ITEM_DATA[] pResourceIdDataArray; /* pointer to an array of resource Id Item Data */
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct PDU_RSC_ID_ITEM_DATA
    {
        internal uint hMod;                        /* MVCI Protocol Module Handle */
        internal uint NumIds;                      /* number of resources that match PDU_RSC_DATA */
        internal uint[] pResourceIdArray;           /* pointer to a list of resource ids */
    }
}