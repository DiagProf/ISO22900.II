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
using System.Threading;
using Microsoft.Extensions.Logging;

namespace ISO22900.II
{
    internal class ModuleLevel : ManagedDisposable
    {
        private readonly ILogger _logger = ApiLibLogging.CreateLogger<ModuleLevel>();

        internal readonly DiagPduApiOneSysLevel DiagPduApiOneSysLevel;

        protected internal uint ModuleHandle { get; }
        protected internal uint ComLogicalLinkHandle { get; } = PduConst.PDU_HANDLE_UNDEF;

        public event EventHandler<CallbackEventArgs> DataLost;
        public event EventHandler<PduEventItem> EventFired;


        internal ModuleLevel(DiagPduApiOneSysLevel diagPduApiOneSysLevel, uint moduleHandle)
        {
            DiagPduApiOneSysLevel = diagPduApiOneSysLevel;
            ModuleHandle = moduleHandle;

            DiagPduApiOneSysLevel.EventItemProvider.RegisterEventDataCallback(ModuleHandle, ComLogicalLinkHandle, CallbackPduEventItemReceived, CallbackDataLost);
        }

        internal ComLogicalLinkLevel OpenComLogicalLink(string busTypeName, string protocolName, List<KeyValuePair<uint, string>> dlcPinToTypeNamePairs)
        {
            return OpenComLogicalLink(busTypeName,protocolName,dlcPinToTypeNamePairs, new PduFlagDataCllCreateFlag());
        }

        internal ComLogicalLinkLevel OpenComLogicalLink(string busTypeName, string protocolName, List<KeyValuePair<uint, string>> dlcPinToTypeNamePairs, PduFlagDataCllCreateFlag cllCreateFlag)
        {
            var busTypId = DiagPduApiOneSysLevel.Nwa.PduGetObjectId(PduObjt.PDU_OBJT_BUSTYPE, busTypeName);
            var protocolTypId = DiagPduApiOneSysLevel.Nwa.PduGetObjectId(PduObjt.PDU_OBJT_PROTOCOL, protocolName);
            var dlcPinToTypeIdPairs = DiagPduApiOneSysLevel.DlcTypeNameToTypeId(dlcPinToTypeNamePairs);

            var pduResourceData = new PduResourceData(busTypId, protocolTypId, dlcPinToTypeIdPairs.ToList());
            var comLogicalLinkHandle = DiagPduApiOneSysLevel.Nwa.PduCreateComLogicalLink(ModuleHandle, pduResourceData,
                PduConst.PDU_ID_UNDEF,
                0, cllCreateFlag);


            var cll = new ComLogicalLinkLevel(this, comLogicalLinkHandle);
            Disposing += cll.Dispose;
            return cll;
        }

        /// <summary>
        ///     Current time (hardware clock) from an MVCI protocol module
        /// </summary>
        /// <returns>timestamp in microseconds</returns>
        internal uint Timestamp()
        {
            return DiagPduApiOneSysLevel.Nwa.PduGetTimestamp(ModuleHandle);
        }

        internal PduVersionData VersionData()
        {
            return DiagPduApiOneSysLevel.Nwa.PduGetVersionData(ModuleHandle);
        }


        /// <summary>
        ///     Measures the vehicle supply voltage (convenience function)
        /// </summary>
        /// <returns>Voltage (mV)</returns>
        public uint MeasureBatteryVoltage()
        {
            //Attention some VCIs do not support this!!
            //Then we can fake the result with the "ApiModifications" flags
            try
            {
                var ioCtlCommandId = DiagPduApiOneSysLevel.Nwa.PduGetObjectId(PduObjt.PDU_OBJT_IO_CTRL, "PDU_IOCTL_READ_VBATT");
                if ( !ioCtlCommandId.Equals(PduConst.PDU_ID_UNDEF) )
                {
                    var pduIoCtlData = DiagPduApiOneSysLevel.Nwa.PduIoCtl(ModuleHandle, ComLogicalLinkHandle, ioCtlCommandId, null);
                    if ( pduIoCtlData is PduIoCtlOfTypeUint ctlDataUnum32 )
                    {
                        if ( DiagPduApiOneSysLevel.ApiModBitField.HasFlag(ApiModifications.VOLTAGE_TO_LOW_FIX) )
                        {
                            if ( ctlDataUnum32.Value > 500 )
                            {
                                return ctlDataUnum32.Value + 1500; //add 1,5 Volt
                            }
                            //like zero ...
                            return ctlDataUnum32.Value;
                        }
                        return ctlDataUnum32.Value;
                    }
                }
            }
            catch ( Iso22900IIException e )
            {
                if ( !(DiagPduApiOneSysLevel.ApiModBitField.HasFlag(ApiModifications.VOLTAGE_FIX) &&
                       (e.PduError == PduError.PDU_ERR_VOLTAGE_NOT_SUPPORTED || e.PduError == PduError.PDU_ERR_ID_NOT_SUPPORTED)) )
                {
                    throw;
                }
            }

            return 13999;  //13.999 Volt
        }

        /// <summary>
        ///     Used to read the switched vehicle battery voltage (Ignition on/off) pin. (convenience function)
        /// </summary>
        /// <param name="dlcPinNumber">
        ///     Pin number of the vehicles data link connector (DLC) which contains the vehicle switched battery voltage.
        ///     If DLCPinNumber = 0, then the ignition sense is read from the MVCI protocol module pin 24 and not! from a DLC pin.
        /// </param>
        /// <returns>true/false</returns>
        public bool IsIgnitionOn(byte dlcPinNumber)
        {
            //Attention some VCIs do not support this!!
            //Then we can fake the result with the "ApiModifications" flags
            try
            {
                var ioCtlCommandId = DiagPduApiOneSysLevel.Nwa.PduGetObjectId(PduObjt.PDU_OBJT_IO_CTRL, "PDU_IOCTL_READ_IGNITION_SENSE_STATE");
                if ( !ioCtlCommandId.Equals(PduConst.PDU_ID_UNDEF) )
                {
                    var input = new PduIoCtlOfTypeUint(dlcPinNumber);
                    var output = DiagPduApiOneSysLevel.Nwa.PduIoCtl(ModuleHandle, ComLogicalLinkHandle, ioCtlCommandId, input);
                    if ( output is PduIoCtlOfTypeUint ctlDataUnum32 )
                    {
                        return (ctlDataUnum32.Value & 0x1) != 0;
                    }
                }
            }
            catch ( Iso22900IIException e )
            {
                if ( !(DiagPduApiOneSysLevel.ApiModBitField.HasFlag(ApiModifications.IGNITION_FIX) && e.PduError == PduError.PDU_ERR_ID_NOT_SUPPORTED) )
                {
                    throw;
                }
            }

            return true;
        }

        internal PduExStatusData Status()
        {
            return DiagPduApiOneSysLevel.Nwa.PduGetStatus(ModuleHandle, ComLogicalLinkHandle, PduConst.PDU_HANDLE_UNDEF);
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
                var ioCtlCommandId = DiagPduApiOneSysLevel.Nwa.PduGetObjectId(PduObjt.PDU_OBJT_IO_CTRL, ioCtlShortName);
                if ( !ioCtlCommandId.Equals(PduConst.PDU_ID_UNDEF) )
                {
                    DiagPduApiOneSysLevel.Nwa.PduIoCtl(ModuleHandle, ComLogicalLinkHandle, ioCtlCommandId, null);
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
        ///     For IoCtl which takes the name and a uint as parameters
        /// </summary>
        /// <param name="ioCtlShortName"></param>
        /// <param name="value">a uint</param>
        /// <returns>true or false</returns>
        internal bool TryIoCtlGeneral(string ioCtlShortName, uint valueIn, out uint valueOut)
        {
            try
            {
                var ioCtlCommandId = DiagPduApiOneSysLevel.Nwa.PduGetObjectId(PduObjt.PDU_OBJT_IO_CTRL, ioCtlShortName);
                if (!ioCtlCommandId.Equals(PduConst.PDU_ID_UNDEF))
                {
                    valueOut = ((PduIoCtlOfTypeUint)DiagPduApiOneSysLevel.Nwa.PduIoCtl(ModuleHandle, ComLogicalLinkHandle, ioCtlCommandId, new PduIoCtlOfTypeUint(valueIn))).Value;
                    return true;
                }
            }
            catch ( Iso22900IIException e )
            {
                _logger.LogWarning(e, ioCtlShortName);
            }
            valueOut = default;
            return false;
        }


        /// <summary>
        ///     For IoCtl which takes the name and a uint as in parameters and uint as out
        /// </summary>
        /// <param name="ioCtlShortName"></param>
        /// <param name="value">a uint</param>
        /// <returns>true or false</returns>
        internal bool TryIoCtlGeneral(string ioCtlShortName, out uint value)
        {
            try
            {
                var ioCtlCommandId = DiagPduApiOneSysLevel.Nwa.PduGetObjectId(PduObjt.PDU_OBJT_IO_CTRL, ioCtlShortName);
                if (!ioCtlCommandId.Equals(PduConst.PDU_ID_UNDEF))
                {
                    value = ((PduIoCtlOfTypeUint)DiagPduApiOneSysLevel.Nwa.PduIoCtl(ModuleHandle, ComLogicalLinkHandle, ioCtlCommandId, null)).Value;
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
        ///     For IoCtl which takes the name and byteField as parameters
        /// </summary>
        /// <param name="ioCtlShortName"></param>
        /// <param name="value"></param>
        /// <returns>true or false</returns>
        internal bool TryIoCtlGeneral(string ioCtlShortName, byte[] value)
        {
            try
            {
                var ioCtlCommandId = DiagPduApiOneSysLevel.Nwa.PduGetObjectId(PduObjt.PDU_OBJT_IO_CTRL, ioCtlShortName);
                if (!ioCtlCommandId.Equals(PduConst.PDU_ID_UNDEF))
                {
                    DiagPduApiOneSysLevel.Nwa.PduIoCtl(ModuleHandle, ComLogicalLinkHandle, ioCtlCommandId, new PduIoCtlOfTypeByteField(value));
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
        ///     For IoCtl which takes the name and PduIoCtlOfTypeProgVoltage as parameters
        /// </summary>
        /// <param name="ioCtlShortName"></param>
        /// <param name="value"></param>
        /// <returns>true or false</returns>
        internal bool TryIoCtlGeneral(string ioCtlShortName, PduIoCtlOfTypeProgVoltage value)
        {
            try
            {
                var ioCtlCommandId = DiagPduApiOneSysLevel.Nwa.PduGetObjectId(PduObjt.PDU_OBJT_IO_CTRL, ioCtlShortName);
                if (!ioCtlCommandId.Equals(PduConst.PDU_ID_UNDEF))
                {
                    DiagPduApiOneSysLevel.Nwa.PduIoCtl(ModuleHandle, ComLogicalLinkHandle, ioCtlCommandId, value);
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
        ///     For IoCtl which takes the name and PduIoCtlOfTypeSetEthSwitchState as parameters
        /// </summary>
        /// <param name="ioCtlShortName"></param>
        /// <param name="value"></param>
        /// <returns>true or false</returns>
        internal bool TryIoCtlGeneral(string ioCtlShortName, PduExEthernetActivationPin ethernetActivationPin, uint ethernetActDlcPinNumber = 8)
        {
            try
            {
                var ioCtlCommandId = DiagPduApiOneSysLevel.Nwa.PduGetObjectId(PduObjt.PDU_OBJT_IO_CTRL, ioCtlShortName);
                if (!ioCtlCommandId.Equals(PduConst.PDU_ID_UNDEF))
                {
                    DiagPduApiOneSysLevel.Nwa.PduIoCtl(ModuleHandle, ComLogicalLinkHandle, ioCtlCommandId, new PduIoCtlOfTypeEthSwitchState(ethernetActivationPin, ethernetActDlcPinNumber));
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
        ///     For IoCtl which takes the name and PduIoCtlOfTypeVehicleIdRequest as parameters
        /// </summary>
        /// <param name="ioCtlShortName"></param>
        /// <param name="vehicleIdRequestData"></param>
        /// <returns>true or false</returns>
        internal bool TryIoCtlGeneral(string ioCtlShortName, PduIoCtlVehicleIdRequestData vehicleIdRequestData)
        {
            try
            {
                var ioCtlCommandId = DiagPduApiOneSysLevel.Nwa.PduGetObjectId(PduObjt.PDU_OBJT_IO_CTRL, ioCtlShortName);
                if (!ioCtlCommandId.Equals(PduConst.PDU_ID_UNDEF))
                {
                    DiagPduApiOneSysLevel.Nwa.PduIoCtl(ModuleHandle, ComLogicalLinkHandle, ioCtlCommandId, new PduIoCtlOfTypeVehicleIdRequest(vehicleIdRequestData));
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
        ///     For IoCtl which takes the name and PduIoCtlOfTypeEntityAddress (logicalAddress and doIpCtrlTimeout) as parameters
        /// </summary>
        /// <param name="ioCtlShortName"></param>
        /// <param name="logicalAddress"></param>
        /// <param name="doIpCtrlTimeout"></param>
        /// <param name="value"></param>
        /// <returns>true or false</returns>
        internal bool TryIoCtlGeneral(string ioCtlShortName, uint logicalAddress, uint doIpCtrlTimeout, out uint value)
        {
            try
            {
                var ioCtlCommandId = DiagPduApiOneSysLevel.Nwa.PduGetObjectId(PduObjt.PDU_OBJT_IO_CTRL, ioCtlShortName);
                if ( !ioCtlCommandId.Equals(PduConst.PDU_ID_UNDEF) )
                {
                    value = ((PduIoCtlOfTypeUint)DiagPduApiOneSysLevel.Nwa.PduIoCtl(ModuleHandle, ComLogicalLinkHandle, ioCtlCommandId, new PduIoCtlOfTypeEntityAddress(logicalAddress, doIpCtrlTimeout))).Value;
                    return true;
                }
            }
            catch ( Iso22900IIException e )
            {
                _logger.LogWarning(e, ioCtlShortName);
            }

            value = default;
            return false;
        }

        /// <summary>
        ///     For IoCtl which takes the name and PduIoCtlOfTypeEntityAddress as parameters PduIoCtlEntityStatusData
        /// </summary>
        /// <param name="ioCtlShortName"></param>
        /// <param name="logicalAddress"></param>
        /// <param name="doIpCtrlTimeout"></param>
        /// <param name="value">PduIoCtlEntityStatusData</param>
        /// <returns>true or false</returns>
        internal bool TryIoCtlGeneral(string ioCtlShortName, uint logicalAddress, uint doIpCtrlTimeout, out PduIoCtlEntityStatusData value)
        {
            try
            {
                var ioCtlCommandId = DiagPduApiOneSysLevel.Nwa.PduGetObjectId(PduObjt.PDU_OBJT_IO_CTRL, ioCtlShortName);
                if (!ioCtlCommandId.Equals(PduConst.PDU_ID_UNDEF))
                {
                    value = ((PduIoCtlOfTypeEntityStatus)DiagPduApiOneSysLevel.Nwa.PduIoCtl(ModuleHandle, ComLogicalLinkHandle, ioCtlCommandId, new PduIoCtlOfTypeEntityAddress(logicalAddress, doIpCtrlTimeout))).Value;
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
            if ( item.EventArgs.ComLogicalLinkHandle == PduConst.PDU_HANDLE_UNDEF )
            {
                //Event is Modul event
                OnPduEventItemReceived(item);
            }
            else if ( item.CopHandle == PduConst.PDU_HANDLE_UNDEF )
            {
                //Event is CLL event
                OnPduEventItemReceived(item);
            }
            else
            {
                //Event is ComPrimitiveLevel event
                OnPduEventItemReceived(item);
            }
        }

        #region DisposeBehavior

        protected override void FreeUnmanagedResources()
        {
            //First of all, we carefully ask what the VCI is doing
            //No exception is to be expected here
            var statusVci = Status().Status;

            if ((statusVci == PduStatus.PDU_MODST_NOT_AVAIL || statusVci == PduStatus.PDU_MODST_NOT_READY))
            {
                //that's the situation VCI lost
                //Now we have to be very tolerant because every manufacturer does it differently
                try
                {
                    //First the event because we need valid handles for native PduRegisterEventCallback function
                    DiagPduApiOneSysLevel.EventItemProvider.UnRegisterEventDataCallback(ModuleHandle, ComLogicalLinkHandle);
                    //well:
                    //- Softing
                    //- Bosch
                    //- Samtec
                }
                catch (Iso22900IIException ex)
                {
                    //bad:
                    //- Actia
                    _logger.LogWarning("Can't UnRegisterEventDataCallback: {error}", ex.Message);
                }

                try
                {
                    DiagPduApiOneSysLevel.Nwa.PduModuleDisconnect(ModuleHandle);
                    //well:
                    //- Softing
                    //- Bosch
                    //- Samtec
                    //- Actia
                }
                catch (Iso22900IIException ex)
                {
                    //bad:
                    //-
                    _logger.LogWarning("Can't PduModuleDisconnect: {error}", ex.Message);
                }
            }
            else
            {
                //First the event because we need valid handles for PduRegisterEventCallback
                DiagPduApiOneSysLevel.EventItemProvider.UnRegisterEventDataCallback(ModuleHandle, ComLogicalLinkHandle);
                DiagPduApiOneSysLevel.Nwa.PduModuleDisconnect(ModuleHandle);
            }

        }

        protected override void FreeManagedResources()
        {
            DiagPduApiOneSysLevel.Disposing -= Dispose;

            if ( EventFired != null )
            {
                foreach ( var d in EventFired.GetInvocationList() )
                {
                    EventFired -= (EventHandler<PduEventItem>)d;
                }
            }
        }

        /// <summary>
        ///     When the finalizer thread gets around to running, it runs all the destructors of the object.
        ///     Destructors will run in order from most derived to least derived.
        /// </summary>
        ~ModuleLevel()
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
                    var builder = new System.Text.StringBuilder();
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
