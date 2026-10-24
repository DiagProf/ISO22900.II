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

        /// <summary>
        /// Restores the cancellation behavior of earlier versions of <c>ComPrimitive.WaitForCopResult(ct)</c> and
        /// <c>ComPrimitive.WaitForCopResultAsync(ct)</c>: when <c>ct</c> is cancelled, the <see cref="OperationCanceledException"/>
        /// is swallowed and the result collected so far (usually an empty queue) is returned.
        /// </summary>
        /// <remarks>
        /// By default the <see cref="OperationCanceledException"/> is rethrown (standard .NET behavior),
        /// so a caller can no longer mistake a cancellation for "no response":
        /// <list type="bullet">
        /// <item><description><c>WaitForCopResultAsync(ct)</c> since 1.3.2-beta</description></item>
        /// <item><description><c>WaitForCopResult(ct)</c> since 1.4.0-beta</description></item>
        /// </list>
        /// Set this flag if your code relies on the old behavior, e.g. receive loops like
        /// <c>while (!ct.IsCancellationRequested) { var result = cop.WaitForCopResult(ct); ... }</c>.
        /// The flag applies to the whole <c>DiagPduApiOneSysLevel</c>; <c>DiagPduApiOneFactory.GetApi</c> caches one
        /// instance per library path, so only the flags of the first <c>GetApi</c> call are effective.
        /// </remarks>
        LEGACY_SWALLOW_COP_CANCELLATION = 0b0000_0001_0000_0000,
    }
}