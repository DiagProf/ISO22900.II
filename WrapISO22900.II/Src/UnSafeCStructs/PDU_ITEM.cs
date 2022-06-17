using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming
namespace ISO22900.II.UnSafeCStructs
{
    // Generic Item for type evaluation and casting
    // This is a generic item used for casting to item specific structures. PDU_ITEM is used in the function PDUDestroyItem.
    [StructLayout(LayoutKind.Sequential)]
    internal struct PDU_ITEM
    {
        PduIt ItemType;  /* See section D-PDU API item type values */
    }
}