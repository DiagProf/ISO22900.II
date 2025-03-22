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

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ISO22900.II
{
    internal class ComLogicalLinkLevel : ManagedDisposable
    {
        private readonly ILogger _logger = ApiLibLogging.CreateLogger<ComLogicalLinkLevel>();
 
        private readonly object _syncStartCop = new();//_syncStartCop to prevent an event from having to be handled by a COP before class creation is complete

        internal readonly ModuleLevel Vci;

        internal ConcurrentDictionary<uint, ChannelWriter<PduEventItem>> CopChannels { get; } = new();
        internal Channel<PduEventItem> TraceCllEvent = Channel.CreateBounded<PduEventItem>(new BoundedChannelOptions(10)
        {
            FullMode = BoundedChannelFullMode.DropOldest,
            SingleReader = true,
            SingleWriter = true
        });

        protected internal uint ModuleHandle { get; }
        protected internal uint ComLogicalLinkHandle { get; }

        public event EventHandler<CallbackEventArgs> DataLost;
        public event EventHandler<PduEventItem> EventFired;


        public ComLogicalLinkLevel(ModuleLevel vci, uint comLogicalLinkHandle)
        {
            Vci = vci;
            ModuleHandle = vci.ModuleHandle;
            ComLogicalLinkHandle = comLogicalLinkHandle;

            Vci.SysLevel.EventItemProvider.RegisterEventDataCallback(ModuleHandle, ComLogicalLinkHandle, CallbackPduEventItemReceived, CallbackDataLost);
        }

        internal ComPrimitiveLevel StartCop(PduCopt pduCopType, byte[] copData, PduCopCtrlData copCtrlData, uint copTag)
        {

            Channel<PduEventItem> channel;

            if ((copCtrlData.NumSendCycles < 0) || copCtrlData.NumReceiveCycles == -1)
            {
                //LongRunning ComPrimitive (must be canceled)
                //the reason for a "Bounded channel" is that the cop will run indefinitely (until they are channeled)
                //and the channels might fill up if not emptied. So we limit the channel
                channel = Channel.CreateBounded<PduEventItem>(new BoundedChannelOptions(50)
                {
                    FullMode = BoundedChannelFullMode.DropOldest,
                    SingleReader = true,
                    SingleWriter = true
                });
            }
            else
            {
                channel = Channel.CreateUnbounded<PduEventItem>(new UnboundedChannelOptions
                {
                    SingleReader = true,
                    SingleWriter = true
                });
            }

            lock (_syncStartCop)
            {
                var comPrimitiveHandle = Vci.SysLevel.Nwa.PduStartComPrimitive(ModuleHandle, ComLogicalLinkHandle,
                    pduCopType, copData, copCtrlData, copTag);

                CopChannels.GetOrAdd(comPrimitiveHandle, channel.Writer);
                
                var comPrimitive = new ComPrimitiveLevel(this, comPrimitiveHandle, channel.Reader, pduCopType, copData, copCtrlData, copTag);
                Disposing += comPrimitive.Dispose;
                return comPrimitive;
            }
        }

        public void Connect()
        {
            Vci.SysLevel.Nwa.PduConnect(ModuleHandle, ComLogicalLinkHandle);
        }

        public void Disconnect()
        {
            using (var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(5000)))
            {
                try
                {
                    _ = DisconnectAsync(cts.Token);
                }
                catch (AggregateException e)
                {
                    foreach (var ex in e.InnerExceptions)
                    {
                        if (ex is not OperationCanceledException)
                        {
                            throw ex;
                        }
                    }
                }
            }


            //var linkStatus = PduStatus.PDU_CLLST_OFFLINE;
            //try
            //{
            //    linkStatus = Status().Status;
            //}
            //catch (Iso22900IIException)
            //{
            //    _logger.LogWarning("The ComLogicalLink can no longer be reached.This could be normal if a VCI was lost previously.");
            //}

            //if (linkStatus == PduStatus.PDU_CLLST_ONLINE || linkStatus == PduStatus.PDU_CLLST_COMM_STARTED)
            //{
            //    Vci.SysLevel.Nwa.PduDisconnect(ModuleHandle, ComLogicalLinkHandle);
            //}
        }

        public async Task DisconnectAsync(CancellationToken ct)
        {
            var linkStatus = PduStatus.PDU_CLLST_OFFLINE;
            try
            {
                linkStatus = Status().Status;
            }
            catch (Iso22900IIException)
            {
                _logger.LogWarning("The ComLogicalLink can no longer be reached.This could be normal if a VCI was lost previously.");
            }

            if (linkStatus == PduStatus.PDU_CLLST_ONLINE || linkStatus == PduStatus.PDU_CLLST_COMM_STARTED)
            {
                Vci.SysLevel.Nwa.PduDisconnect(ModuleHandle, ComLogicalLinkHandle);

                while ( await TraceCllEvent.Reader.WaitToReadAsync(ct).ConfigureAwait(false) )
                {
                    if ( TraceCllEvent.Reader.TryRead(out var item) )
                    {
                        if ( item.PduItemType == PduIt.PDU_IT_STATUS )
                        {
                            //all status infos are not put into the queue.
                            if ( ((PduEventItemStatus)item).PduStatus == PduStatus.PDU_CLLST_OFFLINE)
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }

        public PduComParam GetComParam(string name)
        {
            var cpId = Vci.SysLevel.Nwa.PduGetObjectId(PduObjt.PDU_OBJT_COMPARAM, name);
            var cp = Vci.SysLevel.Nwa.PduGetComParam(ModuleHandle, ComLogicalLinkHandle, cpId);
            cp.ComParamShortName = name;
            return cp;
        }


        public PduComParam SetComParamValueViaGet(string name, long value)
        {
            var cpId = Vci.SysLevel.Nwa.PduGetObjectId(PduObjt.PDU_OBJT_COMPARAM, name);

            if ( cpId != PduConst.PDU_ID_UNDEF )
            {
                var cp = Vci.SysLevel.Nwa.PduSetViaGetComParam(ModuleHandle, ComLogicalLinkHandle, cpId, value);
                cp.ComParamShortName = name;
                return cp;
            }

            return null;
        }

        public PduComParam SetComParamValueViaGet(string name, byte[] value)
        {
            var cpId = Vci.SysLevel.Nwa.PduGetObjectId(PduObjt.PDU_OBJT_COMPARAM, name);

            if ( cpId != PduConst.PDU_ID_UNDEF )
            {
                var cp = Vci.SysLevel.Nwa.PduSetViaGetComParam(ModuleHandle, ComLogicalLinkHandle, cpId, value);
                cp.ComParamShortName = name;
                return cp;
            }

            return null;
        }

        public PduComParam SetComParamValueViaGet(string name, uint[] value)
        {
            var cpId = Vci.SysLevel.Nwa.PduGetObjectId(PduObjt.PDU_OBJT_COMPARAM, name);

            if ( cpId != PduConst.PDU_ID_UNDEF )
            {
                var cp = Vci.SysLevel.Nwa.PduSetViaGetComParam(ModuleHandle, ComLogicalLinkHandle, cpId, value);
                cp.ComParamShortName = name;
                return cp;
            }

            return null;
        }

        public PduComParam SetComParamValueViaGet(string name, PduParamStructFieldData value)
        {
            var cpId = Vci.SysLevel.Nwa.PduGetObjectId(PduObjt.PDU_OBJT_COMPARAM, name);

            if ( cpId != PduConst.PDU_ID_UNDEF )
            {
                var cp = Vci.SysLevel.Nwa.PduSetViaGetComParam(ModuleHandle, ComLogicalLinkHandle, cpId, value);
                cp.ComParamShortName = name;
                return cp;
            }

            return null;
        }

        public void SetComParam(PduComParam cp)
        {
            try
            {
                var cpId = Vci.SysLevel.Nwa.PduGetObjectId(PduObjt.PDU_OBJT_COMPARAM, cp.ComParamShortName);
                if ( cpId == PduConst.PDU_ID_UNDEF )
                {
                    _logger.LogInformation("Object-Id for ComParam {ComParamShortName} not defined in the API used", cp.ComParamShortName);
                    return;
                }

                cp.ComParamId = cpId;
                Vci.SysLevel.Nwa.PduSetComParam(ModuleHandle, ComLogicalLinkHandle, cp);
            }
            catch ( Iso22900IIException e )
            {
                if ( e.PduError == PduError.PDU_ERR_INVALID_PARAMETERS )
                {
                    //Unfortunately, Actia, for example, uses this method if the Object ID cannot be determined
                    //It would be better to return //cpId = PduConst.PDU_ID_UNDEF
                    _logger.LogWarning(e, "Trouble with ComParam {ComParamShortName} not supported Object-Id", cp.ComParamShortName);
                    return;
                }

                if ( e.PduError == PduError.PDU_ERR_COMPARAM_NOT_SUPPORTED )
                {
                    _logger.LogWarning("ComParam {ComParamShortName} not supported (doesn't have to be bad)", cp.ComParamShortName);
                    return;
                }

                throw;
            }
        }

        /// <summary>
        /// For those who don't want to register a logger.
        /// But still want to know if something goes wrong when setting the ComParam.
        /// </summary>
        /// <param name="cp">PduComParam</param>
        /// <returns>true or false</returns>
        public bool TrySetComParam(PduComParam cp)
        {
            try
            {
                var cpId = Vci.SysLevel.Nwa.PduGetObjectId(PduObjt.PDU_OBJT_COMPARAM, cp.ComParamShortName);
                if (cpId == PduConst.PDU_ID_UNDEF)
                {
                    _logger.LogInformation("Object-Id for ComParam {ComParamShortName} not defined in the API used", cp.ComParamShortName);
                    return false;
                }

                cp.ComParamId = cpId;
                Vci.SysLevel.Nwa.PduSetComParam(ModuleHandle, ComLogicalLinkHandle, cp);
            }
            catch (Iso22900IIException ex)
            {
                _logger.LogWarning(ex,"Trouble with ComParam {ComParamShortName} ", cp.ComParamShortName);
                return false;
            }
            return true;
        }

        /// <summary>
        ///     Get single communication parameter value from a page.
        /// </summary>
        /// <param name="uniqueRespIdentifier">
        ///     UniqueRespIdTable page identifier (You can think of it as the page/worksheet "name" of a
        ///     worksheet in excel)
        ///     default is PduConst.PDU_ID_UNDEF but some APIs have problems when using the default value
        /// </param>
        /// <param name="name">Name of the communication parameter.</param>
        public uint GetUniqueIdComParamValue(uint uniqueRespIdentifier, string name)
        {
            var cpId = Vci.SysLevel.Nwa.PduGetObjectId(PduObjt.PDU_OBJT_COMPARAM, name);

            List<PduEcuUniqueRespData> currentPages;
            currentPages = Vci.SysLevel.Nwa.PduGetUniqueRespIdTable(ModuleHandle, ComLogicalLinkHandle);

            var page = currentPages.Single(data => data.UniqueRespIdentifier.Equals(uniqueRespIdentifier));

            foreach ( var pduComParam in page.ComParams )
            {
                if ( pduComParam.ComParamId == cpId )
                {
                    return (pduComParam as PduComParamOfTypeUint)!.ComParamData;
                }
            }

            return 0;
        }


        /// <summary>
        ///     Set a single communication parameter from a single page.
        /// </summary>
        /// <param name="uniqueRespIdentifier">
        ///     UniqueRespIdTable page identifier (You can think of it as the page (worksheet tab) "name" of a
        ///     worksheet in excel)
        ///     default is PduConst.PDU_ID_UNDEF but some APIs have problems when using the default value
        /// </param>
        /// <param name="name">Name of the communication parameter.</param>
        /// <param name="value">New communication parameter value.</param>
        public PduComParam SetUniqueIdComParamValue(uint uniqueRespIdentifier, string name, long value)
        {
            PduComParam returnPduComParam = null;

            var cpId = Vci.SysLevel.Nwa.PduGetObjectId(PduObjt.PDU_OBJT_COMPARAM, name);

            if ( cpId != PduConst.PDU_ID_UNDEF )
            {
                List<PduEcuUniqueRespData> currentPages;
                currentPages = Vci.SysLevel.Nwa.PduGetUniqueRespIdTable(ModuleHandle, ComLogicalLinkHandle);

                var page = currentPages.Single(data => data.UniqueRespIdentifier.Equals(uniqueRespIdentifier));

                foreach ( var cp in page.ComParams )
                {
                    if ( cp.ComParamId == cpId )
                    {
                        cp.UpdateData(value);
                        cp.ComParamShortName = name;
                        returnPduComParam = cp;
                    }
                }

                Vci.SysLevel.Nwa.PduSetUniqueRespIdTable(ModuleHandle, ComLogicalLinkHandle, currentPages);
            }

            return returnPduComParam;
        }

        /// <summary>
        ///     Set communication parameters from a single page.
        /// </summary>
        /// <param name="uniqueRespIdentifier">
        ///     UniqueRespIdTable page identifier (You can think of it as the page/worksheet "name" of a
        ///     worksheet in excel)
        ///     default is PduConst.PDU_ID_UNDEF but some APIs have problems when using the default value
        /// </param>
        /// <param name="listComParamNameToValuePairs">List with pairs of ComParamName to Value </param>
        public List<PduComParam> SetUniqueIdComParamValues(uint uniqueRespIdentifier, List<KeyValuePair<string, uint>> listComParamNameToValuePairs)
        {
            List<PduComParam> returnPduComParamList = new List<PduComParam>();

            var listComParamIdAndComParamNameAndValuePairs = new List<KeyValuePair<uint, KeyValuePair<string, uint>>>(listComParamNameToValuePairs.Count);

            foreach ( var pair in listComParamNameToValuePairs )
            {
                var cpId = Vci.SysLevel.Nwa.PduGetObjectId(PduObjt.PDU_OBJT_COMPARAM, pair.Key);
                if ( cpId != PduConst.PDU_ID_UNDEF )
                {
                    listComParamIdAndComParamNameAndValuePairs.Add(new KeyValuePair<uint, KeyValuePair<string, uint>>(cpId, pair));
                }
            }

            if (listComParamIdAndComParamNameAndValuePairs.Count > 0 )
            {
                var currentPages = Vci.SysLevel.Nwa.PduGetUniqueRespIdTable(ModuleHandle, ComLogicalLinkHandle);

                var page = currentPages.Single(data => data.UniqueRespIdentifier.Equals(uniqueRespIdentifier));

                foreach ( var cp in page.ComParams )
                {
                    foreach ( var pair in listComParamIdAndComParamNameAndValuePairs)
                    {
                        if ( cp.ComParamId == pair.Key )
                        {
                            cp.UpdateData(pair.Value.Value);
                            cp.ComParamShortName = pair.Value.Key;
                            returnPduComParamList.Add(cp);
                        }
                    }
                }

                Vci.SysLevel.Nwa.PduSetUniqueRespIdTable(ModuleHandle, ComLogicalLinkHandle, currentPages);
            }

            return returnPduComParamList;
        }

        /// <summary>
        ///     By default a ComLogicalLink has one page with PduEcuUniqueComParams.
        ///     Only with this function it is possible to add further pages.
        ///     Which is essentially only needed for functional communication.
        /// </summary>
        /// <param name="ecuUniqueRespDatas"></param>
        public void SetUniqueRespIdTable(List<PduEcuUniqueRespData> ecuUniqueRespDatas)
        {
            foreach ( var pduEcuUniqueRespData in ecuUniqueRespDatas )
            foreach ( var pduComParam in pduEcuUniqueRespData.ComParams )
            {
                var cpId = Vci.SysLevel.Nwa.PduGetObjectId(PduObjt.PDU_OBJT_COMPARAM, pduComParam.ComParamShortName);
                pduComParam.ComParamId = cpId;
            }

            Vci.SysLevel.Nwa.PduSetUniqueRespIdTable(ModuleHandle, ComLogicalLinkHandle, ecuUniqueRespDatas);
        }

        public void CloneUniqueRespIdTablePageOneWithNewUniqueRespIdentifier(uint uniqueRespIdentifier)
        {
            List<PduEcuUniqueRespData> currentPages;
            currentPages = Vci.SysLevel.Nwa.PduGetUniqueRespIdTable(ModuleHandle, ComLogicalLinkHandle);

            if (currentPages.Any())
            {
                currentPages.Add(currentPages.First().Clone());
                currentPages.Last().UniqueRespIdentifier = uniqueRespIdentifier;
            }

            Vci.SysLevel.Nwa.PduSetUniqueRespIdTable(ModuleHandle, ComLogicalLinkHandle, currentPages);
        }

        public void DeleteAllPagesExceptPageOne()
        {
            List<PduEcuUniqueRespData> currentPages;
            currentPages = Vci.SysLevel.Nwa.PduGetUniqueRespIdTable(ModuleHandle, ComLogicalLinkHandle);

            if (currentPages.Any())
            {
                var temp = currentPages.First();
                currentPages.Clear();
                currentPages.Add(temp);
            }

            Vci.SysLevel.Nwa.PduSetUniqueRespIdTable(ModuleHandle, ComLogicalLinkHandle, currentPages);
        }

        public void SetUniqueRespIdTablePageOneUniqueRespIdentifier(uint uniqueRespIdentifier)
        {
            List<PduEcuUniqueRespData> currentPages;
            currentPages = Vci.SysLevel.Nwa.PduGetUniqueRespIdTable(ModuleHandle, ComLogicalLinkHandle);

            if ( currentPages.Any() )
            {
                currentPages.First().UniqueRespIdentifier = uniqueRespIdentifier;
            }

            Vci.SysLevel.Nwa.PduSetUniqueRespIdTable(ModuleHandle, ComLogicalLinkHandle, currentPages);
        }

        public uint GetUniqueRespIdTableNumberOfPages()
        {
            return (uint)Vci.SysLevel.Nwa.PduGetUniqueRespIdTable(ModuleHandle, ComLogicalLinkHandle).Count;
        }

        public uint GetPageUniqueRespIdentifier(uint pageIndex)
        {
            List<PduEcuUniqueRespData> currentPages;
            currentPages = Vci.SysLevel.Nwa.PduGetUniqueRespIdTable(ModuleHandle, ComLogicalLinkHandle);

            try
            {
                return currentPages[(int)pageIndex].UniqueRespIdentifier;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                _logger.LogCritical(ex, "Page with index {pageIndex} does not exist!", pageIndex);
                throw new DiagPduApiException($"Page with index {pageIndex} does not exist!");
            }

        }

        #region PduIoControlsOnComLogicalLinkLevel

        /// <summary>
        /// MeasureBatteryVoltage is a convenience function on cll
        /// Normally this is provided by the module
        /// </summary>
        /// <returns></returns>
        public uint MeasureBatteryVoltage()
        {
            return Vci.MeasureBatteryVoltage();
        }

        /// <summary>
        /// IsIgnitionOn is a convenience function on cll
        /// Normally this is provided by the module
        /// </summary>
        /// <param name="dlcPinNumber"></param>
        /// <returns></returns>
        public bool IsIgnitionOn(byte dlcPinNumber = 0)
        {
            return Vci.IsIgnitionOn(dlcPinNumber);
        }

        /// <summary>
        ///     For IoCtl which takes only the name and as parameter
        /// </summary>
        /// <param name="ioCtlShortName"></param>
        /// <returns>true or false</returns>
        internal bool TryIoCtlGeneral(string ioCtlShortName)
        {
            try
            {
                var ioCtlCommandId = Vci.SysLevel.Nwa.PduGetObjectId(PduObjt.PDU_OBJT_IO_CTRL, ioCtlShortName);
                if (!ioCtlCommandId.Equals(PduConst.PDU_ID_UNDEF))
                {
                    Vci.SysLevel.Nwa.PduIoCtl(ModuleHandle, ComLogicalLinkHandle, ioCtlCommandId, null);
                    return true;
                }
            }
            catch (Iso22900IIException e)
            {
                _logger.LogWarning(e, ioCtlShortName);
            }

            return false;
        }

        /// <summary>
        ///     For IoCtl which takes the name and a uint as parameters
        /// </summary>
        /// <param name="ioCtlShortName"></param>
        /// <param name="value"></param>
        /// <returns>true or false</returns>
        internal bool TryIoCtlGeneral(string ioCtlShortName, uint value)
        {
            try
            {
                var ioCtlCommandId = Vci.SysLevel.Nwa.PduGetObjectId(PduObjt.PDU_OBJT_IO_CTRL, ioCtlShortName);
                if (!ioCtlCommandId.Equals(PduConst.PDU_ID_UNDEF))
                {
                    Vci.SysLevel.Nwa.PduIoCtl(ModuleHandle, ComLogicalLinkHandle, ioCtlCommandId, new PduIoCtlOfTypeUint(value));
                    return true;
                }
            }
            catch (Iso22900IIException e)
            {
                _logger.LogWarning(e, ioCtlShortName);
            }

            return false;
        }


        /// <summary>
        ///     For IoCtl which takes the name and a PduIoCtlOfTypeFilterList as parameters
        /// </summary>
        /// <param name="ioCtlShortName"></param>
        /// <param name="value"></param>
        /// <returns>true or false</returns>
        internal bool TryIoCtlGeneral(string ioCtlShortName, PduIoCtlOfTypeFilterList value)
        {
            try
            {
                var ioCtlCommandId = Vci.SysLevel.Nwa.PduGetObjectId(PduObjt.PDU_OBJT_IO_CTRL, ioCtlShortName);
                if (!ioCtlCommandId.Equals(PduConst.PDU_ID_UNDEF))
                {
                    Vci.SysLevel.Nwa.PduIoCtl(ModuleHandle, ComLogicalLinkHandle, ioCtlCommandId, value);
                    return true;
                }
            }
            catch (Iso22900IIException e)
            {
                _logger.LogWarning(e, ioCtlShortName);
            }

            return false;
        }

        /// <summary>
        ///     For IoCtl which takes the name and uint as out
        /// </summary>
        /// <param name="ioCtlShortName"></param>
        /// <param name="value">a uint</param>
        /// <returns>true or false</returns>
        internal bool TryIoCtlGeneral(string ioCtlShortName, out uint value)
        {
            try
            {
                var ioCtlCommandId = Vci.SysLevel.Nwa.PduGetObjectId(PduObjt.PDU_OBJT_IO_CTRL, ioCtlShortName);
                if (!ioCtlCommandId.Equals(PduConst.PDU_ID_UNDEF))
                {
                    value = ((PduIoCtlOfTypeUint)Vci.SysLevel.Nwa.PduIoCtl(ModuleHandle, ComLogicalLinkHandle, ioCtlCommandId, null)).Value;
                    return true;
                }
            }
            catch (Iso22900IIException e)
            {
                _logger.LogWarning(e, ioCtlShortName);
            }

            value = default;
            return false;
        }



        /// <summary>
        ///     For IoCtl which takes the name as parameters and byteField as out
        /// </summary>
        /// <param name="ioCtlShortName"></param>
        /// <param name="value">When the method returns, contains the output byte array if successful; otherwise, an empty array.</param>
        /// <returns>true or false</returns>
        internal bool TryIoCtlGeneral(string ioCtlShortName, out byte[] value)
        {
            try
            {
                var ioCtlCommandId = Vci.SysLevel.Nwa.PduGetObjectId(PduObjt.PDU_OBJT_IO_CTRL, ioCtlShortName);
                if (!ioCtlCommandId.Equals(PduConst.PDU_ID_UNDEF))
                {
                    value = ((PduIoCtlOfTypeByteField)Vci.SysLevel.Nwa.PduIoCtl(ModuleHandle, ComLogicalLinkHandle, ioCtlCommandId, null)).Value;
                    return true;
                }
            }
            catch (Iso22900IIException e)
            {
                _logger.LogWarning(e, ioCtlShortName);
            }

            value = [];
            return false;
        }

        #endregion

        public PduExStatusData Status()
        {
            return Vci.SysLevel.Nwa.PduGetStatus(ModuleHandle, ComLogicalLinkHandle, PduConst.PDU_HANDLE_UNDEF);
        }

        internal PduExLastErrorData LastError()
        {
            return Vci.SysLevel.Nwa.PduGetLastError(ModuleHandle, ComLogicalLinkHandle);
        }

        protected void OnDataLost(CallbackEventArgs eventArgs)
        {
            DataLost?.Invoke(this, eventArgs);
        }

        protected void CallbackDataLost(CallbackEventArgs eventArgs)
        {
            //ToDo... if needed code that influences the behavior of this class
            OnDataLost(eventArgs);
        }

        protected void OnPduEventItemReceived(PduEventItem pduEventItem)
        {
            EventFired?.Invoke(this, pduEventItem);
        }

        protected void CallbackPduEventItemReceived(PduEventItem item)
        {
            if ( item.CopHandle == PduConst.PDU_HANDLE_UNDEF )
            {
                //Event is CLL event
                OnPduEventItemReceived(item);
                TraceCllEvent.Writer.TryWrite(item);
            }
            else
            {
                //Event is COP event
                if (!AddItemToMatchingChannel(item))
                {
                    //the event from the native code was faster than
                    //creating the ComPrimitive instance in the managed code
                    lock (_syncStartCop)
                    {
                        AddItemToMatchingChannel(item);
                    }
                }
                OnPduEventItemReceived(item);
            }

            //Local function
            bool AddItemToMatchingChannel(PduEventItem item)
            {
                if (CopChannels.TryGetValue(item.CopHandle, out var channel))
                {
                    channel.TryWrite(item);
                    if (item is PduEventItemStatus status)
                    {
                        if (status.PduStatus == PduStatus.PDU_COPST_FINISHED || status.PduStatus == PduStatus.PDU_COPST_CANCELLED)
                        {
                            channel.TryComplete();
                            // Remove the key (old handle) from the dictionary
                            CopChannels.TryRemove(item.CopHandle, out _);
                        }
                    }
                    return true;
                }
                return false;
            }
        }

        #region DisposeBehavior

        protected override void FreeUnmanagedResources()
        {
            TraceCllEvent.Writer.Complete();
            //First of all, we carefully ask what the VCI is doing
            //No exception is to be expected here
            var statusVci = Vci.Status().Status;

            //Now we ask for the status of the ComLogicalLink
            //That's where it falls apart. Some can do that. Other answers with an exception
            var statusCll = PduStatus.PDU_CLLST_OFFLINE;
            try
            {
                statusCll = Status().Status;
                //well:
                //- Softing
                //- Bosch
                //- Samtec
                //- Vector but with the wrong status (SW:V1.0 SP16)
            }
            catch (Iso22900IIException ex)
            {
                //bad:
                //- Actia
                _logger.LogWarning("Can't read the State of ComLogicalLink: {error}", ex.Message);
            }


            if ( (statusVci == PduStatus.PDU_MODST_AVAIL || statusVci == PduStatus.PDU_MODST_READY) &&
                 statusCll == PduStatus.PDU_CLLST_OFFLINE )
            {
                //the normal way if everything is going well (without VCI lost)

                //First the event because we need valid handles for native PduRegisterEventCallback function
                Vci.SysLevel.EventItemProvider.UnRegisterEventDataCallback(ModuleHandle, ComLogicalLinkHandle);
                Vci.SysLevel.Nwa.PduDestroyComLogicalLink(ModuleHandle, ComLogicalLinkHandle);
            }
            else if ( (statusVci == PduStatus.PDU_MODST_AVAIL || statusVci == PduStatus.PDU_MODST_READY) &&
                      (statusCll == PduStatus.PDU_CLLST_ONLINE || statusCll == PduStatus.PDU_CLLST_COMM_STARTED) )
            {
                //the the com logical link is left without performing a disconnect
                //(may have been forgotten or due to an exception within the com logical link using block)
                _logger.LogError("Forgot to put the ComLogicLink in the offline state with Disconnect()?");
                Vci.SysLevel.Nwa.PduDisconnect(ModuleHandle, ComLogicalLinkHandle);

                //First the event because we need valid handles for native PduRegisterEventCallback function
                Vci.SysLevel.EventItemProvider.UnRegisterEventDataCallback(ModuleHandle, ComLogicalLinkHandle);

                Vci.SysLevel.Nwa.PduDestroyComLogicalLink(ModuleHandle, ComLogicalLinkHandle);
            }
            else if ( (statusVci == PduStatus.PDU_MODST_NOT_AVAIL || statusVci == PduStatus.PDU_MODST_NOT_READY) &&
                      statusCll == PduStatus.PDU_CLLST_OFFLINE )
            {
                //that's the situation VCI lost
                //Now we have to be very tolerant because every manufacturer does it differently
                try
                {
                    //First the event because we need valid handles for native PduRegisterEventCallback function
                    Vci.SysLevel.EventItemProvider.UnRegisterEventDataCallback(ModuleHandle, ComLogicalLinkHandle);
                    //well:
                    //- Softing
                    //- Bosch
                    //- Samtec
                }
                catch ( Iso22900IIException ex )
                {
                    //bad:
                    //- Actia
                    _logger.LogWarning("Can't UnRegisterEventDataCallback: {error}", ex.Message);
                }

                try
                {
                    Vci.SysLevel.Nwa.PduDestroyComLogicalLink(ModuleHandle, ComLogicalLinkHandle);
                    //well:
                    //- Softing
                }
                catch ( Iso22900IIException ex )
                {
                    //bad:
                    //- Actia
                    //- Bosch
                    //- Samtec
                    _logger.LogWarning("Can't PduDestroyComLogicalLink: {error}", ex.Message);
                }
            }
            else if ( (statusVci == PduStatus.PDU_MODST_NOT_AVAIL || statusVci == PduStatus.PDU_MODST_NOT_READY) &&
                      statusCll != PduStatus.PDU_CLLST_OFFLINE )
            {
                //You can't believe it, but Vector's API creates this situation.
                //No VCI but a online CLL.. what's that supposed to mean?
                //the whole if block is for Vector
                try
                {
                    _logger.LogError("Forgot to put the ComLogicLink in the offline state with Disconnect() (Vector workaround)");
                    Vci.SysLevel.Nwa.PduDisconnect(ModuleHandle, ComLogicalLinkHandle);
                }
                catch ( Iso22900IIException ex )
                {
                    _logger.LogWarning("Can't PduDisconnect (Vector workaround): {error}", ex.Message);
                }

                try
                {
                    //First the event because we need valid handles for native PduRegisterEventCallback function
                    Vci.SysLevel.EventItemProvider.UnRegisterEventDataCallback(ModuleHandle, ComLogicalLinkHandle);
                }
                catch (Iso22900IIException ex)
                {
                    _logger.LogWarning("Can't UnRegisterEventDataCallback (Vector workaround): {error}", ex.Message);
                }

                try
                {
                    Vci.SysLevel.Nwa.PduDestroyComLogicalLink(ModuleHandle, ComLogicalLinkHandle);
                }
                catch (Iso22900IIException ex)
                {
                    _logger.LogWarning("Can't UnRegisterEventDataCallback (Vector workaround): {error}", ex.Message);
                }
                
            }

        }

        protected override void FreeManagedResources()
        {
            Vci.Disposing -= Dispose;

            if ( EventFired != null )
            {
                foreach ( var d in EventFired.GetInvocationList() )
                {
                    EventFired -= (EventHandler<PduEventItem>)d;
                }
            }

            //loop is not needed only when something goes wrong
            foreach ( var entry in CopChannels )
            {
                entry.Value.TryComplete();
            }

            CopChannels.Clear();
        }

        /// <summary>
        ///     When the finalizer thread gets around to running, it runs all the destructors of the object.
        ///     Destructors will run in order from most derived to least derived.
        /// </summary>
        ~ComLogicalLinkLevel()
        {
            try
            {
                Dispose(false);
            }
            catch ( Exception exception )
            {
                //This is bad.
                //At least attempt to get a log message out if this happens.
                try
                {
                    var builder = new StringBuilder();
                    builder.AppendLine($"{DateTime.Now} - Exception in type '{GetType()}'");
                    builder.Append(exception.StackTrace);
                    builder.Append(exception.Message);
                    var innerException = exception.InnerException;

                    while ( innerException != null )
                    {
                        builder.Append(innerException.Message);
                        innerException = innerException.InnerException;
                    }

                    File.AppendAllText(@"FinalizerException.txt", builder.ToString());
                }
                catch
                {
                } //Swallow any exceptions inside a finalizer
            }
        }

        #endregion
    }
}
