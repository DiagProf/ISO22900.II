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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ISO22900.II
{
    public class DiagPduApiOneFactory : IDisposable
    {
        private static readonly Dictionary<string, DiagPduApiOneSysLevel> Cache = new();

        /// <summary>
        /// Creates or returns a cached DiagPduApiOneSysLevel instance for the given API library path.
        /// </summary>
        /// <param name="dPduApiLibraryPath">Full path to the D-PDU API library DLL.</param>
        /// <param name="loggerFactory">Logger factory for logging. Can be null for default logging.</param>
        /// <param name="optionStr">Optional string for D-PDU API options. Can be null or empty.</param>
        /// <param name="apiModFlags">API modification flags. Use ApiModifications.NONE for default behavior.</param>
        /// <param name="postPduConstructHook">
        /// Optional hook, called after PDU construct. Use for custom actions or debugging preferably not for production use yet.
        /// e.g.
        /// public void ObdEthernetCableAsVciPostConstructHook(DiagPduApiOneSysLevel apiSysLevel)
        /// {
        ///     // Example for a hook
        ///     // Make OBD Ethernet cable visible as VCI at vehicle connection
        ///     // The cable will then behave like a real VCI and can be selected normally.
        ///     // NOTE: A connection to the vehicle must already exist!

        ///     // For playful/testing purposes only. Do not use in production!
        ///     var ioCtlVehicleIdRequestData = new PduIoCtlVehicleIdRequestData(PduExPreselectionMode.NoPreselection, "", PduExCombinationMode.VinCombination, 5000);

        ///     if ( apiSysLevel.TryIoCtlVehicleIdRequest(ioCtlVehicleIdRequestData) )
        ///     {
        ///         //Some debug output if desired
        ///     }
        /// }
        /// </param>

        /// <returns></returns>
        ///
        public void ObdEthernetCableAsVciPostConstructHook(DiagPduApiOneSysLevel apiSysLevel)
        {
            // Example for a hook
            // Make OBD Ethernet cable visible as VCI at vehicle connection
            // The cable will then behave like a real VCI and can be selected normally.
            // NOTE: A connection to the vehicle must already exist!

            // For playful/testing purposes only. Do not use in production!
            var ioCtlVehicleIdRequestData = new PduIoCtlVehicleIdRequestData(PduExPreselectionMode.NoPreselection, "", PduExCombinationMode.VinCombination, 5000);

            if ( apiSysLevel.TryIoCtlVehicleIdRequest(ioCtlVehicleIdRequestData) )
            {
                //Some debug output if desired
            }
        }

        public static DiagPduApiOneSysLevel GetApi(string dPduApiLibraryPath, ILoggerFactory loggerFactory, string optionStr, ApiModifications apiModFlags = ApiModifications.NONE, Action<DiagPduApiOneSysLevel> postPduConstructHook = null)
        {
            lock ( Cache )
            {
                if ( !Cache.ContainsKey(dPduApiLibraryPath) )
                {
                    if ( !File.Exists(dPduApiLibraryPath) )
                    {
                        throw new DllNotFoundException($"DLL Not Found! {dPduApiLibraryPath}");
                    }

                    ApiLibLogging.LoggerFactory = loggerFactory ?? NullLoggerFactory.Instance;

                    //ToDo is a hack removed if safe code is ready
                    apiModFlags |= ApiModifications.UNSAFE_API;


                    var nwa = new Iso22900NativeWrapAccess(dPduApiLibraryPath, apiModFlags);
                    var sys = new DiagPduApiOneSysLevel(nwa, optionStr, apiModFlags, postPduConstructHook);

                    sys.Disposing += () =>
                    {
                        if ( !Cache.Remove(dPduApiLibraryPath) )
                        {
                            ApiLibLogging.LoggerFactory.CreateLogger<DiagPduApiOneFactory>().LogWarning(
                                "Remove PduApiLibraryPath {dPduApiLibraryPath} more than once", dPduApiLibraryPath);
                        }
                    };
                    Cache.Add(dPduApiLibraryPath, sys);
                }

                return Cache[dPduApiLibraryPath];
            }
        }

        public static DiagPduApiOneSysLevel GetApi(string dPduApiLibraryPath, string optionStr, ApiModifications apiModFlags = ApiModifications.UNSAFE_API, Action<DiagPduApiOneSysLevel> postPduConstructHook = null)
        {
            return GetApi(dPduApiLibraryPath, null, optionStr, apiModFlags, postPduConstructHook);
        }

        public static DiagPduApiOneSysLevel GetApi(string dPduApiLibraryPath, ILoggerFactory loggerFactory, ApiModifications apiModFlags = ApiModifications.UNSAFE_API, Action<DiagPduApiOneSysLevel> postPduConstructHook = null)
        {
            return GetApi(dPduApiLibraryPath, loggerFactory, null, apiModFlags, postPduConstructHook);
        }

        public static DiagPduApiOneSysLevel GetApi(string dPduApiLibraryPath, ApiModifications apiModFlags = ApiModifications.UNSAFE_API, Action<DiagPduApiOneSysLevel> postPduConstructHook = null)
        {
            return GetApi(dPduApiLibraryPath, null, null, apiModFlags, postPduConstructHook);
        }

        /// <summary>
        /// </summary>
        /// <param name="dPduApiLibraryPath"></param>
        /// <param name="apiModFlags"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="optionStr"></param>
        /// <param name="vciModuleName"></param>
        /// <returns></returns>
        public static Module GetVci(string dPduApiLibraryPath, ILoggerFactory loggerFactory, string optionStr, ApiModifications apiModFlags,
            string vciModuleName = "")
        {
            return GetApi(dPduApiLibraryPath, loggerFactory, optionStr, apiModFlags).ConnectVci(vciModuleName);
        }

        public static Module GetVci(string dPduApiLibraryPath, string optionStr, ApiModifications apiModFlags, string vciModuleName = "")
        {
            return GetApi(dPduApiLibraryPath, optionStr, apiModFlags).ConnectVci(vciModuleName);
        }

        public static Module GetVci(string dPduApiLibraryPath, ILoggerFactory loggerFactory, ApiModifications apiModFlags,
            string vciModuleName = "")
        {
            return GetApi(dPduApiLibraryPath, loggerFactory, apiModFlags).ConnectVci(vciModuleName);
        }

        public static Module GetVci(string dPduApiLibraryPath, ILoggerFactory loggerFactory, string optionStr, string vciModuleName = "")
        {
            return GetApi(dPduApiLibraryPath, loggerFactory, optionStr).ConnectVci(vciModuleName);
        }

        public static Module GetVci(string dPduApiLibraryPath, ApiModifications apiModFlags, string vciModuleName = "")
        {
            return GetApi(dPduApiLibraryPath, apiModFlags).ConnectVci(vciModuleName);
        }

        public static Module GetVci(string dPduApiLibraryPath, string vciModuleName = "")
        {
            return GetApi(dPduApiLibraryPath).ConnectVci(vciModuleName);
        }

        /// <summary>
        /// </summary>
        /// <param name="busTypeName"></param>
        /// <param name="protocolName"></param>
        /// <param name="dlcPinData"></param>
        /// <param name="dPduApiLibraryPath"></param>
        /// <param name="apiModFlags"></param>
        /// <param name="logger"></param>
        /// <param name="optionStr"></param>
        /// <param name="vciModuleName"></param>
        /// <returns></returns>
        public static ComLogicalLink GetCll(string dPduApiLibraryPath, string optionStr, ApiModifications apiModFlags, string vciModuleName,
            string busTypeName = "ISO_11898_2_DWCAN", string protocolName = "ISO_15765_3_on_ISO_15765_2",
            List<KeyValuePair<uint, string>> dlcPinData = null)
        {
            dlcPinData ??= new Dictionary<uint, string> { { 6, "HI" }, { 14, "LOW" } }.ToList();
            return GetApi(dPduApiLibraryPath, optionStr, apiModFlags).ConnectVci(vciModuleName)
                .OpenComLogicalLink(busTypeName, protocolName, dlcPinData);
        }

        public static ComLogicalLink GetCll(string dPduApiLibraryPath, ILoggerFactory loggerFactory, ApiModifications apiModFlags,
            string vciModuleName, string busTypeName = "ISO_11898_2_DWCAN", string protocolName = "ISO_15765_3_on_ISO_15765_2",
            List<KeyValuePair<uint, string>> dlcPinData = null)
        {
            dlcPinData ??= new Dictionary<uint, string> { { 6, "HI" }, { 14, "LOW" } }.ToList();
            return GetApi(dPduApiLibraryPath, loggerFactory, apiModFlags).ConnectVci(vciModuleName)
                .OpenComLogicalLink(busTypeName, protocolName, dlcPinData);
        }

        public static ComLogicalLink GetCll(string dPduApiLibraryPath, ILoggerFactory loggerFactory, string optionStr, string vciModuleName,
            string busTypeName = "ISO_11898_2_DWCAN", string protocolName = "ISO_15765_3_on_ISO_15765_2",
            List<KeyValuePair<uint, string>> dlcPinData = null)
        {
            dlcPinData ??= new Dictionary<uint, string> { { 6, "HI" }, { 14, "LOW" } }.ToList();
            return GetApi(dPduApiLibraryPath, loggerFactory, optionStr).ConnectVci(vciModuleName)
                .OpenComLogicalLink(busTypeName, protocolName, dlcPinData);
        }

        public static ComLogicalLink GetCll(string dPduApiLibraryPath, ILoggerFactory loggerFactory, string optionStr,
            ApiModifications apiModFlags, string busTypeName = "ISO_11898_2_DWCAN", string protocolName = "ISO_15765_3_on_ISO_15765_2",
            List<KeyValuePair<uint, string>> dlcPinData = null)
        {
            dlcPinData ??= new Dictionary<uint, string> { { 6, "HI" }, { 14, "LOW" } }.ToList();
            return GetApi(dPduApiLibraryPath, loggerFactory, optionStr).ConnectVci().OpenComLogicalLink(busTypeName, protocolName, dlcPinData);
        }

        public static ComLogicalLink GetCll(string dPduApiLibraryPath, ApiModifications apiModFlags, string vciModuleName,
            string busTypeName = "ISO_11898_2_DWCAN", string protocolName = "ISO_15765_3_on_ISO_15765_2",
            List<KeyValuePair<uint, string>> dlcPinData = null)
        {
            dlcPinData ??= new Dictionary<uint, string> { { 6, "HI" }, { 14, "LOW" } }.ToList();
            return GetApi(dPduApiLibraryPath, null, null).ConnectVci(vciModuleName).OpenComLogicalLink(busTypeName, protocolName, dlcPinData);
        }

        /// <summary>
        ///     With default for probably the most widely used protocol
        /// </summary>
        /// <param name="dPduApiLibraryPath"></param>
        /// <param name="vciModuleName"></param>
        /// <param name="busTypeName"></param>
        /// <param name="protocolName"></param>
        /// <param name="dlcPinData"></param>
        /// <returns></returns>
        public static ComLogicalLink GetCll(string dPduApiLibraryPath, string vciModuleName, string busTypeName = "ISO_11898_2_DWCAN",
            string protocolName = "ISO_15765_3_on_ISO_15765_2", List<KeyValuePair<uint, string>> dlcPinData = null)
        {
            dlcPinData ??= new Dictionary<uint, string> { { 6, "HI" }, { 14, "LOW" } }.ToList();
            return GetApi(dPduApiLibraryPath, null, null).ConnectVci(vciModuleName).OpenComLogicalLink(busTypeName, protocolName, dlcPinData);
        }

        // public static ComLogicalLinkLevel GetCll(string dPduApiLibraryPath, string busTypeName = "ISO_11898_2_DWCAN", string protocolName = "ISO_15765_3_on_ISO_15765_2", List<KeyValuePair<uint, string>> dlcPinData = null)
        // {
        //     dlcPinData ??= new Dictionary<uint, string> { { 6, "HI" }, { 14, "LOW" } }.ToList();
        //     return GetAPI(dPduApiLibraryPath, null, null).ConnectVci("").OpenComLogicalLink(busTypeName, protocolName, dlcPinData);
        // }

        #region DisposeBehavior

        public void Dispose()
        {
            foreach ( var sys in Cache.Values.ToArray() )
            {
                sys.Dispose();
            }
        }

        #endregion
    }
}
