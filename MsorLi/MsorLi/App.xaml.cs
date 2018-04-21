using MsorLi.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace MsorLi
{
    public partial class App : Application
	{
        public App ()
		{
            InitializeComponent();
            MainPage = new NavigationPage(new ItemListPage());

            // NavigationBar color
            MainPage.SetValue(NavigationPage.BarBackgroundColorProperty, "#00BCD4");
            MainPage.SetValue(NavigationPage.BarTextColorProperty, Color.White);
        }
    }
}