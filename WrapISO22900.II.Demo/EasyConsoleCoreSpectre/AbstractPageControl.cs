using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace ISO22900.II.Demo
{
    public abstract class AbstractPageControl
    {
        public ILoggerFactory LoggerFactory { get; }
        public IConfiguration Preferences { get; }

        protected string Title { get; set; }

        public bool BreadcrumbHeader { get; private set; }

        protected Page CurrentPage
        {
            get
            {
                return (History.Any()) ? History.Peek() : null;
            }
        }

        private Dictionary<Type, Page> Pages { get; set; }

        public Stack<Page> History { get; private set; }

        public bool NavigationEnabled { get { return History.Count > 1; } }

        protected AbstractPageControl(string title, bool breadcrumbHeader, IConfiguration config, ILoggerFactory loggerFactory)
        {
            Title = title;
            Pages = new Dictionary<Type, Page>();
            History = new Stack<Page>();
            BreadcrumbHeader = breadcrumbHeader;

            Preferences = config;
            LoggerFactory = loggerFactory;
        }

        public virtual void Run()
        {
            try
            {
                Console.Title = Title;
                CurrentPage.Display();
            }
            catch (Exception e)
            {
                AnsiConsole.WriteException(e, new ExceptionSettings
                {
                    Format = ExceptionFormats.ShortenEverything | ExceptionFormats.ShowLinks,
                    Style = new ExceptionStyle
                    {
                        Exception = new Style().Foreground(Color.Grey),
                        Message = new Style().Foreground(Color.White),
                        NonEmphasized = new Style().Foreground(Color.Cornsilk1),
                        Parenthesis = new Style().Foreground(Color.Cornsilk1),
                        Method = new Style().Foreground(Color.Red),
                        ParameterName = new Style().Foreground(Color.Cornsilk1),
                        ParameterType = new Style().Foreground(Color.Red),
                        Path = new Style().Foreground(Color.Red),
                        LineNumber = new Style().Foreground(Color.Cornsilk1),
                    }
                });
            }
            finally
            {
                if (Debugger.IsAttached)
                {
                    AnsiConsole.Console.ReadKey("Press [red bold][[Enter]][/] to exit.");
                }
            }
        }

        public void AddPage(Page page)
        {
            var pageType = page.GetType();

            if ( Pages.ContainsKey(pageType) )
            {
                Pages[pageType] = page;
            }
            else
            {
                Pages.Add(pageType, page);
            }
        }

        public void NavigateHome()
        {
            while (History.Count > 1)
                History.Pop();

            AnsiConsole.Clear();
            CurrentPage.Display();
        }

        public T SetPage<T>() where T : Page
        {
            Type pageType = typeof(T);

            if (CurrentPage != null && CurrentPage.GetType() == pageType)
                return CurrentPage as T;

            // leave the current page

            // select the new page
            Page nextPage;
            if (!Pages.TryGetValue(pageType, out nextPage))
                throw new KeyNotFoundException("The given page \"{0}\" was not present in the program".Format(pageType));

            // enter the new page
            History.Push(nextPage);

            return CurrentPage as T;
        }

        public T NavigateTo<T>() where T : Page
        {
            SetPage<T>();

            AnsiConsole.Clear();
            CurrentPage.Display();
            return CurrentPage as T;
        }

        public Page NavigateBack()
        {
            History.Pop();

            AnsiConsole.Clear();
            CurrentPage.Display();
            return CurrentPage;
        }
    }
}
