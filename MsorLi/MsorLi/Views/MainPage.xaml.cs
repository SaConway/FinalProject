using Xamarin.Forms;

namespace MsorLi.Views
{
    public partial class MainPage : TabbedPage
    {
        public static MainPage mainPage;

        public MainPage()
        {
            // Disable Navigation Bar
            NavigationPage.SetHasNavigationBar(this, false);

            InitializeComponent();

            mainPage = this;
        }
    }
}
