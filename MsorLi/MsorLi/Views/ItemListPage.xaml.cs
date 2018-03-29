using MsorLi.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MsorLi.Models;
using System.Collections.ObjectModel;
using System.Collections;
using System.Collections.Generic;

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

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await RefreshItems(true, syncItems: true);
        }

        public async void OnRefresh(object sender, EventArgs e)
        {
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
                list.EndRefresh();
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
            using (var scope = new ActivityIndicatorScope(syncIndicator, showActivityIndicator))
            {
                AllImages = await _azureImageService.GetAllPriorityImages();
                CreateImagePairs();
                listView_items.ItemsSource = ImagePairs;
            }
        }

        async void OnSelection(object sender, EventArgs e)
        {
            try
            {
                TappedEventArgs obj = e as TappedEventArgs;
                var itemId = obj.Parameter.ToString();

                await Navigation.PushAsync(new ItemPage(itemId));
            }

            catch (Exception) { }
        }

        private void CreateImagePairs()
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

        public async void OpenMenu(object sender, SelectedItemChangedEventArgs e)
        {

            try
            {
                
                await Navigation.PushAsync(new MenuPage());
            }

            catch (Exception) { }
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

            private void SetIndicatorActivity(bool isActive)
            {
                _indicator.IsVisible = isActive;
                _indicator.IsRunning = isActive;
            }

            public void Dispose()
            {
                if (_showIndicator)
                {
                    _indicatorDelay.ContinueWith(t => SetIndicatorActivity(false), TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
        }
    }
}