using MsorLi.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MsorLi.Models;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using MsorLi.Utilities;
using System.Linq;

namespace MsorLi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemListPage : ContentPage
    {
        //---------------------------------
        // MEMBERS
        //---------------------------------

        ObservableCollection<ItemImage> AllImages = new ObservableCollection<ItemImage>();
        ObservableCollection<Tuple<string, string, string, string>> ImagePairs =
                            new ObservableCollection<Tuple<string, string, string, string>>();

        bool _startupRefresh = false;

        // Two variables for OnSelection function
        Boolean _isRunningItem = false;
        Object _lockObject = new Object();

        // The key is category name, the value is boolean,
        // depending on the current category selected.
        // i.e., only one value will be true, the rest false.
        // The defult true value is all items.
        Dictionary<string, bool> _categoryStatus = new Dictionary<string, bool>();

        // The category button that is currently selected
        Button _categoryBtn = new Button();

        //---------------------------------
        // FUNCTIONS
        //---------------------------------

        // Contrusctor
        public ItemListPage()
        {
            // Disable Navigation Bar
            NavigationPage.SetHasNavigationBar(this, false);

            InitializeComponent();

            // Disable Selection item
            listView_items.ItemTapped += (object sender, ItemTappedEventArgs e) => {
                // don't do anything if we just de-selected the row
                if (e.Item == null) return;
                // do something with e.SelectedItem
                ((ListView)sender).SelectedItem = null; // de-select the row
            };

            AddBtn.Text = "פרסום" + Environment.NewLine + "מוצר";

            listView_items.RowHeight = Constants.ScreenWidth / 2;
        }

        protected async override void OnAppearing()
        {
            try
            {
                base.OnAppearing();

                if (!_startupRefresh)
                {
                    _startupRefresh = true;

                    await CreateCategories();
                    
                    await RefreshItems(true, syncItems: true);
                }
            }
            catch
            {
                await DisplayAlert("שגיאה", "לא ניתן לטעון נתונים", "אישור");
            }
        }

        //---------------------------------
        // Event Functions
        //---------------------------------

        private async void OnRefresh(object sender, EventArgs e)
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
                if (list != null)
                {
                    list.EndRefresh();

                }
            }

            if (error != null)
            {
                await DisplayAlert("שגיאה", "לא ניתן לטעון נתונים.", "אישור");
            }
        }

        private async void OnCategoryClick(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            var category = btn.Text;

            if (category == _categoryBtn.Text) return;  // Clicked on the selected category btn

            var CurrentCategory = _categoryStatus.FirstOrDefault(x => x.Value == true).Key;

            _categoryStatus[CurrentCategory] = false;
            _categoryStatus[category] = true;

            btn.BackgroundColor = Color.FromHex("00BCD4");
            _categoryBtn.BackgroundColor = Color.Transparent;
            _categoryBtn = btn;

            await RefreshItems(true, syncItems: true);
        }

        private async void OnSyncItems(object sender, EventArgs e)
        {
            await RefreshItems(true, true);
        }

        private async void OnItemClick(object sender, TappedEventArgs e)
        {
            try
            {
                // To prevent double tap on images
                lock (_lockObject)
                {
                    if (_isRunningItem)
                        return;
                    else
                        _isRunningItem = true;
                }

                var itemId = e.Parameter.ToString();

                if (itemId != "")
                    await Navigation.PushAsync(new ItemPage(itemId));

                _isRunningItem = false;
            }

            catch (Exception)
            {
                await DisplayAlert("שגיאה", "לא ניתן לטעון עמוד מבוקש.", "אישור");
            }
        }

        private async void OnMenuClick(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new MenuPage());
            }

            catch (Exception) { }
        }

        private async void OnAddItemClick(object sender, EventArgs e)
        {
            try
            {
                if(Session.IsLogged()){
                    await Navigation.PushAsync(new AddItemPage());
                }
                else
                {
                    await Navigation.PushAsync(new LoginPage());

                    //when login is finish with success load save item page
                    MessagingCenter.Subscribe<LoginPage>(this, "Success", async (send) => {

                        MessagingCenter.Unsubscribe<LoginPage>(this, "Success");
                        await Navigation.PushAsync(new AddItemPage());
                    });
                }

            }
            catch (Exception)
            {

            }
        }

        //---------------------------------
        // Private functions
        //---------------------------------

        private async Task RefreshItems(bool showActivityIndicator, bool syncItems)
        {
            try
            {
                using (var scope = new ActivityIndicatorScope(syncIndicator, showActivityIndicator))
                {
                    var category = _categoryStatus.FirstOrDefault(x => x.Value == true).Key;

                    AllImages = await AzureImageService.DefaultManager.GetAllPriorityImages(category);
                    if (AllImages != null)
                    {
                        CreateImagePairs();

                        if (ImagePairs != null)
                        {
                            listView_items.ItemsSource = ImagePairs;
                        }
                    }
                }
            }
            catch
            {
                await DisplayAlert("שגיאה", "לא ניתן לטעון נתונים.", "אישור");
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

            catch (Exception)
            {

            }
        }

        private async Task CreateCategories()
        {
            var task = CategoryStorage.GetCategories();
            await Task.WhenAll(task);
            var categories = task.Result;

            // Add categoris to dictionary

            // First add category "הכל"
            _categoryStatus.Add("הכל", true);

            // Then add the categories from the db
            foreach (var c in categories)
            {
                _categoryStatus.Add(c.Name, false);
            }

            // Create button for each category
            for (int i = categories.Count - 1; i >= 0; i--)
            {
                var btn = new Button
                {
                    BackgroundColor = Color.Transparent,
                    Text = categories[i].Name,
                    BorderColor = Color.FromHex("BDBDBD"),
                    BorderWidth = 1,
                    TextColor = Color.FromHex("212121"),
                    CornerRadius = 15,
                };


                btn.Clicked += OnCategoryClick;

                StackCategory.Children.Add(btn);

        
            }

            foreach (Button btn in StackCategory.Children){
                
                if (Device.RuntimePlatform == Device.iOS)
                {
                  //  btn.WidthRequest = btn.Width + 2;
                }
                
            }

            // Create button for all items
            var button = new Button
            {
                BackgroundColor = Color.FromHex("00BCD4"),
                Text = "הכל",
                BorderColor = Color.FromHex("BDBDBD"),
                BorderWidth = 1,
                TextColor = Color.FromHex("212121"),
                CornerRadius = 15,
            };
            button.Clicked += OnCategoryClick;

            // Update current btn to be "הכל" category
            _categoryBtn = button;

            StackCategory.Children.Add(button);

            // Scroll to the right.
            await CategoryScroll.ScrollToAsync(_categoryBtn, ScrollToPosition.MakeVisible, false);
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
                }
            }
        }
    }
}