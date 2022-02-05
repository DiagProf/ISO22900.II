using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Spectre.Console;

namespace ISO22900.II.Demo
{
    public abstract class Page
    {
        public string Title { get; private set; }

        public AbstractPageControl AbstractPageControl { get; set; }

        public Page(string title, AbstractPageControl abstractPageControl)
        {
            Title = title;
            AbstractPageControl = abstractPageControl;
        }

        public virtual void Display()
        {
            if (AbstractPageControl.History.Count > 1 && AbstractPageControl.BreadcrumbHeader)
            {
                string breadcrumb = "";
                string separator = " > ";

                var breadcrumbParts = new List<string>();
                var titelEnumerable = AbstractPageControl.History.Select((page) => page.Title);
                var length = 0;
                foreach (var titel in titelEnumerable)
                {
                    if (titel.Equals(titelEnumerable.First()))
                    {
                        if (length + titel.Length < Console.WindowWidth)
                        {
                            breadcrumbParts.Add(titel);
                        }
                        else
                        {
                            breadcrumbParts.Add("...");
                            break;
                        }
                    }
                    else
                    {
                        if ((length + titel.Length + separator.Length) < Console.WindowWidth)
                        {
                            breadcrumbParts.Add(separator);
                            length += separator.Length;
                            breadcrumbParts.Add(titel);
                        }
                        else
                        {
                            breadcrumbParts.Add(separator);
                            breadcrumbParts.Add("...");
                            break;
                        }
                    }

                    length += titel.Length;
                }

                for (var index = 0; index < breadcrumbParts.Count; index++)
                {
                    if (index % 2 == 0)
                    {
                        //is even -> a titel
                        breadcrumb = breadcrumbParts[index] + breadcrumb;
                    }
                    else
                    {
                        //is odd  -> a separator
                        breadcrumb = "[Blue]" + breadcrumbParts[index] +"[/]" + breadcrumb;
                    }
                }

                AnsiConsole.MarkupLine(breadcrumb);
            }
            else
            {
                AnsiConsole.MarkupLine(Title);
            }

            var rule = new Rule { Border = BoxBorder.Double, Style = "Blue" }; 
            
            AnsiConsole.Write(rule);
            AnsiConsole.WriteLine();
        }
    }
}
