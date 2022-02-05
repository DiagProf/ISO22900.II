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
using Microsoft.Extensions.Logging;

namespace ISO22900.II
{
    // Basic data types used in ISO22900-2
    // C# using alias directive     //C typedef...
    using UNUM8 = System.Byte;      //typedef unsigned char UNUM8;      // Unsigned numeric 8 bits
    using SNUM8 = System.SByte;     //typedef signed char SNUM8;        // Signed numeric 8 bits
    using UNUM16 = System.UInt16;   //typedef unsigned short UNUM16;    // Unsigned numeric 16 bits
    using SNUM16 = System.Int16;    //typedef signed short SNUM16;      // Signed numeric 16 bits
    using UNUM32 = System.UInt32;   //typedef unsigned long UNUM32;     // Unsigned numeric 32 bits
    using SNUM32 = System.Int32;    //typedef signed long SNUM32;       // Signed numeric 32 bits
    //using CHAR8 = System.Byte;      //typedef char CHAR8;               // ASCII-coded 8-bit character value (ISO8859-1 (Latin 1))
    
    internal class ApiCallPduSetViaGetComParamUnsafe : ApiCallPduSetViaGetComParam
    {
        internal override unsafe PduComParam PduSetViaGetComParam(uint moduleHandle, uint comLogicalLinkHandle, uint objectId, long value)
        {
            PDU_PARAM_ITEM* pPduParamItem;
            CheckResultThrowException(PDUGetComParam(moduleHandle, comLogicalLinkHandle, objectId, &pPduParamItem));

            PduComParam returnPduComParam; 

            switch (pPduParamItem->ComParamDataType)
            {
                case PduPt.PDU_PT_UNUM8:
                    (*(UNUM8*)pPduParamItem->pComParamData) = (UNUM8)Convert.ToByte(value);
                    returnPduComParam = new PduComParamOfTypeByte(objectId, pPduParamItem->ComParamClass, (byte)value);
                    break;
                case PduPt.PDU_PT_SNUM8:
                    (*(SNUM8*)pPduParamItem->pComParamData) = (SNUM8)Convert.ToSByte(value);
                    returnPduComParam = new PduComParamOfTypeSbyte(objectId, pPduParamItem->ComParamClass, (sbyte)value);
                    break;
                case PduPt.PDU_PT_UNUM16:
                    (*(UNUM16*)pPduParamItem->pComParamData) = (UNUM16)Convert.ToUInt16(value);
                    returnPduComParam = new PduComParamOfTypeUshort(objectId, pPduParamItem->ComParamClass, (ushort)value);
                    break;
                case PduPt.PDU_PT_SNUM16:
                    (*(SNUM16*)pPduParamItem->pComParamData) = (SNUM16)Convert.ToInt16(value);
                    returnPduComParam = new PduComParamOfTypeShort(objectId, pPduParamItem->ComParamClass, (short)value);
                    break;
                case PduPt.PDU_PT_UNUM32:
                    (*(UNUM32*)pPduParamItem->pComParamData) = (UNUM32)Convert.ToUInt32(value);
                    returnPduComParam = new PduComParamOfTypeUint(objectId, pPduParamItem->ComParamClass, (uint)value);
                    break;
                case PduPt.PDU_PT_SNUM32:
                    (*(SNUM32*)pPduParamItem->pComParamData) = (SNUM32)Convert.ToInt32(value);
                    returnPduComParam = new PduComParamOfTypeInt(objectId, pPduParamItem->ComParamClass, (int)value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            CheckResultThrowException(PDUSetComParam(moduleHandle, comLogicalLinkHandle, pPduParamItem));
            
            CheckResultThrowException(PDUDestroyItem((PDU_ITEM*)pPduParamItem));
            return returnPduComParam;
        }

        internal override unsafe PduComParam PduSetViaGetComParam(uint moduleHandle, uint comLogicalLinkHandle, uint objectId, byte[] value)
        {
            PDU_PARAM_ITEM* pPduParamItem;
            CheckResultThrowException(PDUGetComParam(moduleHandle, comLogicalLinkHandle, objectId, &pPduParamItem));

            if (pPduParamItem->ComParamDataType != PduPt.PDU_PT_BYTEFIELD)
            {
                throw new ArgumentOutOfRangeException();
            }
            ;
            var fieldData = ((*(PDU_PARAM_BYTEFIELD_DATA*)pPduParamItem->pComParamData));

            //I pray that every supplier has read this sentence under
            //e.g. B.3.3.1 ComParam BYTEFIELD data type to ParamMaxLen "This is also the amount of memory the D-PDU API allocates prior to a call of PDUGetComParam."
            //and don't use ParamActLen to calculate the memory to be allocated
            if (value.Length > fieldData.ParamMaxLen)
            {
                throw new ArgumentOutOfRangeException();
            }

            PduComParam returnPduComParam = new PduComParamOfTypeByteField(objectId, pPduParamItem->ComParamClass, new PduParamByteFieldData(value, fieldData.ParamMaxLen));

            for (var i = 0; i < value.Length; i++)
            {
                fieldData.pDataArray[i] = value[i];
            }

            fieldData.ParamActLen = (uint)value.Length;


            CheckResultThrowException(PDUSetComParam(moduleHandle, comLogicalLinkHandle, pPduParamItem));

            CheckResultThrowException(PDUDestroyItem((PDU_ITEM*)pPduParamItem));

            return returnPduComParam;
        }

        internal override unsafe PduComParam PduSetViaGetComParam(uint moduleHandle, uint comLogicalLinkHandle, uint objectId, uint[] value)
        {
            PDU_PARAM_ITEM* pPduParamItem;
            CheckResultThrowException(PDUGetComParam(moduleHandle, comLogicalLinkHandle, objectId, &pPduParamItem));

            if (pPduParamItem->ComParamDataType != PduPt.PDU_PT_LONGFIELD)
            {
                throw new ArgumentOutOfRangeException();
            }

            var fieldData = ((*(PDU_PARAM_LONGFIELD_DATA*)pPduParamItem->pComParamData));

            //I pray that every supplier has read this sentence under
            //e.g. B.3.3.3 ComParam LONGFIELD data type to ParamMaxLen "This is also the amount of memory the D-PDU API allocates prior to a call of PDUGetComParam."
            //and don't use ParamActLen to calculate the memory to be allocated
            if (value.Length > fieldData.ParamMaxLen)
            {
                throw new ArgumentOutOfRangeException();
            }

            PduComParam returnPduComParam = new PduComParamOfTypeUintField(objectId, pPduParamItem->ComParamClass, new PduParamUintFieldData(value, fieldData.ParamMaxLen));

            for (var i = 0; i < value.Length; i++)
            {
                fieldData.pDataArray[i] = value[i];
            }

            fieldData.ParamActLen = (uint)value.Length;

            CheckResultThrowException(PDUSetComParam(moduleHandle, comLogicalLinkHandle, pPduParamItem));

            CheckResultThrowException(PDUDestroyItem((PDU_ITEM*)pPduParamItem));

            return returnPduComParam;
        }

        internal override unsafe PduComParam PduSetViaGetComParam(uint moduleHandle, uint comLogicalLinkHandle, uint objectId,
            PduParamStructFieldData data)
        {
            PDU_PARAM_ITEM* pPduParamItem;
            CheckResultThrowException(PDUGetComParam(moduleHandle, comLogicalLinkHandle, objectId, &pPduParamItem));

            if (pPduParamItem->ComParamDataType != PduPt.PDU_PT_STRUCTFIELD)
            {
                throw new ArgumentOutOfRangeException();
            }
            var fieldData = ((*(PDU_PARAM_STRUCTFIELD_DATA*)pPduParamItem->pComParamData));

            //I pray that every supplier has read this sentence under
            //e.g. B.3.3.2 ComParam STRUCTFIELD data type to ParamMaxEntries "The D-PDU-API allocates this amount of memory based on the size of the structure type prior to a call of PDUGetComParam."
            //and don't use ParamActEntries to calculate the memory to be allocated
            if (data.StructArray.Length > fieldData.ParamMaxEntries)
            {
                throw new ArgumentOutOfRangeException();
            }

            PduComParam returnPduComParam = new PduComParamOfTypeStructField(objectId, pPduParamItem->ComParamClass, data);

            if (fieldData.ComParamStructType == PduCpSt.PDU_CPST_SESSION_TIMING)
            {
                var pStructSessTiming = (((PDU_PARAM_STRUCT_SESS_TIMING*)fieldData.pStructArray));
                for (var i = 0; i < data.StructArray.Length; i++)
                {
                    var structSessionTiming = data.StructArray[i] as PduParamStructSessionTiming;
                    if (structSessionTiming == null)
                    {
                        continue;
                    }

                    pStructSessTiming[i].session = structSessionTiming.Session;
                    pStructSessTiming[i].P2Max_high = structSessionTiming.P2MaxHigh;
                    pStructSessTiming[i].P2Max_low = structSessionTiming.P2MaxLow;
                    pStructSessTiming[i].P2Star_high = structSessionTiming.P2StarHigh;
                    pStructSessTiming[i].P2Star_low = structSessionTiming.P2StarLow;
                }
            }
            else if (fieldData.ComParamStructType == PduCpSt.PDU_CPST_ACCESS_TIMING)
            {
                var pStructAccessTiming = (((PDU_PARAM_STRUCT_ACCESS_TIMING*)fieldData.pStructArray));
                for (var i = 0; i < data.StructArray.Length; i++)
                {
                    var structAccessTiming = data.StructArray[i] as PduParamStructAccessTiming;
                    if (structAccessTiming == null)
                    {
                        continue;
                    }

                    pStructAccessTiming[i].P2Min = structAccessTiming.P2Min;
                    pStructAccessTiming[i].P2Max = structAccessTiming.P2Max;
                    pStructAccessTiming[i].P3Min = structAccessTiming.P3Min;
                    pStructAccessTiming[i].P3Max = structAccessTiming.P3Max;
                    pStructAccessTiming[i].P4Min = structAccessTiming.P4Min;
                    pStructAccessTiming[i].TimingSet = structAccessTiming.TimingSet;
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }


            CheckResultThrowException(PDUSetComParam(moduleHandle, comLogicalLinkHandle, pPduParamItem));

            CheckResultThrowException(PDUDestroyItem((PDU_ITEM*)pPduParamItem));

            return returnPduComParam;
        }


        internal ApiCallPduSetViaGetComParamUnsafe(IntPtr handleToLoadedNativeLibrary) : base(handleToLoadedNativeLibrary)
        {

        }

        //should look like the C function as much as possible.
        // ReSharper disable InconsistentNaming
        private unsafe PduError PDUGetComParam(UNUM32 hMod, UNUM32 hCLL, UNUM32 ParamId,
            PDU_PARAM_ITEM** pParamItem)
        {
            return ((delegate* unmanaged[Stdcall]<UNUM32, UNUM32, UNUM32, PDU_PARAM_ITEM**, PduError>)
                AddressOfNativeMethod)(hMod, hCLL, ParamId, pParamItem);
        }

        private unsafe PduError PDUDestroyItem(PDU_ITEM* pItem)
        {
            return ((delegate* unmanaged[Stdcall]<PDU_ITEM*, PduError>) AddressOfNativeMethodDestroyItem)(pItem);
        }

        private unsafe PduError PDUSetComParam(UNUM32 hMod, UNUM32 hCLL, PDU_PARAM_ITEM* pParamItem)
        {
            return ((delegate* unmanaged[Stdcall]<UNUM32, UNUM32, PDU_PARAM_ITEM*, PduError>)AddressOfNativeMethodPDUSetComParam)(hMod, hCLL, pParamItem);
        }
        // ReSharper restore InconsistentNaming
    }
}
