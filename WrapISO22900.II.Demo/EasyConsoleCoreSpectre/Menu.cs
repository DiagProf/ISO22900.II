using System;
using System.Collections.Generic;
using System.Linq;
using Spectre.Console;

namespace ISO22900.II.Demo
{
    public class Menu
    {
        private IList<Option> Options { get; set; }

        public Menu()
        {
            Options = new List<Option>();
        }

        public void Display()
        {
            var option = AnsiConsole.Prompt(
                new SelectionPrompt<Option>
                {
                    Converter = o => o.Name,
                    Title = "Choose an [LightCyan1]option[/]:",
                    PageSize = 12,
                    MoreChoicesText = "[grey](Move up and down to reveal more options)[/]",
                    HighlightStyle = "black on LightCyan1"
                }.AddChoices(Options)
            );

            option.Callback();
        }

        public Menu Add(string option, Action callback)
        {
            return Add(new Option(option, callback));
        }

        public Menu Add(Option option)
        {
            Options.Add(option);
            return this;
        }

        public bool Contains(string option)
        {
            return Options.FirstOrDefault((op) => op.Name.Equals(option)) != null;
        }
    }
}
