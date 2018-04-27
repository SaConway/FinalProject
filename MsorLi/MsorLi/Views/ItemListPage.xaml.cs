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

        StackLayout _currentCategoryStackLayout = new StackLayout();

        List<string> _categoryIconSources = new List<string>();

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
                if (e.Item == null) return;
                ((ListView)sender).SelectedItem = null; // de-select the row
            };

            //AddBtn.Text = "+";

            listView_items.RowHeight = Constants.ScreenWidth / 2;

            // Add catwgory icon sources
            _categoryIconSources.Add("fashion.png");
            _categoryIconSources.Add("electronic.png");
            _categoryIconSources.Add("picture.png");
            _categoryIconSources.Add("pingpong.png");
            _categoryIconSources.Add("all.png");
        }

        protected async override void OnAppearing()
        {
            try
            {
                base.OnAppearing();

                if (!_startupRefresh)
                {
                    Task t1 = CreateCategories();
                    Task t2 = RefreshItems(true, syncItems: true);
                    await Task.WhenAll(t1, t2);

                    _startupRefresh = true;
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
            var category = (e as TappedEventArgs).Parameter.ToString();

            // Update new category
            var s = sender as StackLayout;
            (s.Children[1] as Label).TextColor = Color.FromHex("00BCD4");
            (s.Children[2] as BoxView).IsVisible = true;

            // Update old category
            (_currentCategoryStackLayout.Children[1] as Label).TextColor = Color.FromHex("212121");
            (_currentCategoryStackLayout.Children[2] as BoxView).IsVisible = false;

            _currentCategoryStackLayout = s;

            await RefreshItems(true, syncItems: true);
        }

        private async void OnSyncItems(object sender, EventArgs e)
        {
            await RefreshItems(true, true);
        }

        private async void OnItemClick(object sender, EventArgs e)
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

                var itemId = (e as TappedEventArgs).Parameter.ToString();

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
                    string category = null;
                    try
                    {
                        category = (_currentCategoryStackLayout.Children[1] as Label).Text;
                    }
                    catch
                    {
                        category = "כל המוצרים";
                    }

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
            var categories = await CategoryStorage.GetCategories();

            // Create button for each category
            for (int i = categories.Count - 1; i >= 0; i--)
            {
                CreateCategory(categories[i].Name, _categoryIconSources[i]);
            }

            // Create button for all items
            CreateCategory("כל המוצרים",
                _categoryIconSources[_categoryIconSources.Count - 1],
                defultCategory : true);

            // Scroll to the right.
            await CategoryScroll.ScrollToAsync(StackCategory.Children[StackCategory.Children.Count - 1], ScrollToPosition.MakeVisible, true);
        }

        void CreateCategory(string categoryName, string categoryIconSource, bool defultCategory = false)
        {
            var stack = new StackLayout();

            var label = new Label
            {
                Text = categoryName,
                Margin = new Thickness(5, 0, 5, 0),
                HorizontalOptions = LayoutOptions.Center,
            };

            var img = new Xamarin.Forms.Image
            {
                Source = categoryIconSource,
                 HorizontalOptions = LayoutOptions.Center,
            };

            var box = new BoxView {
                 HeightRequest = 5,
                 HorizontalOptions = LayoutOptions.FillAndExpand,
                 BackgroundColor = Color.FromHex("00BCD4"),
            };

            if (defultCategory)
            {
                label.TextColor = Color.FromHex("00BCD4");
            }
            else
            {
                label.TextColor = Color.FromHex("212121");
                box.IsVisible = false;
            }

            stack.Children.Add(img);
            stack.Children.Add(label);
            stack.Children.Add(box);

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += OnCategoryClick;
            tapGestureRecognizer.CommandParameter = categoryName;

            stack.GestureRecognizers.Add(tapGestureRecognizer);


            StackCategory.Children.Add(stack);

            if (defultCategory) _currentCategoryStackLayout = stack;
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