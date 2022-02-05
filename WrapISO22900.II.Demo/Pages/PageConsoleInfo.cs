using System.Threading.Tasks;
using Spectre.Console;

namespace ISO22900.II.Demo
{
    internal class PageConsoleInfo : Page
    {
        public PageConsoleInfo(AbstractPageControl abstractPageControl)
            : base("Console info", abstractPageControl)
        {
        }

        public override void Display()
        {
            base.Display();

            AnsiConsole.MarkupLine("[red]This Information has nothing to do with the program, only with the console itself.[/]");
            AnsiConsole.MarkupLine("Is required if the representation does not fit.");
            AnsiConsole.WriteLine();
            var grid = new Grid()
                       .AddColumn(new GridColumn().NoWrap().PadRight(4))
                       .AddColumn()
                       .AddRow("[b]Enrichers[/]", string.Join(", ", AnsiConsole.Profile.Enrichers))
                       .AddRow("[b]Color system[/]", $"{AnsiConsole.Profile.Capabilities.ColorSystem}")
                       .AddRow("[b]Unicode?[/]", $"{YesNo(AnsiConsole.Profile.Capabilities.Unicode)}")
                       .AddRow("[b]Supports ansi?[/]", $"{YesNo(AnsiConsole.Profile.Capabilities.Ansi)}")
                       .AddRow("[b]Supports links?[/]", $"{YesNo(AnsiConsole.Profile.Capabilities.Links)}")
                       .AddRow("[b]Legacy console?[/]", $"{YesNo(AnsiConsole.Profile.Capabilities.Legacy)}")
                       .AddRow("[b]Interactive?[/]", $"{YesNo(AnsiConsole.Profile.Capabilities.Interactive)}")
                       .AddRow("[b]Terminal?[/]", $"{YesNo(AnsiConsole.Profile.Out.IsTerminal)}")
                       .AddRow("[b]Buffer width[/]", $"{AnsiConsole.Console.Profile.Width}")
                       .AddRow("[b]Buffer height[/]", $"{AnsiConsole.Console.Profile.Height}")
                       .AddRow("[b]Encoding[/]", $"{AnsiConsole.Console.Profile.Encoding.EncodingName}");

            AnsiConsole.Write(
                new Panel(grid)
                    .Header("Information"));

            AnsiConsole.Console.ReadKey("Press [DodgerBlue1][[Enter]][/] to navigate home");
            AbstractPageControl.NavigateHome();
        }

        private static string YesNo(bool value)
        {
            return value ? "Yes" : "No";
        }
    }
}
