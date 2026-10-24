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

namespace ISO22900.II
{
    public class PduExpectedResponseData
    {
        /// <summary>
        /// 0=positive response; >0 negative response
        /// </summary>
        public PduExResponseType ResponseType;

        /// <summary>
        /// ID assigned by application to be returned in PDU_RESULT_DATA,
        /// which indicates which expected response matched
        /// </summary>
        public uint AcceptanceId;

        /// <summary>
        /// number of bytes in the Mask Data and Pattern Data
        /// </summary>
        public MaskAndPatternBytes MaskAndPatternPair;

        ///// <summary>
        ///// Pointer to Mask Data. Bits set to a '1' are care bits, '0' are don't care bits.
        ///// </summary>
        //internal UNUM8* pMaskData;
        //public byte[] DataArray { get; init; }
        
        /// <summary>
        /// Pointer to Pattern Data. Bytes to compare after the mask is applied
        /// </summary>
        /// internal UNUM8* pPatternData;

        /// <summary>
        /// Number of items in the following array of unique response identifiers.
        /// If the number is set to 0, then responses with any unique response identifier are considered,
        /// when trying to match them to this expected response.
        /// </summary>
        /// internal uint NumUniqueRespIds;

        /// <summary>
        /// Array containing unique response identifiers. Only responses with a unique response identifier found in this array are considered, when trying to match them to this expected response.
        /// </summary>
        /// internal uint* pUniqueRespIds;
        public UniqueRespIds UniqueRespIds { get; init; }

        internal void Accept(IVisitorPduComPrimitiveControlData visitorPduComPrimitiveControlData)
        {
            visitorPduComPrimitiveControlData.VisitConcretePduExpectedResponseData(this);
        }

        public PduExpectedResponseData(PduExResponseType responseType, uint acceptanceId, MaskAndPatternBytes maskAndPatternPair, UniqueRespIds uniqueRespIds)
        {
            ResponseType = responseType;
            AcceptanceId = acceptanceId;
            MaskAndPatternPair = maskAndPatternPair;
            UniqueRespIds = uniqueRespIds;
        }
    }

    public class MaskAndPatternBytes
    {
        public byte[] MaskDataArray { get; init; }
        public byte[] PatternDataArray { get; init; }
        
        public uint NumMaskPatternBytes => (uint)PatternDataArray.Length;
        
        public byte this[int index]
        {
            get => PatternDataArray[index];
            set => PatternDataArray[index] = value;
        }

        internal void Accept(IVisitorPduComPrimitiveControlData visitorPduComPrimitiveControlData)
        {
            visitorPduComPrimitiveControlData.VisitConcretePduMaskAndPatternBytes(this);
        }
        public MaskAndPatternBytes(byte[] maskDataArray, byte[] patternDataArray)
        {
            if (maskDataArray.Length == patternDataArray.Length)
            {
                MaskDataArray = maskDataArray;
                PatternDataArray = patternDataArray;
            }
        }
    }


    /// <summary>
    ///     Is called Longfield in ISO22900
    ///     I have renamed it so that there are no associations with the c# long data type
    /// </summary>
    public class UniqueRespIds
    {
        public uint NumberOfUniqueRespIds => (uint) DataArray.Length;

        private uint[] DataArray { get; init; }

        public uint this[int index]
        {
            get => DataArray[index];
            set => DataArray[index] = value;
        }

        internal void Accept(IVisitorPduComPrimitiveControlData visitorPduComPrimitiveControlData)
        {
            visitorPduComPrimitiveControlData.VisitConcretePduUniqueRespIds(this);
        }

        public UniqueRespIds(uint[] dataArray)
        {
            DataArray = dataArray;
        }
    }
}