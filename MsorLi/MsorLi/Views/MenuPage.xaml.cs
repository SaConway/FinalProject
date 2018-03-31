using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace MsorLi.Views
{
    public partial class MenuPage : ContentPage
    {
        public MenuPage()
        {
            InitializeComponent();
        }

        private async void SavedItemsClickEvent(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SavedItemsPage());
        }
    }
}
