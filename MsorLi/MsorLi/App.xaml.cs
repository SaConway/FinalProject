using MsorLi.Views;
using Plugin.Connectivity;
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
			MainPage.SetValue(NavigationPage.BarBackgroundColorProperty, "#19a4b4");
            MainPage.SetValue(NavigationPage.BarTextColorProperty, Color.White);
        }
    }
}