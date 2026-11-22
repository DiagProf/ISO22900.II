#region License

// // MIT License
// //
// // Copyright (c) 2024 Joerg Frank
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

using System;
using System.Threading;
using System.Threading.Tasks;

namespace ISO22900.II.Interface;

/// <summary>
/// One running ComPrimitive (COP) of a <see cref="IComLogicalLink"/>, created by <see cref="IComLogicalLink.StartCop(PduCopt, byte[], PduCopCtrlData, uint)"/>.
/// The native implementation is <c>ComPrimitive</c> from the ISO22900.II-Sharp package.
/// Own implementations (simulators, test doubles) should follow the same contract:
/// <list type="bullet">
/// <item><description>The result contains all <see cref="PduEventItemResult"/>, <see cref="PduEventItemError"/> and <see cref="PduEventItemInfo"/> items, but no <see cref="PduEventItemStatus"/> items.</description></item>
/// <item><description>When the token passed to <see cref="WaitForCopResult"/> or <see cref="WaitForCopResultAsync"/> is cancelled, an <see cref="OperationCanceledException"/> is thrown
/// (unless the legacy flag <see cref="ApiModifications.LEGACY_SWALLOW_COP_CANCELLATION"/> is active on the native implementation).</description></item>
/// <item><description><see cref="IDisposable.Dispose"/> cancels a still running COP.</description></item>
/// </list>
/// </summary>
public interface IComPrimitive : IDisposable
{
    /// <summary>
    /// Blocking variant of <see cref="WaitForCopResultAsync"/> with the same behavior.
    /// </summary>
    /// <exception cref="OperationCanceledException"><paramref name="ct"/> was cancelled.</exception>
    ComPrimitiveResult WaitForCopResult(CancellationToken ct = default);

    /// <summary>
    /// Waits until the COP is finished (or delivers one result for cyclic/infinite COPs) and returns the collected event items.
    /// </summary>
    /// <exception cref="OperationCanceledException"><paramref name="ct"/> was cancelled.</exception>
    Task<ComPrimitiveResult> WaitForCopResultAsync(CancellationToken ct = default);

    PduExStatusData Status();

    void Cancel();

    /// <summary>
    /// Attempts to restore the status of the ComPrimitive (and its ComLogicalLink)
    /// Catch all "Iso22900IIException" exceptions
    /// </summary>
    bool TryToRecover(out string exMessage);
}
