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

        ObservableCollection<SavedItem> _savedItems = new ObservableCollection<SavedItem>();
        ObservableCollection<Item> _items = new ObservableCollection<Item>();
        ObservableCollection<string> _imageURLs = new ObservableCollection<string>();

        ObservableCollection<Tuple<int, string, string, string>> _myCollection =
                    new ObservableCollection<Tuple<int, string, string, string>>();

        //---------------------------------
        // FUNCTIONS
        //---------------------------------

        public SavedItemsPage()
        {
            // User has just looged in
            MessagingCenter.Subscribe<LoginPage>(this, "Success", async (sender) => {

                MessagingCenter.Unsubscribe<LoginPage>(this, "Success");
                await InitializeAsync();
            });

            // User is not logged in and he is back from log in page
            MessagingCenter.Subscribe<LoginPage>(this, "NotSuccess", async (sender) => {

                MessagingCenter.Unsubscribe<LoginPage>(this, "NotSuccess");
                await Navigation.PopAsync();

            });

            MessagingCenter.Subscribe<MenuPage>(this, "FirstApearing", async (sender) => {

                MessagingCenter.Unsubscribe<MenuPage>(this, "FirstApearing");

                if (Settings.UserId != "")
                {
                    // User is looged in and its his first appearing
                    await InitializeAsync();
                }
                else
                {
                    // User is not logged in
                    await Navigation.PushAsync(new LoginPage());
                }
            });

            MessagingCenter.Subscribe<ItemPage>(this, "Item Deleted", async (sender) => {

                MessagingCenter.Unsubscribe<LoginPage>(this, "Item Deleted");
                //Delete item

            });
        }

        private async Task InitializeAsync()
        {
            _savedItems = await AzureSavedItemService.DefaultManager.GetAllSavedOfUser(Settings.UserId);

            if (_savedItems.Count > 0)
            {
                // There Are Saved Items

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
            
            // Disable Selection item
            listView_items.ItemTapped += (object sender, ItemTappedEventArgs e) => {
                // don't do anything if we just de-selected the row
                if (e.Item == null) return;
                // do something with e.SelectedItem
                ((ListView)sender).SelectedItem = null; // de-select the row
            };

            if (_savedItems.Count == 0)
            {
                NoItems.IsVisible = true;
                return;
            }

            listView_items.IsVisible = true;
            listView_items.RowHeight = Constants.ScreenHeight / 5;

            _myCollection.Clear();

            for (int i = 0; i < _items.Count; i++)
            {
                _myCollection.Add(new Tuple<int, string, string, string>
                        (i, _items[i].Category, _items[i].Location, _imageURLs[i]));
            }

            listView_items.ItemsSource = _myCollection;
        }

        // EVENT FUNCTIONS
        //----------------------------------------------------------

        private async void DeleteSavedItem(object sender, EventArgs e)
        {
            try
            {
                TappedEventArgs obj = e as TappedEventArgs;
                int index = (int)obj.Parameter;

                DeleteItemFromList(index);

                await AzureSavedItemService.DefaultManager.DeleteSavedItem(new SavedItem { Id = _savedItems[index].Id });
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
            itemPage._unSaveItem = false;

            //await itemPage.InitializeAsync();
            await Navigation.PushAsync(itemPage);
        }

        // PRIVATE FUNCTIONS
        //----------------------------------------------------------

        private void DeleteItemFromList(int index)
        {
            _myCollection.Remove(new Tuple<int, string, string, string>
                    (index, _items[index].Category, _items[index].Location, _imageURLs[index]));

            listView_items.ItemsSource = _myCollection;

            if (_myCollection.Count == 0)
            {
                NoItems.IsVisible = true;
                listView_items.IsVisible = false;
            }
        }

        private async Task SetItemAsync(string itemId, int itemIndex)
        {
            Item item = await AzureItemService.DefaultManager.GetItemAsync(itemId);
            _items[itemIndex] = item;
        }

        private async Task SetImageUrlAsync(string itemId, int itemIndex)
        {
            List<string> imageUrl = await AzureImageService.DefaultManager.GetImageUrl(itemId);
            _imageURLs[itemIndex] = imageUrl[0];
        }
    }
}