using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ISO22900.II.UnSafeCStructs;
using Microsoft.Extensions.Logging;

namespace ISO22900.II
{
    // Basic data types used in ISO22900-2
    // C# using alias directive     //C typedef...
    //using UNUM8 = System.Byte;      //typedef unsigned char UNUM8;      // Unsigned numeric 8 bits
    //using SNUM8 = System.SByte;     //typedef signed char SNUM8;        // Signed numeric 8 bits
    //using UNUM16 = System.UInt16;   //typedef unsigned short UNUM16;    // Unsigned numeric 16 bits
    //using SNUM16 = System.Int16;    //typedef signed short SNUM16;      // Signed numeric 16 bits
    //using UNUM32 = System.UInt32;   //typedef unsigned long UNUM32;     // Unsigned numeric 32 bits
    //using SNUM32 = System.Int32;    //typedef signed long SNUM32;       // Signed numeric 32 bits
    using CHAR8 = System.Byte;      //typedef char CHAR8;               // ASCII-coded 8-bit character value (ISO8859-1 (Latin 1))
    
    internal class ApiCallPduGetModuleIdsUnsafe : ApiCallPduGetModuleIds
    {
        internal override unsafe List<PduModuleData> PduGetModuleIds()
        {
            var pduModuleDataList = new List<PduModuleData>();

            PDU_MODULE_ITEM* pPduModuleItem;
            CheckResultThrowException(PDUGetModuleIds(&pPduModuleItem));

            if (pPduModuleItem->ItemType == PduIt.PDU_IT_MODULE_ID)
            {
                pduModuleDataList = new List<PduModuleData>((int)pPduModuleItem->NumEntries);
                for (var index = 0; index < pPduModuleItem->NumEntries; index++)
                {
                    var p = &(pPduModuleItem->pModuleData)[index];

                    var pduModuleData = new PduModuleData
                    (
                        p->ModuleTypeId,
                        p->hMod,
                        Marshal.PtrToStringAnsi((IntPtr)(CHAR8*)p->pVendorModuleName),
                        Marshal.PtrToStringAnsi((IntPtr)(CHAR8*)p->pVendorAdditionalInfo), 
                        p->ModuleStatus
                    );
                    pduModuleDataList.Add(pduModuleData);
                }
            }

            CheckResultThrowException(PDUDestroyItem((PDU_ITEM*)pPduModuleItem));

            return pduModuleDataList;
        }

        internal ApiCallPduGetModuleIdsUnsafe(IntPtr handleToLoadedNativeLibrary) : base(handleToLoadedNativeLibrary)
        {
        }

        //should look like the C function as much as possible.
        // ReSharper disable InconsistentNaming
        private unsafe PduError PDUGetModuleIds(PDU_MODULE_ITEM** pModuleIdList) => ((delegate* unmanaged[Stdcall]<PDU_MODULE_ITEM**, PduError>)AddressOfNativeMethod)(pModuleIdList);
        private unsafe PduError PDUDestroyItem(PDU_ITEM* pItem) => ((delegate* unmanaged[Stdcall]<PDU_ITEM*, PduError>)AddressOfNativeMethodDestroyItem)(pItem);
        // ReSharper restore InconsistentNaming

    }
}
