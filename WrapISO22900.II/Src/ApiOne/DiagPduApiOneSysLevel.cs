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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;

// ReSharper disable InconsistentlySynchronizedField

namespace ISO22900.II
{
    public class DiagPduApiOneSysLevel : ManagedDisposable
    {
        private readonly ILogger _logger = ApiLibLogging.CreateLogger<DiagPduApiOneSysLevel>();
        private readonly Dictionary<uint, Module> _vciCache = new();
        private readonly string _optionStrBackUp;

        internal readonly PduEventItemCallbackProvider EventItemProvider;

        protected internal static uint ComLogicalLinkHandle => PduConst.PDU_HANDLE_UNDEF;
        protected internal static uint ModuleHandle => PduConst.PDU_HANDLE_UNDEF;

        internal ApiModifications ApiModBitField { get; init; }
        internal Iso22900NativeWrapAccess Nwa { get; init; }

        public List<PduModuleData> PduModuleDataSets => Nwa.PduGetModuleIds();

        public event EventHandler<CallbackEventArgs> DataLost;
        public event EventHandler<PduEventItem> EventFired;


        internal DiagPduApiOneSysLevel(Iso22900NativeWrapAccess nwa, string optionStr, ApiModifications apiModFlags, Action<DiagPduApiOneSysLevel> postPduConstructHook)
        {
            Nwa = nwa;
            ApiModBitField = apiModFlags;
            _optionStrBackUp = optionStr;

            if ( string.IsNullOrWhiteSpace(optionStr) )
            {
                Nwa.PduConstruct();
            }
            else
            {
                _logger.LogInformation("OptionStr: {optionStr}", optionStr);
                Nwa.PduConstruct(optionStr);
            }

            postPduConstructHook?.Invoke(this);

            EventItemProvider = new PduEventItemCallbackProvider(Nwa);
            EventItemProvider.RegisterEventDataCallback(ModuleHandle, ComLogicalLinkHandle, CallbackPduEventItemReceived, CallbackDataLost);
        }


        /// <summary>
        ///     List of all VCI names that are currently available
        /// </summary>
        /// <returns>List of VCI Module names</returns>
        public List<string> NamesOfAvailableVciModules()
        {
            var vciShortNameList = new List<string>();

            var pduModuleDatas = Nwa.PduGetModuleIds();

            foreach ( var pduModuleData in pduModuleDatas )
            {
                if ( !string.IsNullOrWhiteSpace(pduModuleData.VendorModuleName) &&
                     pduModuleData.ModuleStatus != PduStatus.PDU_MODST_NOT_READY &&
                     pduModuleData.ModuleStatus != PduStatus.PDU_MODST_NOT_AVAIL )
                {
                    vciShortNameList.Add(pduModuleData.VendorModuleName);
                }
            }

            return vciShortNameList;
        }

        /// <summary>
        ///     ConnectVci
        /// </summary>
        /// <param name="vciModuleName">
        ///     Name of a VCI e.g. obtained from NamesOfAvailableVciModules() or
        ///     PduModuleData.VendorModuleName
        /// </param>
        /// <returns>Module</returns>
        public Module ConnectVci(string vciModuleName = "")
        {
            var moduleData = PduModuleDataFromVciModuleName(vciModuleName);

            lock ( _vciCache )
            {
                if ( !_vciCache.ContainsKey(moduleData.ModuleHandle) )
                {
                    Nwa.PduModuleConnect(moduleData.ModuleHandle);
                    var vci = new ModuleLevel(this, moduleData.ModuleHandle);
                    vci.Disposing += () =>
                    {
                        if ( !_vciCache.Remove(moduleData.ModuleHandle) )
                        {
                            //that should never happen
                            _logger.LogWarning("Remove ModuleHandle {ModuleHandle} more than once", moduleData.ModuleHandle);
                        }
                    };
                    _vciCache.Add(moduleData.ModuleHandle, new Module(this, vci, moduleData.VendorModuleName));
                    Disposing += vci.Dispose;
                }

                return _vciCache[moduleData.ModuleHandle];
            }
        }

        /// <summary>
        /// Get PduModuleData from Name or Exception if the module is unreachable
        /// Name is the hole VendorModuleName e.g.
        /// VendorName='ACTIA I+ME GmbH' ModuleName='VAS6154' SerialNumber='00000815' PersonalVciID='00' IP='192.168.13.69' ConnectionType='USB' Status='available'
        /// or
        /// VendorModuleName = "VendorName='samtec gmbh' VCIName='HS+ Interface' VCISerialNumber='04711'
        /// </summary>
        /// <param name="vciModuleName"></param>
        /// <returns></returns>
        /// <exception cref="DiagPduApiException"></exception>
        private PduModuleData PduModuleDataFromVciModuleName(string vciModuleName)
        {
            var pduModuleDatas = Nwa.PduGetModuleIds();
            PduModuleData moduleData;
            try
            {
                moduleData = string.IsNullOrWhiteSpace(vciModuleName)
                    ? pduModuleDatas.First()
                    : pduModuleDatas.First(pduModuleData => pduModuleData.VendorModuleName.Equals(vciModuleName));
            }
            catch ( InvalidOperationException ex )
            {
                _logger.LogCritical(ex, "VCI with name {vciModuleName} not found", vciModuleName);
                throw new DiagPduApiException($"VCI with name {vciModuleName} not found!");
            }

            return moduleData;
        }

        /// <summary>
        ///     Is only required with TryToRecover
        ///     The Module is just a shell, ModuleLevel is the core.
        ///     We let ourselves be given a new core. And put it under the old shell.
        /// </summary>
        /// <param name="vciModuleName"></param>
        /// <param name="existingModule"></param>
        /// <exception cref="DiagPduApiException"></exception>
        internal void ConnectVci(string vciModuleName, Module existingModule)
        {
            var moduleData = PduModuleDataFromVciModuleName(vciModuleName);

            lock ( _vciCache )
            {
                if ( !_vciCache.ContainsKey(moduleData.ModuleHandle) )
                {
                    Nwa.PduModuleConnect(moduleData.ModuleHandle);
                    var vci = new ModuleLevel(this, moduleData.ModuleHandle);
                    vci.Disposing += () =>
                    {
                        if ( !_vciCache.Remove(moduleData.ModuleHandle) )
                        {
                            //that should never happen
                            _logger.LogWarning("Remove ModuleHandle {ModuleHandle} more than once", moduleData.ModuleHandle);
                        }
                    };
                    existingModule.ModuleLevel = vci;
                    _vciCache.Add(moduleData.ModuleHandle, existingModule);
                    Disposing += vci.Dispose;
                }
            }
        }

        /// <summary>
        ///     Is only required with TryToRecover
        ///     for some VCI's we need this ugly way
        /// </summary>
        /// <returns></returns>
        internal bool TryToRecoverHelper()
        {
            try
            {
                EventItemProvider.UnRegisterEventDataCallback(ModuleHandle, ComLogicalLinkHandle);
                Nwa.PduDestruct();
                if ( string.IsNullOrWhiteSpace(_optionStrBackUp) )
                {
                    Nwa.PduConstruct();
                }
                else
                {
                    _logger.LogInformation("OptionStr: {optionStr}", _optionStrBackUp);
                    Nwa.PduConstruct(_optionStrBackUp);
                }

                EventItemProvider.RegisterEventDataCallback(ModuleHandle, ComLogicalLinkHandle, CallbackPduEventItemReceived, CallbackDataLost);
                return true;
            }
            catch ( Iso22900IIException)
            {
                return false;
            }
        }

        public PduExStatusData Status()
        {
            return Nwa.PduGetStatus(ModuleHandle, ComLogicalLinkHandle, PduConst.PDU_HANDLE_UNDEF);
        }

        /// <summary>
        ///     An alternative way to find out if there is a resource-id for this parameters.
        ///     Indirectly this is the question: "Is there a VCI that can do that?"
        ///     PduRscIdItemData is not ready please only use whether the list is filled or empty
        /// </summary>
        /// <param name="busTypeName"></param>
        /// <param name="protocolName"></param>
        /// <param name="dlcPinToTypeNamePairs"></param>
        /// <returns>if the list is empty... no VCI (in cooperation with this D-PDU-API (dll)) can do the job</returns>
        public List<PduRscIdItemData> GetResourceIds(string busTypeName, string protocolName,
            List<KeyValuePair<uint, string>> dlcPinToTypeNamePairs)
        {
            var busTypId = Nwa.PduGetObjectId(PduObjt.PDU_OBJT_BUSTYPE, busTypeName);
            var protocolTypId = Nwa.PduGetObjectId(PduObjt.PDU_OBJT_PROTOCOL, protocolName);

            var dlcPinToTypeIdPairs = DlcTypeNameToTypeId(dlcPinToTypeNamePairs);

            // This check was added because, for example, the D-PDU API from Softing does NOT return 0 resources 
            // when calling PduResourceData with PduConst.PDU_ID_UNDEF values. 
            // Softing treat PduConst.PDU_ID_UNDE as a wildcard and return all matching resources instead of none.
            // This intentional behaviour causes unpredictable results for us, so we filter it out to keep the resource count accurate.
            // We need to handle this case to avoid incorrect results.
            if (busTypId == PduConst.PDU_ID_UNDEF
                || protocolTypId == PduConst.PDU_ID_UNDEF
                || dlcPinToTypeIdPairs.Any(pair => pair.Value == PduConst.PDU_ID_UNDEF))
            {
                // One or more IDs are undefined, return empty list
                return [];
            }


            var pduResourceData = new PduResourceData(busTypId, protocolTypId, dlcPinToTypeIdPairs);
            var pduRscIdItemDatas = Nwa.PduGetResourceIds(ModuleHandle, pduResourceData);

            if ( pduRscIdItemDatas.Any() )
            {
                //resolve the module handle to module name
                var pduModuleDatas = Nwa.PduGetModuleIds();
                foreach ( var pduRscIdItemData in pduRscIdItemDatas )
                {
                    foreach ( var pduModuleData in pduModuleDatas )
                    {
                        if ( pduRscIdItemData.ModuleHandle == pduModuleData.ModuleHandle )
                        {
                            pduRscIdItemData.VendorModuleName = pduModuleData.VendorModuleName;
                        }
                    }
                }
            }
            return pduRscIdItemDatas;
        }


        internal List<KeyValuePair<uint, uint>> DlcTypeNameToTypeId(List<KeyValuePair<uint, string>> dlcPinToTypeNamePairs)
        {
            //pin number <-> type-Name pair   becomes to  pin number <-> type-Id pair
            var dlcPinToTypeIdPairs = new List<KeyValuePair<uint, uint>>();
            foreach ( var dlcPinToTypeNamePair in dlcPinToTypeNamePairs )
            {
                var pinTypeId = Nwa.PduGetObjectId(PduObjt.PDU_OBJT_PINTYPE, dlcPinToTypeNamePair.Value);
                dlcPinToTypeIdPairs.Add(new KeyValuePair<uint, uint>(dlcPinToTypeNamePair.Key, pinTypeId));
            }

            return dlcPinToTypeIdPairs;
        }


        #region PduIoControlsOnSysLevel

        /// <summary>
        /// PDU_IOCTL_VEHICLE_ID_REQUEST 
        /// </summary>
        /// <param name="vehicleIdRequestData"></param>
        /// <returns></returns>
        public bool TryIoCtlVehicleIdRequest(PduIoCtlVehicleIdRequestData vehicleIdRequestData)
        {
            return TryIoCtlGeneral("PDU_IOCTL_VEHICLE_ID_REQUEST", vehicleIdRequestData);
        }

        /// <summary>
        ///     For IoCtl which takes the name and PduIoCtlOfTypeVehicleIdRequest as parameters
        /// </summary>
        /// <param name="ioCtlShortName"></param>
        /// <param name="vehicleIdRequestData"></param>
        /// <returns>true or false</returns>
        public bool TryIoCtlGeneral(string ioCtlShortName, PduIoCtlVehicleIdRequestData vehicleIdRequestData)
        {
            try
            {
                var ioCtlCommandId = Nwa.PduGetObjectId(PduObjt.PDU_OBJT_IO_CTRL, ioCtlShortName);
                if ( !ioCtlCommandId.Equals(PduConst.PDU_ID_UNDEF) )
                {
                    Nwa.PduIoCtl(ModuleHandle, ComLogicalLinkHandle, ioCtlCommandId, new PduIoCtlOfTypeVehicleIdRequest(vehicleIdRequestData));
                    return true;
                }
            }
            catch ( Iso22900IIException e )
            {
                _logger.LogWarning(e, ioCtlShortName);
            }

            return false;
        }

        /// <summary>
        ///     For IoCtl which takes the name and byteField as parameters and byteField as out
        /// </summary>
        /// <param name="ioCtlShortName"></param>
        /// <param name="value"></param>
        /// <returns>true or false</returns>
        internal bool TryIoCtlGeneral(string ioCtlShortName, byte[] valueIn, out byte[] valueOut)
        {
            try
            {
                var ioCtlCommandId = Nwa.PduGetObjectId(PduObjt.PDU_OBJT_IO_CTRL, ioCtlShortName);
                if (!ioCtlCommandId.Equals(PduConst.PDU_ID_UNDEF))
                {
                    valueOut = ((PduIoCtlOfTypeByteField)Nwa.PduIoCtl(ModuleHandle, ComLogicalLinkHandle, ioCtlCommandId, new PduIoCtlOfTypeByteField(valueIn))).Value;
                    return true;
                }
            }
            catch (Iso22900IIException e)
            {
                _logger.LogWarning(e, ioCtlShortName);
            }

            valueOut = [];
            return false;
        }

        /// <summary>
        ///     For IoCtl which takes the name and byteField as parameters
        /// </summary>
        /// <param name="ioCtlShortName"></param>
        /// <param name="value"></param>
        /// <returns>true or false</returns>
        internal bool TryIoCtlGeneral(string ioCtlShortName, byte[] value)
        {
            try
            {
                var ioCtlCommandId = Nwa.PduGetObjectId(PduObjt.PDU_OBJT_IO_CTRL, ioCtlShortName);
                if (!ioCtlCommandId.Equals(PduConst.PDU_ID_UNDEF))
                {
                    Nwa.PduIoCtl(ModuleHandle, ComLogicalLinkHandle, ioCtlCommandId, new PduIoCtlOfTypeByteField(value));
                    return true;
                }
            }
            catch (Iso22900IIException e)
            {
                _logger.LogWarning(e, ioCtlShortName);
            }

            return false;
        }




        #endregion

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
            if ( item.EventArgs.ModuleHandle == PduConst.PDU_HANDLE_UNDEF )
            {
                //Event is System event
                OnPduEventItemReceived(item);
            }
            else if ( item.EventArgs.ComLogicalLinkHandle == PduConst.PDU_HANDLE_UNDEF )
            {
                //Event is Module event
                OnPduEventItemReceived(item);
            }
            else if ( item.CopHandle == PduConst.PDU_HANDLE_UNDEF )
            {
                //Event is CLL event
                OnPduEventItemReceived(item);
            }
            else
            { 
                //Event is COP event
                OnPduEventItemReceived(item);
            }
        }

        #region DisposeBehavior

        /// <summary>
        ///     Alternative to Dispose()
        ///     The function name is based on ISO22900-2. And can be used if NO using keyword is used
        /// </summary>
        public virtual void Destruct()
        {
            Dispose();
        }

        protected override void FreeUnmanagedResources()
        {
            //First the event because we need valid handles for PduRegisterEventCallback
            EventItemProvider.UnRegisterEventDataCallback(ModuleHandle, ComLogicalLinkHandle);
            Nwa.PduDestruct();
            Nwa.Dispose();
        }

        protected override void FreeManagedResources()
        {
            if ( EventFired != null )
            {
                foreach ( var d in EventFired.GetInvocationList() )
                {
                    EventFired -= (EventHandler<PduEventItem>)d;
                }
            }

            EventItemProvider.ReleaseManagedResources();
        }

        /// <summary>
        ///     When the finalizer thread gets around to running, it runs all the destructors of the object.
        ///     Destructors will run in order from most derived to least derived.
        /// </summary>
        ~DiagPduApiOneSysLevel()
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
