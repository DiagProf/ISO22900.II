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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ISO22900.II.Demo
{
    internal class ConsoleHeart : AbstractPageControl, IConsoleHeart
    {
        private readonly ILogger _logger;

        public ConsoleHeart(IConfiguration config, ILoggerFactory loggerFactory)
            : base("D-PDU-API (ISO22900-2) Console", true, config, loggerFactory)
        {
            AddPage(new MainPage(this));
            AddPage(new MenuPageSelectedApiVci(this));
            AddPage(new PageInstalledApiVciOverview(this));
            AddPage(new PageApiVciPreferences(this));
            AddPage(new PageApiOnlyPreference(this));
            AddPage(new PageNoteOfThanks(this));
            AddPage(new PageConsoleInfo(this));
            AddPage(new PageUseCaseSharingUserPreference(this));
            AddPage(new PageUseCaseSharingVciInstance(this));
            AddPage(new PageUseCaseSimpleSendAndReceive(this));
            AddPage(new PageUseCaseSimpleSendAndReceiveKline(this));
            AddPage(new PageUseCaseSimpleSendAndReceiveTP20(this));
            AddPage(new PageUseCaseSimpleSendAndReceiveDoIpVersion1p0WithoutPhysicalVci(this));
            AddPage(new PageUseCaseSimpleSendAndReceiveDoIpWithoutPhysicalVci(this));
            AddPage(new PageUseCaseSimpleSendAndReceiveDoIpWithoutPhysicalVciButWithVirtualVci(this));
            AddPage(new PageUseCaseSimpleSendAndReceiveDoIpWithPhysicalVci(this));
            AddPage(new PageUseCaseSimpleSendAndReceiveObd2OverCan(this));
            AddPage(new PageUseCaseSimpleSendAndReceiveObd2OverKline(this));
            AddPage(new PageUseCaseSimpleSendAndReceiveWithOptionString(this));
            AddPage(new PageUseCaseSimpleSendAndReceiveWithEventDrivenCanTrace(this));
            AddPage(new PageUseCaseEventAllOver(this));
            AddPage(new PagePlayground(this));
            AddPage(new PageUseCaseEcuSimulator(this));
            AddPage(new PageUseCaseReadIgnitionAndUbat(this));
            AddPage(new PageUseCaseReadVciVersionData(this));
            AddPage(new PageUseCaseRecoverVciAfterVciLost(this));
            AddPage(new PageUseCaseRecoverComLogicalLinkAfterVciLost(this));
            AddPage(new PageUseCaseRecoverComLogicalLinkAfterVciLostKline(this));
            AddPage(new PageUseCaseRecoverComPrimitiveAfterVciLost(this));
            AddPage(new PageUseCaseCable(this));
            SetPage<MainPage>();

            _logger = loggerFactory.CreateLogger<ConsoleHeart>();
        }

        public void Run(string[] args)
        {
            _logger.LogInformation("Application Started at {dateTime}", DateTime.UtcNow);
            base.Run();
            _logger.LogInformation("Application Completed at {dateTime}", DateTime.UtcNow);
        }
    }
}
