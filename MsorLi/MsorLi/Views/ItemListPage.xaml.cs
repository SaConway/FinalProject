using MsorLi.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MsorLi.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MsorLi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemListPage : ContentPage
    {
        //---------------------------------
        // MEMBERS
        //---------------------------------

        AzureItemService _azureItemService = AzureItemService.DefaultManager;
        AzureImageService _azureImageService = AzureImageService.DefaultManager;

        public ObservableCollection<ItemImage> AllImages = new ObservableCollection<ItemImage>();
        public ObservableCollection<Tuple<string, string, string, string>> ImagePairs =
                            new ObservableCollection<Tuple<string, string, string, string>>();
        bool _startupRefresh = false;
        //---------------------------------
        // FUNCTIONS
        //---------------------------------

        // Contrusctor
        public ItemListPage()
        {
            // Disable Navigation Bar
            NavigationPage.SetHasNavigationBar(this, false);

            InitializeComponent();

        }
        protected async override void OnAppearing()

        {
            try
            {
                base.OnAppearing();

                if (!_startupRefresh)
                {
                    await RefreshItems(true, syncItems: true);
                    _startupRefresh = true;
                }
            }
            catch
            {
                await DisplayAlert("שגיאה", "לא ניתן לטעון נתונים", "אישור");

            }
        }

        public async void OnRefresh(object sender, EventArgs e)
        {
            if (sender == null) await DisplayAlert("Refresh Error", "Couldn't refresh data", "OK");

            var list = (ListView)sender;

            Exception error = null;
            try
            {
                await RefreshItems(false, true);
            }
            catch (Exception ex)
            {
                error = ex;
            }
            finally
            {
                if (list != null)
                {
                    list.EndRefresh();

                }
            }

            if (error != null)
            {
                await DisplayAlert("Refresh Error", "Couldn't refresh data (" + error.Message + ")", "OK");
            }
        }

        public async void OnSyncItems(object sender, EventArgs e)
        {
            await RefreshItems(true, true);
        }

        private async Task RefreshItems(bool showActivityIndicator, bool syncItems)
        {
            try
            {
                using (var scope = new ActivityIndicatorScope(syncIndicator, showActivityIndicator))
                {
                    AllImages = await _azureImageService.GetAllPriorityImages();
                    if (AllImages != null)
                    {
                        CreateImagePairs();

                        if (ImagePairs != null)
                        {
                            //listView_items.HasUnevenRows = true;
                            listView_items.ItemsSource = ImagePairs;
                        }
                        else
                            listView_items.ItemsSource = null;
                    }
                }
            }
            catch
            {
                await DisplayAlert("שגיאה", "לא ניתן לטעון נתונים בתוך רענון", "אישור");
            }
        }

        async void OnSelection(object sender, EventArgs e)
        {
            try
            {
                TappedEventArgs obj = e as TappedEventArgs;
                var itemId = obj.Parameter.ToString();

                if (itemId == "") return;

                ItemPage itemPage = new ItemPage(itemId);
                await itemPage.InitializeAsync();
                await Navigation.PushAsync(itemPage);
            }

            catch (Exception)
            {
                await DisplayAlert("שגיאה", "לא ניתן לטעון עמוד מבוקש", "אישור");
            }
        }

        private void CreateImagePairs()
        {
            try
            {
                ImagePairs.Clear();

                for (int i = 0; i < AllImages.Count; i += 2)
                {
                    string Item1 = AllImages[i].Url;
                    string Item2 = AllImages[i].ItemId;
                    string Item3 = i + 1 < AllImages.Count ? AllImages[i + 1].Url : "";
                    string Item4 = i + 1 < AllImages.Count ? AllImages[i + 1].ItemId : "";

                    ImagePairs.Add(new Tuple<string, string, string, string>(Item1, Item2, Item3, Item4));
                }
            }

            catch (Exception) { }
        }

        public async void OpenMenu(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new MenuPage());
            }

            catch (Exception) { }
        }

        private async void AddItem_EventClick(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new AddItemPage());
            }
            catch (Exception)
            {

            }
        }

        //---------------------------------
        // ActivityIndicator
        //---------------------------------
        private class ActivityIndicatorScope : IDisposable
        {
            //---------------------------------
            // MEMBERS
            //---------------------------------
            private bool _showIndicator;
            private ActivityIndicator _indicator;
            private Task _indicatorDelay;

            //---------------------------------
            // FUNCTIONS
            //---------------------------------
            public ActivityIndicatorScope(ActivityIndicator indicator, bool showIndicator)
            {
                try
                {


                    _indicator = indicator;
                    _showIndicator = showIndicator;

                    if (showIndicator)
                    {
                        _indicatorDelay = Task.Delay(2000);
                        SetIndicatorActivity(true);
                    }
                    else
                    {
                        _indicatorDelay = Task.FromResult(0);
                    }
                }
                catch
                {
                    Debug.WriteLine("שגיאה במכוון הטעינה");
                }
            }

            private void SetIndicatorActivity(bool isActive)
            {
                _indicator.IsVisible = isActive;
                _indicator.IsRunning = isActive;
            }

            public void Dispose()
            {
                try
                {
                    if (_showIndicator)
                    {
                        _indicatorDelay.ContinueWith(t => SetIndicatorActivity(false), TaskScheduler.FromCurrentSynchronizationContext());
                    }
                }
                catch
                {
                    Debug.WriteLine("שגיאה במכוון הטעינה");

                }
            }
        }
    }
}