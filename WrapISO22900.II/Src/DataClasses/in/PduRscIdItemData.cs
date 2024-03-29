﻿#region License

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

namespace ISO22900.II
{
    public class PduRscIdItemData
    {
        //ToDo resolve the ModuleHandle (ModuleHandle -> ModuleName)
        //because the handles are not accessible to the user or in other words the user can't do anything with the handle because there is no function that he could give the handle to
        /// <summary>
        ///     Vendor specific information string for the unique module identification. * E.g. Module serial number or Module
        ///     friendly name
        /// </summary>
        public string VendorModuleName { get; internal set; } = "";


        /// <summary>
        /// handle of MVCI Protocol Module assigned by D-PDU API
        /// </summary>
        internal uint ModuleHandle { get; }

        /// <summary>
        /// number of resource Ids
        /// </summary>
        public uint NumberOfResourceIds => (uint)ResourceIdArray.Length;

        public uint this[int index]
        {
            get => ResourceIdArray[index];
        }

        internal PduRscIdItemData(uint moduleHandle, uint[] resourceIds)
        {
            ModuleHandle = moduleHandle;
            ResourceIdArray = resourceIds;
        }

        private uint[] ResourceIdArray { get; init; }
    }
}