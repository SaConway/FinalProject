using MsorLi.Models;
using MsorLi.Services;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MsorLi.Utilities;
using System.Net.Http;
using System.Threading.Tasks;

namespace MsorLi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        //---------------------------------
        // MEMBERS
        //---------------------------------

        int _passwordLen = 0;
        int _emailLen = 0;

        bool _succcess = false;

        //---------------------------------
        // FUNCTIONS
        //---------------------------------

        // C-tor
        public LoginPage()
        {
            InitializeComponent();
        }

        // EVENT FUNCTIONS
        //----------------------------------------------------------

        // Override OnDisappearing
        protected override void OnDisappearing()
        {
            if (_succcess)
            {
                MessagingCenter.Send<LoginPage>(this, "Success");
            }
            else
            {
                MessagingCenter.Send<LoginPage>(this, "NotSuccess");
            }
        }

        private async void SubmitBtnClicked(object sender, EventArgs e)
        {
            try
            {
                var email = Email.Text;
                var password = Password.Text;

                // Get User with this email
                User user = await AzureUserService.DefaultManager.GetUserAsync(email, password);

                if (user != null)
                {
                    // Check if the password is correct
                    if (EncryptDecrypt.Decrypt(user.Password) != password)
                    {
                        throw new Exception("סיסמה שהוזנה אינה תקינה. נסה שנית.");
                    }

                    Settings.UpdateUserInfo(user);
     
                    _succcess = true;
                    await Navigation.PopAsync();
                }

                else
                {
                    throw new Exception("אימייל שהוזן אינו תקין. נסה שנית.");
                }

            }
            catch (Exception exc)
            {
                await DisplayAlert("", exc.Message, "אישור");
            }

        }

        private async void RegBtnClicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new RegisterPage());
            }
            catch (Exception) { }
        }

        private void OnFacebookClick(object sender, EventArgs e)
        {
            var apiRequest = "https://www.facebook.com/v2.12/dialog/oauth?%20client_id=317123308817276%20&response_type=token&redirect_uri=https://www.facebook.com/connect/login_success.html%20";

            var webView = new WebView
            {
                Source = apiRequest,
                HeightRequest = 1
            };

            webView.Navigated += WebViewOnNavigated;
            //change the page to web view
            Content = webView;
        }

        private async void WebViewOnNavigated(object sender, WebNavigatedEventArgs e)
        {

            var accessToken = FacebookServices.ExtractAccessTokenFromUrl(e.Url);

            if (accessToken != "")
            {
                FacebookProfile userJson = await FacebookServices.GetFacebookProfileAsync(accessToken);
                User user = await AzureUserService.DefaultManager.IsFacebookIdExistAsync(userJson.FacebookId);
                User facebookUser = null;
                //if user logged for the first time with facebook create new user
                if(user == null)
                {
                    facebookUser = new User
                    {
                        FirstName = userJson.FirstName,
                        LastName = userJson.LastName,
                        Email = userJson.Email,
                        Address = userJson.Address,
                        Permission = "User",
                        NumOfItems = 0,
                        NumOfItemsUserLike = 0,
                        ImgUrl = userJson.Picture.Data.Url,
                        FacebookId = userJson.FacebookId
                    };
                    await AzureUserService.DefaultManager.UploadToServer(facebookUser,facebookUser.Id);

                }
                //facebook user is exist update info from facebook database and insert on msorli db
                else
                {
                    facebookUser = new User
                    {
                        Id = user.Id,
                        FirstName = userJson.FirstName,
                        LastName = userJson.LastName,
                        Email = userJson.Email,
                        Address = userJson.Address,
                        Permission = user.Permission,
                        NumOfItems = user.NumOfItems,
                        NumOfItemsUserLike = user.NumOfItemsUserLike,
                        ImgUrl = userJson.Picture.Data.Url,
                        FacebookId = userJson.FacebookId
                    };
                    await AzureUserService.DefaultManager.UploadToServer(facebookUser, facebookUser.Id);
                }

                Settings.UpdateUserInfo(facebookUser);
                await Navigation.PopToRootAsync();
            }
        }

        private void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Entry entry = sender as Entry;
                String val = entry.Text;
                int len = val.Length;

                if (entry.Placeholder == "אימייל")
                {
                    _emailLen = len;
                }
                else
                {
                    _passwordLen = len;
                }

                if (_emailLen > 0 && _passwordLen > 0)
                {
                    // User can submit
                    MySubmitBtn.IsEnabled = true;
                    MySubmitBtn.BackgroundColor = Color.FromHex("00BCD4");
                }
                else
                {
                    //User can't submit
                    MySubmitBtn.IsEnabled = false;
                    MySubmitBtn.BackgroundColor = Color.FromHex("#999999");
                }
            }

            catch (Exception) { }
        }
    }
}