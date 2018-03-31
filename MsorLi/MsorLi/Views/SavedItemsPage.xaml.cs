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

        //---------------------------------
        // FUNCTIONS
        //---------------------------------

        async protected override void OnAppearing()
        {
            try
            {
                if (Settings._GeneralSettings == "")
                {
                    await Navigation.PushAsync(new LoginPage());
                }

                else // User has Logged In
                {
                    InitializeComponent();
                    
                    // Create List of Saved Items

                    var userId = Settings._GeneralSettings;
                    AzureSavedItemService savedItemService = AzureSavedItemService.DefaultManager;
                    var savedItemIds = await savedItemService.GetAllSavedOfUser(userId);

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

        // For Android Only
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
    }
}