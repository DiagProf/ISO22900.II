using System;
using Microsoft.Extensions.Logging;

// ReSharper disable BuiltInTypeReferenceStyle

namespace ISO22900.II
{
    // Basic data types used in ISO22900-2
    // C# using alias directive     //C typedef...
    //using UNUM8 = System.Byte;      //typedef unsigned char UNUM8;      // Unsigned numeric 8 bits
    //using SNUM8 = System.SByte;     //typedef signed char SNUM8;        // Signed numeric 8 bits
    //using UNUM16 = System.UInt16;   //typedef unsigned short UNUM16;    // Unsigned numeric 16 bits
    //using SNUM16 = System.Int16;    //typedef signed short SNUM16;      // Signed numeric 16 bits
    using UNUM32 = UInt32; //typedef unsigned long UNUM32;     // Unsigned numeric 32 bits
    //using SNUM32 = System.Int32;    //typedef signed long SNUM32;       // Signed numeric 32 bits
    //using CHAR8 = System.Byte;      //typedef char CHAR8;               // ASCII-coded 8-bit character value (ISO8859-1 (Latin 1))

    internal class ApiCallPduGetStatusUnsafe : ApiCallPduGetStatus
    {
        internal override unsafe PduExStatusData PduGetStatus(uint moduleHandle, uint comLogicalLinkHandle,
            uint comPrimitiveHandle)
        {
            PduStatus moduleStatus;
            UNUM32 timestamp;
            UNUM32 extraInfo;
            CheckResultThrowException(PDUGetStatus(moduleHandle, comLogicalLinkHandle, comPrimitiveHandle,
                &moduleStatus, &timestamp, &extraInfo));
            return new PduExStatusData(moduleStatus, timestamp, extraInfo);
        }

        internal ApiCallPduGetStatusUnsafe(IntPtr handleToLoadedNativeLibrary) : base(handleToLoadedNativeLibrary)
        {
        }

        // should look like the C function as much as possible.
        // ReSharper disable InconsistentNaming
        private unsafe PduError PDUGetStatus(UNUM32 hMod, UNUM32 hCLL, UNUM32 hCoP, PduStatus* pStatusCode,
            UNUM32* pTimestamp, UNUM32* pExtraInfo)
        {
            return ((delegate* unmanaged[Stdcall]<UInt32, UInt32, UInt32, PduStatus*, UInt32*, UInt32*, PduError >)
                AddressOfNativeMethod)(hMod, hCLL, hCoP, pStatusCode, pTimestamp, pExtraInfo);
        }
        // ReSharper restore InconsistentNaming
    }
}
