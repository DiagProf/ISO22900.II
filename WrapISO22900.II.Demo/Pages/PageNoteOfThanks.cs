using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Spectre.Console;

namespace ISO22900.II.Demo
{
    internal class PageNoteOfThanks : Page
    {
        public PageNoteOfThanks(AbstractPageControl program)
            : base("Note of thanks", program)
        {
        }

        public override void Display()
        {
            base.Display();
            AnsiConsole.MarkupLine("[b]I would like to say thank you for the following projects.[/]");
            AnsiConsole.MarkupLine("[b]They made my work easier and also served as an inspiration.[/]");

            AnsiConsole.WriteLine();
            var grid = new Grid()
                .AddColumn(new GridColumn().NoWrap().PadRight(4))
                .AddColumn()
                .AddRow("[b]EasyConsole.Core[/]", "https://github.com/jimtsikos/EasyConsole.Core")
                .AddRow("[b]Spectre.Console[/]", "https://github.com/spectreconsole/spectre.console")
                .AddRow("[b]J2534-Sharp[/]", "https://github.com/BrianHumlicek/J2534-Sharp");

            AnsiConsole.Write(
                new Panel(grid)
                    .Header("Thank you!"));

            AnsiConsole.Console.ReadKey("Press [DodgerBlue1][[Enter]][/] to navigate home");
            AbstractPageControl.NavigateHome();
        }
    }
}
