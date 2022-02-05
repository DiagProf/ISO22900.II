using System;
using System.Runtime.InteropServices;
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
    
    internal class ApiCallPduConstructUnsafe : ApiCallPduConstruct
    {
        internal override unsafe void PduConstruct(string optionStr, uint apiTag)
        {
            //inside dll nobody dereferences the pointer,so we can use it like a 32 bit id (uint(32bit) is well for 64 and 32 bit DLLs).
            //Downsides is only if apiTag with zero, that should be avoided
            var pApiTag = apiTag == 0 ? null : ((IntPtr)apiTag).ToPointer();
            
            var ptrOptionStr = Marshal.StringToHGlobalAnsi(optionStr);
            var result = PDUConstruct((CHAR8*)ptrOptionStr.ToPointer(), pApiTag);
            Marshal.FreeHGlobal(ptrOptionStr);
            CheckResultThrowException(result);
        }

        internal override unsafe void PduConstruct(string optionStr)
        {
            var ptrOptionStr = Marshal.StringToHGlobalAnsi(optionStr);
            var result = PDUConstruct((CHAR8*)ptrOptionStr.ToPointer(),null);
            Marshal.FreeHGlobal(ptrOptionStr);
            CheckResultThrowException(result);
        }

        internal override unsafe void PduConstruct(uint apiTag)
        {
            //inside dll nobody dereferences the pointer,so we can use it like a 32 bit id (uint(32bit) is well for 64 and 32 bit DLLs).
            //Downsides is only if apiTag with zero, that should be avoided
            var pApiTag = apiTag == 0 ? null : ((IntPtr)apiTag).ToPointer();
            CheckResultThrowException(PDUConstruct(null, pApiTag));
        }

        internal override unsafe void PduConstruct()
        {
            CheckResultThrowException(PDUConstruct(null, null));
        }

        internal ApiCallPduConstructUnsafe(IntPtr handleToLoadedNativeLibrary) : base(handleToLoadedNativeLibrary)
        {
            
        }

        //should look like the C function as much as possible.
        private unsafe PduError PDUConstruct(CHAR8* OptionStr, void* pAPITag) => ((delegate* unmanaged[Stdcall]<CHAR8*, void*, PduError>)AddressOfNativeMethod)(OptionStr, pAPITag);
    }
}
