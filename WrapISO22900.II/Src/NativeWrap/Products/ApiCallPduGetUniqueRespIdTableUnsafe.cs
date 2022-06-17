using System;
using System.Collections.Generic;
using ISO22900.II.UnSafeCStructs;
using Microsoft.Extensions.Logging;

// ReSharper disable BuiltInTypeReferenceStyle

namespace ISO22900.II
{
    // Basic data types used in ISO22900-2
    // C# using alias directive       //C typedef...
    using UNUM8 = Byte; //              typedef unsigned char UNUM8;      // Unsigned numeric 8 bits
    //using SNUM8 = System.SByte;     //typedef signed char SNUM8;        // Signed numeric 8 bits
    //using UNUM16 = System.UInt16;   //typedef unsigned short UNUM16;    // Unsigned numeric 16 bits
    //using SNUM16 = System.Int16;    //typedef signed short SNUM16;      // Signed numeric 16 bits
    using UNUM32 = UInt32; //           typedef unsigned long UNUM32;     // Unsigned numeric 32 bits
    //using SNUM32 = System.Int32;    //typedef signed long SNUM32;       // Signed numeric 32 bits
    //using CHAR8 = System.Byte;      //typedef char CHAR8;               // ASCII-coded 8-bit character value (ISO8859-1 (Latin 1))

    internal class ApiCallPduGetUniqueRespIdTableUnsafe : ApiCallPduGetUniqueRespIdTable
    {
        private readonly PduUniqueRespIdTableUnsafeFactory _pduUniqueRespIdTableUnsafeFactory;
        
        internal override unsafe List<PduEcuUniqueRespData> PduGetUniqueRespIdTable(uint moduleHandle,
            uint comLogicalLinkHandle)
        {
            PDU_UNIQUE_RESP_ID_TABLE_ITEM* pPduUniqueRespIdTableItem;

            CheckResultThrowException(PDUGetUniqueRespIdTable(moduleHandle, comLogicalLinkHandle, &pPduUniqueRespIdTableItem));

            var pduEcuUniqueRespDatas= _pduUniqueRespIdTableUnsafeFactory.GetListOfPduEcuUniqueRespDataFromPduUniqueRespIdTablePointer(pPduUniqueRespIdTableItem);

            CheckResultThrowException(PDUDestroyItem((PDU_ITEM*)pPduUniqueRespIdTableItem));

            return pduEcuUniqueRespDatas;
        }

        internal ApiCallPduGetUniqueRespIdTableUnsafe(IntPtr handleToLoadedNativeLibrary) : base(handleToLoadedNativeLibrary)
        {
            _pduUniqueRespIdTableUnsafeFactory = new PduUniqueRespIdTableUnsafeFactory();
        }

        //should look like the C function as much as possible.
        // ReSharper disable InconsistentNaming
        private unsafe PduError PDUGetUniqueRespIdTable(UNUM32 hMod, UNUM32 hCLL, PDU_UNIQUE_RESP_ID_TABLE_ITEM** pUniqueRespIdTable)
        {
            return ((delegate* unmanaged[Stdcall]<UNUM32, UNUM32, PDU_UNIQUE_RESP_ID_TABLE_ITEM**, PduError>)
                AddressOfNativeMethod)(hMod, hCLL, pUniqueRespIdTable);
        }

        private unsafe PduError PDUDestroyItem(PDU_ITEM* pItem)
        {
            return ((delegate* unmanaged[Stdcall]<PDU_ITEM*, PduError>) AddressOfNativeMethodDestroyItem)(pItem);
        }
        // ReSharper restore InconsistentNaming
    }
}
