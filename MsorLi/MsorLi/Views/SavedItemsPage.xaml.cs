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

        ObservableCollection<Item> _items = new ObservableCollection<Item>();
        ObservableCollection<string> _imageURLs = new ObservableCollection<string>();

        bool _isItems = false;
        string _userId = Settings._GeneralSettings;
        bool _myBoolean = true;

        //---------------------------------
        // FUNCTIONS
        //---------------------------------

        protected async override void OnAppearing()
        {
            // Check if the user is logged in
            if (Settings._GeneralSettings == "" && _myBoolean)
            {
                _myBoolean = false;

                // If not, go to login
                await Navigation.PushAsync(new LoginPage());
            }

            else if (Settings._GeneralSettings == "" && !_myBoolean)
            {
                await Navigation.PopToRootAsync();
            }

            // Check if user has just looged in
            if (Settings._GeneralSettings != "" && !_myBoolean)
            {
                _myBoolean = true;
                await InitializeAsync();
            }
        }

        public async Task InitializeAsync()
        {
            var savedItemIds = await _savedItemService.GetAllSavedOfUser(Settings._GeneralSettings);

            if (savedItemIds.Count > 0)
            {
                // There Are Saved Items

                _isItems = true;

                for (int i = 0; i < savedItemIds.Count; i++)
                {
                    _items.Add(null);
                    _imageURLs.Add(null);
                }

                List<Task> TaskList = new List<Task>();
                for (int i = 0; i < savedItemIds.Count; i++)
                {
                    var task1 = SetItemAsync(savedItemIds[i], i);
                    TaskList.Add(task1);

                    var task2 = SetImageUrlAsync(savedItemIds[i], i);
                    TaskList.Add(task2);
                }

                await Task.WhenAll(TaskList);
            }

            MyInitializeComponent();
        }

        public void MyInitializeComponent()
        {
            InitializeComponent();

            if (!_isItems)
            {
                NoItems.IsVisible = true;
            }

            ObservableCollection<Tuple<string, string, string, int>> savedItems =
                    new ObservableCollection<Tuple<string, string, string, int>>();

            for (int i = 0; i < _items.Count; i++)
            {
                savedItems.Add(new Tuple<string, string, string, int>
                        (_imageURLs[i], _items[i].Category, _items[i].Location, (App.ScreenHeight / 5)));
            }

            listView_items.ItemsSource = savedItems;
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

            }

            catch (Exception)
            {
                await DisplayAlert("שגיאה", "לא ניתן למחוק מוצר. נסה שנית מאוחר יותר.", "אישור");
            }
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