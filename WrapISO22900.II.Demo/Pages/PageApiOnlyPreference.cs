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
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Spectre.Console;

namespace ISO22900.II.Demo
{
    internal class PageApiOnlyPreference : Page
    {
        public PageApiOnlyPreference(AbstractPageControl program)
            : base("API preference only (for DoIP with simple Ethernet<->OBD cable)", program)
        {
        }

        public static void AddOrUpdateAppSetting<T>(string key, T value)
        {
            try
            {
                var filePath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
                var json = File.ReadAllText(filePath);
                dynamic jsonObj = JsonConvert.DeserializeObject(json);

                var sectionPath = key.Split(":")[0];
                if ( !string.IsNullOrEmpty(sectionPath) )
                {
                    var keyPath = key.Split(":")[1];
                    jsonObj[sectionPath][keyPath] = value;
                }
                else
                {
                    jsonObj[sectionPath] = value; // if no section path just set the value
                }

                string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                File.WriteAllText(filePath, output);
            }
            catch ( Exception ) //ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }

        public override void Display()
        {
            base.Display();


            var prompt = new SelectionPrompt<ApiTree>
            {
                Converter = v => v.GetFormattedPrompt(),
                PageSize = 2,
                Title = "What's your [DodgerBlue1]favorite API[/]?",
                Mode = SelectionMode.Independent,
                HighlightStyle = new Style(Color.DeepSkyBlue1)
            };

            var ApiCount = 0;
            AnsiConsole.Status()
                .AutoRefresh(true)
                .SpinnerStyle(new Style(Color.DeepSkyBlue1))
                .Spinner(Spinner.Known.BouncingBar)
                .Start("[DodgerBlue1]Collecting API's[/]", ctx =>
                {
                    var allInstalledPduApisDetails = DiagPduApiHelper.InstalledMvciPduApiDetails();
                    foreach ( var mvciPduApiDetail in allInstalledPduApisDetails )
                    {
                        prompt.AddChoice(new ApiTree(mvciPduApiDetail.ShortName, shortNameApi =>
                        {
                            // Add some nodes
                            shortNameApi.AddNode($"Supplier: {mvciPduApiDetail.SupplierName}");
                            shortNameApi.AddNode($"Description: {mvciPduApiDetail.Description}");
                            shortNameApi.AddNode($"Module description file (MDF): {mvciPduApiDetail.ModuleDescriptionFile}");
                            shortNameApi.AddNode($"Cable description file (CDF): {mvciPduApiDetail.CableDescriptionFile}");
                            shortNameApi.AddNode($"Library file: {mvciPduApiDetail.LibraryFile}");

                            WriteLogMessage($"discovering API {mvciPduApiDetail.ShortName}");
                        }));
                        ApiCount++;
                    }

                    //like clear the log but Status has no clear
                    AnsiConsole.Clear();
                    base.Display();
                });
            AnsiConsole.MarkupLine($"Number of installed APIs [white]{ApiCount}[/]");
            AnsiConsole.WriteLine();
            var apiShortName = AnsiConsole.Prompt(prompt).Title;

            if (ApiCount  > 0)
            {

                AnsiConsole.WriteLine($"Selected API ShortName: {apiShortName}");

                if ( AnsiConsole.Confirm("Store to appsettings.json ?", false) ) //"Store to appsettings.json ?"
                {
                    AbstractPageControl.Preferences.GetSection("ApiVci:Api").Value = apiShortName;
                    AddOrUpdateAppSetting("ApiVci:Api", apiShortName);
                    AbstractPageControl.Preferences.GetSection("ApiVci:Vci").Value = String.Empty;
                    AddOrUpdateAppSetting("ApiVci:Vci", String.Empty);
                }
            }
            else
            {
                AnsiConsole.Console.ReadKey("Press [DodgerBlue1][[Enter]][/] to navigate home");
            }

            AbstractPageControl.NavigateHome();
        }

        private static void WriteLogMessage(string message)
        {
            AnsiConsole.MarkupLine($"[grey]LOG:[/] {message}[grey]...[/]");
        }
    }
}
