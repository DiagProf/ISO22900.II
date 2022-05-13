#region License

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

using System;
using System.Xml.XPath;
using Spectre.Console;

namespace ISO22900.II.Demo
{
    internal class PageUseCaseCable : Page
    {
        public PageUseCaseCable(AbstractPageControl program)
            : base("Use case read cable id from an MVCI protocol module and than use CDF", program)
        {
        }

        public override void Display()
        {
            base.Display();
            var infoGrid = new Grid();
            var info = "Read version information from VCI";
            infoGrid.AddColumn(new GridColumn().Centered());
            infoGrid.AddRow($"[yellow]{info}[/]");
            AnsiConsole.Write(infoGrid);

            using ( var api =
                   DiagPduApiOneFactory.GetApi(
                       DiagPduApiHelper.FullLibraryPathFormApiShortName(AbstractPageControl.Preferences.GetSection("ApiVci:Api").Value),
                       AbstractPageControl.LoggerFactory) )
            {
                using ( var vci = api.ConnectVci(AbstractPageControl.Preferences.GetSection("ApiVci:Vci").Value) )
                {
                    if ( vci.TryIoCtlGetCableId(out var cableId) )
                    {
                        AnsiConsole.WriteLine();
                        var grid = new Grid()
                            .AddColumn(new GridColumn().NoWrap().PadRight(4))
                            .AddColumn()
                            .AddRow("[b]Cable Id[/]", $"{cableId}");

                        //here is just an idea. You could get more out of the cdf file.
                        var fullCdfPath =
                            DiagPduApiHelper.FullCdfPathFormApiShortName((AbstractPageControl.Preferences.GetSection("ApiVci:Api").Value));
                        if ( fullCdfPath.EndsWith(".xml", StringComparison.OrdinalIgnoreCase) )
                        {
                            ////Load the file and create a navigator object. 
                            var xPath = new XPathDocument(fullCdfPath);
                            var navigator = xPath.CreateNavigator();
                            var nodeIterator = navigator.SelectSingleNode(
                                "MVCI_CABLE_DESCRIPTION/CABLE/CABLE_IDENTIFICATION/CABLE_ID[normalize-space(text()) = '" + $"{cableId}" + "']/../.." +
                                "/DESCRIPTION");
                            if ( nodeIterator != null )
                            {
                                grid.AddRow("[b]Cable description[/]", $"{nodeIterator.Value}");
                            }
                        }

                        AnsiConsole.Write(
                            new Panel(grid)
                                .Header("Information"));
                        AnsiConsole.WriteLine();
                    }
                    else
                    {
                        AnsiConsole.WriteLine("It is not possible to detect the diagnostic cable");
                    }
                }
            }

            AnsiConsole.Console.ReadKey("Press [DodgerBlue1][[Enter]][/] to navigate home");
            AbstractPageControl.NavigateHome();
        }
    }
}
