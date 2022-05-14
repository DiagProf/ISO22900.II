using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;


// ReSharper disable InconsistentNaming

namespace ISO22900.II
{
    public class Iso22900NativeWrapAccess : IDisposable
    {
        private readonly object sync = new();

        private readonly ApiCallPduConstruct _apiCallPduConstruct;
        private readonly ApiCallPduDestruct _apiCallPduDestruct;
        private readonly ApiCallPduModuleConnect _apiCallPduModuleConnect;
        private readonly ApiCallPduModuleDisconnect _apiCallPduModuleDisconnect;
        private readonly ApiCallPduCreateComLogicalLink _apiCallPduCreateComLogicalLink;
        private readonly ApiCallPduDestroyComLogicalLink _apiCallPduDestroyComLogicalLink;
        private readonly ApiCallPduGetModuleIds _apiCallPduGetModuleIds;
        private readonly ApiCallPduGetEventItem _apiCallPduGetEventItem;
        private readonly ApiCallPduRegisterEventCallback _apiCallPduRegisterEventCallback;
        private readonly ApiCallPduGetStatus _apiCallPduGetStatus;
        private readonly ApiCallPduGetTimestamp _apiCallPduGetTimestamp;
        private readonly ApiCallPduGetObjectId _apiCallPduGetObjectId;
        private readonly ApiCallPduGetComParam _apiCallPduGetComParam;
        private readonly ApiCallPduSetComParam _apiCallPduSetComParam;
        private readonly ApiCallPduSetViaGetComParam _apiCallPduSetViaGetComParam;
        private readonly ApiCallPduGetUniqueRespIdTable _apiCallPduGetUniqueRespIdTable;
        private readonly ApiCallPduSetUniqueRespIdTable _apiCallPduSetUniqueRespIdTable;
        private readonly ApiCallPduConnect _apiCallPduConnect;
        private readonly ApiCallPduDisconnect _apiCallPduDisconnect;
        private readonly ApiCallPduStartComPrimitive _apiCallPduStartComPrimitive;
        private readonly ApiCallPduCancelComPrimitive _apiCallPduCancelComPrimitive;
        private readonly ApiCallPduGetVersion _apiCallPduGetVersion;
        private readonly ApiCallPduGetResourceIds _apiCallPduGetResourceIds;
        private readonly ApiCallPduIoCtl _apiCallPduIoCtl;

        private bool disposedValue;
        private IntPtr _handleNativeLibrary;

        private readonly ILogger _logger = ApiLibLogging.CreateLogger<Iso22900NativeWrapAccess>();
        private string LibraryPath { get; }

        public Iso22900NativeWrapAccess(string libraryPath) : this(libraryPath, ApiModifications.UNSAFE_API)
        {
        }

        public Iso22900NativeWrapAccess(string libraryPath, ApiModifications ApiModFlags)
        {
            LibraryPath = libraryPath;
            _logger.LogInformation("Loading: {LibraryPath}", LibraryPath);
            _handleNativeLibrary = NativeLibrary.Load(libraryPath);

            _apiCallPduConstruct = new ApiCallFactoryPduConstruct(ApiModFlags, _handleNativeLibrary).GetApiCall();
            _apiCallPduDestruct = new ApiCallFactoryPduDestruct(ApiModFlags, _handleNativeLibrary).GetApiCall();
            _apiCallPduModuleConnect = new ApiCallFactoryPduModuleConnect(ApiModFlags, _handleNativeLibrary).GetApiCall();
            _apiCallPduModuleDisconnect = new ApiCallFactoryPduModuleDisconnect(ApiModFlags, _handleNativeLibrary).GetApiCall();
            _apiCallPduCreateComLogicalLink = new ApiCallFactoryPduCreateComLogicalLink(ApiModFlags, _handleNativeLibrary).GetApiCall();
            _apiCallPduDestroyComLogicalLink = new ApiCallFactoryPduDestroyComLogicalLink(ApiModFlags, _handleNativeLibrary).GetApiCall();
            _apiCallPduGetModuleIds = new ApiCallFactoryPduGetModuleIds(ApiModFlags, _handleNativeLibrary).GetApiCall();
            _apiCallPduGetEventItem = new ApiCallFactoryPduGetEventItem(ApiModFlags, _handleNativeLibrary).GetApiCall();
            _apiCallPduRegisterEventCallback = new ApiCallFactoryPduRegisterEventCallback(ApiModFlags, _handleNativeLibrary).GetApiCall();
            _apiCallPduGetStatus = new ApiCallFactoryPduGetStatus(ApiModFlags, _handleNativeLibrary).GetApiCall();
            _apiCallPduGetTimestamp = new ApiCallFactoryPduGetTimestamp(ApiModFlags, _handleNativeLibrary).GetApiCall();
            _apiCallPduGetObjectId = new ApiCallFactoryPduGetObjectId(ApiModFlags, _handleNativeLibrary).GetApiCall();
            _apiCallPduGetComParam = new ApiCallFactoryPduGetComParam(ApiModFlags, _handleNativeLibrary).GetApiCall();
            _apiCallPduSetComParam = new ApiCallFactoryPduSetComParam(ApiModFlags, _handleNativeLibrary).GetApiCall();
            _apiCallPduSetViaGetComParam = new ApiCallFactoryPduSetViaGetComParam(ApiModFlags, _handleNativeLibrary).GetApiCall();
            _apiCallPduGetUniqueRespIdTable = new ApiCallFactoryPduGetUniqueRespIdTable(ApiModFlags, _handleNativeLibrary).GetApiCall();
            _apiCallPduSetUniqueRespIdTable = new ApiCallFactoryPduSetUniqueRespIdTable(ApiModFlags, _handleNativeLibrary).GetApiCall();
            _apiCallPduConnect = new ApiCallFactoryPduConnect(ApiModFlags, _handleNativeLibrary).GetApiCall();
            _apiCallPduDisconnect = new ApiCallFactoryPduDisconnect(ApiModFlags, _handleNativeLibrary).GetApiCall();
            _apiCallPduStartComPrimitive = new ApiCallFactoryPduStartComPrimitive(ApiModFlags, _handleNativeLibrary).GetApiCall();
            _apiCallPduCancelComPrimitive = new ApiCallFactoryPduCancelComPrimitive(ApiModFlags, _handleNativeLibrary).GetApiCall();
            _apiCallPduGetVersion = new ApiCallFactoryPduGetVersion(ApiModFlags, _handleNativeLibrary).GetApiCall();
            _apiCallPduGetResourceIds = new ApiCallFactoryPduGetResourceIds(ApiModFlags, _handleNativeLibrary).GetApiCall();
            _apiCallPduIoCtl = new ApiCallFactoryPduIoCtl(ApiModFlags, _handleNativeLibrary).GetApiCall();
        }

        public void PduConstruct()
        {
            lock ( sync )
            {
                _apiCallPduConstruct.PduConstruct();
            }
        }

        public void PduConstruct(string optionStr)
        {
            lock ( sync )
            {
                _apiCallPduConstruct.PduConstruct(optionStr);
            }
        }

        public void PduConstruct(uint apiTag)
        {
            lock ( sync )
            {
                _apiCallPduConstruct.PduConstruct(apiTag);
            }
        }

        public void PduDestruct()
        {
            lock ( sync )
            {
                _apiCallPduDestruct.PduDestruct();
            }
        }

        public List<PduModuleData> PduGetModuleIds()
        {
            lock ( sync )
            {
                return _apiCallPduGetModuleIds.PduGetModuleIds();
            }
        }

        public PduExStatusData PduGetStatus(uint moduleHandle, uint comLogicalLinkHandle, uint comPrimitiveHandle)
        {
            lock ( sync )
            {
                return _apiCallPduGetStatus.PduGetStatus(moduleHandle, comLogicalLinkHandle, comPrimitiveHandle);
            }
        }

        public void PduModuleConnect(uint moduleHandle)
        {
            lock ( sync )
            {
                _apiCallPduModuleConnect.PduModuleConnect(moduleHandle);
            }
        }

        public void PduModuleDisconnect(uint moduleHandle)
        {
            lock ( sync )
            {
                _apiCallPduModuleDisconnect.PduModuleDisconnect(moduleHandle);
            }
        }

        public uint PduCreateComLogicalLink(uint moduleHandle, PduResourceData pduResourceData,
            uint resourceId, uint cllTag, PduFlagDataCllCreateFlag pduFlagDataCllCreateFlag)
        {
            lock ( sync )
            {
                return _apiCallPduCreateComLogicalLink.PduCreateComLogicalLink(moduleHandle, pduResourceData,
                    resourceId, cllTag, pduFlagDataCllCreateFlag);
            }
        }

        public void PduDestroyComLogicalLink(uint moduleHandle, uint comLogicalLinkHandle)
        {
            lock ( sync )
            {
                _apiCallPduDestroyComLogicalLink.PduDestroyComLogicalLink(moduleHandle, comLogicalLinkHandle);
            }
        }

        public PduComParam PduGetComParam(uint moduleHandle, uint comLogicalLinkHandle, uint objectId)
        {
            lock ( sync )
            {
                return _apiCallPduGetComParam.PduGetComParam(moduleHandle, comLogicalLinkHandle, objectId);
            }
        }

        public void PduSetComParam(uint moduleHandle, uint comLogicalLinkHandle, PduComParam cp)
        {
            lock ( sync )
            {
                _apiCallPduSetComParam.PduSetComParam(moduleHandle, comLogicalLinkHandle, cp);
            }
        }

        public PduComParam PduSetViaGetComParam(uint moduleHandle, uint comLogicalLinkHandle, uint objectId, long value)
        {
            lock ( sync )
            {
               return _apiCallPduSetViaGetComParam.PduSetViaGetComParam(moduleHandle, comLogicalLinkHandle, objectId, value);
            }
        }

        public PduComParam PduSetViaGetComParam(uint moduleHandle, uint comLogicalLinkHandle, uint objectId, byte[] value)
        {
            lock ( sync )
            {
                return _apiCallPduSetViaGetComParam.PduSetViaGetComParam(moduleHandle, comLogicalLinkHandle, objectId, value);
            }
        }

        public PduComParam PduSetViaGetComParam(uint moduleHandle, uint comLogicalLinkHandle, uint objectId, uint[] value)
        {
            lock ( sync )
            {
               return _apiCallPduSetViaGetComParam.PduSetViaGetComParam(moduleHandle, comLogicalLinkHandle, objectId, value);
            }
        }

        public PduComParam PduSetViaGetComParam(uint moduleHandle, uint comLogicalLinkHandle, uint objectId, PduParamStructFieldData value)
        {
            lock ( sync )
            {
               return _apiCallPduSetViaGetComParam.PduSetViaGetComParam(moduleHandle, comLogicalLinkHandle, objectId, value);
            }
        }

        public void PduSetUniqueRespIdTable(uint moduleHandle, uint comLogicalLinkHandle, List<PduEcuUniqueRespData> ecuUniqueRespDatas)
        {
            lock ( sync )
            {
                _apiCallPduSetUniqueRespIdTable.PduSetUniqueRespIdTable(moduleHandle, comLogicalLinkHandle, ecuUniqueRespDatas);
            }
        }

        public List<PduEcuUniqueRespData> PduGetUniqueRespIdTable(uint moduleHandle, uint comLogicalLinkHandle)
        {
            lock ( sync )
            {
                return _apiCallPduGetUniqueRespIdTable.PduGetUniqueRespIdTable(moduleHandle, comLogicalLinkHandle);
            }
        }

        public uint PduStartComPrimitive(uint moduleHandle, uint comLogicalLinkHandle, PduCopt pduCopType, byte[] copData, PduCopCtrlData copCtrlData,
            uint copTag)
        {
            lock ( sync )
            {
                return _apiCallPduStartComPrimitive.PduStartComPrimitive(moduleHandle, comLogicalLinkHandle, pduCopType, copData, copCtrlData,
                    copTag);
            }
        }

        public void PduCancelComPrimitive(uint moduleHandle, uint comLogicalLinkHandle, uint comPrimitiveHandle)
        {
            lock ( sync )
            {
                _apiCallPduCancelComPrimitive.PduCancelComPrimitive(moduleHandle, comLogicalLinkHandle, comPrimitiveHandle);
            }
        }

        /// <summary>
        ///     Physically connects the previously created ComLogicalLink to the communication line.
        /// </summary>
        /// <param name="moduleHandle"></param>
        /// <param name="comLogicalLinkHandle"></param>
        public void PduConnect(uint moduleHandle, uint comLogicalLinkHandle)
        {
            lock ( sync )
            {
                _apiCallPduConnect.PduConnect(moduleHandle, comLogicalLinkHandle);
            }
        }

        /// <summary>
        ///     Physically disconnect ComLogicalLink from vehicle interface.
        ///     After calling the function, no more communication to the vehicle ECU may take place.
        ///     But the ComLogicalLink is not destroyed. ComLogicalLink can go back online with PduConnect.
        ///     Or finally destroyed with PDUDestroyComLogicalLink
        /// </summary>
        /// <param name="moduleHandle"></param>
        /// <param name="comLogicalLinkHandle"></param>
        public void PduDisconnect(uint moduleHandle, uint comLogicalLinkHandle)
        {
            lock ( sync )
            {
                _apiCallPduDisconnect.PduDisconnect(moduleHandle, comLogicalLinkHandle);
            }
        }

        public void PduRegisterEventCallback(uint moduleHandle, uint comLogicalLinkHandle,
            Action<PduEvtData, uint, uint, uint, uint> eventHandler)
        {
            lock ( sync )
            {
                _apiCallPduRegisterEventCallback.PduRegisterEventCallback(moduleHandle, comLogicalLinkHandle, eventHandler);
            }
        }

        public uint PduGetTimestamp(uint moduleHandle)
        {
            lock ( sync )
            {
                return _apiCallPduGetTimestamp.PduGetTimestamp(moduleHandle);
            }
        }

        /// <summary>
        ///     The id will be set to PduConst.PDU_ID_UNDEF if the D‐PDU API has no valid Object Id for the requested object type
        ///     and shortname.
        /// </summary>
        /// <param name="pduObjectType"></param>
        /// <param name="shortName"></param>
        /// <returns>id</returns>
        public uint PduGetObjectId(PduObjt pduObjectType, string shortName)
        {
            lock ( sync )
            {
                return _apiCallPduGetObjectId.PduGetObjectId(pduObjectType, shortName);
            }
        }

        public Queue<PduEventItem> PduGetEventItem(uint moduleHandle, uint comLogicalLinkHandle)
        {
            lock ( sync )
            {
                return _apiCallPduGetEventItem.PduGetEventItem(moduleHandle, comLogicalLinkHandle);
            }
        }

        public PduVersionData PduGetVersionData(uint moduleHandle)
        {
            lock ( sync )
            {
                return _apiCallPduGetVersion.PduGetVersion(moduleHandle);
            }
        }

        public List<PduRscIdItemData> PduGetResourceIds(uint moduleHandle, PduResourceData pduResourceData)
        {
            lock ( sync )
            {
                return _apiCallPduGetResourceIds.PduGetResourceIds(moduleHandle, pduResourceData);
            }
        }

        public PduIoCtl PduIoCtl(uint moduleHandle, uint comLogicalLinkHandle, uint ioCtlCommandId, PduIoCtl pduIoCtlData)
        {
            lock ( sync )
            {
                return _apiCallPduIoCtl.PduIoCtl(moduleHandle, comLogicalLinkHandle, ioCtlCommandId, pduIoCtlData);
            }
        }

        #region DisposeBehavior

        public void Dispose()
        {
            // If this function is being called the user wants to release the
            // resources. lets call the Dispose which will do this for us.
            Dispose(true);

            // Now since we have done the cleanup already there is nothing left
            // for the Finalizer to do. So lets tell the GC not to call it later.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if ( !disposedValue )
            {
                if ( disposing )
                {
                    // TODO: dispose managed state (managed objects)
                }

                // free unmanaged resources (unmanaged objects) and override finalizer
                if ( _handleNativeLibrary != IntPtr.Zero )
                {
                    _logger.LogInformation("Unloading: {LibraryPath}", LibraryPath);
                    NativeLibrary.Free(_handleNativeLibrary);
                    _handleNativeLibrary = IntPtr.Zero;
                }

                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        ~Iso22900NativeWrapAccess()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(false);
        }

        #endregion
    }
}
