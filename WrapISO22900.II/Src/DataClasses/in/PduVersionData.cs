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
using System.Linq;
using System.Text;

namespace ISO22900.II
{
    public class PduVersionData
    {
        private readonly uint _pduApiSwDate;
        private readonly uint _pduApiSwVersion;
        private readonly uint _mvciPart1StandardVersion;
        private readonly uint _mvciPart2StandardVersion;
        private readonly uint _hwVersion;
        private readonly uint _hwDate;
        private readonly uint _fwVersion;
        private readonly uint _fwDate;

        /// <summary>
        /// Release version of supported MVCI Part 1 standard
        /// </summary>
        public string MvciPart1StandardVersion => VersionNumberToString(_mvciPart1StandardVersion);
        
        /// <summary>
        /// Release version of supported MVCI Part 2 standard
        /// </summary>
        public string MvciPart2StandardVersion => VersionNumberToString(_mvciPart2StandardVersion);
        
        /// <summary>
        /// Unique Serial number of MVCI HW module from a vendor
        /// </summary>
        public uint HwSerialNumber { get; }
        
        /// <summary>
        /// Name of MVCI HW module
        /// </summary>
        public string HwName { get; }
        
        /// <summary>
        /// Version number of MVCI HW module
        /// </summary>
        public string HwVersion => VersionNumberToString(_hwVersion);
        
        /// <summary>
        /// Manufacturing date of MVCI HW module
        /// </summary>
        public string HwDate => DateNumberToString(_hwDate);
        
        /// <summary>
        /// Type of MVCI HW module (Manufacture is aware of this)
        /// </summary>
        public uint HwInterface { get; }
        
        /// <summary>
        /// Name of the firmware available in the MVCI HW module
        /// </summary>
        public string FwName { get; }
        
        /// <summary>
        /// Version number of MVCI HW module
        /// </summary>
        public string FwVersion => VersionNumberToString(_fwVersion);
        
        /// <summary>
        /// Manufacturing date of the firmware in the MVCI HW module
        /// </summary>
        public string FwDate => DateNumberToString(_fwDate);
        
        /// <summary>
        /// Name of vendor
        /// </summary>
        public string VendorName { get; }
        
        /// <summary>
        /// Name of the D-PDU API software
        /// </summary>
        public string PduApiSwName { get; }
        
        /// <summary>
        /// Version number of D-PDU API software
        /// </summary>
        public string PduApiSwVersion => VersionNumberToString(_pduApiSwVersion);


        /// <summary>
        /// Manufacturing date of the D-PDU API software
        /// </summary>
        public string PduApiSwDate => DateNumberToString(_pduApiSwDate);


        internal PduVersionData(uint mvciPart1StandardVersion, uint mvciPart2StandardVersion, uint hwSerialNumber, byte[] hwName, uint hwVersion, uint hwDate, uint hwInterface, byte[] fwName, uint fwVersion, uint fwDate, byte[] vendorName, byte[] pduApiSwName, uint pduApiSwVersion, uint pduApiSwDate)
        {
            _mvciPart1StandardVersion = mvciPart1StandardVersion;
            _mvciPart2StandardVersion = mvciPart2StandardVersion;
            HwSerialNumber = hwSerialNumber;

            // Name of MVCI HW module; zero terminated
            HwName = Encoding.ASCII.GetString(hwName, 0, hwName.Length).Split("\0").First();

            _hwVersion = hwVersion;
            _hwDate = hwDate;
            HwInterface = hwInterface;

            // Name of the firmware available in the MVCI HW module; zero terminated???? was definitely forgotten to write in the spec, I think
            FwName = Encoding.ASCII.GetString(fwName, 0, fwName.Length).Split("\0").First();

            _fwVersion = fwVersion;
            _fwDate = fwDate;

            // Name of vendor; zero terminated
            VendorName = Encoding.ASCII.GetString(vendorName, 0,vendorName.Length).Split("\0").First();

            // Name of the D‐PDU API software; zero terminated
            PduApiSwName = Encoding.ASCII.GetString(pduApiSwName, 0,pduApiSwName.Length).Split("\0").First();

            _pduApiSwVersion = pduApiSwVersion;
            _pduApiSwDate = pduApiSwDate;
        }

        private static string VersionNumberToString(uint number)
        {
            return ((number >> 24) & 0xFF) + "."
                                           + ((number >> 16) & 0xFF) + "."
                                           + ((number >> 8) & 0xFF) + "."
                                           + (number & 0xFF);
        }

        private static string DateNumberToString(uint number)
        {
            if (number == 0)
                return "0.0.0";
            var week = (number & 0xFF) == 0 ? "" : $"(Calendar week {number & 0xFF})";
            return string.Format($"{((number >> 8) & 0xFF)}.{((number >> 16) & 0xFF)}.{(((number >> 24) & 0xFF) + 1970)} {week}");
        }
    }
}
