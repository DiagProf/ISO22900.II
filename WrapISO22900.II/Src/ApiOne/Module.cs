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
using Microsoft.Extensions.Logging;

namespace ISO22900.II
{
    /// <summary>
    ///     I'm implementing a wrapper class over the actual VCI binding.
    ///     The main reason why I exist is that in the event of an error (e.g. VCI lost)
    ///     I am able to replace the real VCI connection (which I wrap) with a new instance.
    ///     The application has an instance of me that doesn't change from the application point of view.
    /// </summary>
    public sealed class Module : IDisposable
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
            lock ( _sync )
            {
                var cll = _vci.OpenComLogicalLink(busTypeName, protocolName, dlcPinToTypeNamePairs);
                return new ComLogicalLink(this, cll, busTypeName, protocolName, dlcPinToTypeNamePairs);
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

        public PduExStatusData Status()
        {
            lock ( _sync )
            {
                return _vci.Status();
            }
        }

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
