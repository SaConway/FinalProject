using MsorLi.Models;
using MsorLi.Services;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MsorLi.Utilities;

namespace MsorLi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemPage : ContentPage
    {
        //---------------------------------
        // MEMBERS
        //---------------------------------

        string _itemId = "";

        //---------------------------------
        // FUNCTIONS
        //---------------------------------

        public ItemPage(string itemId)
        {
            try
            {
                InitializeComponent();

                _itemId = itemId;

                imagesView.HeightRequest = (double)(App.ScreenHeight / 3.5);

                UpdateItemDetails(itemId);
            }
            catch (Exception)
            {

            }
        }

        private async void UpdateItemDetails(string itemId)
        {
            try
            {
                AzureImageService imageService = AzureImageService.DefaultManager;
                AzureItemService itemService = AzureItemService.DefaultManager;

                Item item = await itemService.GetItemAsync(itemId);

                title.Text = item.Category;

                ObservableCollection<Models.Image> images = new ObservableCollection<Models.Image>();

                var itemImages = await imageService.GetItemImages(item.Id);

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

            catch (Exception) { }
        }

        private async void SaveButtonClick(object sender, EventArgs e)
        {
            try
            {
                if (Settings._GeneralSettings == "")
                {
                    await Navigation.PushAsync(new LoginPage());
                }
                else
                {
                    var userId = Settings._GeneralSettings;

                    AzureSavedItemService savedItemService = AzureSavedItemService.DefaultManager;
                    await savedItemService.UploadToServer(new SavedItem { ItemId = _itemId, UserId = userId }, null);
                }
            }

            catch (Exception) { }
        }
    }
}