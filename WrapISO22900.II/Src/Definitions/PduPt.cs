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
// ReSharper disable IdentifierTypo
// ReSharper disable BuiltInTypeReferenceStyle

namespace ISO22900.II
{
    /// <summary>
    /// Definition of ODX ComParam data types
    /// </summary>
    public enum PduPt : UInt32   //ENUM are also treated as a 32-bit value under 64-bit OS, this is more or less a reminder
    {
        PDU_PT_UNUM8 = 0x00000101,   // Unsigned byte
        PDU_PT_SNUM8 = 0x00000102,   // Signed byte
        PDU_PT_UNUM16 = 0x00000103,   // Unsigned two bytes
        PDU_PT_SNUM16 = 0x00000104,   // Signed two bytes
        PDU_PT_UNUM32 = 0x00000105,   // Unsigned four bytes
        PDU_PT_SNUM32 = 0x00000106,   // Signed four bytes
        PDU_PT_BYTEFIELD = 0x00000107,   // Structure contains an array of UNUM8 bytes with a maximum length and actual length fields. See B.2.3.1 ComParam BYTEFIELD data type for the definition.  
        PDU_PT_STRUCTFIELD = 0x00000108, // Structure contains a void * pointer to an array of structures. The ComParamStructType item determines the type of structure to be typecasted onto the void * pointer.  This structure contains a field for maximum number of struct entries and the actual number of struct entries. B.2.3.2 ComParam STRUCTFIELD data type for the definition.
        PDU_PT_LONGFIELD = 0x00000109    // Structure contains an array of UNUM32 entries with a maximum length and actual length fields. See B.2.3.3 ComParam LONGFIELD Data Type for the definition.  
    }
}