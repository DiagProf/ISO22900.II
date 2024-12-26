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
public interface IComLogicalLink
{
    event EventHandler<CallbackEventArgs> DataLost;
    event EventHandler<PduEventItem> EventFired;
    ComPrimitive StartCop(PduCopt pduCopType, TimeSpan delayTimeMs = default, uint copTag = 0);
    ComPrimitive StartCop(PduCopt pduCopType, byte[] copData, uint copTag = 0);
    ComPrimitive StartCop(PduCopt pduCopType, int sendCycles, int receiveCycles, byte[] copData, uint copTag = 0);
    ComPrimitive StartCop(PduCopt pduCopType, byte[] copData, PduCopCtrlData copCtrlData, uint copTag = 0);
    void Connect();
    void Disconnect();
    PduComParam GetComParam(string name);
    PduComParam SetComParamValueViaGet(string name, long value);
    PduComParam SetComParamValueViaGet(string name, byte[] value);
    PduComParam SetComParamValueViaGet(string name, uint[] value);
    PduComParam SetComParamValueViaGet(string name, PduParamStructFieldData value);
    void SetComParam(PduComParam cp);

    /// <summary>
    /// For those who don't want to register a logger.
    /// But still want to know if something goes wrong when setting the ComParam.
    /// </summary>
    /// <param name="cp">PduComParam</param>
    /// <returns>true or false</returns>
    bool TrySetComParam(PduComParam cp);

    uint GetUniqueIdComParamValue(uint uniqueRespIdentifier, string name);
    void SetUniqueIdComParamValue(uint uniqueRespIdentifier, string name, long value);
    void SetUniqueIdComParamValues(uint uniqueRespIdentifier, List<KeyValuePair<string, uint>> listComParamNameToValuePairs);
    void SetUniqueRespIdTable(List<PduEcuUniqueRespData> ecuUniqueRespDatas);
    void SetUniqueRespIdTablePageOneUniqueRespIdentifier(uint uniqueRespIdentifier);

    /// <summary>
    /// returns the number of the current pages
    /// usually in preparation for the function GetPageUniqueRespIdentifier
    /// </summary>
    /// <returns></returns>
    uint GetUniqueRespIdTableNumberOfPages();

    /// <summary>
    /// returns the UniqueRespIdentifier from page one
    /// actually there should always be at least one page
    /// but I think there are also wrong implementations where you can delete all pages. Then you have to be careful with this function
    /// </summary>
    /// <returns></returns>
    uint GetPageOneUniqueRespIdentifier();

    /// <summary>
    /// returns the UniqueRespIdentifier from page with index x
    /// use GetUniqueRespIdTableNumberOfPages to see how many there are
    /// </summary>
    /// <param name="pageIndex">page index</param>
    /// <returns></returns>
    uint GetPageUniqueRespIdentifier(uint pageIndex);

    uint MeasureBatteryVoltage();
    bool IsIgnitionOn(byte dlcPinNumber = 0);
    PduExStatusData Status();

    /// <summary>
    /// Unfortunately, the function does not work very well.
    /// Because some manufacturers of the D-PDU API have not implemented it well. So please don't use it.
    /// The reason method is here anyway is because some users of the ApiOne use ApiOne to verify D-PDU API's (before they buy).
    /// </summary>
    /// <returns>PduExLastErrorData</returns>
    PduExLastErrorData LastError();

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
    /// Usually you don't need to change anything with this method
    /// </summary>
    /// <returns></returns>
    bool TryIoCtlClearTxQueue();

    /// <summary>
    /// Usually you don't need to change anything with this method
    /// </summary>
    /// <returns></returns>
    bool TryIoCtlSuspendTxQueue();

    /// <summary>
    /// Usually you don't need to change anything with this method
    /// </summary>
    /// <returns></returns>
    bool TryIoCtlResumeTxQueue();

    /// <summary>
    /// Usually you don't need to change anything with this method
    /// </summary>
    /// <returns></returns>
    bool TryIoCtlClearRxQueue();

    /// <summary>
    /// Usually you don't need to change anything with this method
    /// </summary>
    /// <returns></returns>
    bool TryIoCtlClearMsgFilter();

    /// <summary>
    /// For SAE J1850 VPW
    /// Usually you don't need to change anything with this method
    /// </summary>
    /// <returns></returns>
    bool TryIoCtlSendBreak();

    /// <summary>
    /// Sets the maximum buffer size of the received PDU on a ComLogicalLink
    /// Usually you don't need to change anything with this method
    /// </summary>
    /// <param name="value">maximum sizeof a received PDU for the ComLogicalLink</param>
    /// <returns>true or false</returns>
    bool TryIoCtlSetBufferSize(uint value);

    /// <summary>
    /// Stops the specified filter, based on filter number
    /// Usually you don't need to change anything with this method
    /// </summary>
    /// <param name="value">Filter number to stop</param>
    /// <returns>true or false</returns>
    bool TryIoCtlStopMsgFilter(uint value);

    /// <summary>
    ///  Starts filtering of incoming messages for the specified ComLogicalLink.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    bool TryIoCtlStartMsgFilter(PduIoCtlOfTypeFilterList value);

    /// <summary>
    ///     Attempts to restore the status of the ComLogicalLink
    ///     Catch all "Iso22900IIException" exceptions
    /// </summary>
    /// <returns></returns>
    bool TryToRecover(out string exMessage);

    /// <summary>
    ///     Alternative to Dispose()
    ///     The function name is based on ISO22900-2. And can be used if NO using keyword is used
    /// </summary>
    void DestroyComLogicalLink();

    void Dispose();
}
