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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using ISO22900.II.Interface;
using Microsoft.Extensions.Logging;

namespace ISO22900.II
{
    /// <summary>
    ///     I'm implementing a wrapper class over the actual VCI binding.
    ///     The main reason why I exist is that in the event of an error (e.g. VCI lost)
    ///     I am able to replace the real VCI connection (which I wrap) with a new instance.
    ///     The application has an instance of me that doesn't change from the application point of view.
    /// </summary>
    public class ComLogicalLink : IDisposable, IComLogicalLink
    {
        private readonly ILogger _logger = ApiLibLogging.CreateLogger<ComLogicalLink>();
        private readonly string _busTypeName;
        private readonly string _protocolName;
        private readonly Dictionary<string, PduComParam> _cpParamBackupDic = new();
        private readonly List<KeyValuePair<uint, string>> _dlcPinToTypeNamePairs;
        private readonly PduFlagDataCllCreateFlag _cllCreateFlag;
        private readonly uint _resourceId;

        private readonly Module _module;
        
        private readonly PduCopCtrlData _readyToUseCopControl = new(new[]
        {
            new PduExpectedResponseData(PduExResponseType.PositiveResponse, 1,
                new MaskAndPatternBytes(new byte[] {}, new byte[] {}),
                new UniqueRespIds(new uint[] {}))
        });

        private readonly object _sync = new();
        private readonly Dictionary<uint, Dictionary<string, PduComParam>> ecuUniqueRespDatasBackup = new();

        private ComLogicalLinkLevel _cll;
        private bool _isComLogicalLinkConnected; //track the ComParams only if the cll is offline
        private bool _isPduStartComUsed;
        private byte[] _pduStartComCopData;
        private int _pduStartComReceiveCycles;
        private int _pduStartComSendCycles;
       


        private event EventHandler<CallbackEventArgs> BackingUpEventHandlerDataLost;

        private event EventHandler<PduEventItem> BackingUpEventHandlerEventFired;

        public event EventHandler<CallbackEventArgs> DataLost
        {
            add
            {
                lock ( _sync )
                {
                    _cll.DataLost += value;
                    BackingUpEventHandlerDataLost += value;
                }
            }

            remove
            {
                lock ( _sync )
                {
                    BackingUpEventHandlerDataLost -= value;
                    _cll.DataLost -= value;
                }
            }
        }

        public event EventHandler<PduEventItem> EventFired
        {
            add
            {
                lock ( _sync )
                {
                    _cll.EventFired += value;
                    BackingUpEventHandlerEventFired += value;
                }
            }

            remove
            {
                lock ( _sync )
                {
                    BackingUpEventHandlerEventFired -= value;
                    _cll.EventFired -= value;
                }
            }
        }

        internal ComLogicalLink(Module module, ComLogicalLinkLevel cll, string busTypeName, string protocolName,
            List<KeyValuePair<uint, string>> dlcPinToTypeNamePairs, PduFlagDataCllCreateFlag cllCreateFlag)
        {
            _module = module;
            _cll = cll;

            //Backup items which needed inside function TryToRecover
            //Todo should I deep copy all parameters? To ensure that no subsequent changes are made to the objects.
            _busTypeName = busTypeName;
            _protocolName = protocolName;
            _dlcPinToTypeNamePairs = dlcPinToTypeNamePairs;
            _cllCreateFlag = (PduFlagDataCllCreateFlag)cllCreateFlag.Clone();
            _resourceId = PduConst.PDU_ID_UNDEF;
        }

        internal ComLogicalLink(Module module, ComLogicalLinkLevel cll, uint resourceId, PduFlagDataCllCreateFlag cllCreateFlag)
        {
            _module = module;
            _cll = cll;

            //Backup items which needed inside function TryToRecover
            _busTypeName = String.Empty;
            _protocolName = String.Empty;
            _dlcPinToTypeNamePairs = null;
            _cllCreateFlag = (PduFlagDataCllCreateFlag)cllCreateFlag.Clone();
            _resourceId = resourceId;
        }

        public ComPrimitive StartCop(PduCopt pduCopType, TimeSpan delayTimeMs = default, uint copTag = 0)
        {
            lock ( _sync )
            {
                _readyToUseCopControl.Time = (uint)delayTimeMs.Milliseconds;
                //only that there is no irritation. It would also work with 1
                _readyToUseCopControl.NumSendCycles = 0;
                _readyToUseCopControl.NumReceiveCycles = 0;
                var backup = _readyToUseCopControl.PduExpectedResponseDatas;
                _readyToUseCopControl.PduExpectedResponseDatas = Array.Empty<PduExpectedResponseData>();
                try
                {
                    return StartCop(pduCopType, new byte[] {}, _readyToUseCopControl, copTag);
                }
                finally
                {
                    //Reset to defaults
                    _readyToUseCopControl.Time = 0;
                    _readyToUseCopControl.NumSendCycles = 1;
                    _readyToUseCopControl.NumReceiveCycles = 1;
                    _readyToUseCopControl.PduExpectedResponseDatas = backup;
                }
            }
        }

        public ComPrimitive StartCop(PduCopt pduCopType, byte[] copData, uint copTag = 0)
        {
            return StartCop(pduCopType, 1, 1, copData, copTag);
        }

        public ComPrimitive StartCop(PduCopt pduCopType, int sendCycles, int receiveCycles, byte[] copData, uint copTag = 0)
        {
            lock ( _sync )
            {
                _readyToUseCopControl.NumSendCycles = sendCycles;
                _readyToUseCopControl.NumReceiveCycles = receiveCycles;
                try
                {
                    return StartCop(pduCopType, copData, _readyToUseCopControl, copTag);
                }
                finally
                {
                    //Reset to defaults
                    _readyToUseCopControl.NumSendCycles = 1;
                    _readyToUseCopControl.NumReceiveCycles = 1;
                }
            }
        }

        public ComPrimitive StartCop(PduCopt pduCopType, byte[] copData, PduCopCtrlData copCtrlData, uint copTag = 0)
        {
            lock ( _sync )
            {
                var comPrimitiveLevel = _cll.StartCop(pduCopType, copData, copCtrlData, copTag);
                if ( pduCopType == PduCopt.PDU_COPT_STARTCOMM )
                {
                    //Backup the StartCom data 
                    _isPduStartComUsed = true;
                    _pduStartComSendCycles = copCtrlData.NumSendCycles;
                    _pduStartComReceiveCycles = copCtrlData.NumReceiveCycles;
                    _pduStartComCopData = copData;
                }

                return new ComPrimitive(this, comPrimitiveLevel, pduCopType, copData, copCtrlData, copTag);
            }
        }

        public void Connect()
        {
            lock ( _sync )
            {
                _cll.Connect();
                _isComLogicalLinkConnected = true;
            }
        }

        public void Disconnect()
        {
            lock ( _sync )
            {
                _isComLogicalLinkConnected = false;
                _cll.Disconnect();
            }
        }

        public PduComParam GetComParam(string name)
        {
            lock ( _sync )
            {
                return _cll.GetComParam(name);
            }
        }

        public PduComParam SetComParamValueViaGet(string name, long value)
        {
            lock ( _sync )
            {
                var retCp = _cll.SetComParamValueViaGet(name, value);
                if ( retCp != null )
                {
                    StoreComParam(retCp);
                }

                return retCp;
            }
        }

        public PduComParam SetComParamValueViaGet(string name, byte[] value)
        {
            lock ( _sync )
            {
                var retCp = _cll.SetComParamValueViaGet(name, value);
                if ( retCp != null )
                {
                    StoreComParam(retCp);
                }

                return retCp;
            }
        }

        public PduComParam SetComParamValueViaGet(string name, uint[] value)
        {
            lock ( _sync )
            {
                var retCp = _cll.SetComParamValueViaGet(name, value);
                if ( retCp != null )
                {
                    StoreComParam(retCp);
                }

                return retCp;
            }
        }

        public PduComParam SetComParamValueViaGet(string name, PduParamStructFieldData value)
        {
            lock ( _sync )
            {
                var retCp = _cll.SetComParamValueViaGet(name, value);
                if ( retCp != null )
                {
                    StoreComParam(retCp);
                }

                return retCp;
            }
        }

        public void SetComParam(PduComParam cp)
        {
            lock ( _sync )
            {
                _cll.SetComParam(cp);
                StoreComParam(cp);
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
            lock (_sync)
            {
                var check = _cll.TrySetComParam(cp);
                if ( check )
                {
                    StoreComParam(cp);
                }

                return check;
            }
        }

        private void StoreComParam(PduComParam cp)
        {
            //we no longer save ComParams when the CLL is connected (online).
            if ( !_isComLogicalLinkConnected )
            {
                if ( !_cpParamBackupDic.TryAdd(cp.ComParamShortName, cp) )
                {
                    _cpParamBackupDic[cp.ComParamShortName] = cp;
                }
            }
        }

        public uint GetUniqueIdComParamValue(uint uniqueRespIdentifier, string name)
        {
            lock (_sync)
            {
                return _cll.GetUniqueIdComParamValue(uniqueRespIdentifier, name);
            }
        }


        public void SetUniqueIdComParamValue(uint uniqueRespIdentifier, string name, long value)
        {
            lock ( _sync )
            {
                var retCp = _cll.SetUniqueIdComParamValue(uniqueRespIdentifier, name, value);
                if ( retCp != null )
                {
                    if ( !_isComLogicalLinkConnected )
                    {
                        if ( ecuUniqueRespDatasBackup.ContainsKey(uniqueRespIdentifier) )
                        {
                            if ( !ecuUniqueRespDatasBackup[uniqueRespIdentifier].TryAdd(retCp.ComParamShortName, retCp) )
                            {
                                ecuUniqueRespDatasBackup[uniqueRespIdentifier][retCp.ComParamShortName] = retCp;
                            }
                        }
                        else
                        {
                            ecuUniqueRespDatasBackup.Add(uniqueRespIdentifier, new Dictionary<string, PduComParam>
                            {
                                { retCp.ComParamShortName, retCp }
                            });
                        }
                    }
                }
            }
        }

        public void SetUniqueIdComParamValues(uint uniqueRespIdentifier, List<KeyValuePair<string, uint>> listComParamNameToValuePairs)
        {
            lock ( _sync )
            {
                var retCpList = _cll.SetUniqueIdComParamValues(uniqueRespIdentifier, listComParamNameToValuePairs);
                if ( retCpList.Any() )
                {
                    if ( !_isComLogicalLinkConnected )
                    {
                        foreach ( var retCp in retCpList )
                        {
                            if ( !ecuUniqueRespDatasBackup[uniqueRespIdentifier].TryAdd(retCp.ComParamShortName, retCp) )
                            {
                                if (ecuUniqueRespDatasBackup.ContainsKey(uniqueRespIdentifier))
                                {
                                    if (!ecuUniqueRespDatasBackup[uniqueRespIdentifier].TryAdd(retCp.ComParamShortName, retCp))
                                    {
                                        ecuUniqueRespDatasBackup[uniqueRespIdentifier][retCp.ComParamShortName] = retCp;
                                    }
                                }
                                else
                                {
                                    ecuUniqueRespDatasBackup.Add(uniqueRespIdentifier, new Dictionary<string, PduComParam>
                                    {
                                        { retCp.ComParamShortName, retCp }
                                    });
                                }
                            }
                        }
                    }
                }
            }
        }

        public void SetUniqueRespIdTable(List<PduEcuUniqueRespData> ecuUniqueRespDatas)
        {
            lock ( _sync )
            {
                _cll.SetUniqueRespIdTable(ecuUniqueRespDatas);

                //we no longer save ComParams when the CLL is connected (online)
                if ( !_isComLogicalLinkConnected )
                {
                    foreach ( var ecuUniqueRespData in ecuUniqueRespDatas )
                    {
                        if ( ecuUniqueRespDatasBackup.ContainsKey(ecuUniqueRespData.UniqueRespIdentifier) )
                        {
                            StoreComParamsFromPduEcuUniqueRespData(ecuUniqueRespData);
                        }
                        else
                        {
                            ecuUniqueRespDatasBackup.Add(ecuUniqueRespData.UniqueRespIdentifier, new Dictionary<string, PduComParam>());
                            StoreComParamsFromPduEcuUniqueRespData(ecuUniqueRespData);
                        }
                    }
                }
            }
        }

        public void SetUniqueRespIdTablePageOneUniqueRespIdentifier(uint uniqueRespIdentifier)
        {
            lock ( _sync )
            {
                _cll.SetUniqueRespIdTablePageOneUniqueRespIdentifier(uniqueRespIdentifier);
            }
        }



        /// <summary>
        /// returns the number of the current pages
        /// usually in preparation for the function GetPageUniqueRespIdentifier
        /// </summary>
        /// <returns></returns>
        public uint GetUniqueRespIdTableNumberOfPages()
        {
            lock (_sync)
            {
                return _cll.GetUniqueRespIdTableNumberOfPages();
            }
        }


        /// <summary>
        /// returns the UniqueRespIdentifier from page one
        /// actually there should always be at least one page
        /// but I think there are also wrong implementations where you can delete all pages. Then you have to be careful with this function
        /// </summary>
        /// <returns></returns>
        public uint GetPageOneUniqueRespIdentifier()
        {
            return this.GetPageUniqueRespIdentifier(0);
        }

        /// <summary>
        /// returns the UniqueRespIdentifier from page with index x
        /// use GetUniqueRespIdTableNumberOfPages to see how many there are
        /// </summary>
        /// <param name="pageIndex">page index</param>
        /// <returns></returns>
        public uint GetPageUniqueRespIdentifier(uint pageIndex)
        {
            lock (_sync)
            {
                return _cll.GetPageUniqueRespIdentifier(pageIndex);
            }
        }

        private void StoreComParamsFromPduEcuUniqueRespData(PduEcuUniqueRespData ecuUniqueRespData)
        {
            foreach ( var pduComParam in ecuUniqueRespData.ComParams )
            {
                if ( !ecuUniqueRespDatasBackup[ecuUniqueRespData.UniqueRespIdentifier].TryAdd(pduComParam.ComParamShortName, pduComParam) )
                {
                    ecuUniqueRespDatasBackup[ecuUniqueRespData.UniqueRespIdentifier][pduComParam.ComParamShortName] = pduComParam;
                }
            }
        }

        public uint MeasureBatteryVoltage()
        {
            lock ( _sync )
            {
                return _cll.MeasureBatteryVoltage();
            }
        }

        public bool IsIgnitionOn(byte dlcPinNumber = 0)
        {
            lock ( _sync )
            {
                return _cll.IsIgnitionOn(dlcPinNumber);
            }
        }

        public PduExStatusData Status()
        {
            lock ( _sync )
            {
                return _cll.Status();
            }
        }

        /// <summary>
        /// Unfortunately, the function does not work very well.
        /// Because some manufacturers of the D-PDU API have not implemented it well. So please don't use it.
        /// The reason method is here anyway is because some users of the ApiOne use ApiOne to verify D-PDU API's (before they buy).
        /// </summary>
        /// <returns>PduExLastErrorData</returns>
        [Obsolete("Method is only for VCI evaluation. Do not use it in real projects. The result is not reliable.")]
        public PduExLastErrorData LastError()
        {
            lock (_sync)
            {
                return _cll.LastError();
            }
        }

        /// <summary>
        ///     You can use this method if you want to try something
        ///     For IoCtl where the name is the only one parameter
        ///     E.g. API for manufacturer specific things
        ///     For real application prefer to use the methods that call this method with the appropriate parameter
        /// </summary>
        /// <param name="ioCtlShortName"></param>
        /// <returns>true or false</returns>
        public bool TryIoCtlGeneral(string ioCtlShortName)
        {
            lock (_sync)
            {
                return _cll.TryIoCtlGeneral(ioCtlShortName);
            }
        }

        /// <summary>
        /// Usually you don't need to change anything with this method
        /// </summary>
        /// <returns></returns>
        public bool TryIoCtlClearTxQueue() => TryIoCtlGeneral("PDU_IOCTL_CLEAR_TX_QUEUE");

        /// <summary>
        /// Usually you don't need to change anything with this method
        /// </summary>
        /// <returns></returns>
        public bool TryIoCtlSuspendTxQueue() => TryIoCtlGeneral("PDU_IOCTL_SUSPEND_TX_QUEUE");

        /// <summary>
        /// Usually you don't need to change anything with this method
        /// </summary>
        /// <returns></returns>
        public bool TryIoCtlResumeTxQueue() => TryIoCtlGeneral("PDU_IOCTL_RESUME_TX_QUEUE");

        /// <summary>
        /// Usually you don't need to change anything with this method
        /// </summary>
        /// <returns></returns>
        public bool TryIoCtlClearRxQueue() => TryIoCtlGeneral("PDU_IOCTL_CLEAR_RX_QUEUE");

        /// <summary>
        /// Usually you don't need to change anything with this method
        /// </summary>
        /// <returns></returns>
        public bool TryIoCtlClearMsgFilter() => TryIoCtlGeneral("PDU_IOCTL_CLEAR_MSG_FILTER");

        /// <summary>
        /// For SAE J1850 VPW
        /// Usually you don't need to change anything with this method
        /// </summary>
        /// <returns></returns>
        public bool TryIoCtlSendBreak() => TryIoCtlGeneral("PDU_IOCTL_SEND_BREAK");


        /// <summary>
        ///     You can use this method if you want to try something
        ///     For IoCtl which takes the name and a uint as parameters
        ///     E.g. API for manufacturer specific things
        ///     For real application prefer to use the methods that call this method with the appropriate parameter
        /// </summary>
        /// <param name="ioCtlShortName"></param>
        /// <param name="value"></param>
        /// <returns>true or false</returns>
        internal bool TryIoCtlGeneral(string ioCtlShortName, uint value)
        {
            lock (_sync)
            {
                return _cll.TryIoCtlGeneral(ioCtlShortName, value);
            }
        }

        /// <summary>
        /// Sets the maximum buffer size of the received PDU on a ComLogicalLink
        /// Usually you don't need to change anything with this method
        /// </summary>
        /// <param name="value">maximum sizeof a received PDU for the ComLogicalLink</param>
        /// <returns>true or false</returns>
        public bool TryIoCtlSetBufferSize(uint value) => TryIoCtlGeneral("PDU_IOCTL_SET_BUFFER_SIZE", value);


        /// <summary>
        /// Stops the specified filter, based on filter number
        /// Usually you don't need to change anything with this method
        /// </summary>
        /// <param name="value">Filter number to stop</param>
        /// <returns>true or false</returns>
        public bool TryIoCtlStopMsgFilter(uint value) => TryIoCtlGeneral("PDU_IOCTL_STOP_MSG_FILTER", value);


        /// <summary>
        ///  Starts filtering of incoming messages for the specified ComLogicalLink.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryIoCtlStartMsgFilter(PduIoCtlOfTypeFilterList value)
        {
            lock (_sync)
            {
                return _cll.TryIoCtlGeneral("PDU_IOCTL_START_MSG_FILTER", value);
            }
        }


        /// <summary>
        ///     Attempts to restore the status of the ComLogicalLink
        ///     Catch all "Iso22900IIException" exceptions
        /// </summary>
        /// <returns></returns>
        public bool TryToRecover(out string exMessage)
        {
            //If a VCI is lost, the individual D-PDU-APIs behave completely differently...
            //and only few manufacturers as described in ISO22900 - 2

            lock ( _sync )
            {
                //First of all, we carefully ask what the VCI is doing
                //No exception is to be expected here
                var statusVci = _cll.Vci.Status().Status;

                //Now we ask for the status of the ComLogicalLink
                //That's where it falls apart. Some can do that. Other answers with an exception
                var statusCll = PduStatus.PDU_CLLST_OFFLINE;
                try
                {
                    statusCll = _cll.Status().Status;
                    //well:
                    //- Softing
                    //- Bosch
                    //- Samtec
                    //- Vector but with the wrong status (SW:V1.0 SP16)
                }
                catch ( Iso22900IIException ex )
                {
                    //bad:
                    //- Actia
                    _logger.LogWarning("Can't read the State of ComLogicalLink: {error}", ex.Message);
                }

                if ( (statusVci == PduStatus.PDU_MODST_NOT_AVAIL || statusVci == PduStatus.PDU_MODST_NOT_READY) &&
                     statusCll != PduStatus.PDU_CLLST_OFFLINE )
                {
                    //You can't believe it, but Vector's API creates this situation.
                    //No VCI but a CLL.. what's that supposed to mean?
                    statusCll = PduStatus.PDU_CLLST_OFFLINE;
                }


                try
                {
                    if ( statusCll == PduStatus.PDU_CLLST_OFFLINE )
                    {
                        _cll.Dispose();

                        if ( !_module.TryToRecover(out var msg) )
                        {
                            exMessage = msg;
                            return false;
                        }

                        //this is where the magic happens.. the new instance is assigned under the hood

                        _cll = _resourceId == PduConst.PDU_ID_UNDEF ? _module.OpenComLogicalLink(_busTypeName, _protocolName, _dlcPinToTypeNamePairs, _cllCreateFlag)._cll : _module.OpenComLogicalLink(_resourceId, _cllCreateFlag)._cll;


                        if ( BackingUpEventHandlerEventFired != null )
                        {
                            foreach ( var d in BackingUpEventHandlerEventFired.GetInvocationList() )
                            {
                                _cll.EventFired += (EventHandler<PduEventItem>)d;
                            }
                        }

                        if ( BackingUpEventHandlerDataLost != null )
                        {
                            foreach ( var d in BackingUpEventHandlerDataLost.GetInvocationList() )
                            {
                                _cll.DataLost += (EventHandler<CallbackEventArgs>)d;
                            }
                        }

                        foreach ( var pair in _cpParamBackupDic )
                        {
                            _cll.SetComParam(pair.Value);
                        }

                        var ecuUniqueRespDatas = new List<PduEcuUniqueRespData>();
                        foreach ( var uniqueDataSet in ecuUniqueRespDatasBackup )
                        {
                            ecuUniqueRespDatas.Add(new PduEcuUniqueRespData(uniqueDataSet.Key,
                                uniqueDataSet.Value.Values.ToList()));
                        }

                        _cll.SetUniqueRespIdTable(ecuUniqueRespDatas);

                        _cll.Connect();
                        if ( _isPduStartComUsed )
                        {
                            var comPrim = StartCop(PduCopt.PDU_COPT_STARTCOMM, _pduStartComSendCycles, _pduStartComReceiveCycles, _pduStartComCopData);
                            var queue = comPrim.WaitForCopResult();
                        }

                        if ( _resourceId == PduConst.PDU_ID_UNDEF )
                        {
                            _logger.Log(LogLevel.Information, "ComLogicalLink recovering done for ComLogicalLink: {_stackName}",
                                _protocolName + "_on_" + _busTypeName);
                        }
                        else
                        {
                            _logger.Log(LogLevel.Information, "ComLogicalLink recovering done for ComLogicalLink: {_resourceId}", _resourceId);
                        }
                    }


                    if ( _resourceId == PduConst.PDU_ID_UNDEF )
                    {
                        _logger.Log(LogLevel.Information, "ComLogicalLink is back or no reason to recover for ComLogicalLink: {_stackName}",
                            _protocolName + "_on_" + _busTypeName);
                    }
                    else
                    {
                        _logger.Log(LogLevel.Information, "ComLogicalLink is back or no reason to recover for ComLogicalLink: {_resourceId}", _resourceId);
                    }
                }

                catch ( Iso22900IIExceptionBase ex )
                {
                    exMessage = ex.Message;
                    return false;
                }

                exMessage = string.Empty;
                return true;
            }
        }

        #region DisposeBehavior

        /// <summary>
        ///     Alternative to Dispose()
        ///     The function name is based on ISO22900-2. And can be used if NO using keyword is used
        /// </summary>
        public void DestroyComLogicalLink()
        {
            Dispose();
        }

        public void Dispose()
        {
            _isComLogicalLinkConnected = false;
            if ( BackingUpEventHandlerEventFired != null )
            {
                foreach ( var d in BackingUpEventHandlerEventFired.GetInvocationList() )
                {
                    EventFired -= (EventHandler<PduEventItem>)d;
                }
            }

            if ( BackingUpEventHandlerDataLost != null )
            {
                foreach ( var d in BackingUpEventHandlerDataLost.GetInvocationList() )
                {
                    DataLost -= (EventHandler<CallbackEventArgs>)d;
                }
            }

            _cll.Dispose();
        }

        /// <summary>
        ///     State of dispose more for testing
        /// </summary>
        internal bool IsDisposed
        {
            get
            {
                lock ( _sync )
                {
                    return _cll.IsDisposed;
                }
            }
        }

        #endregion
    }
}
