using MsorLi.Models;
using MsorLi.Services;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MsorLi.Utilities;
using System.Threading.Tasks;

namespace MsorLi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemPage : ContentPage
    {
        //---------------------------------
        // MEMBERS
        //---------------------------------

        public bool _saveItem = false;
        public bool _itemWasSaved = false;

        Item _item = new Item();
        string _userId = Settings.UserId;
        ObservableCollection<ItemImage> _images = new ObservableCollection<ItemImage>();
        string _savedId = "";

        AzureSavedItemService _savedItemService = AzureSavedItemService.DefaultManager;
        AzureImageService _imageService = AzureImageService.DefaultManager;
        AzureItemService _itemService = AzureItemService.DefaultManager;

        //---------------------------------
        // FUNCTIONS
        //---------------------------------

        // INITIALIZE FUNCTIONS
        //----------------------------------------------------------

        // c-tor
        public ItemPage(string itemId)
        {
            _item.Id = itemId;
        }

        public async Task InitializeAsync()
        {
            var task1 = SetItemAsync();
            var task2 = SetItemImagesAsync();
            var task3 = SetItemSavedAsync();

            await Task.WhenAll(task1, task2, task3);

            MyInitializeComponent();
        }

        public void MyInitializeComponent()
        {
            InitializeComponent();

            // Update item details

            title.Text = _item.Category;
            description.Text = _item.Description;
            condition.Text = _item.Condition;
            location.Text = _item.Location;
            contact_name.Text = _item.ContactName;
            contact_number.Text = _item.ContactNumber;
            date.Text = _item.Date;

            // Update item images

            imagesView.HeightRequest = (double)(App.ScreenHeight / 2.5);

            ObservableCollection<Models.Image> images = new ObservableCollection<Models.Image>();

            for (int i = 0; i < _images.Count; ++i)
            {
                Models.Image image = new Models.Image { ImageUrl = _images[i].Url, ImageNumber = (i + 1).ToString() + " מתוך " + _images.Count.ToString() };
                images.Add(image);
            }

            imagesView.ItemsSource = images;

            // Update saved item details

            if (_userId == "") return;    // User is not logged in, nothing to change

            if (_savedId != "")
            {
                // Item is not saved by the user

                _itemWasSaved = true;
                SaveButton.Text = "בטל שמירת מוצר";
            }
            else
            {
                // Item is saved by the user

                _itemWasSaved = false;
                SaveButton.Text = "שמור מוצר";
            }
        }

        // Override OnDisappearing
        protected async override void OnDisappearing()
        {
            if (_saveItem && !_itemWasSaved)
            {
                await _savedItemService.UploadToServer(new SavedItem { ItemId = _item.Id, UserId = _userId }, null);
            }
            else if (!_saveItem && _itemWasSaved)
            {
                await _savedItemService.DeleteSavedItem(new SavedItem { Id = _savedId });
            }
        }

        // EVENT FUNCTIONS
        //----------------------------------------------------------

        private async void SaveButtonClick(object sender, EventArgs e)
        {
            try
            {
                if (_userId == "")
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

            catch (Exception)
            {
                await DisplayAlert("שגיאה", "לא ניתן לשמור מוצר מבוקש. נסה שנית.", "אישור");
            }
        }
        
        // For android only, return to item list
        protected override bool OnBackButtonPressed()
        {
            try
            {
                Navigation.PopAsync();
                return true;
            }

            catch (Exception)
            {
                return false;
            }
        }

        // PRIVATE FUNCTIONS
        //----------------------------------------------------------

        private async Task SetItemAsync()
        {
            _item = await _itemService.GetItemAsync(_item.Id);
        }

        private async Task SetItemImagesAsync()
        {
            _images = await _imageService.GetItemImages(_item.Id);
        }

        private async Task SetItemSavedAsync()
        {
            _savedId = await _savedItemService.IsItemSaved(_item.Id, _userId);
        }
    }
}