using System;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace ISO22900.II
{
    internal abstract class ApiCallPduDestroyItem : ApiCall
    {
        protected readonly IntPtr AddressOfNativeMethodDestroyItem;
        protected ApiCallPduDestroyItem(IntPtr handleToLoadedNativeLibrary) : base(handleToLoadedNativeLibrary)
        {
            AddressOfNativeMethodDestroyItem = NativeLibrary.GetExport(handleToLoadedNativeLibrary, "PDUDestroyItem");
        }
    }
}
