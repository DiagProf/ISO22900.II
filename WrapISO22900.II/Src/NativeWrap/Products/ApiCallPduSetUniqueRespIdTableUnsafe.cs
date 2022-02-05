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
using Microsoft.Extensions.Logging;

namespace ISO22900.II
{
    internal class ApiCallPduSetUniqueRespIdTableUnsafe : ApiCallPduSetUniqueRespIdTable
    {
        private readonly VisitorPduComParamAndUniqueRespIdTableMemorySizeUnsafe _memorySizeVisitor;
        private readonly VisitorPduComParamAndUniqueRespIdTableToUnmanagedMemoryUnsafe _visitorPduComParamAndUniqueRespIdTable;
        private readonly PduUniqueRespIdTable _dummyPduUniqueRespIdTable;

        internal override unsafe void PduSetUniqueRespIdTable(uint moduleHandle, uint comLogicalLinkHandle, List<PduEcuUniqueRespData> ecuUniqueRespDatas)
        {
            _dummyPduUniqueRespIdTable.TableEntries = ecuUniqueRespDatas;
            _memorySizeVisitor.MemorySize = 0;
            _dummyPduUniqueRespIdTable.Accept(_memorySizeVisitor);
            void* pUniqueRespIdTableDataOnStack = stackalloc byte[_memorySizeVisitor.MemorySize];
            _visitorPduComParamAndUniqueRespIdTable.PointerForUniqueRespIdTable = pUniqueRespIdTableDataOnStack;
            _dummyPduUniqueRespIdTable.Accept(_visitorPduComParamAndUniqueRespIdTable);
            
            CheckResultThrowException(PDUSetUniqueRespIdTable(moduleHandle, comLogicalLinkHandle, (PDU_UNIQUE_RESP_ID_TABLE_ITEM*) pUniqueRespIdTableDataOnStack));
        }

        internal ApiCallPduSetUniqueRespIdTableUnsafe(IntPtr handleToLoadedNativeLibrary) : base(handleToLoadedNativeLibrary)
        {
            _dummyPduUniqueRespIdTable = new PduUniqueRespIdTable();
            _memorySizeVisitor = new VisitorPduComParamAndUniqueRespIdTableMemorySizeUnsafe();
            _visitorPduComParamAndUniqueRespIdTable = new VisitorPduComParamAndUniqueRespIdTableToUnmanagedMemoryUnsafe();
        }

        // should look like the C function as much as possible.
        // ReSharper disable InconsistentNaming
        private unsafe PduError PDUSetUniqueRespIdTable(UInt32 hMod, UInt32 hCLL, PDU_UNIQUE_RESP_ID_TABLE_ITEM* pUniqueRespIdTable)
        {
            return ((delegate* unmanaged[Stdcall]<UInt32, UInt32, PDU_UNIQUE_RESP_ID_TABLE_ITEM*, PduError >) AddressOfNativeMethod)(hMod, hCLL, pUniqueRespIdTable);
        }
        // ReSharper restore InconsistentNaming
    }
}
