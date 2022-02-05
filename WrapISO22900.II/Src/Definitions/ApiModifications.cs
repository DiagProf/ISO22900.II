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

// ReSharper disable InconsistentNaming

namespace ISO22900.II
{
    [Flags]
    public enum ApiModifications
    {
        NONE               = 0b0000_0000_0000_0000,
        UNSAFE_API         = 0b0000_0000_0000_0001,
        IGNITION_FIX       = 0b0000_0000_0001_0000,  //for VCI that can't detect an ignition state on diagnostic link connector (DLC)
        VOLTAGE_FIX        = 0b0000_0000_0010_0000,  //for VCI that can't measure a supply voltage on diagnostic link connector (DLC)
        VOLTAGE_TO_LOW_FIX = 0b0000_0000_0100_0000,  //for VCI that measure a supply voltage but this voltage is fixed e.g. to 12V
                                                     //(but the application needs 13V to go ahead) (add 1,5 Volt)
    }
}