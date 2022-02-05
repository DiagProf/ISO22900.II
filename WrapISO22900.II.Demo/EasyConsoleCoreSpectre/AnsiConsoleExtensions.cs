using System;
using Spectre.Console;

namespace ISO22900.II.Demo
{
    public static class AnsiConsoleExtensions
    {
        public static void ReadKey(this IAnsiConsole ansiConsole, string text)
        {
            ansiConsole.Markup(text);
            Console.ReadKey(true);
        }
    }
}