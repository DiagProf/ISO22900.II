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

using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming
// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable IdentifierTypo

namespace ISO22900.II.SafeCStructs
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct PDU_IO_FILTER_LIST
    {
        internal uint NumFilterEntries;        // number of Filter entries in the filter list array
        internal PDU_IO_FILTER_DATA[] pFilterData;// pointer to an array of filter data
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct PDU_IO_FILTER_DATA
    {
        internal PduFilter FilterType;          // type of filter being configured. D.1.10
        internal uint FilterNumber;           // Filter Number. Used to replace filters and stop filters. Range depends implementation (see 9.5.13 PDU_IOCTL_START_MSG_FILTER)
        internal uint FilterCompareSize;      // Number of bytes used out of each of the filter messages arrays Range 1‐12.
        internal byte[] FilterMaskMessage = new byte[12];      // Mask message to be ANDed to each incoming message. When using the CAN  protocol, setting the first 4 bytes of FilterMaskMessage to 0xFF makes the filter 
                                                         // specific to one CAN ID. Using other values allows for the reception or blocking of multiple CAN identifiers.
        internal byte[] FilterPatternMessage = new byte[12];   // Pattern message to be compared to the incoming message after the FilterMaskMessage has been applied. If the result matches this pattern message 

        public PDU_IO_FILTER_DATA()
        {
            FilterType = default;
            FilterNumber = default;
            FilterCompareSize = default;
        }
        // and the FilterType is a pass filter, then the incoming message will be processed for further reception (otherwise it will be discarded). If the result matches this  pattern message and the FilterType is a block filter, then the incoming message 
        // will be discarded (otherwise it will be processed for further reception). Message 
        // bytes in the received message that are beyond the FilterCompareSize of the pattern message will be treated as “don't care”.
    }
}
