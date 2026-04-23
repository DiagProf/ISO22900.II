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

using Microsoft.Extensions.Logging;

namespace ISO22900.II
{
    public partial class ComLogicalLink
    {
        private readonly ILogger _logger = ApiLibLogging.CreateLogger<ComLogicalLink>();

        [LoggerMessage(Level = LogLevel.Warning, Message = "Can't read the State of ComLogicalLink: {error}")]
        partial void LogReadComLogicalLinkStateFailed(string error);

        [LoggerMessage(Level = LogLevel.Information, Message = "ComLogicalLink recovering done for ComLogicalLink: {stackName}")]
        partial void LogComLogicalLinkRecoveringDoneByStackName(string stackName);

        [LoggerMessage(Level = LogLevel.Information, Message = "ComLogicalLink recovering done for ComLogicalLink: {resourceId}")]
        partial void LogComLogicalLinkRecoveringDoneByResourceId(uint resourceId);

        [LoggerMessage(Level = LogLevel.Information, Message = "ComLogicalLink is back or no reason to recover for ComLogicalLink: {stackName}")]
        partial void LogComLogicalLinkIsBackByStackName(string stackName);

        [LoggerMessage(Level = LogLevel.Information, Message = "ComLogicalLink is back or no reason to recover for ComLogicalLink: {resourceId}")]
        partial void LogComLogicalLinkIsBackByResourceId(uint resourceId);
    }
}
