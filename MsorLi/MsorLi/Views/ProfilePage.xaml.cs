using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
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
                await GetUserItems();

            }
            catch (Exception)
            {

            }

        }

        private async Task GetUserItems(){
            


            AllImages = await _azureImageService.GetAllImgByUserId(Settings.UserId);

            if (AllImages != null){
                    
                ImagePairs.Clear();

                for (int i = 0; i < AllImages.Count; i ++)
                {
                    string Url = AllImages[i].Url;
                    string ItemId = AllImages[i].ItemId;
                    ImagePairs.Add(new Tuple<string, string>(Url, ItemId));
                }

                listView_items.ItemsSource = ImagePairs;
            }


        }
        async void OnItemSelection(object sender, TappedEventArgs e)
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



    }
}
