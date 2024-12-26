using System;
using System.Text;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace ISO22900.II.Demo
{
    public class ApiTree
    {
        private readonly Tree _tree;

        public ApiTree(string title, Action<Tree> additionalInformation)
        {
            Title = title;
            //AdditionalInformation = additionalInformation;

            _tree = new Tree(Title);
            additionalInformation(_tree);
        }

        public string Titlett { get; set; }


        public string Title { get; set; }
        //public Action<Tree> AdditionalInformation { get; set; }

        public string GetFormattedPrompt()
        {
            //is too slow here if we get tree sting from hardware devices otherwise the menu will get stuck
            //var tree = new Tree(Title);
            //AdditionalInformation(tree);
            var width = Console.WindowWidth;
            var simpleRenderContext = new RenderOptions(new SimpleCapabilities(), new Size(AnsiConsole.Profile.Width, AnsiConsole.Profile.Height));
            var sb = new StringBuilder();
            var segments = ( (IRenderable) _tree ).Render(simpleRenderContext, width);
            foreach ( var segment in segments )
            {
                sb.Append(segment.Text);
                if ( segment.IsLineBreak )
                {
                    sb.Append("  ");
                }
            }

            return sb.ToString();
        }
    }
}