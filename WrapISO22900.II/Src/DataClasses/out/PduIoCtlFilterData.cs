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

namespace ISO22900.II
{
    public abstract class PduIoCtlFilterData : ICloneable<PduIoCtlFilterData>
    {
        internal byte[] FilterMaskMessage = new byte[12];
        internal byte[] FilterPatternMessage = new byte[12];

        /// <summary>
        ///     Defines the type of the filter. See section D.1.10
        /// </summary>
        public PduFilter FilterType { get;}

        public uint FilterNumber { get; set; }

        public uint FilterCompareSize { get; set; }

        public abstract PduIoCtlFilterData Clone();

        object ICloneable.Clone()
        {
            return Clone();
        }

        internal void Accept(IVisitorPduIoCtl visitorPduIoCtl)
        {
            visitorPduIoCtl.VisitConcretePduIoCtlFilterData(this);
        }

        protected PduIoCtlFilterData(PduFilter filterType, uint filterNumber, uint filterCompareSize,  byte[] filterMaskMsg, byte[] filterPatternMsg)
        {
            FilterType = filterType;
            FilterNumber = filterNumber;
            FilterCompareSize = filterCompareSize;

            if (filterMaskMsg.Length > 12)
                throw new ArgumentOutOfRangeException();
            for ( var i = 0; i < filterMaskMsg.Length; i++ )
            {
                FilterMaskMessage[i] = filterMaskMsg[i];
            }

            if (filterPatternMsg.Length > 12)
                throw new ArgumentOutOfRangeException();
            for (var i = 0; i < filterPatternMsg.Length; i++)
            {
                FilterPatternMessage[i] = filterPatternMsg[i];
            }
        }
    }

    public class PduIoCtlFilterDataForPassFilter : PduIoCtlFilterData
    {
        public PduIoCtlFilterDataForPassFilter(uint filterNumber, uint filterCompareSize, byte[] filterMaskMsg, byte[] filterPatternMsg) : base(
            PduFilter.PDU_FLT_PASS, filterNumber, filterCompareSize, filterMaskMsg, filterPatternMsg)
        {
        }

        public override PduIoCtlFilterData Clone()
        {
            return new PduIoCtlFilterDataForPassFilter(FilterNumber, FilterCompareSize, (byte[]) FilterMaskMessage.Clone(),(byte[]) FilterPatternMessage.Clone());
        }
    }
}
