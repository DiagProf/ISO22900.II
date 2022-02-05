using System;
using Microsoft.Extensions.Logging;

// ReSharper disable BuiltInTypeReferenceStyle

namespace ISO22900.II
{
    // Basic data types used in ISO22900-2
    // C# using alias directive     //C typedef...
    using UNUM8 = Byte; //typedef unsigned char UNUM8;      // Unsigned numeric 8 bits
    //using SNUM8 = System.SByte;     //typedef signed char SNUM8;        // Signed numeric 8 bits
    //using UNUM16 = System.UInt16;   //typedef unsigned short UNUM16;    // Unsigned numeric 16 bits
    //using SNUM16 = System.Int16;    //typedef signed short SNUM16;      // Signed numeric 16 bits
    using UNUM32 = UInt32; //typedef unsigned long UNUM32;     // Unsigned numeric 32 bits
    //using SNUM32 = System.Int32;    //typedef signed long SNUM32;       // Signed numeric 32 bits
    //using CHAR8 = System.Byte;      //typedef char CHAR8;               // ASCII-coded 8-bit character value (ISO8859-1 (Latin 1))

    internal class ApiCallPduCreateComLogicalLinkUnsafe : ApiCallPduCreateComLogicalLink
    {
        internal override uint PduCreateComLogicalLink(uint moduleHandle, PduResourceData pduResourceData,
            uint resourceId, uint cllTag, PduFlagDataCllCreateFlag pduFlagDataCllCreateFlag)
        {

            var pduRscDataNative = new PDU_RSC_DATA
            {
                BusTypeId = pduResourceData.BusTypeId,
                ProtocolId = pduResourceData.ProtocolId,
                NumPinData = (UNUM32) pduResourceData.DlcPinData.Count
            };

            var pduPinDataNative = new PDU_PIN_DATA[pduRscDataNative.NumPinData]; //Array of a value type
            for (var index = 0; index < pduRscDataNative.NumPinData; index++)
            {
                pduPinDataNative[index].DLCPinNumber = pduResourceData.DlcPinData[index].Key;
                pduPinDataNative[index].DLCPinTypeId = pduResourceData.DlcPinData[index].Value;
            }

            var pduFlagDataNative = new PDU_FLAG_DATA
            {
                NumFlagBytes = (uint) pduFlagDataCllCreateFlag.FlagData.Length
            };
            
            var flagBytes = new UNUM8[pduFlagDataNative.NumFlagBytes];
            for (var index = 0; index < pduFlagDataNative.NumFlagBytes; index++)
                flagBytes[index] = pduFlagDataCllCreateFlag.FlagData[index];

            PduError result;
            uint comLogicalLinkHandle; //this is why we are here :-)
            
            unsafe
            {
                //inside dll nobody dereferences the pointer,so we can use it like a 32 bit id (uint(32bit) is well for 64 and 32 bit DLLs).
                //Downsides is only if cllTag with zero, that should be avoided
                var pCllTag = cllTag == 0 ? null : ((IntPtr)cllTag).ToPointer();
                
                fixed (PDU_PIN_DATA* pPinData = pduPinDataNative)
                {
                    pduRscDataNative.pDLCPinData = pPinData;  //complete the PDU_RSC_DATA structure

                    fixed (UNUM8* pFlagBytes = flagBytes)
                    {
                        pduFlagDataNative.pFlagData = pFlagBytes; //complete the PDU_FLAG_DATA structure

                        result = PDUCreateComLogicalLink(moduleHandle, &pduRscDataNative, resourceId,
                            pCllTag, &comLogicalLinkHandle, &pduFlagDataNative);
                    }
                }
            }

            CheckResultThrowException(result);
            return comLogicalLinkHandle;
        }

        internal ApiCallPduCreateComLogicalLinkUnsafe(IntPtr handleToLoadedNativeLibrary) : base(handleToLoadedNativeLibrary)
        {
        }

        // should look like the C function as much as possible.
        // ReSharper disable InconsistentNaming
        // ReSharper disable BuiltInTypeReferenceStyle
        private unsafe PduError PDUCreateComLogicalLink(UNUM32 hMod, PDU_RSC_DATA* pRscData, UNUM32 resourceId,
            void* pCllTag, UNUM32* phCLL, PDU_FLAG_DATA* pCllCreateFlag)
        {
            return ((delegate* unmanaged[Stdcall]<UNUM32, PDU_RSC_DATA*, UNUM32, void*, UNUM32*, PDU_FLAG_DATA*,
                PduError>) AddressOfNativeMethod)(hMod, pRscData, resourceId, pCllTag, phCLL, pCllCreateFlag);
        }
        // ReSharper restore InconsistentNaming
        // ReSharper restore BuiltInTypeReferenceStyle
    }
}
