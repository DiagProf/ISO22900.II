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
// OUT OF OR IN DisconnectION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// */

#endregion

using System;
using Microsoft.Extensions.Logging;

namespace ISO22900.II
{
    internal class ApiCallPduDisconnectUnsafe : ApiCallPduDisconnect
    {
        
        internal override unsafe void PduDisconnect(uint moduleHandle, uint comLogicalLinkHandle)
        {
            CheckResultThrowException(PDUDisconnect(moduleHandle, comLogicalLinkHandle));
        }

        internal ApiCallPduDisconnectUnsafe(IntPtr handleToLoadedNativeLibrary) : base(handleToLoadedNativeLibrary)
        {
        }

        // should look like the C function as much as possible.
        // ReSharper disable InconsistentNaming
        private unsafe PduError PDUDisconnect(UInt32 hMod, UInt32 hCLL)
        {
            return ((delegate* unmanaged[Stdcall]<UInt32, UInt32, PduError >) AddressOfNativeMethod)(hMod, hCLL);
        }
        // ReSharper restore InconsistentNaming
    }
}
