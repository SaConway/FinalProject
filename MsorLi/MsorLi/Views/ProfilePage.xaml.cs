using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using FFImageLoading.Forms;
using FFImageLoading.Transformations;
using MsorLi.Models;
using MsorLi.Services;
using MsorLi.Utilities;
using Xamarin.Forms;

namespace MsorLi.Views
{
    public partial class ProfilePage : ContentPage
    {

        //---------------------------------
        // MEMBERS
        //---------------------------------

        AzureImageService _azureImageService = AzureImageService.DefaultManager;
        public ObservableCollection<ItemImage> AllImages = new ObservableCollection<ItemImage>();
        public ObservableCollection<Tuple<string, string>> ImagePairs = new ObservableCollection<Tuple<string, string>>();

        // Two variables for OnItemSelection function
        Boolean _isRunningItem = false;
        Object _lockObject = new Object();

        //---------------------------------
        // FUNCTIONS
        //---------------------------------
        public ProfilePage()
        {
            InitializeComponent();
            MyItemLabel.IsVisible = NoItemLabel.IsVisible = false;

            UpdateUserData();
            MessagingCenter.Subscribe<ItemPage>(this, "Update Like Counter", (sender) => {
                ItemUserLikeCounter.Text = Settings.NumOfItemsUserLike;
            });


        }

        async protected override void OnAppearing()
        {
            try
            {
                UpdateUserData();
                await GetUserItems();
                if (AllImages.Count > 0)
                    NoItemLabel.IsVisible = false;
            }
            catch (Exception)
            {
                await DisplayAlert("שגיאה", "שגיאה בטעינת נתונים", "אישור");
            }
        }

        private void UpdateUserData()
        {
            UserName.Text = Settings.UserFirstName;

            //if user doesnt have profile picture
            if (String.IsNullOrEmpty(Settings.ImgUrl))
                UserImg.Source = "unknownuser.png";
            else
                UserImg.Source = Settings.ImgUrl;
            
            myItemCounter.Text = Settings.NumOfItems;
            ItemUserLikeCounter.Text = Settings.NumOfItemsUserLike;
            
        }
        private async Task GetUserItems()
        {

            AllImages = await _azureImageService.GetAllImgByUserId(Settings.UserId);

            if (AllImages.Count > 0)
            {
                MyItemLabel.IsVisible = true;
                ItemList.IsVisible = true;
                ShowImages();
            }
            else
            {
                ItemList.IsVisible = false;
                MyItemLabel.IsVisible = false;
                NoItemLabel.IsVisible = true;
            }
        }

        private void ShowImages()
        {
            ImagePairs.Clear();
            StackCategory.Children.Clear();
            for (int i = 0; i < AllImages.Count; i++)
            {
                var image = new CachedImage
                {
                    Source = AllImages[i].Url,
                    WidthRequest = Utilities.Constants.ScreenWidth / 2,
                    HeightRequest = Utilities.Constants.ScreenWidth / 2

                };

                image.Transformations.Add(new RoundedTransformation(15));
                var tap = new TapGestureRecognizer();
                tap.CommandParameter = AllImages[i].ItemId;

                //image tap function loading the item page 
                tap.Tapped += async (s, e) =>
                {
                    try
                    {
                        var item = (CachedImage)s;
                        var gets = item.GestureRecognizers;
                        // To prevent double tap on images
                        lock (_lockObject)
                        {
                            if (_isRunningItem)
                                return;
                            else
                                _isRunningItem = true;
                        }

                        string itemId = (string)((TapGestureRecognizer)gets[0]).CommandParameter;

                        if (itemId != "")
                            await Navigation.PushAsync(new ItemPage(itemId));

                        _isRunningItem = false;
                    }
                    catch (Exception)
                    {
                        await DisplayAlert("שגיאה", "לא ניתן לטעון עמוד מבוקש.", "אישור");
                    }
                };

                image.GestureRecognizers.Add(tap);
                StackCategory.Children.Add(image);

            }
        }

        private async void LikeBtnClick(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SavedItemsPage());
            MessagingCenter.Send<ProfilePage>(this, "FirstApearing");
        }

        private async void EditBtnClicked(object sender, EventArgs e)
        {
			if (Settings.FacebookId != "")
            {
				DependencyService.Get<IMessage>().LongAlert("משתמש פייסבוק לא יכול לערוך פרטים");
            }
			else
                await Navigation.PushAsync(new EditUserInfoPage());
        }

        private async void AddNewItemClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddItemPage());
        }

    }
}
