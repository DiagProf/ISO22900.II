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
    /// internal is ok here because the user just uses it as a uint
    /// </summary>
    internal class PduIoCtlOfTypeEntityStatus : PduIoCtl
    {
        internal PduIoCtlEntityStatusData Value { get; }

        internal PduIoCtlOfTypeEntityStatus(PduIoCtlEntityStatusData value ) : base(PduIt.PDU_IT_IO_ENTITY_STATUS)
        {
            Value = value;
        }

        internal override void Accept(IVisitorPduIoCtl visitorPduIoCtl)
        {
            //At the moment there is no reason because the value only comes from the API but does not have to go to the API
            //visitorPduIoCtl.VisitConcretePduIoCtlOfTypeEntityStatus(this);
        }
    }
}
