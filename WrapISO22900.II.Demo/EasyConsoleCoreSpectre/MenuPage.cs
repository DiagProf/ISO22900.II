using System.Threading.Tasks;

namespace ISO22900.II.Demo
{
    public abstract class MenuPage : Page
    {
        protected Menu Menu { get; set; }

        public MenuPage(string title, AbstractPageControl abstractPageControl, params Option[] options)
            : base(title, abstractPageControl)
        {
            Menu = new Menu();
           
            foreach (var option in options)
                Menu.Add(option);
        }

        public override void Display()
        {
            base.Display();

            if (AbstractPageControl.NavigationEnabled && !Menu.Contains("Go back"))
                Menu.Add("Go back", () => { AbstractPageControl.NavigateBack(); });

            Menu.Display();
        }
    }
}
