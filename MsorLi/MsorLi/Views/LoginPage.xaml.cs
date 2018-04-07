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

                // Get User with this email
                User user = await AzureUserService.DefaultManager.GetUserAsync(email, password);

                if (user != null)
                {
                    // Check if the password is correct
                    if (EncryptDecrypt.Decrypt(user.Password) != password)
                    {
                        throw new Exception("סיסמה שהוזנה אינה תקינה. נסה שנית.");
                    }

                    Settings.UserId = user.Id;
                    Settings.UserFirstName = user.FirstName;
                    Settings.UserLastName = user.LastName;
                    Settings.ImgUrl = user.ImgUrl;
                    Settings.Email = user.Email;
                    Settings.Phone = user.Phone;
                    Settings.Address = user.Address;
                    Settings.Permission = user.Permission;

                    await Navigation.PopToRootAsync();
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