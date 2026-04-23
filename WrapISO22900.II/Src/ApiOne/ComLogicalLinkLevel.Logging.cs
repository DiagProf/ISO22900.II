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
using Microsoft.Extensions.Logging;

namespace ISO22900.II
{
    // Why is payload logging built into the wrapper here instead of the application?
    // After repeatedly catching myself adding payload logging in my own applications
    // (because ECUs almost never do exactly what the spec says, especially with older protocols :-) ),
    // I decided this wrapper is the right place for it — it is the best spot for reuse.
    //
    // Depending on the configured log level, this does affect runtime behavior slightly.
    // Therefore, note the following:
    //
    //   - If no logger is set in ApiOne, nothing is logged at all.
    //   - For a production application running at Warning level, nothing is logged here either.
    //   - In support cases I typically ask the user to switch from Warning to Information.
    //     That is why payload logging starts at Information level.
    //
    // Even at Information, payloads are truncated at 260 bytes.
    // Most protocol issues show up in the service ID or routine responses,
    // not in long transfers like flashing — and I do not want the log file to explode in size.
    //
    // Because there is no built-in formatter for byte arrays, I had to write one here.
    // That naturally led to the idea of allowing a bit more at Debug level (up to 4100 bytes),
    // in case a deeper look at a longer payload is needed.
    //
    // To keep the log easy to parse later (e.g. for building a simulation from recorded data),
    // I chose "XX" as a deterministic truncation marker at the end of a cut-off payload.
    // That way a parser always knows: two characters, then a space — no ambiguous free text.
    internal partial class ComLogicalLinkLevel
    {
        private readonly ILogger _logger = ApiLibLogging.CreateLogger<ComLogicalLinkLevel>();

        private void LogStartComPrimitive(uint moduleHandle, uint comLogicalLinkHandle, uint comPrimitiveHandle, PduCopt pduCopType, byte[]? copData, PduCopCtrlData copCtrlData, uint copTag)
        {
            if (!_logger.IsEnabled(LogLevel.Information))
                return;

            var maxBytes = _logger.IsEnabled(LogLevel.Debug) ? 4100 : 260;
            var payloadRealSize = copData?.Length ?? 0;
            var payload = FormatByteArrayForLogging(copData, maxBytes);
            var txFlagHex = FormatByteArrayForLogging(copCtrlData.TxFlag.FlagData);
            var customExpectedResponses = HasNonDefaultExpectedResponseDatas(copCtrlData.PduExpectedResponseDatas);

            LogStartComPrimitiveCore(moduleHandle, comLogicalLinkHandle, comPrimitiveHandle, pduCopType, payloadRealSize, payload,
                copCtrlData.NumSendCycles, copCtrlData.NumReceiveCycles, copCtrlData.TempParamUpdate,
                txFlagHex, copCtrlData.Time, customExpectedResponses, copTag);
        }

        [LoggerMessage(Level = LogLevel.Information,
            Message = """
            ComPrimitive start:  hMod=0x{moduleHandle:X8}, hCLL=0x{comLogicalLinkHandle:X8}, hCoP=0x{comPrimitiveHandle:X8}, payloadRealSize={payloadRealSize}, payload=[{payload}], pduCopType={pduCopType}, numSendCycles={numSendCycles}, numReceiveCycles={numReceiveCycles}, tempParamUpdate={tempParamUpdate}, txFlag=[{txFlagHex}], time={time}ms, customExpectedResponses={customExpectedResponses}, copTag={copTag}
            """)]
        partial void LogStartComPrimitiveCore(uint moduleHandle, uint comLogicalLinkHandle, uint comPrimitiveHandle, PduCopt pduCopType, int payloadRealSize, string payload, int numSendCycles, int numReceiveCycles, uint tempParamUpdate, string txFlagHex, uint time, bool customExpectedResponses, uint copTag);



        [LoggerMessage(Level = LogLevel.Information, 
            Message = """
            ComPrimitive status: hMod=0x{moduleHandle:X8}, hCLL=0x{comLogicalLinkHandle:X8}, hCoP=0x{comPrimitiveHandle:X8}, status={status}, timestamp={timestamp}µs, copTag={copTag}
            """)]
        partial void LogComPrimitiveStatus(uint moduleHandle, uint comLogicalLinkHandle, uint comPrimitiveHandle, PduStatus status, uint timestamp, uint copTag);



        private void LogComPrimitiveResult(uint moduleHandle, uint comLogicalLinkHandle, uint comPrimitiveHandle, byte[]? dataBytes, PduFlagDataRxFlag rxFlag, uint uniqueRespIdentifier, uint timestamp, uint copTag)
        {
            if (!_logger.IsEnabled(LogLevel.Information))
                return;

            var maxBytes = _logger.IsEnabled(LogLevel.Debug) ? 4100 : 260;
            var payloadRealSize = dataBytes?.Length ?? 0;
            var payload = FormatByteArrayForLogging(dataBytes, maxBytes);
            var rxFlagHex = FormatByteArrayForLogging(rxFlag.FlagData);

            LogComPrimitiveResultCore(moduleHandle, comLogicalLinkHandle, comPrimitiveHandle, payloadRealSize, payload, rxFlagHex, uniqueRespIdentifier, timestamp, copTag);
        }

        [LoggerMessage(Level = LogLevel.Information, 
            Message = """
            ComPrimitive result: hMod=0x{moduleHandle:X8}, hCLL=0x{comLogicalLinkHandle:X8}, hCoP=0x{comPrimitiveHandle:X8}, payloadRealSize={payloadRealSize}, payload=[{payload}], rxFlag=[{rxFlagHex}], uniqueRespIdentifier=0x{uniqueRespIdentifier:X8}, timestamp={timestamp}µs, copTag={copTag}
            """)]
        partial void LogComPrimitiveResultCore(uint moduleHandle, uint comLogicalLinkHandle, uint comPrimitiveHandle, int payloadRealSize, string payload, string rxFlagHex, uint uniqueRespIdentifier, uint timestamp, uint copTag);

        // Information level (not Warning) because a ComPrimitive error only makes sense in context
        // with the preceding ComPrimitive start & status, which is also logged at Information level.
        // If the user has not switched to Information for support, seeing an isolated error
        // entry at Warning without the corresponding request would be more confusing than helpful.
        [LoggerMessage(Level = LogLevel.Information,
            Message = """
            ComPrimitive error:  hMod=0x{moduleHandle:X8}, hCLL=0x{comLogicalLinkHandle:X8}, hCoP=0x{comPrimitiveHandle:X8}, errorCodeId={errorCodeId}, extraErrorInfoId=0x{extraErrorInfoId:X8}, timestamp={timestamp}µs, copTag={copTag}
            """)]
        partial void LogComPrimitiveError(uint moduleHandle, uint comLogicalLinkHandle, uint comPrimitiveHandle, PduErrEvt errorCodeId, uint extraErrorInfoId, uint timestamp, uint copTag);

        // Information level — same reasoning as LogComPrimitiveError: only meaningful in context
        // with the surrounding ComPrimitive start/result entries at Information level.
        [LoggerMessage(Level = LogLevel.Information,
            Message = """
            ComPrimitive info:   hMod=0x{moduleHandle:X8}, hCLL=0x{comLogicalLinkHandle:X8}, hCoP=0x{comPrimitiveHandle:X8}, infoCode={infoCode}, extraInfoData=0x{extraInfoData:X8}, timestamp={timestamp}µs, copTag={copTag}
            """)]
        partial void LogComPrimitiveInfo(uint moduleHandle, uint comLogicalLinkHandle, uint comPrimitiveHandle, PduInfo infoCode, uint extraInfoData, uint timestamp, uint copTag);

        [LoggerMessage(Level = LogLevel.Warning,
            Message = "The ComLogicalLink can no longer be reached. This could be normal if a VCI was lost previously.")]
        partial void LogComLogicalLinkUnreachable();

        [LoggerMessage(Level = LogLevel.Information,
            Message = "Object-Id for ComParam {comParamShortName} not defined in the API used")]
        partial void LogComParamObjectIdUndefined(string comParamShortName);

        [LoggerMessage(Level = LogLevel.Warning,
            Message = "Trouble with ComParam {comParamShortName}: Object-Id not supported by this API")]
        partial void LogComParamObjectIdNotSupported(Exception ex, string comParamShortName);

        [LoggerMessage(Level = LogLevel.Warning,
            Message = "ComParam {comParamShortName} not supported (doesn't have to be bad)")]
        partial void LogComParamNotSupported(string comParamShortName);

        [LoggerMessage(Level = LogLevel.Warning,
            Message = "Trouble with ComParam {comParamShortName}")]
        partial void LogComParamTrouble(Exception ex, string comParamShortName);

        [LoggerMessage(Level = LogLevel.Critical,
            Message = "Page with index {pageIndex} does not exist!")]
        partial void LogPageIndexOutOfRange(Exception ex, uint pageIndex);

        [LoggerMessage(Level = LogLevel.Warning,
            Message = "IoCtl {ioCtlShortName} failed")]
        partial void LogIoCtlFailed(Exception ex, string ioCtlShortName);

        [LoggerMessage(Level = LogLevel.Warning,
            Message = "Can't read the state of ComLogicalLink: {error}")]
        partial void LogCllStateReadFailed(string error);

        [LoggerMessage(Level = LogLevel.Error,
            Message = "Forgot to put the ComLogicalLink in the offline state with Disconnect()?")]
        partial void LogCllNotDisconnected();

        [LoggerMessage(Level = LogLevel.Warning,
            Message = "Can't UnRegisterEventDataCallback: {error}")]
        partial void LogUnRegisterEventDataCallbackFailed(string error);

        [LoggerMessage(Level = LogLevel.Warning,
            Message = "Can't PduDestroyComLogicalLink: {error}")]
        partial void LogDestroyComLogicalLinkFailed(string error);

        [LoggerMessage(Level = LogLevel.Error,
            Message = "Forgot to put the ComLogicalLink in the offline state with Disconnect() (Vector workaround)")]
        partial void LogCllNotDisconnectedVectorWorkaround();

        [LoggerMessage(Level = LogLevel.Warning,
            Message = "Can't PduDisconnect (Vector workaround): {error}")]
        partial void LogPduDisconnectVectorWorkaroundFailed(string error);

        [LoggerMessage(Level = LogLevel.Warning,
            Message = "Can't UnRegisterEventDataCallback (Vector workaround): {error}")]
        partial void LogUnRegisterEventDataCallbackVectorWorkaroundFailed(string error);


        // Returns true only if PduExpectedResponseDatas contains something other than the typical
        // default: a single PositiveResponse entry with empty mask/pattern and no UniqueRespIds.
        // That default is used in ~90% of cases, so logging true only for the non-default cases
        // keeps the log uncluttered while still surfacing custom response filtering when it matters.
        // The "default" referred to here is exactly what ComLogicalLink uses as its ready-to-use
        // control data (_readyToUseCopControl):
        //   new PduExpectedResponseData(PduExResponseType.PositiveResponse, 1,
        //       new MaskAndPatternBytes(new byte[] {}, new byte[] {}),
        //       new UniqueRespIds(new uint[] {}))
        private static bool HasNonDefaultExpectedResponseDatas(PduExpectedResponseData[] datas)
        {
            if (datas.Length == 0)
                return false;
            if (datas.Length > 1)
                return true;

            // Only Length == 1 reaches here — 0 and > 1 are already handled by the guards above.
            // Since Length cannot be negative, this is the only remaining case, so datas[0] is always safe.
            var d = datas[0];
            return d.ResponseType != PduExResponseType.PositiveResponse
                   || d.MaskAndPatternPair.NumMaskPatternBytes != 0
                   || d.UniqueRespIds.NumberOfUniqueRespIds != 0;
        }

        private static string FormatByteArrayForLogging(byte[]? bytes, int maxBytes = 260)
        {
            // Return empty string if input is null or empty
            if (bytes == null || bytes.Length == 0)
                return string.Empty;

            // Limit processing to maxBytes
            int count = Math.Min(bytes.Length, maxBytes);
            bool truncated = bytes.Length > maxBytes;

            // Each byte needs 2 hex chars + 1 space, add " XX" if truncated (4 chars)
            int stringLength = count * 3 - 1 + (truncated ? 4 : 0);

            // string.Create is the fastest way to allocate and fill a string in one go
            return string.Create(stringLength, (bytes, count, truncated), static (chars, state) =>
            {
                var (b, cnt, trunc) = state;

                for (int i = 0; i < cnt; i++)
                {
                    // Format current byte as 2-digit uppercase hex directly into the string buffer
                    b[i].TryFormat(chars.Slice(i * 3), out _, "X2");

                    // Add a space after each hex pair, except for the last one (or if truncated)
                    if (i < cnt - 1)
                    {
                        chars[i * 3 + 2] = ' ';
                    }
                }

                // Add " XX" suffix if truncated (parser-friendly indicator)
                if (trunc)
                {
                    int pos = cnt * 3 - 1;
                    chars[pos++] = ' ';
                    chars[pos++] = 'X';
                    chars[pos++] = 'X';
                }
            });
        }
    }
}
