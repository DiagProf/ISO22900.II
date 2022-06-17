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

using System;
using System.Collections.Generic;
using ISO22900.II.UnSafeCStructs;
using Microsoft.Extensions.Logging;

namespace ISO22900.II
{
    // Basic data types used in ISO22900-2
    // C# using alias directive     //C typedef...
    //using UNUM8 = Byte; //typedef unsigned char UNUM8;      // Unsigned numeric 8 bits
    //using SNUM8 = System.SByte;     //typedef signed char SNUM8;        // Signed numeric 8 bits
    //using UNUM16 = System.UInt16;   //typedef unsigned short UNUM16;    // Unsigned numeric 16 bits
    //using SNUM16 = System.Int16;    //typedef signed short SNUM16;      // Signed numeric 16 bits
    using UNUM32 = UInt32; //typedef unsigned long UNUM32;     // Unsigned numeric 32 bits
    //using SNUM32 = System.Int32;    //typedef signed long SNUM32;       // Signed numeric 32 bits
    //using CHAR8 = System.Byte;      //typedef char CHAR8;               // ASCII-coded 8-bit character value (ISO8859-1 (Latin 1))

    internal class ApiCallPduGetResourceIdsUnsafe : ApiCallPduGetResourceIds
    {
        internal override unsafe List<PduRscIdItemData> PduGetResourceIds(uint moduleHandle, PduResourceData pduResourceData)
        {
            var pduRscDataNative = new PDU_RSC_DATA
            {
                BusTypeId = pduResourceData.BusTypeId,
                ProtocolId = pduResourceData.ProtocolId,
                NumPinData = (UNUM32)pduResourceData.DlcPinData.Count
            };

            var pduPinDataNative = new PDU_PIN_DATA[pduRscDataNative.NumPinData]; //Array of a value type
            for (var index = 0; index < pduRscDataNative.NumPinData; index++)
            {
                pduPinDataNative[index].DLCPinNumber = pduResourceData.DlcPinData[index].Key;
                pduPinDataNative[index].DLCPinTypeId = pduResourceData.DlcPinData[index].Value;
            }

            PDU_RSC_ID_ITEM* pResourceIdList;
            fixed (PDU_PIN_DATA* pPinData = pduPinDataNative)
            {
                pduRscDataNative.pDLCPinData = pPinData;  //complete the PDU_RSC_DATA structure
                CheckResultThrowException(PduGetResourceIds(moduleHandle, &pduRscDataNative, &pResourceIdList));
            }

            var pduRscIdItemDatas = new List<PduRscIdItemData>();
            
            if (pResourceIdList->ItemType == PduIt.PDU_IT_RSC_ID)
            {
                for (var index = 0; index < pResourceIdList->NumModules; index++)
                {
                    var pduRscIdItemData = &(pResourceIdList->pResourceIdDataArray)[index];

                    var handleModule = pduRscIdItemData->hMod;
                    var ids = new uint[pduRscIdItemData->NumIds];
                    
                    for (var i = 0; i < ids.Length; i++)
                    {
                        ids[i] = pduRscIdItemData->pResourceIdArray[i];
                    }
                    
                    pduRscIdItemDatas.Add(new PduRscIdItemData(handleModule, ids));
                }
            }

            CheckResultThrowException(PDUDestroyItem((PDU_ITEM*)pResourceIdList));
            return pduRscIdItemDatas;
        }

        internal ApiCallPduGetResourceIdsUnsafe(IntPtr handleToLoadedNativeLibrary) : base(handleToLoadedNativeLibrary)
        {
           
        }

        //should look like the C function as much as possible.
        // ReSharper disable InconsistentNaming
        private unsafe PduError PduGetResourceIds(UNUM32 hMod, PDU_RSC_DATA* pRscData, PDU_RSC_ID_ITEM** pResourceIdList)
        {
            return ((delegate* unmanaged[Stdcall]<UNUM32, PDU_RSC_DATA*, PDU_RSC_ID_ITEM**, PduError>)
                AddressOfNativeMethod)(hMod, pRscData, pResourceIdList);
        }

        private unsafe PduError PDUDestroyItem(PDU_ITEM* pItem)
        {
            return ((delegate* unmanaged[Stdcall]<PDU_ITEM*, PduError>) AddressOfNativeMethodDestroyItem)(pItem);
        }
        // ReSharper restore InconsistentNaming
    }
}
