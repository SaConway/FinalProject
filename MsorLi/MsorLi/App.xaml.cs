using MsorLi.Views;
using Xamarin.Forms;

namespace MsorLi
{
    public partial class App : Application
	{
        public App ()
		{
            InitializeComponent();
            MainPage = new NavigationPage(new ItemListPage());
            MainPage.SetValue(NavigationPage.BarBackgroundColorProperty, "#00BCD4");
            MainPage.SetValue(NavigationPage.BarTextColorProperty, Color.White);
        }
	}
}