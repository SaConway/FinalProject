using MsorLi.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;


[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace MsorLi
{
    public partial class App : Application
	{
        public App ()
		{
            InitializeComponent();
            MainPage = new NavigationPage(new ItemListPage());
            AppCenter.Start("ios=b1dfb024-6de6-49f5-8fee-f00a20294999;" +
                  "uwp={Your UWP App secret here};" +
                  "android={Your Android App secret here}",
                            typeof(Analytics), typeof(Crashes));

            // NavigationBar color
			MainPage.SetValue(NavigationPage.BarBackgroundColorProperty, "#19a4b4");
            MainPage.SetValue(NavigationPage.BarTextColorProperty, Color.White);
        }
    }
}