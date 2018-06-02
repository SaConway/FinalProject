using System;

using Xamarin.Forms;

namespace MsorLi.Views
{
    public partial class NoConnctionPage : ContentPage
    {
        private static Boolean _loaded;
        public static Boolean Loaded
        {
            get => _loaded;
            set
            {
                _loaded = value;
            }
        }


        public NoConnctionPage()
        {
            _loaded = true;
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
        }
    }
}
