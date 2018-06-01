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
            myItemCounter.Text = " ";
            ItemUserLikeCounter.Text = " ";
            MyItemLabel.IsVisible = NoItemLabel.IsVisible = false;


            UserName.Text = Settings.UserFirstName;

            //if user doesnt have profile picture
            if (String.IsNullOrEmpty(Settings.ImgUrl))
                UserImg.Source = "unknownuser.png";
            else
                UserImg.Source = Settings.ImgUrl;

            MessagingCenter.Subscribe<ItemPage>(this, "Update Like Counter", async (sender) => {
                int num = await AzureSavedItemService.DefaultManager.NumOfItemsSavedByUser(Settings.UserId);

                ItemUserLikeCounter.Text = num.ToString();
            });

            MessagingCenter.Subscribe<SavedItemsPage>(this, "Update Like Counter", async (sender) => {
                int num = await AzureSavedItemService.DefaultManager.NumOfItemsSavedByUser(Settings.UserId);

                ItemUserLikeCounter.Text = num.ToString();
            });

        }

        async protected override void OnAppearing()
        {
            try
            {

                var t1 =  UpdateUserData();
                var t2 =  GetUserItems();

                await Task.WhenAll(t1,t2);

                if (AllImages.Count > 0)
                {
                    await ItemList.ScrollToAsync(StackUserItems.Children[StackUserItems.Children.Count - 1], ScrollToPosition.MakeVisible, true);
                    NoItemLabel.IsVisible = false;
                }
            }
            catch (Exception)
            {
                await DisplayAlert("שגיאה", "שגיאה בטעינת נתונים", "אישור");
            }
        }


        private async Task UpdateUserData()
        {

            try
            {
                //item counter
                int item_counter = await AzureItemService.DefaultManager.NumOfItemsByUserId(Settings.UserId);
                myItemCounter.Text = item_counter.ToString();
                myItemCounter.Opacity = 0;


                //like counter
                int like_counter = await AzureSavedItemService.DefaultManager.NumOfItemsSavedByUser(Settings.UserId);
                ItemUserLikeCounter.Text = like_counter.ToString();
                ItemUserLikeCounter.Opacity = 0;
                var t1 = myItemCounter.FadeTo(1);
                var t2 = ItemUserLikeCounter.FadeTo(1);
                await Task.WhenAll(t1, t2);
            }
            catch(Exception){}
            
        }
        private async Task GetUserItems()
        {
            try
            {
                AllImages = await _azureImageService.GetAllImgByUserId(Settings.UserId);

                if (AllImages.Count > 0)
                {
                    MyItemLabel.IsVisible = true;
                    ItemList.Opacity = 0;
                    ItemList.IsVisible = true;
                    ShowImages();
                    await ItemList.FadeTo(1);
                }
                else
                {
                    ItemList.IsVisible = false;
                    MyItemLabel.IsVisible = false;
                    NoItemLabel.IsVisible = true;
                }
            }
            catch(Exception){}
        }

        private void ShowImages()
        {
            ImagePairs.Clear();
            StackUserItems.Children.Clear();
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
                StackUserItems.Children.Add(image);

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
