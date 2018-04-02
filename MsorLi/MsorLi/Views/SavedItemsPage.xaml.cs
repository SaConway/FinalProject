using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MsorLi.Utilities;
using MsorLi.Services;
using System.Collections.ObjectModel;
using System;

namespace MsorLi.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SavedItemsPage : ContentPage
	{
        //---------------------------------
        // MEMBERS
        //---------------------------------

        AzureSavedItemService _savedItemService = AzureSavedItemService.DefaultManager;


        //---------------------------------
        // FUNCTIONS
        //---------------------------------

        async protected override void OnAppearing()
        {
            try
            {
                // Check if the user is logged in
                if (Settings._GeneralSettings == "")
                {
                    // If not, go to login
                    await Navigation.PushAsync(new LoginPage());
                }

                else
                {
                    InitializeComponent();
                    
                    // Create List of Saved Items

                    var userId = Settings._GeneralSettings;
                    var savedItemIds = await _savedItemService.GetAllSavedOfUser(userId);

                    if (savedItemIds.Count == 0)
                    {
                        // There Are None Saved Items
                        NoItems.IsVisible = true;
                    }

                    AzureItemService itemService = AzureItemService.DefaultManager;
                    AzureImageService imageService = AzureImageService.DefaultManager;
                    ObservableCollection<Tuple<string, string, string, int>> savedItems =
                        new ObservableCollection<Tuple<string, string, string, int>>();

                    foreach (var itemId in savedItemIds)
                    {
                        var item = await itemService.GetItemAsync(itemId);
                        var imageUrls = await imageService.GetImageUrl(itemId);

                        if (imageUrls == null)
                        {
                            return;
                        }

                        savedItems.Add(new Tuple<string, string, string, int>
                            (imageUrls[0], item.Category, item.Location, (App.ScreenHeight / 5)));
                    }

                    listView_items.ItemsSource = savedItems;
                }
            }

            catch (Exception)
            {
                await DisplayAlert("שגיאה","לא ניתן לטעון מוצרים. נסה שנית מאוחר יותר.", "אישור");
            }
        }

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

    }
}