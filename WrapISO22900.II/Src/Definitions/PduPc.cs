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
// ReSharper disable IdentifierTypo
// ReSharper disable BuiltInTypeReferenceStyle

namespace ISO22900.II
{
    /// <summary>
    /// Definition of the ODX ComParam classes
    /// </summary>
    public enum PduPc : UInt32   //ENUM are also treated as a 32-bit value under 64-bit OS, this is more or less a reminder
    {
        PDU_PC_TIMING = 1,    // Message flow timing ComParams, e.g., inter-byte time or time between request and response.
        
        PDU_PC_INIT = 2,      // ComParams for initiation of communication e.g., trigger address or wakeup pattern.
        
        PDU_PC_COM = 3,       // General communication ComParam.
        
        PDU_PC_ERRHDL = 4,    // ComParam defining the behaviour of the runtime system in case an error occurred, 
                              // e.g., runtime system could either continue communication after a timeout 
                              // was detected, or stop and reactivate.
                              
        PDU_PC_BUSTYPE = 5,   // This is used to define a bustype specific ComParam (e.g., baudrate).
                              // Most of these ComParams affect the physical hardware. 
                              // These ComParams can only be modified by the first Com Logical Link that acquired 
                              // the physical resource (PDUCreateComLogicalLink()).  
                              // When a second Com Logical Link is created for the same resource, 
                              // these ComParams that were previously set by the initial 
                              // Com Logical Link will be active for the new Com Logical Link.
                              
        PDU_PC_UNIQUE_ID = 6,    // This type of ComParam is used to indicate to both the ComLogicalLinkLayer and 
                                 // the Application that the information is used for protocol response handling 
                                 // from a physical or functional group of ECUs to uniquely define an ECU response.
                                 
        PDU_PC_TESTER_PRESENT = 7   // This type of ComParam is used for tester present type of ComParams (CP_TesterPresentXXX).
                                    // Tester Present ComParams cannot be changed temporarily using the TempParamUpdate flag 
                                    // like other ComParams. Using this type of ComParam class enables an application and
                                    // database to properly configure and use Tester Present ComParams.B.2.3.3 ComParam LONGFIELD Data Type for the definition.  
    }
}