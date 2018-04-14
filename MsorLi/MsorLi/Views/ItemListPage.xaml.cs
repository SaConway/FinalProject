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

        // The key is vategory name, the value is boolean value,
        // depending on the current category selected.
        // The defult is all items.
        Dictionary<string, bool> _categoryStatus = new Dictionary<string, bool>();
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

        private async Task CreateCategories()
        {
            var task = CategoryStorage.GetCategories();
            await Task.WhenAll(task);
            var categories = task.Result;

            // Add categoris to dictionary
            _categoryStatus.Add("הכל", true);
            foreach (var c in categories)
            {
                _categoryStatus.Add(c.Name, false);
            }

            // Create buttons for each category

            for (int i = categories.Count - 1; i >= 0; i--)
            {
                var btn = new Button
                {
                    BackgroundColor = Color.Transparent,
                    Text = categories[i].Name,
                    BorderColor = Color.FromHex("212121"),
                    BorderWidth = 1,
                    TextColor = Color.FromHex("212121"),
                    CornerRadius = 15,
                };
                btn.Clicked += OnCategoryBtnClicked;

                StackCategory.Children.Add(btn);
            }

            // Create button for all items
            var button = new Button
            {
                BackgroundColor = Color.FromHex("00BCD4"),
                Text = "הכל",
                BorderColor = Color.FromHex("212121"),
                BorderWidth = 1,
                TextColor = Color.FromHex("212121"),
                CornerRadius = 15,
            };
            button.Clicked += OnCategoryBtnClicked;

            _categoryBtn = button;

            StackCategory.Children.Add(button);
        }

        private void OnCategoryBtnClicked(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            var category = btn.Text;

            var CurrentCategory = _categoryStatus.FirstOrDefault(x => x.Value == true).Key;

            _categoryStatus[CurrentCategory] = false;
            _categoryStatus[category] = true;

            btn.BackgroundColor = Color.FromHex("00BCD4");
            _categoryBtn.BackgroundColor = Color.Transparent;
            _categoryBtn = btn;
        }

        protected async override void OnAppearing()
        {
            try
            {
                base.OnAppearing();

                if (!_startupRefresh)
                {
                    await CreateCategories();

                    await RefreshItems(true, syncItems: true);
                    _startupRefresh = true;
                }
            }
            catch
            {
                await DisplayAlert("שגיאה", "לא ניתן לטעון נתונים", "אישור");
            }
        }

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

        private async void OnSyncItems(object sender, EventArgs e)
        {
            await RefreshItems(true, true);
        }

        private async Task RefreshItems(bool showActivityIndicator, bool syncItems)
        {
            try
            {
                using (var scope = new ActivityIndicatorScope(syncIndicator, showActivityIndicator))
                {
                    var category = _categoryStatus.FirstOrDefault(x => x.Value == true).Key;

                    AllImages = await AzureImageService.DefaultManager.GetAllPriorityImages();
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

        private async void OnSelection(object sender, TappedEventArgs e)
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

        private async void OpenMenu(object sender, SelectedItemChangedEventArgs e)
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
                MessagingCenter.Send<ItemListPage>(this, "FirstApearing");
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