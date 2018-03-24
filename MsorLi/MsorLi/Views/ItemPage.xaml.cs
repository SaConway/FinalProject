using Microsoft.WindowsAzure.Storage.Table;
using MsorLi.Models;
using MsorLi.Services;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MsorLi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemPage : ContentPage
    {
        //---------------------------------
        // MEMBERS
        //---------------------------------

        AzureImageService _azureImageService = AzureImageService.DefaultManager;

        //---------------------------------
        // FUNCTIONS
        //---------------------------------

        public ItemPage(Item item)
        {
            try
            {
                InitializeComponent();

                imagesView.HeightRequest = (double)(App.ScreenHeight / 3.5);

                UpdateItemDetails(item);
            }
            catch
            {

            }
        }

        private async void UpdateItemDetails(Item item)
        {
            title.Text = item.Title;

            ObservableCollection<Models.Image> images = new ObservableCollection<Models.Image>();

            var itemImages = await _azureImageService.GetItemImages(item.Id);

            for (int i = 0; i < itemImages.Count; ++i)
            {
                Models.Image image = new Models.Image { ImageUrl = itemImages[i].Url, ImageNumber = (i + 1).ToString() + " מתוך " + itemImages.Count.ToString() };
                images.Add(image);
            }

            imagesView.ItemsSource = images;

            description.Text = item.Description;
            condition.Text = item.Condition;
            location.Text = item.Location;
            contact_name.Text = item.ContactName;
            contact_number.Text = item.ContactNumber;
            date.Text = item.Date;
        }
    }
}
