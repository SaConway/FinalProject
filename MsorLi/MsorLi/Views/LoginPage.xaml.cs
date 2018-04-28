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