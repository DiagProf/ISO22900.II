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

using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ISO22900.II.Demo
{
    internal class MenuPageSelectedApiVci : MenuPage
    {

        public MenuPageSelectedApiVci(AbstractPageControl program)
            : base("Play with the selected API / VCI", program,
                new Option("Playground", () => program.NavigateTo<PagePlayground>()),
                new Option("Use case sharing user preference", () => program.NavigateTo<PageUseCaseSharingUserPreference>()),
                new Option("Use case sharing vci instance", () => program.NavigateTo<PageUseCaseSharingVciInstance>()),
                new Option("Use case simple send and receive", () => program.NavigateTo<PageUseCaseSimpleSendAndReceive>()),
                new Option("Use case simple send and receive use K-Line", () => program.NavigateTo<PageUseCaseSimpleSendAndReceiveKline>()),
                new Option("Use case simple send and receive use TP2.0 (SAE J2819)", () => program.NavigateTo<PageUseCaseSimpleSendAndReceiveTP20>()),
                new Option("Use case simple send and receive use DoIP with physical Vci[[does not work yet!!]]", () => program.NavigateTo<PageUseCaseSimpleSendAndReceiveDoIpWithPhysicalVci>()),
                new Option("Use case simple send and receive use DoIP without physical Vci", () => program.NavigateTo<PageUseCaseSimpleSendAndReceiveDoIpWithoutPhysicalVci>()),
                new Option("Use case simple send and receive with option string", () => program.NavigateTo<PageUseCaseSimpleSendAndReceiveWithOptionString>()),
                new Option("Use case simple send and receive with event all over", () => program.NavigateTo<PageUseCaseEventAllOver>()),
                new Option("Use case ECU simulator", () => program.NavigateTo < PageUseCaseEcuSimulator>()),
                new Option("Use case read battery voltage and ignition state from selected VCI", () => program.NavigateTo<PageUseCaseReadIgnitionAndUbat>()),
                new Option("Use case read version infos from selected VCI", () => program.NavigateTo<PageUseCaseReadVciVersionData>()),
                new Option("Use case recover VCI after VCI lost", () => program.NavigateTo<PageUseCaseRecoverVciAfterVciLost>()),
                new Option("Use case recover ComLogicalLink after VCI lost", () => program.NavigateTo<PageUseCaseRecoverComLogicalLinkAfterVciLost>()),
                new Option("Use case recover ComLogicalLink after VCI lost (K-Line)", () => program.NavigateTo<PageUseCaseRecoverComLogicalLinkAfterVciLostKline>()),
                new Option("Use case recover ComPrimitive after VCI lost", () => program.NavigateTo<PageUseCaseRecoverComPrimitiveAfterVciLost>()),
                new Option("Use case detecting the diagnostic cable", () => program.NavigateTo<PageUseCaseCable>())
            )
        {
        }
    }
}
