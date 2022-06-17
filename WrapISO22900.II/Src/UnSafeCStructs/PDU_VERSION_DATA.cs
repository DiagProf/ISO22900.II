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

// ReSharper disable InconsistentNaming
// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable IdentifierTypo

namespace ISO22900.II.UnSafeCStructs
{
    // Basic data types used in ISO22900-2
    // C# using alias directive     //C typedef...
    //using UNUM8 = System.Byte;      //typedef unsigned char UNUM8;      // Unsigned numeric 8 bits
    //using SNUM8 = System.SByte;     //typedef signed char SNUM8;        // Signed numeric 8 bits
    //using UNUM16 = System.UInt16;   //typedef unsigned short UNUM16;    // Unsigned numeric 16 bits
    //using SNUM16 = System.Int16;    //typedef signed short SNUM16;      // Signed numeric 16 bits
    using UNUM32 = System.UInt32;   //typedef unsigned long UNUM32;     // Unsigned numeric 32 bits
    //using SNUM32 = System.Int32;    //typedef signed long SNUM32;       // Signed numeric 32 bits
    using CHAR8 = System.Byte;      //typedef char CHAR8;               // ASCII-coded 8-bit character value (ISO8859-1 (Latin 1))
    
    /// <summary>
    /// Structure for version information (used by PDUGetVersion)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct PDU_VERSION_DATA
    {
        internal UNUM32 MVCI_Part1StandardVersion; /* Release version of supported MVCI Part 1 standard (See Coding of version numbers)*/
        internal UNUM32 MVCI_Part2StandardVersion; /* Release version of supported MVCI Part 2 standard (See Coding of version numbers)*/
        internal UNUM32 HwSerialNumber;            /* Unique Serial number of MVCI HW module from a vendor */
        internal fixed CHAR8 HwName[64];           /* Name of MVCI HW module; zero terminated */
        internal UNUM32 HwVersion;                 /* Version number of MVCI HW module (See Coding of version numbers)*/
        internal UNUM32 HwDate;                    /* Manufacturing date of MVCI HW module (See Coding of dates)*/
        internal UNUM32 HwInterface;               /* Type of MVCI HW module; zero terminated */
        internal fixed CHAR8 FwName[64];           /* Name of the firmware available in the MVCI HW module */
        internal UNUM32 FwVersion;                 /* Version number of the firmware in the MVCI HW module (See Coding of version numbers)*/
        internal UNUM32 FwDate;                    /* Manufacturing date of the firmware in the MVCI HW module (See Coding of dates)*/
        internal fixed CHAR8 VendorName[64];       /* Name of vendor; zero terminated */
        internal fixed CHAR8 PDUApiSwName[64];     /* Name of the D-PDU API software; zero terminated */
        internal UNUM32 PDUApiSwVersion;           /* Version number of D-PDU API software (See Coding of version numbers)*/
        internal UNUM32 PDUApiSwDate;              /* Manufacturing date of the D-PDU API software (See Coding of dates)*/
    }
}