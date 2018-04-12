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
        public ObservableCollection<Tuple<string, string>> ImagePairs =
                    new ObservableCollection<Tuple<string, string>>();
        //---------------------------------
        // FUNCTIONS
        //---------------------------------
        public ProfilePage()
        {

            InitializeComponent();
            if (Settings.UserId != "")
            {
                UserName.Text = "שלום " + Settings.UserFirstName;
                UserImg.Source = Settings.ImgUrl;
            }

        }
        async protected override void OnAppearing()
        {

            try
            {
                //if the user is not logged
                if (Settings.UserId == "")
                {
                    await Navigation.PushAsync(new LoginPage());
                }
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



    }
}
