using MsorLi.Views;
using Xamarin.Forms;

namespace MsorLi
{
    public partial class App : Application
	{
        public static int ScreenHeight { get; set; }
        public static int ScreenWidth { get; set; }

        public App ()
		{
            InitializeComponent();
            MainPage = new NavigationPage (new ItemListPage());
        }
	}
}