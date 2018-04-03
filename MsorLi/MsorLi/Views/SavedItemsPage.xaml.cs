using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MsorLi.Utilities;
using MsorLi.Services;
using MsorLi.Models;
using System.Collections.ObjectModel;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;

namespace MsorLi.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SavedItemsPage : ContentPage
	{
        //---------------------------------
        // MEMBERS
        //---------------------------------

        AzureSavedItemService _savedItemService = AzureSavedItemService.DefaultManager;
        AzureItemService _itemService = AzureItemService.DefaultManager;
        AzureImageService _imageService = AzureImageService.DefaultManager;

        ObservableCollection<SavedItem> _savedItems = new ObservableCollection<SavedItem>();
        ObservableCollection<Item> _items = new ObservableCollection<Item>();
        ObservableCollection<string> _imageURLs = new ObservableCollection<string>();

        ObservableCollection<Tuple<string, string, string, int, int>> _myCollection =
                    new ObservableCollection<Tuple<string, string, string, int, int>>();

        bool _isItems = false;
        string _userId = Settings._GeneralSettings;
        bool _myBoolean = true;
        bool _firstAppearing = true;

        //---------------------------------
        // FUNCTIONS
        //---------------------------------

        protected async override void OnAppearing()
        {
            try
            {
                // User is not logged in
                if (Settings._GeneralSettings == "" && _myBoolean)
                {
                    _myBoolean = false;
                    await Navigation.PushAsync(new LoginPage());
                }

                // User is not logged in and he is back from loog in page
                else if (Settings._GeneralSettings == "" && !_myBoolean)
                {
                    await Navigation.PopToRootAsync();
                }

                // User has just looged in
                else if (Settings._GeneralSettings != "" && !_myBoolean)
                {
                    _myBoolean = true;
                    await InitializeAsync();
                }

                // User is looged in and its his first appearing
                else if (Settings._GeneralSettings != "" && _firstAppearing)
                {
                    _firstAppearing = false;
                    await InitializeAsync();
                }
            }

            catch (Exception)
            {
                await DisplayAlert("שגיאה", "לא לטעון דף מבוקש. נסה שנית מאוחר יותר.", "אישור");
                await Navigation.PopToRootAsync();
            }
        }

        private async Task InitializeAsync()
        {
            _savedItems = await _savedItemService.GetAllSavedOfUser(Settings._GeneralSettings);

            if (_savedItems.Count > 0)
            {
                // There Are Saved Items

                _isItems = true;

                for (int i = 0; i < _savedItems.Count; i++)
                {
                    _items.Add(null);
                    _imageURLs.Add(null);
                }

                List<Task> TaskList = new List<Task>();
                for (int i = 0; i < _savedItems.Count; i++)
                {
                    var task1 = SetItemAsync(_savedItems[i].ItemId, i);
                    TaskList.Add(task1);

                    var task2 = SetImageUrlAsync(_savedItems[i].ItemId, i);
                    TaskList.Add(task2);
                }

                await Task.WhenAll(TaskList);
            }

            MyInitializeComponent();
        }

        private void MyInitializeComponent()
        {
            InitializeComponent();
            listView_items.IsVisible = true;

            if (!_isItems)
            {
                NoItems.IsVisible = true;
            }

            _myCollection.Clear();

            for (int i = 0; i < _items.Count; i++)
            {
                _myCollection.Add(new Tuple<string, string, string, int, int>
                        (_imageURLs[i], _items[i].Category, _items[i].Location, (App.ScreenHeight / 5), i));
            }

            listView_items.ItemsSource = _myCollection;
        }

        // EVENT FUNCTIONS
        //----------------------------------------------------------

        // For android only, return to item list
        protected override bool OnBackButtonPressed()
        {
            try
            {
                Navigation.PopToRootAsync();
                return true;
            }

            catch (Exception)
            {
                return false;
            }
        }

        private async void DeleteSavedItem(object sender, EventArgs e)
        {
            try
            {
                TappedEventArgs obj = e as TappedEventArgs;
                int index = (int)obj.Parameter;

                _myCollection.Remove(new Tuple<string, string, string, int, int>
                    (_imageURLs[index], _items[index].Category, _items[index].Location, (App.ScreenHeight / 5), index));

                listView_items.ItemsSource = _myCollection;

                if (_myCollection.Count == 0)
                {
                    NoItems.IsVisible = true;
                    listView_items.IsVisible = false;
                }

                await _savedItemService.DeleteSavedItem(new SavedItem { Id = _savedItems[index].Id });
            }

            catch (Exception)
            {
                await DisplayAlert("שגיאה", "לא ניתן למחוק מוצר. נסה שנית מאוחר יותר.", "אישור");
            }
        }

        private async void GoToItemPage(object sender, EventArgs e)
        {
            TappedEventArgs obj = e as TappedEventArgs;
            int index = (int)obj.Parameter;

            ItemPage itemPage = new ItemPage(_savedItems[index].ItemId);
            itemPage._itemWasSaved = true;
            itemPage._saveItem = true;

            await itemPage.InitializeAsync();
            await Navigation.PushAsync(itemPage);
        }

        // PRIVATE FUNCTIONS
        //----------------------------------------------------------

        private async Task SetItemAsync(string itemId, int itemIndex)
        {
            Item item = await _itemService.GetItemAsync(itemId);
            _items[itemIndex] = item;
        }

        private async Task SetImageUrlAsync(string itemId, int itemIndex)
        {
            List<string> imageUrl = await _imageService.GetImageUrl(itemId);
            _imageURLs[itemIndex] = imageUrl[0];
        }
    }
}