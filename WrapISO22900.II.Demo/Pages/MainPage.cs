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

namespace ISO22900.II.Demo
{
    internal class MainPage : MenuPage
    {
        public MainPage(AbstractPageControl program)
            : base("Main Page", program,
                new Option("Overview of installed D-PDU APIs and VCIs that can be reached via the corresponding API",
                    () => program.NavigateTo<PageInstalledApiVciOverview>()),
                new Option("Set API/VCI preferences", () => program.NavigateTo<PageApiVciPreferences>()),
                new Option("Play with the selected API/VCI", () => program.NavigateTo<MenuPageSelectedApiVci>()),
                new Option("Console info", () => program.NavigateTo<PageConsoleInfo>()),
                new Option("Note of thanks", () => program.NavigateTo<PageNoteOfThanks>()),
                new Option("Exit", () => {}))

        {
        }
    }
}
