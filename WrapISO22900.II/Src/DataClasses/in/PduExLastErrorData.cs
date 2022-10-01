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

namespace ISO22900.II
{
    /// <summary>
    /// I am not a structure from ISO 22900.
    /// That is why my name start with PduEx (Ex -> Extension).
    /// I am an encapsulation of values that are returned individually in C as call-by-reference.
    /// </summary>
    public class PduExLastErrorData
    {
        internal uint ComPrimitiveHandle { get; }

        /// <summary>
        ///     Last ErrorCode. If no last error has been stored then ErrorCode will contain PduErrEvt.PDU_ERR_EVT_NOERROR 
        /// </summary>
        public PduErrEvt ErrorCode { get; }

        /// <summary>
        ///     timestamp in microseconds.
        /// </summary>
        public uint Timestamp { get; }


        /// <summary>
        ///     For MVCI protocol modules and ComLogicalLinks, ExtraInfo contains additional information which is defined by the
        ///     MVCI protocol module supplier. Use MDF file to get a text from code
        ///     If no information is available, it shall be 0.
        /// </summary>
        public uint ExtraInfo { get; }

        internal PduExLastErrorData( uint comPrimitiveHandle, PduErrEvt errorCode, uint timestamp, uint extraInfo)
        {
            ComPrimitiveHandle = comPrimitiveHandle;
            ErrorCode = errorCode;
            Timestamp = timestamp;
            ExtraInfo = extraInfo;
        }
    }
}
