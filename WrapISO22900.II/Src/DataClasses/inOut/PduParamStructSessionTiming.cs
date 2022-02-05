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
    public class PduParamStructSessionTiming : PduParamStructData
    {
        /// <summary>
        /// Session Number,for the diagnostic session of ISO15765‐3/ISO14229‐3
        /// </summary>
        public ushort Session { get; set; }

        /// <summary>
        /// 1ms resolution, High byte of the default P2Can_Server_max timing supported by the
        /// server for the activated diagnostic session. Used for ComParamCP_P2Max.
        /// </summary>
        public byte P2MaxHigh { get; set; }

        /// <summary>
        /// 1ms resolution, Low byte of the default P2Can_Server_max timing supported by the
        /// server for the activated diagnostic session. Used for ComParamCP_P2Max.
        /// </summary>
        public byte P2MaxLow { get; set; }

        /// <summary>
        /// 10ms resolution. High byte of the enhanced (NRC78hex) P2Can_Server_max supported
        /// by the server for the activated diagnostic session. Used for ComParam CP_P2Star
        /// server for the activated diagnostic session. Used for ComParamCP_P2Max.
        /// </summary>
        public byte P2StarHigh { get; set; }

        /// <summary>
        /// 10ms resolution. Low byte of the enhanced (NRC78hex) P2Can_Server_max supported
        /// by the server for the activated diagnostic session. Used for ComParam CP_P2Star
        /// server for the activated diagnostic session. Used for ComParamCP_P2Max.
        /// </summary>
        public byte P2StarLow { get; set; }

        public PduParamStructSessionTiming(ushort session, byte p2MaxHigh, byte p2MaxLow, byte p2StarHigh, byte p2StarLow)
        {
            Session = session;
            P2MaxHigh = p2MaxHigh;
            P2MaxLow = p2MaxLow;
            P2StarHigh = p2StarHigh;
            P2StarLow = p2StarLow;
        }

        internal override void Accept(IVisitorPduComParamAndUniqueRespIdTable visitorPduComParamAndUniqueRespIdTable)
        {
            visitorPduComParamAndUniqueRespIdTable.VisitConcretePduParamStructSessionTiming(this);
        }

        public override PduParamStructData Clone()
        {
            return new PduParamStructSessionTiming(Session, P2MaxHigh, P2MaxLow, P2StarHigh, P2StarLow);
        }
    }
}