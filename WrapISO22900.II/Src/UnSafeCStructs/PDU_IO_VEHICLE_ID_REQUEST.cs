﻿#region License

// // MIT License
// //
// // Copyright (c) 2022 Joerg Frank
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

using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming
// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable IdentifierTypo

namespace ISO22900.II.UnSafeCStructs
{
    // Basic data types used in ISO22900-2
    // C# using alias directive     //C typedef...
    using UNUM8 = System.Byte;      //typedef unsigned char UNUM8;      // Unsigned numeric 8 bits
    //typedef signed char SNUM8;        // Signed numeric 8 bits
    //typedef unsigned short UNUM16;    // Unsigned numeric 16 bits
    //typedef signed short SNUM16;      // Signed numeric 16 bits
    using UNUM32 = System.UInt32;   //typedef unsigned long UNUM32;     // Unsigned numeric 32 bits
    //typedef signed long SNUM32;       // Signed numeric 32 bits
    using CHAR8 = System.Byte;      //typedef char CHAR8;               // ASCII-coded 8-bit character value (ISO8859-1 (Latin 1))


    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct PDU_IO_VEHICLE_ID_REQUEST
    {
        internal UNUM32 PreselectionMode;       /* Preselection mode: 0=no preselection */
                                                /* 1= select DoIP entities with given VIN */
                                                /* 2= select DoIP entities with given EID */
        internal CHAR8* PreselectionValue;      /* pointer to NULL terminated ASCII string */ 
                                                /* containing optional VIN or EID (depending on PreselectionMode) */
        internal UNUM32 CombinationMode;        /* Combination mode: */
                                                /* 0= no combination */
                                                /* 1= combine DoIP entities with common VIN */
                                                /*    into MVCI of type "DoIP-Vehicle" */
                                                /* 2= combine DoIP entities with common GroupID */
                                                /*    into MVCI of type "DoIP-Group" */
                                                /* 3= combine all DoIP entities */
                                                /*    into MVCI of type "DoIP-Collection" */
        internal UNUM32 VehicleDiscoveryTime; /* Time-out to wait for vehicle identification    responses. 0=return immediately, or time in    milliseconds. */

        internal UNUM32 NumDestinationAddresses; /* Number of broadcast/multicast addresses in the destination address array. May be 0 for default */
        internal PDU_IP_ADDR_INFO* pDestinationAddresses; /* pointer to an array of IP addresses on which broadcast/multicast should be performed */
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct PDU_IP_ADDR_INFO
    {
        internal UNUM32 IpVersion;     // the IP version to be used for DoIP communication: 4=IPv4, 6=IPv6
        internal UNUM8* pAddress;      // IPv4: 4 Byte broadcast address (in network byte    order: MSB first)
                                       // IPv6: 16 Byte multicast address (in network byte   order: MSB first)
    }
}
