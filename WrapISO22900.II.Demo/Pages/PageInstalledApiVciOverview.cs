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


using Spectre.Console;

namespace ISO22900.II.Demo
{
    internal class PageInstalledApiVciOverview : Page
    {
        public PageInstalledApiVciOverview(AbstractPageControl program)
            : base("Overview of installed D-PDU APIs and VCIs", program)
        {
        }

        public override void Display()
        {
            base.Display();

            var root = new Tree("API's").Style("DeepSkyBlue1");

            AnsiConsole.Status()
                       .AutoRefresh(true)
                       .SpinnerStyle(new Style(Color.DeepSkyBlue1))
                       .Spinner(Spinner.Known.BouncingBar)
                       .Start("[DodgerBlue1]Collecting API's and VCI's[/]", ctx =>
                       {
                           var allInstalledPduApisDetails = DiagPduApiHelper.InstalledMvciPduApiDetails();
                           foreach ( var mvciPduApiDetail in allInstalledPduApisDetails )
                           {
                               var treeApiParts = root.AddNode($"[white bold]{mvciPduApiDetail.ShortName}[/]");
                               treeApiParts.AddNode($"ShortName: {mvciPduApiDetail.ShortName}");
                               treeApiParts.AddNode($"Supplier: {mvciPduApiDetail.SupplierName}");
                               treeApiParts.AddNode($"Description: {mvciPduApiDetail.Description}");
                               treeApiParts.AddNode($"Module description file (MDF): {mvciPduApiDetail.ModuleDescriptionFile}");
                               treeApiParts.AddNode($"Cable description file (CDF): {mvciPduApiDetail.CableDescriptionFile}");
                               treeApiParts.AddNode($"Library file: {mvciPduApiDetail.LibraryFile}");

                               WriteLogMessage($"discovering API {mvciPduApiDetail.ShortName}");

                               using ( var sys = DiagPduApiOneFactory.GetApi(mvciPduApiDetail.LibraryFile) )
                               {
                                   var pduModuleDatas = sys.PduModuleDataSets;
                                   var treeVcis = treeApiParts.AddNode(
                                       $"Connected Vehicle Communication Interfaces (VCIs): [white]{pduModuleDatas.Count}[/]");
                                   if ( pduModuleDatas.Count > 0 )
                                   {
                                       foreach ( var moduleData in pduModuleDatas )
                                       {
                                           var treeVci = treeVcis.AddNode(new Text($"VCI: {moduleData.VendorModuleName}"));
                                           treeVci.AddNode(new Text($"Module status: {moduleData.ModuleStatus}"));
                                           treeVci.AddNode(new Text($"Vendor additional info: {moduleData.VendorAdditionalInfo}"));
                                           treeVci.AddNode(new Text($"Module type id: {moduleData.ModuleTypeId}"));
                                       }
                                   }
                               }
                           }

                           //like clear the log but Status has no clear
                           AnsiConsole.Clear();
                           base.Display();
                       });


            AnsiConsole.Write(root);
            AnsiConsole.WriteLine();
            AnsiConsole.Console.ReadKey("Press [DodgerBlue1][[Enter]][/] to navigate home");
            AbstractPageControl.NavigateHome();
        }

        private static void WriteLogMessage(string message)
        {
            AnsiConsole.MarkupLine($"[grey]LOG:[/] {message}[grey]...[/]");
        }
    }
}
