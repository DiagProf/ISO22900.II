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
    public class PduParamStructAccessTiming : PduParamStructData
    {
        /// <summary>
        /// 0,5 ms resolution. Minimum time between tester request and ECU response(s).
        /// Used for ComParam CP_P2Min
        /// </summary>
        public byte P2Min { get; set; }

        /// <summary>
        /// Resolution see table 5 of ISO14230-2.
        /// Maximum time between tester request and ECU response(s).
        /// User for ComParam CP_P2Max.
        /// </summary>
        public byte P2Max { get; set; }

        /// <summary>
        /// 0.5ms resolution. Minimum time between end of ECU responses and start of new tester request.
        /// User for ComParam CP_P3Min
        /// </summary>
        public byte P3Min { get; set; }

        /// <summary>
        /// 250ms resolution. Maximum time between ECU responses and
        /// start of new tester request used for ComParam CP_P3Max_Ecu or CP_P2Star for the tester
        /// </summary>
        public byte P3Max { get; set; }

        /// <summary>
        /// 0.5ms resolution. Minimum inter byte time for tester request. Use for ComParam CP_P4Min
        /// </summary>
        public byte P4Min { get; set; }

        /// <summary>
        /// Set number allowing multiple parameters (ex: default, normal)
        /// </summary>
        public byte TimingSet { get; set; }

        public PduParamStructAccessTiming(byte p2Min, byte p2Max, byte p3Min, byte p3Max, byte p4Min, byte timingSet)
        {
            P2Min = p2Min;
            P2Max = p2Max;
            P3Min = p3Min;
            P3Max = p3Max;
            P4Min = p4Min;
            TimingSet = timingSet;
        }

        internal override void Accept(IVisitorPduComParamAndUniqueRespIdTable visitorPduComParamAndUniqueRespIdTable)
        {
            visitorPduComParamAndUniqueRespIdTable.VisitConcretePduParamStructAccessTiming(this);
        }

        public override PduParamStructData Clone()
        {
            return new PduParamStructAccessTiming(P2Min, P2Max, P3Min, P3Max, P4Min, TimingSet);
        }
    }
}