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
/// System level of one D-PDU API (the entry point after PDUConstruct).
/// The native implementation is <c>DiagPduApiOneSysLevel</c> created by <c>DiagPduApiOneFactory.GetApi(...)</c> from the ISO22900.II-Sharp package.
/// Implement it yourself to plug in a complete simulated VCI stack (sys level -> <see cref="IModule"/> -> <see cref="IComLogicalLink"/> -> <see cref="IComPrimitive"/>).
/// </summary>
public interface IDiagPduApiOneSysLevel : IDisposable
{
    event EventHandler<CallbackEventArgs> DataLost;
    event EventHandler<PduEventItem> EventFired;

    /// <summary>
    /// All modules (VCIs) the D-PDU API knows, including those that are currently not available.
    /// </summary>
    List<PduModuleData> PduModuleDataSets { get; }

    /// <summary>
    ///     List of all VCI names that are currently available
    /// </summary>
    /// <returns>List of VCI Module names</returns>
    List<string> NamesOfAvailableVciModules();

    /// <summary>
    ///     ConnectVci
    /// </summary>
    /// <param name="vciModuleName">
    ///     Name of a VCI e.g. obtained from NamesOfAvailableVciModules() or
    ///     PduModuleData.VendorModuleName. Empty means the first available VCI.
    /// </param>
    /// <returns>the connected module</returns>
    IModule ConnectVci(string vciModuleName = "");

    PduExStatusData Status();

    /// <summary>
    ///     An alternative way to find out if there is a resource-id for this parameters.
    ///     Indirectly this is the question: "Is there a VCI that can do that?"
    /// </summary>
    /// <returns>if the list is empty... no VCI (in cooperation with this D-PDU-API (dll)) can do the job</returns>
    List<PduRscIdItemData> GetResourceIds(string busTypeName, string protocolName, List<KeyValuePair<uint, string>> dlcPinToTypeNamePairs);

    /// <summary>
    /// PDU_IOCTL_VEHICLE_ID_REQUEST
    /// </summary>
    bool TryIoCtlVehicleIdRequest(PduIoCtlVehicleIdRequestData vehicleIdRequestData);

    /// <summary>
    ///     For IoCtl which takes the name and PduIoCtlOfTypeVehicleIdRequest as parameters
    /// </summary>
    bool TryIoCtlGeneral(string ioCtlShortName, PduIoCtlVehicleIdRequestData vehicleIdRequestData);

    /// <summary>
    ///     For IoCtl which takes the name and byteField as parameters and byteField as out
    /// </summary>
    bool TryIoCtlGeneral(string ioCtlShortName, byte[] valueIn, out byte[] valueOut);

    /// <summary>
    ///     For IoCtl which takes the name and byteField as parameters
    /// </summary>
    bool TryIoCtlGeneral(string ioCtlShortName, byte[] value);

    /// <summary>
    ///     Alternative to Dispose()
    ///     The function name is based on ISO22900-2. And can be used if NO using keyword is used
    /// </summary>
    void Destruct();
}
