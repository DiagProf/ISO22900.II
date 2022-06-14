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


using System.Runtime.InteropServices;
using System.Text;

// ReSharper disable InconsistentNaming
// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable IdentifierTypo

namespace ISO22900.II.SafeCStructs
{
    /// <summary>
    /// Structure for version information (used by PDUGetVersion)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct PDU_VERSION_DATA
    {
        internal uint MVCI_Part1StandardVersion; /* Release version of supported MVCI Part 1 standard (See Coding of version numbers)*/
        internal uint MVCI_Part2StandardVersion; /* Release version of supported MVCI Part 2 standard (See Coding of version numbers)*/
        internal uint HwSerialNumber;            /* Unique Serial number of MVCI HW module from a vendor */
        internal string HwName = new string(' ', 64);           /* Name of MVCI HW module; zero terminated */
        internal uint HwVersion;                 /* Version number of MVCI HW module (See Coding of version numbers)*/
        internal uint HwDate;                    /* Manufacturing date of MVCI HW module (See Coding of dates)*/
        internal uint HwInterface;               /* Type of MVCI HW module; zero terminated */
        internal string FwName = new string(' ', 64);           /* Name of the firmware available in the MVCI HW module */
        internal uint FwVersion;                 /* Version number of the firmware in the MVCI HW module (See Coding of version numbers)*/
        internal uint FwDate;                    /* Manufacturing date of the firmware in the MVCI HW module (See Coding of dates)*/
        internal string VendorName = new string(' ', 64);       /* Name of vendor; zero terminated */
        internal string PDUApiSwName = new string(' ', 64);     /* Name of the D-PDU API software; zero terminated */
        internal uint PDUApiSwVersion;           /* Version number of D-PDU API software (See Coding of version numbers)*/
        internal uint PDUApiSwDate;              /* Manufacturing date of the D-PDU API software (See Coding of dates)*/

        public PDU_VERSION_DATA()
        {
            MVCI_Part1StandardVersion = default;
            MVCI_Part2StandardVersion = default;
            HwSerialNumber = default;
            HwVersion = default;
            HwDate = default;
            HwInterface = default;
            FwVersion = default;
            FwDate = default;
            PDUApiSwVersion = default;
            PDUApiSwDate = default;
        }
    }
}