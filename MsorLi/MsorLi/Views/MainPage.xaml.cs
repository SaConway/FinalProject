using Xamarin.Forms;

namespace MsorLi.Views
{
    public partial class MainPage : TabbedPage
    {
        public MainPage()
        {
            // Disable Navigation Bar
            NavigationPage.SetHasNavigationBar(this, false);

            InitializeComponent();

        }


        public MainPage get ()
        {
            return this;
        }

    }
}
