using MsorLi.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MsorLi.Models;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using MsorLi.Utilities;
using Xamarin.Forms.Extended;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MsorLi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemListPage : ContentPage
    {
        //---------------------------------
        // MEMBERS
        //---------------------------------

        ObservableCollection<ItemImage> AllImages = new ObservableCollection<ItemImage>();
        ObservableCollection<Tuple<string, string, string, string, bool, string, string>> ImagePairs =
                            new ObservableCollection<Tuple<string, string, string, string, bool, string, string>>();

        bool _startupRefresh = false;

        Boolean _isRunningItem = false;
        Object _lockObject = new Object();

        StackLayout _currentCategoryStackLayout = new StackLayout();

        List<string> _categoryIconSources = new List<string>();

        string _currentCategory = "כל המוצרים";
        string _currentSubCategory = "";

        private bool _isBusy;
        int _numOfItems = 0;

        //---------------------------------
        // FUNCTIONS
        //---------------------------------

        public bool Is_Busy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
            }
        }

        // Contrusctor
        public ItemListPage()
        {
            
            // Hide Navigation Bar
            NavigationPage.SetHasNavigationBar(this, false);

            InitializeComponent();

            NoItemsLabel.IsVisible = false;


            // Disable Selection Item
            listView_items.ItemTapped += (object sender, ItemTappedEventArgs e) =>
            {
                if (e.Item == null) return;
                ((ListView)sender).SelectedItem = null;
            };

            listView_items.RowHeight = Constants.ScreenWidth / 2;

            // Add catwgory icon sources
            _categoryIconSources.Add("fashion.png");
            _categoryIconSources.Add("electronic.png");
            _categoryIconSources.Add("picture.png");
            _categoryIconSources.Add("pingpong.png");
            _categoryIconSources.Add("all.png");

            // Listen to filters
            MessagingCenter.Subscribe<FilterPage, Tuple<string, string>>(this, "Back From Filter", async (sender, filterResult) =>
            {
                // Update filter results
                _currentCategory = filterResult.Item1;
                _currentSubCategory = filterResult.Item2;

                // Hide categories
                CategoryMainStack.IsVisible = false;
                FilterCategoryLabel.Text = _currentSubCategory != "" ? filterResult.Item1 + ", " + filterResult.Item2 : filterResult.Item1;
                FilterMainStack.IsVisible = true;

                await RefreshItems(true, true);
            });

            ImagePairs = new InfiniteScrollCollection<Tuple<string, string, string, string>>
            {
                OnLoadMore = async () =>
                {
                    Is_Busy = true;

                    // load the next page
                    var page = ImagePairs.Count * 2 / Constants.PAGE_SIZE;

                    AllImages = await AzureImageService.DefaultManager.GetAllPriorityImages(page, _currentCategory, _currentSubCategory); 

                    ObservableCollection<Tuple<string, string, string, string>> ip = new ObservableCollection<Tuple<string, string, string, string>>();

					_numOfItems = await AzureImageService.DefaultManager.NumOfItems(_currentCategory, _currentSubCategory);

                    if (AllImages != null)
                    {
                        ip = CreateImagePairs();

                        if (ImagePairs != null)
                        {
                            listView_items.ItemsSource = ImagePairs;
                        }
                    }
                    Is_Busy = false;

                    // return the items that need to be added
                    return ip;
                },
                OnCanLoadMore = () =>
                {
					Boolean count =  ImagePairCount() < _numOfItems;
					if (count)
					{
						Footer1.IsVisible = true;
						Footer2.IsVisible = false;
					}
					else
					{
						Footer1.IsVisible = false;
                        Footer2.IsVisible = true;
					}
					return count;
                }
            };
        }

        protected async override void OnAppearing()
        {
            try
            {
                base.OnAppearing();

                if (!_startupRefresh)
                {
                    _startupRefresh = true;

                    CategoryMainStack.IsVisible = false;
                    CategoryMainStack.IsEnabled = false;

                    Task t1 = CreateCategories();
                    Task t2 =  RefreshItems(true, true);
                    await Task.WhenAll(t1, t2);

                    CategoryMainStack.IsVisible = true;

                    // Scroll to the right.
                    await CategoryScroll.ScrollToAsync(StackCategory.Children[StackCategory.Children.Count - 1], ScrollToPosition.MakeVisible, true); CategoryMainStack.IsEnabled = true;

                    CategoryMainStack.IsEnabled = true;
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
                //string category = (_currentCategoryStackLayout.Children[1] as Label).Text;
                await RefreshItems(false, true);
            }
            catch (Exception)
            {

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
            try
            {
                var category = (e as TappedEventArgs).Parameter.ToString();

                _currentCategory = category;

                // Update old category
                (_currentCategoryStackLayout.Children[1] as Label).TextColor = Color.FromHex("212121");
                (_currentCategoryStackLayout.Children[2] as BoxView).IsVisible = false;

                // Update new category
                var s = sender as StackLayout;
                (s.Children[1] as Label).TextColor = Color.FromHex("00BCD4");
                (s.Children[2] as BoxView).IsVisible = true;

                _currentCategoryStackLayout = s;

                // Scroll to current category
                //problem with iOS
                //await CategoryScroll.ScrollToAsync(_currentCategoryStackLayout, ScrollToPosition.MakeVisible, true);
                CategoryMainStack.IsEnabled = true;


                await RefreshItems(true, true);
            }
            catch(Exception){

              
            }
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
                lock (_lockObject)
                {
                    if (_isRunningItem)
                        return;
                    else
                        _isRunningItem = true;
                }

                await Navigation.PushAsync(new MenuPage());

                _isRunningItem = false;
            }

            catch (Exception) { }
        }

        private async void OnAddItemClick(object sender, EventArgs e)
        {
            try
            {
                lock (_lockObject)
                {
                    if (_isRunningItem)
                        return;
                    else
                        _isRunningItem = true;
                }

                if (Session.IsLogged())
                {
                    await Navigation.PushAsync(new AddItemPage());
                }
                else
                {
                    await Navigation.PushAsync(new LoginPage());

                    // If login is finish with success, load add item page
                    MessagingCenter.Subscribe<LoginPage>(this, "Success", async (send) =>
                    {
                        MessagingCenter.Unsubscribe<LoginPage>(this, "Success");
                        await Navigation.PushAsync(new AddItemPage());
                    });
                }

                _isRunningItem = false;
            }
            catch (Exception) { }
        }

        private async void OnFilterClick(object sender, EventArgs e)
        {
            try
            {
                lock (_lockObject)
                {
                    if (_isRunningItem)
                        return;
                    else
                        _isRunningItem = true;
                }

                await Navigation.PushAsync(new FilterPage(_currentCategory, _currentSubCategory));

                _isRunningItem = false;
            }
            catch (Exception)
            {

            }
        }

        private async void OnRemoveFilterClick(object sender, EventArgs e)
        {
            try
            {
                _currentCategory = "כל המוצרים";
                _currentSubCategory = "";

                // Update old category
                (_currentCategoryStackLayout.Children[1] as Label).TextColor = Color.FromHex("212121");
                (_currentCategoryStackLayout.Children[2] as BoxView).IsVisible = false;

                _currentCategoryStackLayout = (StackLayout)StackCategory.Children[StackCategory.Children.Count - 1];

                // Update new category
                StackLayout sl = (StackLayout)StackCategory.Children[StackCategory.Children.Count - 1];
                (sl.Children[1] as Label).TextColor = Color.FromHex("212121");
                (sl.Children[2] as BoxView).IsVisible = true;

                CategoryMainStack.IsVisible = true;
                FilterMainStack.IsVisible = false;

                await RefreshItems(true, true);
            }
            catch (Exception)
            {

            }
        }

        private async void OnFilterEreaClick(object sender, EventArgs e)
        {
            try
            {
                lock (_lockObject)
                {
                    if (_isRunningItem)
                        return;
                    else
                        _isRunningItem = true;
                }

                await Navigation.PushAsync(new FilterPage(_currentCategory, _currentSubCategory));

                _isRunningItem = false;
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
                    AllImages = await AzureImageService.DefaultManager.GetAllPriorityImages(0, _currentCategory, _currentSubCategory);
					_numOfItems = await AzureImageService.DefaultManager.NumOfItems(_currentCategory, _currentSubCategory);
                    ImagePairs.Clear();
                    if (AllImages.Count > 0)
                    {
                        NoItemsLabel.IsVisible = false;

                        var temp_image_pair =  CreateImagePairs();
                        ImagePairs.AddRange(temp_image_pair);
                        if (ImagePairs != null)
                        {
                            listView_items.ItemsSource = ImagePairs;
                        }
                        return;
                    }

					Footer2.IsVisible = false;
                    NoItemsLabel.IsVisible = true;
                    Is_Busy = false;
                }
            }
            catch(Exception)
            {
                await DisplayAlert("שגיאה", "לא ניתן לטעון נתונים.", "אישור");
            }
        }

        private ObservableCollection<Tuple<string, string, string, string>> CreateImagePairs()
        {
            try
            {
                //ImagePairs.Clear();
                ObservableCollection<Tuple<string, string, string, string>> ip  = 
                    new ObservableCollection<Tuple<string, string, string, string>>() ;
                
                for (int i = 0; i < AllImages.Count; i += 2)
                {
                    string Item1 = AllImages[i].Url;
                    string Item2 = AllImages[i].ItemId;
                    string Item3 = i + 1 < AllImages.Count ? AllImages[i + 1].Url : "";
                    string Item4 = i + 1 < AllImages.Count ? AllImages[i + 1].ItemId : "";
                    bool Item5 = i + 1 < AllImages.Count ? true : false;
                    string Item6 = AllImages[i].Location;
                    string Item7 = i + 1 < AllImages.Count ? AllImages[i].Location : "";

                    ImagePairs.Add(new Tuple<string, string, string, string, bool, string, string>
                        (Item1, Item2, Item3, Item4, Item5, Item6, Item7));
                }
                return ip;
            }

            catch (Exception)
            {
                return null;
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
                defultCategory: true);
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
                HeightRequest = 24,
                WidthRequest = 24
            };

            var box = new BoxView
            {
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

		public int ImagePairCount()
		{
			int count = 0;

			for (int i = 0; i < ImagePairs.Count; i ++)
			{
				if (ImagePairs[i].Item1 != "")
					count++;
				if (ImagePairs[i].Item3 != "")
					count++;
			}
			return count;
		}
    }
}