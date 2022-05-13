#region License

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

#endregion


using Spectre.Console;

namespace ISO22900.II.Demo
{
    internal class PageUseCaseReadIgnitionAndUbat : Page
    {
        public PageUseCaseReadIgnitionAndUbat(AbstractPageControl program)
            : base("Use case read battery voltage and ignition state from an MVCI protocol module", program)
        {
        }

        public override void Display()
        {
            base.Display();
            var infoGrid = new Grid();
            var info = "Read battery voltage and ignition state from VCI";
            infoGrid.AddColumn(new GridColumn().Centered());
            infoGrid.AddRow($"[yellow]{info}[/]");
            AnsiConsole.Write(infoGrid);

            using ( var api = DiagPduApiOneFactory.GetApi(
                       DiagPduApiHelper.FullLibraryPathFormApiShortName(AbstractPageControl.Preferences.GetSection("ApiVci:Api").Value),
                       AbstractPageControl.LoggerFactory, ApiModifications.IGNITION_FIX | ApiModifications.VOLTAGE_FIX) )
            {
                using ( var vci = api.ConnectVci(AbstractPageControl.Preferences.GetSection("ApiVci:Vci").Value) )
                {
                    var batteryVoltage = vci.MeasureBatteryVoltage();
                    var isIgnitionOn = vci.IsIgnitionOn();
                    AnsiConsole.WriteLine();
                    var grid = new Grid()
                        .AddColumn(new GridColumn().NoWrap().PadRight(4))
                        .AddColumn()
                        .AddRow("[b]Battery voltage[/]", $"{batteryVoltage}")
                        .AddRow("[b]Ignition state[/]", $"{isIgnitionOn}");

                    AnsiConsole.Write(
                        new Panel(grid)
                            .Header("Information"));
                    AnsiConsole.WriteLine();
                }
            }

            AnsiConsole.Console.ReadKey("Press [DodgerBlue1][[Enter]][/] to navigate home");
            AbstractPageControl.NavigateHome();
        }
    }
}
