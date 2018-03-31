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

        AzureSavedItemService _savedItemService = AzureSavedItemService.DefaultManager;
        SavedItem _savedItem = new SavedItem();
        bool _saveItem = false;
        bool _itemWasSaved = false;

        //---------------------------------
        // FUNCTIONS
        //---------------------------------

        public ItemPage(string itemId)
        {
            try
            {
                _savedItem.ItemId = itemId;
                _savedItem.UserId = Settings._GeneralSettings;
            }
            catch (Exception)
            {

            }
        }

        async protected override void OnAppearing()
        {
            try
            {
                InitializeComponent();
                imagesView.HeightRequest = (double)(App.ScreenHeight / 2.5);
                UpdateItemDetails(_savedItem.ItemId);

                if (_savedItem.UserId != "")
                {
                    // User is logged in
                    // Check if the item is saved by the user
                    var itemSavedId = await _savedItemService.IsItemSaved(_savedItem.ItemId, _savedItem.UserId);

                    if (itemSavedId != "")
                    {
                        _savedItem.Id = itemSavedId;
                        _itemWasSaved = true;
                        SaveButton.Text = "בטל שמירת מוצר";
                    }
                    else
                    {
                        _itemWasSaved = false;
                        SaveButton.Text = "שמור מוצר";
                    }
                }
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
                if (_savedItem.UserId == "")
                {
                    // User is Not logedin
                    await Navigation.PushAsync(new LoginPage());
                }
                else
                {
                    // User is allowed to save Item

                    if (SaveButton.Text == "שמור מוצר")
                    {
                        SaveButton.Text = "בטל שמירת מוצר";
                        _saveItem = true;
                    }
                    else
                    {
                        SaveButton.Text = "שמור מוצר";
                        _saveItem = false;
                    }
                }
            }

            catch (Exception) { }
        }

        protected async override void OnDisappearing()
        {
            if (_saveItem && !_itemWasSaved)
            {
                await _savedItemService.UploadToServer(_savedItem, null);
            }
            else if (!_saveItem && _itemWasSaved)
            {
                await _savedItemService.DeleteSavedItem(_savedItem);
            }
        }
    }
}