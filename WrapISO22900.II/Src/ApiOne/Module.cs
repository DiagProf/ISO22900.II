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
    public sealed class Module : IDisposable, IModule
    {
        private readonly ILogger _logger = ApiLibLogging.CreateLogger<Module>();

        /// <summary>
        ///     the main reason of sync is that e.g. not 2 ComLogicalLink try to recover the module
        ///     if the 1st ComLogicalLink has the recover module, the 2nd can use it immediately
        ///     But also as protection e.g. if the 1st CLL is recovering and the 2nd hasn't even noticed
        ///     that there are problems with the VCI, he shouldn't access it pointlessly
        /// </summary>
        private readonly object _sync = new();

        private readonly DiagPduApiOneSysLevel _oneSysLevel;
        private readonly string _vciModuleName;
        private ModuleLevel _vci;
        private event EventHandler<CallbackEventArgs> BackingUpEventHandlerDataLost;

        private event EventHandler<PduEventItem> BackingUpEventHandlerEventFired;

        internal ModuleLevel ModuleLevel
        {
            set
            {
                lock ( _sync )
                {
                    _vci = value;
                }
            }
        }
  

        public event EventHandler<CallbackEventArgs> DataLost
        {
            add
            {
                lock ( _sync )
                {
                    _vci.DataLost += value;
                    BackingUpEventHandlerDataLost += value;
                }
            }

            remove
            {
                lock ( _sync )
                {
                    BackingUpEventHandlerDataLost -= value;
                    _vci.DataLost -= value;
                }
            }
        }

        public event EventHandler<PduEventItem> EventFired
        {
            add
            {
                lock ( _sync )
                {
                    _vci.EventFired += value;
                    BackingUpEventHandlerEventFired += value;
                }
            }

            remove
            {
                lock ( _sync )
                {
                    BackingUpEventHandlerEventFired -= value;
                    _vci.EventFired -= value;
                }
            }
        }

        internal Module(DiagPduApiOneSysLevel oneSysLevel, ModuleLevel vci, string vciModuleName)
        {
            _oneSysLevel = oneSysLevel;
            _vciModuleName = vciModuleName;
            _vci = vci;
        }

        public ComLogicalLink OpenComLogicalLink(string busTypeName, string protocolName, List<KeyValuePair<uint, string>> dlcPinToTypeNamePairs)
        {
            return OpenComLogicalLink(busTypeName, protocolName, dlcPinToTypeNamePairs, new PduFlagDataCllCreateFlag());
        }

        public ComLogicalLink OpenComLogicalLink(string busTypeName, string protocolName, List<KeyValuePair<uint, string>> dlcPinToTypeNamePairs, PduFlagDataCllCreateFlag cllCreateFlag)
        {
            lock (_sync)
            {
                var cll = _vci.OpenComLogicalLink(busTypeName, protocolName, dlcPinToTypeNamePairs, cllCreateFlag);
                return new ComLogicalLink(this, cll, busTypeName, protocolName, dlcPinToTypeNamePairs, cllCreateFlag);
            }
        }

        /// <summary>
        /// open a OpenComLogicalLink via resourceId
        /// This should not be used if possible (better use OpenCom Logical Link with bus TypeName, protocol Name and dlcPinToTypeNamePairs)
        /// Because some VCIs didn't implement it.
        /// In some use cases, however, it can make sense to use this function in conjunction with the function GetResourceIds.
        /// </summary>
        /// <param name="resourceId"></param>
        /// <returns></returns>

        public ComLogicalLink OpenComLogicalLink(uint resourceId)
        {
            return OpenComLogicalLink(resourceId, new PduFlagDataCllCreateFlag());
        }

        public ComLogicalLink OpenComLogicalLink(uint resourceId, PduFlagDataCllCreateFlag cllCreateFlag)
        {
            lock (_sync)
            {
                var cll = _vci.OpenComLogicalLink(resourceId, cllCreateFlag);
                return new ComLogicalLink(this, cll, resourceId, cllCreateFlag);
            }
        }

        public uint Timestamp()
        {
            lock ( _sync )
            {
                return _vci.Timestamp();
            }
        }

        public PduVersionData VersionData()
        {
            lock ( _sync )
            {
                return _vci.VersionData();
            }
        }

        public PduExStatusData Status()
        {
            lock (_sync)
            {
                return _vci.Status();
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
                return _vci.LastError();
            }
        }

        public List<uint> GetResourceIds(string busTypeName, string protocolName, List<KeyValuePair<uint, string>> dlcPinToTypeNamePairs)
        {
            lock ( _sync )
            {
                return _vci.GetResourceIds(busTypeName, protocolName, dlcPinToTypeNamePairs);
            }
        }

        #region PduIoControlsOnModule

        public uint MeasureBatteryVoltage()
        {
            lock ( _sync )
            {
                return _vci.MeasureBatteryVoltage();
            }
        }

        /// <summary>
        ///     Used to read the switched vehicle battery voltage (Ignition on/off) pin. (convenience function)
        /// </summary>
        /// <param name="dlcPinNumber">
        ///     Pin number of the vehicles data link connector (DLC) which contains the vehicle switched battery voltage.
        ///     If DLCPinNumber = 0, then the ignition sense is read from the MVCI protocol module pin 24 and not! from a DLC pin.
        /// </param>
        /// <returns>true/false</returns>
        public bool IsIgnitionOn(byte dlcPinNumber = 0)
        {
            lock ( _sync )
            {
                return _vci.IsIgnitionOn(dlcPinNumber);
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
            lock ( _sync )
            {
                return _vci.TryIoCtlGeneral(ioCtlShortName);
            }
        }

        /// <summary>
        /// Only VCI developers could use this function.
        /// A good application with a good VCI should not use the function.
        /// </summary>
        /// <returns>true or false</returns>
        public bool TryIoCtlReset() => TryIoCtlGeneral("PDU_IOCTL_RESET");


        /// <summary>
        ///     You can use this method if you want to try something
        ///     For IoCtl where the name is the only one parameter and you get uint result
        ///     E.g. API for manufacturer specific things
        ///     For real application prefer to use the methods that call this method with the appropriate parameter
        /// </summary>
        /// <param name="ioCtlShortName"></param>
        /// <param name="value">a uint</param>
        /// <returns>true or false</returns>
        public bool TryIoCtlGeneral(string ioCtlShortName, out uint value)
        {
            lock ( _sync )
            {
                return _vci.TryIoCtlGeneral(ioCtlShortName, out value);
            }
        }

        /// <summary>
        /// The pure version. Better is convenience function MeasureBatteryVoltage
        /// </summary>
        /// <param name="value">vehicle battery in mV</param>
        /// <returns>true or false</returns>
        public bool TryIoCtlReadVbatt(out uint value) => TryIoCtlGeneral("PDU_IOCTL_READ_VBATT", out value);

        /// <summary>
        /// read out the set programming voltage
        /// generally only very, very old ECUs need this
        /// </summary>
        /// <param name="value">programming voltage in mV</param>
        /// <returns>true or false</returns>
        public bool TryIoCtlReadProgVoltage(out uint value) => TryIoCtlGeneral("PDU_IOCTL_READ_PROG_VOLTAGE", out value);

        /// <summary>
        /// Let the application know which cable is currently connected to an MVCI protocol module
        /// </summary>
        /// <param name="value">Cable Id  -> use CDF File for more Info based on Id</param>
        /// <returns>true or false</returns>
        public bool TryIoCtlGetCableId(out uint value) => TryIoCtlGeneral("PDU_IOCTL_GET_CABLE_ID", out value);


        /// <summary>
        ///     You can use this method if you want to try something
        ///     For IoCtl which takes the name and a uint as parameters and you get uint result
        ///     E.g. API for manufacturer specific things
        ///     For real application prefer to use the methods that call this method with the appropriate parameter
        /// </summary>
        /// <param name="ioCtlShortName"></param>
        /// <param name="valueIn"></param>
        /// <param name="valueOut"></param>
        /// <returns>true or false</returns>
        public bool TryIoCtlGeneral(string ioCtlShortName, uint valueIn, out uint valueOut)
        {
            lock ( _sync )
            {
                return _vci.TryIoCtlGeneral(ioCtlShortName, valueIn, out valueOut);
            }
        }

        /// <summary>
        /// ISO 13400-3 defines different Ethernet pin layout options for the OBDII connector, e.g. Option 1 and Option 2.
        /// The IOCTL command PDU_IOCTL_GET_ETH_PIN_OPTION is used to determine the Ethernet option.
        /// For  this determination, the Ethernet activation pin on the DLC connector is used.
        /// </summary>
        /// <param name="valueIn">pin 8 of the dlc is usually used</param>
        /// <param name="valueOut">
        ///     Evaluated option of Ethernet pinout on the vehicle.
        ///     0 = non Ethernet vehicle 
        ///     1 = Ethernet Option 1 
        ///     2 = Ethernet Option 2 
        ///     3 and above reserved for future us
        /// </param>
        /// <returns>true or false</returns>
        public bool TryIoCtlGetEthPinOption(uint valueIn, out uint valueOut) =>
            TryIoCtlGeneral("PDU_IOCTL_GET_ETH_PIN_OPTION", valueIn, out valueOut);

        //default pin 8 of the dlc is usually used
        public bool TryIoCtlGetEthPinOption(out uint valueOut) => TryIoCtlGeneral("PDU_IOCTL_GET_ETH_PIN_OPTION", 8, out valueOut);

        /// <summary>
        /// The pure version. Better is convenience function IsIgnitionOn
        /// </summary>
        /// <param name="valueIn"></param>
        /// <param name="valueOut">
        ///     0 = Ignition OFF
        ///     1 = Ignition ON
        /// </param>
        /// <returns>true or false</returns>
        public bool TryIoCtlReadIgnitionSenseState(uint valueIn, out uint valueOut) =>
            TryIoCtlGeneral("PDU_IOCTL_READ_IGNITION_SENSE_STATE", valueIn, out valueOut);


        /// <summary>
        ///     You can use this method if you want to try something
        ///     For IoCtl which takes the name and a byteField as parameters
        ///     E.g. API for manufacturer specific things
        ///     For real application prefer to use the methods that call this method with the appropriate parameter
        /// </summary>
        /// <param name="ioCtlShortName"></param>
        /// <param name="bytes"></param>
        /// <returns>true or false</returns>
        public bool TryIoCtlGeneral(string ioCtlShortName, byte[] bytes)
        {
            lock ( _sync )
            {
                return _vci.TryIoCtlGeneral(ioCtlShortName, bytes);
            }
        }

        /// <summary>
        /// Allows the application to send a generic message to its drivers.
        /// The message in the Data buffer is sent down to the MVCI protocol module, intercepting or interpreting it.
        /// </summary>
        /// <param name="bytes">Filter number to stop</param>
        /// <returns>true or false</returns>
        public bool TryIoCtlGeneric(byte[] bytes) => TryIoCtlGeneral("PDU_IOCTL_GENERIC", bytes);

        /// <summary>
        /// Set the programmable voltage on the specified pin/resource of the DLC
        /// </summary>
        /// <param name="pduIoCtlOfTypeProgVoltage"></param>
        /// <returns>true or false</returns>
        public bool TryIoCtlSetProgVoltage(PduIoCtlOfTypeProgVoltage pduIoCtlOfTypeProgVoltage)
        {
            lock ( _sync )
            {
                return _vci.TryIoCtlGeneral("PDU_IOCTL_SET_PROG_VOLTAGE", pduIoCtlOfTypeProgVoltage);
            }
        }


        /// <summary>
        /// Set Ethernet switch state
        /// </summary>
        /// <param name="ethernetActivationPin"></param>
        /// <param name="ethernetActDlcPinNumber"></param>
        /// <returns>true or false</returns>
        public bool TryIoCtlSetEthSwitchState(PduExEthernetActivationPin ethernetActivationPin, uint ethernetActDlcPinNumber = 8)
        {
            lock ( _sync )
            {
                return _vci.TryIoCtlGeneral("PDU_IOCTL_SET_ETH_SWITCH_STATE", ethernetActivationPin, ethernetActDlcPinNumber);
            }
        }


        /// <summary>
        /// PDU_IOCTL_VEHICLE_ID_REQUEST
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryIoCtlVehicleIdRequest(PduIoCtlVehicleIdRequestData vehicleIdRequestData)
        {
            lock ( _sync )
            {
                return _vci.TryIoCtlGeneral("PDU_IOCTL_VEHICLE_ID_REQUEST", vehicleIdRequestData);
            }
        }


        /// <summary>
        /// PDU_IOCTL_GET_DIAGNOSTIC_POWER_MODE
        /// </summary>
        /// <param name="doIpCtrlTimeout"></param>
        /// <param name="logicalAddress"></param>
        /// <param name="valueOut"></param>
        ///  <param name="value"></param>
        /// <returns></returns>
        public bool TryIoCtlGetDiagnosticPowerMode(uint logicalAddress, uint doIpCtrlTimeout, out uint valueOut)
        {
            lock ( _sync )
            {
                return _vci.TryIoCtlGeneral("PDU_IOCTL_GET_DIAGNOSTIC_POWER_MODE", logicalAddress, doIpCtrlTimeout, out valueOut);
            }
        }

        /// <summary>
        /// PDU_IOCTL_GET_ENTITY_STATUS
        /// </summary>
        /// <param name="logicalAddress"></param>
        /// <param name="doIpCtrlTimeout"></param
        /// <param name="valueOut"></param>
        /// <returns></returns>
        public bool TryIoCtlGetEntityStatus(uint logicalAddress, uint doIpCtrlTimeout, out PduIoCtlEntityStatusData valueOut)
        {
            lock ( _sync )
            {
                return _vci.TryIoCtlGeneral("PDU_IOCTL_GET_ENTITY_STATUS", logicalAddress, doIpCtrlTimeout, out valueOut);
            }
        }

        #endregion

        /// <summary>
        ///     Attempts to restore the status of the VCI
        ///     Catch all "Iso22900IIException" exceptions
        /// </summary>
        /// <returns></returns>
        public bool TryToRecover(out string exMessage)
        {
            lock ( _sync )
            {
                try
                {
                    if ( _vci.Status().Status == PduStatus.PDU_MODST_NOT_AVAIL )
                    {
                        _vci.Dispose();

                        
                        //this is where the magic happens.. the new instance is assigned under the hood
                        try
                        {
                            _oneSysLevel.ConnectVci(_vciModuleName, this);
                        }
                        catch ( Iso22900IIExceptionBase )
                        {
                            //not so pretty... the hard way.... but these manufacturers need it
                            //- Actia
                            //- Samtec
                            if ( !_oneSysLevel.TryToRecoverHelper() )
                            {
                                throw;
                            }
                            _oneSysLevel.ConnectVci(_vciModuleName, this);
                        }


                        if ( BackingUpEventHandlerEventFired != null )
                        {
                            foreach ( var d in BackingUpEventHandlerEventFired.GetInvocationList() )
                            {
                                _vci.EventFired += (EventHandler<PduEventItem>)d;
                            }
                        }

                        if ( BackingUpEventHandlerDataLost != null )
                        {
                            foreach ( var d in BackingUpEventHandlerDataLost.GetInvocationList() )
                            {
                                _vci.DataLost += (EventHandler<CallbackEventArgs>)d;
                            }
                        }

                        _logger.Log(LogLevel.Information, "VCI recovering done for VCI: {_vciModuleName}", _vciModuleName);
                    }

                    _logger.Log(LogLevel.Information, "VCI is back or no reason to recover for VCI: {_vciModuleName}", _vciModuleName);
                }
                catch (Iso22900IIExceptionBase ex )
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
        public void Disconnect()
        {
            Dispose();
        }


        public void Dispose()
        {
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

            _vci.Dispose();
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
                    return _vci.IsDisposed;
                }
            }
        }

        #endregion
    }
}
