using System;
// ReSharper disable IdentifierTypo
// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable InconsistentNaming

namespace ISO22900.II
{
    public static class PduConst
    {
        // Reserved ID and handle values
        public const UInt32 PDU_ID_UNDEF = 0xFFFFFFFE;  /* Undefined ID value. Used to indicate an ID value is undefined.*/
        public const UInt32 PDU_HANDLE_UNDEF = 0xFFFFFFFF; /* Undefined handle value. Used to indicate a Handle value is undefined.*/
    }
}