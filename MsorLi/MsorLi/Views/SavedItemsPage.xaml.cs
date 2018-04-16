using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MsorLi.Utilities;
using MsorLi.Services;
using MsorLi.Models;
using System.Collections.ObjectModel;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MsorLi.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SavedItemsPage : ContentPage
	{
        //---------------------------------
        // MEMBERS
        //---------------------------------

        Dictionary<string, SavedItemsHelper> _dictionary = new Dictionary<string, SavedItemsHelper>();
        ObservableCollection<SavedItemsHelper> _collection = new ObservableCollection<SavedItemsHelper>();

        //---------------------------------
        // FUNCTIONS
        //---------------------------------

        // C-tor
        public SavedItemsPage()
        {


            MessagingCenter.Subscribe<MenuPage>(this, "FirstApearing", async (sender) => {

                MessagingCenter.Unsubscribe<MenuPage>(this, "FirstApearing");

                // User is looged in and its his first appearing
                await InitializeAsync();
            });

            // Returning from item page, and item was unsaved
            MessagingCenter.Subscribe<ItemPage, string>(this, "Item Deleted", (sender, key) => {

                MessagingCenter.Unsubscribe<LoginPage, string>(this, "Item Deleted");

                try
                {
                    // Delete Item from collection and dictionary
                    _collection.Remove(_dictionary[key]);
                    _dictionary.Remove(key);

                    MyMainStack.IsVisible = true;
                    MyMainStack.Opacity = 1;

                    if (_dictionary.Count == 0)
                    {
                        listView_items.IsVisible = false;
                        NoItems.IsVisible = true;
                    }
                }

                catch
                {

                }

            });

            // Returning from item page, and item was not unsaved
            MessagingCenter.Subscribe<ItemPage>(this, "Back From Item Page", (sender) => {

                MessagingCenter.Unsubscribe<LoginPage>(this, "Back From Item Page");

                MyMainStack.IsVisible = true;
                MyMainStack.Opacity = 1;
            });
            //MessagingCenter.Subscribe<ItemPage>(this, "Item Deleted", async (sender) => {

            //    MessagingCenter.Unsubscribe<LoginPage>(this, "Item Deleted");
            //    //Delete item

            //});
        }

        private async Task InitializeAsync()
        {
            try
            {
                InitializeComponent();
                listView_items.RowHeight = Constants.ScreenHeight / 5;

                await RefreshItems();

                // Disable Selection item
                listView_items.ItemTapped += (object sender, ItemTappedEventArgs e) => {
                    // don't do anything if we just de-selected the row
                    if (e.Item == null) return;
                    // do something with e.SelectedItem
                    ((ListView)sender).SelectedItem = null; // de-select the row
                };
            }

            catch
            {

            }
        }

        private async Task RefreshItems()
        {
            // Hide View
            MyMainStack.IsVisible = false;
            MyMainStack.Opacity = 0;

            // Show Activity Indicator
            MyActivityIndicator.IsRunning = true;
            MyActivityIndicator.IsVisible = true;
            MyActivityIndicator.Opacity = 1;

            var savedItems = await AzureSavedItemService.DefaultManager.GetAllSavedOfUser(Settings.UserId);

            if (savedItems.Count == 0)
            {
                listView_items.IsVisible = false;
                NoItems.IsVisible = true;
            }
            else
            {
                List<Task> TaskList = new List<Task>();
                for (int i = 0; i < savedItems.Count; i++)
                {
                    _dictionary.Add(savedItems[i].ItemId, new SavedItemsHelper {
                        Key = savedItems[i].ItemId, SavedId = savedItems[i].Id
                    });

                    var task1 = SetItemAsync(savedItems[i].ItemId, savedItems[i].ItemId);
                    TaskList.Add(task1);

                    var task2 = SetImageUrlAsync(savedItems[i].ItemId, savedItems[i].ItemId);
                    TaskList.Add(task2);
                }

                await Task.WhenAll(TaskList);

                foreach (var item in _dictionary)
                {
                    _collection.Add(item.Value);
                }

                listView_items.ItemsSource = _collection;
            }

            // Hide Activity Indicator
            await MyActivityIndicator.FadeTo(0, 100);
            MyActivityIndicator.IsRunning = false;
            MyActivityIndicator.IsVisible = false;

            // Show View
            MyMainStack.IsVisible = true;
            await MyMainStack.FadeTo(1, 100);
        }

        // EVENT FUNCTIONS
        //----------------------------------------------------------

        private async void DeleteSavedItem(object sender, TappedEventArgs e)
        {
            try
            {
                string key = (string)e.Parameter;

                // Delete Item from data base
                await AzureSavedItemService.DefaultManager
                    .DeleteSavedItem(new SavedItem { Id = _dictionary[key].SavedId });

                // Delete Item from collection and dictionary
                _collection.Remove(_dictionary[key]);
                _dictionary.Remove(key);

                if (_dictionary.Count == 0)
                {
                    listView_items.IsVisible = false;
                    NoItems.IsVisible = true;
                }
            }

            catch (Exception)
            {
                await DisplayAlert("שגיאה", "לא ניתן למחוק מוצר. נסה שנית מאוחר יותר.", "אישור");
            }
        }

        private async void GoToItemPage(object sender, TappedEventArgs e)
        {
            string id = (string)e.Parameter;

            ItemPage itemPage = new ItemPage(id)
            {
                _itemWasSaved = true,
                _saveItem = true,
                _unSaveItem = false
            };

            await Navigation.PushAsync(itemPage);

            MyMainStack.IsVisible = false;
            MyMainStack.Opacity = 0;
        }

        // PRIVATE FUNCTIONS
        //----------------------------------------------------------

        private async Task SetItemAsync(string itemId, string index)
        {
            Item item = await AzureItemService.DefaultManager.GetItemAsync(itemId);
            _dictionary[index].Category = item.Category;
            _dictionary[index].Location = item.Location;
        }

        private async Task SetImageUrlAsync(string itemId, string index)
        {
            var imageUrl = await AzureImageService.DefaultManager.GetImageUrl(itemId);
            _dictionary[index].ImageUrl = imageUrl;
        }
    }
}