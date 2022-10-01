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

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ISO22900.II
{

    /// <summary>
    ///     I'm implementing a wrapper class over the actual ComPrimitiveLevel instance.
    ///     The main reason why I exist is that in the event of an error (e.g. VCI lost)
    ///     I am able to replace the real VCI connection (which I wrap) with a new instance.
    ///     The application has an instance of me that doesn't change from the application point of view.
    /// </summary>
    public class ComPrimitive : IDisposable
    {
        private readonly ILogger _logger = ApiLibLogging.CreateLogger<ComPrimitive>();
        private readonly ComLogicalLink _comLogicalLink;
        private readonly PduCopt _pduCopType;
        private readonly byte[] _copData;
        private readonly PduCopCtrlData _copCtrlData;
        private readonly uint _copTag;

        private ComPrimitiveLevel _cop;

        internal ComPrimitive(ComLogicalLink comLogicalLink, ComPrimitiveLevel cop, PduCopt pduCopType, byte[] copData,
            PduCopCtrlData copCtrlData,
            uint copTag)
        {
            _comLogicalLink = comLogicalLink;
            _cop = cop;
            _pduCopType = pduCopType;
            _copData = copData;
            _copCtrlData = new PduCopCtrlData()
            {
                NumSendCycles = copCtrlData.NumSendCycles,
                NumReceiveCycles = copCtrlData.NumReceiveCycles,
                TempParamUpdate = copCtrlData.TempParamUpdate,
                Time = copCtrlData.Time,
                TxFlag = new PduFlagDataTxFlag
                {
                    FlagData = (byte[])copCtrlData.TxFlag.FlagData.Clone(),
                },
                PduExpectedResponseDatas = copCtrlData.PduExpectedResponseDatas, //ToDo
            };
            _copTag = copTag;
        }


        public ComPrimitiveResult WaitForCopResult()
        {
            return _cop.WaitForCopResult();
        }

        public async Task<ComPrimitiveResult> WaitForCopResultAsync(CancellationToken ct = default)
        {
            return await _cop.WaitForCopResultAsync(ct);
        }

        public PduExStatusData Status()
        {
            return _cop.Status();
        }

        public void Cancel()
        {
            _cop.Cancel();
        }

        /// <summary>
        /// Attempts to restore the status of the ComLogicalLink
        /// Catch all "Iso22900IIException" exceptions
        /// </summary>
        /// <returns></returns>
        public bool TryToRecover(out string exMessage)
        {
            var pduCopStatus = PduStatus.PDU_COPST_CANCELLED;

            try
            {
                pduCopStatus = _cop.Status().Status;
            }
            catch (Iso22900IIException)
            {
                pduCopStatus = PduStatus.PDU_COPST_CANCELLED;
            }

            try
            {
                if(pduCopStatus != PduStatus.PDU_COPST_EXECUTING)
                {
                    try
                    {
                        _cop.Dispose();
                    }
                    catch (Iso22900IIException )
                    {

                    }
                    

                    //this is where the magic happens.. the new instance is assigned under the hood
                    if (!_comLogicalLink.TryToRecover(out var msg))
                    {
                        exMessage = msg;
                        return false;
                    }
                    _cop = _comLogicalLink.StartCop(_pduCopType, _copData, _copCtrlData, _copTag)._cop;
                    _logger.Log(LogLevel.Information, "ComPrimitive recovering done for ComPrimitive Req: { _copData}", _copData);
                }
            }
            catch (Iso22900IIException ex)
            {
                exMessage = ex.Message;
                return false;
            }

            exMessage = string.Empty;
            return true;
        }

        #region DisposeBehavior

        public void Dispose()
        {
            _cop.Dispose();
        }

        #endregion
    }
}
