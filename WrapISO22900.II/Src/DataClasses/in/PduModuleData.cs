// ReSharper disable IdentifierTypo

using System;
using System.Collections.Generic;
using System.Linq;

namespace ISO22900.II
{
    // Item for module identification (used by PDUGetModuleIds)
    public class PduModuleData
    {
        /// <summary>
        ///     MVCI Protocol ModuleTypeId.
        /// </summary>
        public uint ModuleTypeId { get; }

        /// <summary>
        ///     handle of MVCI Protocol Module assigned by D-PDU API
        /// </summary>
        internal uint ModuleHandle { get; }

        /// <summary>
        ///     Vendor specific information string for the unique module identification. * E.g. Module serial number or Module
        ///     friendly name
        /// </summary>
        public string VendorModuleName { get; }

        /// <summary>
        ///     Vendor specific additional information string
        /// </summary>
        public string VendorAdditionalInfo { get; }

        /// <summary>
        ///     Status of MVCI Protocol Module detected by D-PDU API session
        /// </summary>
        public PduStatus ModuleStatus { get; }


        internal PduModuleData(uint moduleTypeId, uint moduleHandle, string vendorModuleName, string vendorAdditionalInfo,
            PduStatus moduleStatus)
        {
            ModuleTypeId = moduleTypeId;
            ModuleHandle = moduleHandle;
            VendorModuleName = vendorModuleName;
            VendorAdditionalInfo = vendorAdditionalInfo;
            ModuleStatus = moduleStatus;
        }
    }
}
