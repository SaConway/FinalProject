using MsorLi.Models;
using MsorLi.Services;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MsorLi.Utilities;

namespace MsorLi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        //---------------------------------
        // MEMBERS
        //---------------------------------

        AzureUserService _azureUserService = AzureUserService.DefaultManager;

        int _passwordLen = 0;
        int _emailLen = 0;

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

        // For ANDROID only, return to item list
        protected override bool OnBackButtonPressed()
        {
            try
            {
                Navigation.PopToRootAsync();
                return true;
            }

            catch (Exception)
            {
                return false;
            }
        }

        private async void SubmitBtnClicked(object sender, EventArgs e)
        {
            try
            {
                var email = Email.Text;
                var password = Password.Text;

                User ThisUser = await _azureUserService.LoginAsync(email, password);

                if (ThisUser != null)
                {
                    // Success

                    Settings.UserId = ThisUser.Id;
                    Settings.UserFirstName = ThisUser.FirstName;
                    Settings.UserLastName = ThisUser.LastName;
                    Settings.ImgUrl = ThisUser.ImgUrl;
                    Settings.Email = ThisUser.Email;
                    Settings.Phone = ThisUser.Phone;
                    Settings.Address = ThisUser.Address;
                    Settings.Permission = ThisUser.Permission;

                    await Navigation.PopToRootAsync();
                }

                else
                {
                    await DisplayAlert("", "אחד או יותר מפרטי הזיהוי שגויים. נסה שנית.", "אישור");
                }

            }
            catch (Exception) { }

        }

        private async void RegBtnClicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new RegisterPage());
            }
            catch (Exception) { }
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