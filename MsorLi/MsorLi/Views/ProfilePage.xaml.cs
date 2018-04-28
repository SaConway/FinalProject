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

            UserName.Text = Settings.UserFirstName;
            UserImg.Source = Settings.ImgUrl;
            myItemCounter.Text = Settings.NumOfItems;
            ItemUserLikeCounter.Text = Settings.NumOfItemsUserLike;


            MessagingCenter.Subscribe<ItemPage>(this, "Update Like Counter", (sender) => {
                //MessagingCenter.Unsubscribe<LoginPage>(this, "Success");
                ItemUserLikeCounter.Text = Settings.NumOfItemsUserLike;
            });



        }

        async protected override void OnAppearing()
        {
            try
            {
                UserName.Text = Settings.UserFirstName;
                UserImg.Source = Settings.ImgUrl;
                await GetUserItems();
            }
            catch (Exception)
            {
                await DisplayAlert("שגיאה", "שגיאה בטעינת נתונים", "אישור");
            }
        }

        private async Task GetUserItems(){
            
            AllImages = await _azureImageService.GetAllImgByUserId(Settings.UserId);

            if (AllImages != null){
                    
                ImagePairs.Clear();

                for (int i = 0; i < AllImages.Count; i ++)
                {
                    var image = new CachedImage
                    {
                        Source = AllImages[i].Url
                                       
                    };

                    image.Transformations.Add(new RoundedTransformation(15));
                    var tap = new TapGestureRecognizer();
                    tap.CommandParameter = AllImages[i].ItemId;

                    //image tap function loading the item page 
                    tap.Tapped += async (s, e) => {
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
        }

        private async void LikeBtnClick()
        {
            await Navigation.PushAsync(new SavedItemsPage());
            MessagingCenter.Send<ProfilePage>(this, "FirstApearing");
        }
        private async void EditBtnClicked()
        {
            await Navigation.PushAsync(new EditUserInfoPage());
        }

    }
}
