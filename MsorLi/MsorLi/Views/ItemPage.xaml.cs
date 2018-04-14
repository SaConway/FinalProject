using MsorLi.Models;
using MsorLi.Services;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MsorLi.Utilities;
using System.Threading.Tasks;
using System.ComponentModel;

namespace MsorLi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemPage : ContentPage, INotifyPropertyChanged
    {
        //---------------------------------
        // MEMBERS
        //---------------------------------

        public bool _saveItem = false;
        public bool _itemWasSaved = false;
        public bool _unSaveItem = false;

        Item _item = new Item();
        string _userId = Settings.UserId;
        ObservableCollection<ItemImage> _images = new ObservableCollection<ItemImage>();
        string _savedId = "";

        bool _firstAppearing = true;

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

        protected override async void OnAppearing()
        {
            if (_firstAppearing)
            {
                _firstAppearing = false;

                InitializeComponent();

                MyScrollView.IsVisible = false;
                MyScrollView.Opacity = 0;

                MyActivityIndicator.IsRunning = true;
                MyActivityIndicator.IsVisible = true;

                await InitializeAsync();
            }
        }

        async Task InitializeAsync()
        {
            var task1 = SetItemAsync();
            var task2 = SetItemImagesAsync();
            var task3 = SetItemSavedAsync();

            await Task.WhenAll(task1, task2, task3);

            MyInitializeComponent();
        }

        async void MyInitializeComponent()
        {
            // Update item images

            imagesView.HeightRequest = (double)(Constants.ScreenHeight / 2.5);

            ObservableCollection<Models.Image> images = new ObservableCollection<Models.Image>();

            for (int i = 0; i < _images.Count; ++i)
            {
                Models.Image image = new Models.Image { ImageUrl = _images[i].Url, ImageNumber = (i + 1).ToString() + " מתוך " + _images.Count.ToString() };
                images.Add(image);
            }

            imagesView.ItemsSource = images;

            // Update saved item details

            if (_savedId != "")
            {
                // Item is saved by the user

                _itemWasSaved = true;
                UnSaveStack.IsVisible = true;
            }
            else
            {
                // Item is not saved by the user
                
                SaveStack.IsVisible = true;
            }

            // Update item details

            title.Text = _item.Category;
            description.Text = _item.Description;
            condition.Text = _item.Condition;
            location.Text = _item.Location;
            contact_name.Text = _item.ContactName;
            contact_number.Text = _item.ContactNumber;
            date.Text = _item.Date;

            // Hide Activity Indicator
            await MyActivityIndicator.FadeTo(0, 100);
            MyActivityIndicator.IsRunning = false;
            MyActivityIndicator.IsVisible = false;

            // Show View
            MyScrollView.IsVisible = true;
            await MyScrollView.FadeTo(1, 100);
        }

        // Override OnDisappearing
        protected async override void OnDisappearing()
        {
            //Save Item
            if (_saveItem && !_itemWasSaved)
            {
                await AzureSavedItemService.DefaultManager.UploadToServer(new SavedItem { ItemId = _item.Id, UserId = _userId }, null);
                UpdateLikeCounter(1);
            }
            //Unsave Item
            else if (_unSaveItem && _itemWasSaved)
            {
                await AzureSavedItemService.DefaultManager.DeleteSavedItem(new SavedItem { Id = _savedId });

                MessagingCenter.Send<ItemPage, string>(this, "Item Deleted", _item.Id);
            }
            else
            {
                MessagingCenter.Send<ItemPage>(this, "Back From Item Page");
                UpdateLikeCounter(-1);
            }
        }

        // Update liked items counter
        private async void UpdateLikeCounter(int prefix)
        {
            int _numOfLikedItem = await AzureUserService.DefaultManager.UpdateNumOfItemsLiked(Settings.UserId, prefix);
            Settings.NumOfItemsUserLike = _numOfLikedItem.ToString();
            MessagingCenter.Send<ItemPage>(this, "Update Like Counter");

        }

        // EVENT FUNCTIONS
        //----------------------------------------------------------

        // Save Button
        private async void SaveButtonClick(object sender, EventArgs e)
        {
            try
            {
                if (Settings.UserId == "")
                {
                    // User is Not loged in
                    await Navigation.PushAsync(new LoginPage());
                }
                else
                {  // User is allowed to save Item

                    if (SaveStack.IsVisible == true)
                    {
                        // Change from save to unsave

                        UnSaveStack.Opacity = 0;
                        await SaveStack.FadeTo(0, 100);
                        SaveStack.IsVisible = false;
                        UnSaveStack.IsVisible = true;
                        await UnSaveStack.FadeTo(1);

                        _saveItem = true;
                        _unSaveItem = false;
                    }
                    else
                    {
                        // Change from unsave to save

                        SaveStack.Opacity = 0;
                        await UnSaveStack.FadeTo(0, 100);
                        UnSaveStack.IsVisible = false;
                        SaveStack.IsVisible = true;
                        await SaveStack.FadeTo(1);

                        _unSaveItem = true;
                        _saveItem = false;
                    }
                }
            }

            catch (Exception)
            {
                await DisplayAlert("שגיאה", "לא ניתן לשמור מוצר מבוקש. נסה שנית.", "אישור");
            }
        }

        // Share button
        private void ShareButtonClick(object sender, EventArgs e)
        {
            DependencyService.Get<Utilities.IShare>().Share("מוצר למסירה, דרך אפליקציית מסור-לי.",
                 "מוצר למסירה, דרך אפליקציית מסור--לי. להלן פרטי המוצר ודרכי התקשרות:" + Environment.NewLine + Environment.NewLine
                     + "קטגוריה: " + _item.Category + Environment.NewLine
                     + "מיקום: " + _item.Location + Environment.NewLine
                     + "מצב מוצר: " + _item.Condition + Environment.NewLine
                     + "תיאור: " + _item.Description + Environment.NewLine
                     + "שם איש קשר: " + _item.ContactName + Environment.NewLine
                     + "טלפון ליצירת קשר: " + _item.ContactNumber + Environment.NewLine + Environment.NewLine
                     + "למגוון מוצרים נוספים אנא הורד את אפליקציית מסור-לי.",
                    _images[0].Url
                );
        }

        // PRIVATE FUNCTIONS
        //----------------------------------------------------------

        private async Task SetItemAsync()
        {
            _item = await AzureItemService.DefaultManager.GetItemAsync(_item.Id);
        }

        private async Task SetItemImagesAsync()
        {
            _images = await AzureImageService.DefaultManager.GetItemImages(_item.Id);
        }

        private async Task SetItemSavedAsync()
        {
            _savedId = await AzureSavedItemService.DefaultManager.IsItemSaved(_item.Id, _userId);
        }
    }
}