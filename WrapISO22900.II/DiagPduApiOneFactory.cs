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
        ///     Constructs the api
        /// </summary>
        /// <param name="dPduApiLibraryPath"></param>
        /// <param name="apiModFlags"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="optionStr"></param>
        /// <returns></returns>
        public static DiagPduApiOneSysLevel GetApi(string dPduApiLibraryPath, ILoggerFactory loggerFactory, string optionStr, ApiModifications apiModFlags = ApiModifications.UNSAFE_API)
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
                    var sys = new DiagPduApiOneSysLevel(nwa, optionStr, apiModFlags);

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

        public static DiagPduApiOneSysLevel GetApi(string dPduApiLibraryPath, string optionStr, ApiModifications apiModFlags = ApiModifications.UNSAFE_API)
        {
            return GetApi(dPduApiLibraryPath, null, optionStr, apiModFlags);
        }

        public static DiagPduApiOneSysLevel GetApi(string dPduApiLibraryPath, ILoggerFactory loggerFactory, ApiModifications apiModFlags = ApiModifications.UNSAFE_API)
        {
            return GetApi(dPduApiLibraryPath, loggerFactory, null, apiModFlags);
        }

        public static DiagPduApiOneSysLevel GetApi(string dPduApiLibraryPath, ApiModifications apiModFlags = ApiModifications.UNSAFE_API)
        {
            return GetApi(dPduApiLibraryPath, null, null, apiModFlags);
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
