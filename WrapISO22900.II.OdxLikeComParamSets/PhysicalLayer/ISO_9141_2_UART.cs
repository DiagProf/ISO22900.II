#region License

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

#endregion


using System.Collections.Generic;

namespace ISO22900.II.OdxLikeComParamSets.PhysicalLayer
{
    public class ISO_9141_2_UART : AbstractPhysicalLayer
    {
        private readonly PduComParamOfTypeUint _cpBaudrate;
        private readonly PduComParamOfTypeUint _cpK_L_LineInit;
        private readonly PduComParamOfTypeUint _cpK_LinePullup;
        private readonly PduComParamOfTypeUint _cpUartConfig;


        public uint CP_Baudrate
        {
            get => _cpBaudrate.ComParamData;
            set => _cpBaudrate.ComParamData = value;
        }
        public uint CP_K_L_LineInit
        {
            get => _cpK_L_LineInit.ComParamData;
            set => _cpK_L_LineInit.ComParamData = value;
        }
        public uint CP_K_LinePullup
        {
            get => _cpK_LinePullup.ComParamData;
            set => _cpK_LinePullup.ComParamData = value;
        }
        public uint CP_UartConfig
        {
            get => _cpUartConfig.ComParamData;
            set => _cpUartConfig.ComParamData = value;
        }

        public ISO_9141_2_UART()
        {
            _cpBaudrate = (PduComParamOfTypeUint)CreateCp("CP_Baudrate", 10400, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_BUSTYPE);
            _cpK_L_LineInit = (PduComParamOfTypeUint)CreateCp("CP_K_L_LineInit", 0, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_BUSTYPE);
            _cpK_LinePullup = (PduComParamOfTypeUint)CreateCp("CP_K_LinePullup", 1, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_BUSTYPE);
            _cpUartConfig = (PduComParamOfTypeUint)CreateCp("CP_UartConfig", 6, PduPt.PDU_PT_UNUM32, PduPc.PDU_PC_BUSTYPE);
            
            CpList = new List<PduComParam>
            {
                _cpBaudrate, _cpK_L_LineInit, _cpK_LinePullup,
                _cpUartConfig
            };
        }

    }
}
