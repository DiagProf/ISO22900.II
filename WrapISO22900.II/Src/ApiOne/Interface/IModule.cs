#region License

// // MIT License
// //
// // Copyright (c) 2024 Joerg Frank
// // http://www.diagprof.com/
// //
// // Permission is hereby granted, free of charge, to any person obtaining a copy
// // of this software and associated documentation files (the "Software"), to deal
// // in the Software without restriction, including without limitation the rights
// // to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// // copies of the Software, and to permit persons to whom the Software is
// // furnished to do so, subject to the following conditions:
// //
// // The above copyright notice and this permission notice shall be included in all
// // copies or substantial portions of the Software.
// //
// // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// // IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// // FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// // AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// // LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// // OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// // SOFTWARE.

#endregion

using System;
using System.Collections.Generic;

namespace ISO22900.II.Interface;

/// <summary>
/// This interface is currently designed for experimental use only and should not be used in production at this stage.
/// The goal is to allow a non-ISO 22900-2 compliant VCI to work with an application that normally relies on the ApiOne.
/// This interface serves as the bridge for such non-compliant VCI implementations.
/// If this approach proves successful, the interfaces will likely be refactored and published as a standalone NuGet package for broader use.
/// </summary>
public interface IModule
{
    event EventHandler<CallbackEventArgs> DataLost;
    event EventHandler<PduEventItem> EventFired;
    ComLogicalLink OpenComLogicalLink(string busTypeName, string protocolName, List<KeyValuePair<uint, string>> dlcPinToTypeNamePairs);
    ComLogicalLink OpenComLogicalLink(string busTypeName, string protocolName, List<KeyValuePair<uint, string>> dlcPinToTypeNamePairs, PduFlagDataCllCreateFlag cllCreateFlag);

    /// <summary>
    /// open a OpenComLogicalLink via resourceId
    /// This should not be used if possible (better use OpenCom Logical Link with bus TypeName, protocol Name and dlcPinToTypeNamePairs)
    /// Because some VCIs didn't implement it.
    /// In some use cases, however, it can make sense to use this function in conjunction with the function GetResourceIds.
    /// </summary>
    /// <param name="resourceId"></param>
    /// <returns></returns>
    ComLogicalLink OpenComLogicalLink(uint resourceId);

    ComLogicalLink OpenComLogicalLink(uint resourceId, PduFlagDataCllCreateFlag cllCreateFlag);
    uint Timestamp();
    PduVersionData VersionData();
    PduExStatusData Status();

    /// <summary>
    /// Unfortunately, the function does not work very well.
    /// Because some manufacturers of the D-PDU API have not implemented it well. So please don't use it.
    /// The reason method is here anyway is because some users of the ApiOne use ApiOne to verify D-PDU API's (before they buy).
    /// </summary>
    /// <returns>PduExLastErrorData</returns>
    PduExLastErrorData LastError();

    List<uint> GetResourceIds(string busTypeName, string protocolName, List<KeyValuePair<uint, string>> dlcPinToTypeNamePairs);
    uint MeasureBatteryVoltage();

    /// <summary>
    ///     Used to read the switched vehicle battery voltage (Ignition on/off) pin. (convenience function)
    /// </summary>
    /// <param name="dlcPinNumber">
    ///     Pin number of the vehicles data link connector (DLC) which contains the vehicle switched battery voltage.
    ///     If DLCPinNumber = 0, then the ignition sense is read from the MVCI protocol module pin 24 and not! from a DLC pin.
    /// </param>
    /// <returns>true/false</returns>
    bool IsIgnitionOn(byte dlcPinNumber = 0);

    /// <summary>
    ///     You can use this method if you want to try something
    ///     For IoCtl where the name is the only one parameter
    ///     E.g. API for manufacturer specific things
    ///     For real application prefer to use the methods that call this method with the appropriate parameter
    /// </summary>
    /// <param name="ioCtlShortName"></param>
    /// <returns>true or false</returns>
    bool TryIoCtlGeneral(string ioCtlShortName);

    /// <summary>
    ///     You can use this method if you want to try something
    ///     For IoCtl where the name is the only one parameter and you get uint result
    ///     E.g. API for manufacturer specific things
    ///     For real application prefer to use the methods that call this method with the appropriate parameter
    /// </summary>
    /// <param name="ioCtlShortName"></param>
    /// <param name="value">a uint</param>
    /// <returns>true or false</returns>
    bool TryIoCtlGeneral(string ioCtlShortName, out uint value);

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
    bool TryIoCtlGeneral(string ioCtlShortName, uint valueIn, out uint valueOut);

    /// <summary>
    ///     You can use this method if you want to try something
    ///     For IoCtl which takes the name and a byteField as parameters
    ///     E.g. API for manufacturer specific things
    ///     For real application prefer to use the methods that call this method with the appropriate parameter
    /// </summary>
    /// <param name="ioCtlShortName"></param>
    /// <param name="bytes"></param>
    /// <returns>true or false</returns>
    bool TryIoCtlGeneral(string ioCtlShortName, byte[] bytes);

    /// <summary>
    /// Only VCI developers could use this function.
    /// A good application with a good VCI should not use the function.
    /// </summary>
    /// <returns>true or false</returns>
    bool TryIoCtlReset();

    /// <summary>
    /// The pure version. Better is convenience function MeasureBatteryVoltage
    /// </summary>
    /// <param name="value">vehicle battery in mV</param>
    /// <returns>true or false</returns>
    bool TryIoCtlReadVbatt(out uint value);

    /// <summary>
    /// read out the set programming voltage
    /// generally only very, very old ECUs need this
    /// </summary>
    /// <param name="value">programming voltage in mV</param>
    /// <returns>true or false</returns>
    bool TryIoCtlReadProgVoltage(out uint value);

    /// <summary>
    /// Let the application know which cable is currently connected to an MVCI protocol module
    /// </summary>
    /// <param name="value">Cable Id  -> use CDF File for more Info based on Id</param>
    /// <returns>true or false</returns>
    bool TryIoCtlGetCableId(out uint value);

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
    bool TryIoCtlGetEthPinOption(uint valueIn, out uint valueOut);

    bool TryIoCtlGetEthPinOption(out uint valueOut);

    /// <summary>
    /// The pure version. Better is convenience function IsIgnitionOn
    /// </summary>
    /// <param name="valueIn"></param>
    /// <param name="valueOut">
    ///     0 = Ignition OFF
    ///     1 = Ignition ON
    /// </param>
    /// <returns>true or false</returns>
    bool TryIoCtlReadIgnitionSenseState(uint valueIn, out uint valueOut);

    /// <summary>
    /// Allows the application to send a generic message to its drivers.
    /// The message in the Data buffer is sent down to the MVCI protocol module, intercepting or interpreting it.
    /// </summary>
    /// <param name="bytes">Filter number to stop</param>
    /// <returns>true or false</returns>
    bool TryIoCtlGeneric(byte[] bytes);

    /// <summary>
    /// Set the programmable voltage on the specified pin/resource of the DLC
    /// </summary>
    /// <param name="pduIoCtlOfTypeProgVoltage"></param>
    /// <returns>true or false</returns>
    bool TryIoCtlSetProgVoltage(PduIoCtlOfTypeProgVoltage pduIoCtlOfTypeProgVoltage);

    /// <summary>
    /// Set Ethernet switch state
    /// </summary>
    /// <param name="ethernetActivationPin"></param>
    /// <param name="ethernetActDlcPinNumber"></param>
    /// <returns>true or false</returns>
    bool TryIoCtlSetEthSwitchState(PduExEthernetActivationPin ethernetActivationPin, uint ethernetActDlcPinNumber = 8);

    /// <summary>
    /// PDU_IOCTL_VEHICLE_ID_REQUEST
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    bool TryIoCtlVehicleIdRequest(PduIoCtlVehicleIdRequestData vehicleIdRequestData);

    /// <summary>
    /// PDU_IOCTL_GET_DIAGNOSTIC_POWER_MODE
    /// </summary>
    /// <param name="doIpCtrlTimeout"></param>
    /// <param name="logicalAddress"></param>
    /// <param name="valueOut"></param>
    ///  <param name="value"></param>
    /// <returns></returns>
    bool TryIoCtlGetDiagnosticPowerMode(uint logicalAddress, uint doIpCtrlTimeout, out uint valueOut);

    /// <summary>
    /// PDU_IOCTL_GET_ENTITY_STATUS
    /// </summary>
    /// <param name="logicalAddress"></param>
    /// <param name="doIpCtrlTimeout"></param
    /// <param name="valueOut"></param>
    /// <returns></returns>
    bool TryIoCtlGetEntityStatus(uint logicalAddress, uint doIpCtrlTimeout, out PduIoCtlEntityStatusData valueOut);

    /// <summary>
    ///     Attempts to restore the status of the VCI
    ///     Catch all "Iso22900IIException" exceptions
    /// </summary>
    /// <returns></returns>
    bool TryToRecover(out string exMessage);

    /// <summary>
    ///     Alternative to Dispose()
    ///     The function name is based on ISO22900-2. And can be used if NO using keyword is used
    /// </summary>
    void Disconnect();

    void Dispose();
}
