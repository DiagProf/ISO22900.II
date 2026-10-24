#region License

// // MIT License
// //
// // Copyright (c) 2022 Joerg Frank
// // http://www.diagprof.com/
// //
// // Permission is hereby granted, free of charge, to any person obtaining a copy
// // of this software and associated documentation files (the "Software"), to deal
// // in the Software without restriction, including without limitation the rights
// // to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// // copies of the Software, and to permit persons to whom the Software is
// // furnished to do so, subject to the following conditions:
// //
// // The above copyright notice and this permission notice shall be included in all
// // copies or substantial portions of the Software.
// //
// // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// // IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// // FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// // AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// // LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// // OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// // SOFTWARE.

#endregion

using System;
// ReSharper disable IdentifierTypo
// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable InconsistentNaming

namespace ISO22900.II
{
    /// <summary>
    /// Definition of filter types values
    /// </summary>
    public enum PduFilter : UInt32 //ENUM are also treated as a 32-bit value under 64-bit OS, this is more or less a reminder
    {
        PDU_FLT_PASS = 0x00000001,      // Allows matching messages into the receive event queue. For all protocols.
        PDU_FLT_BLOCK = 0x00000002,     // Keeps matching messages out of the event queue. For all protocols.
        PDU_FLT_PASS_UUDT = 0x00000011, // Allows matching messages into the receive event queue which are of a UUDT type only. For ISO 15765 only.
        PDU_FLT_BLOCK_UUDT = 0x00000012 // Keeps matching messages out of the event queue which are of a UUDT type only. For ISO 15765 only.
    }
}
