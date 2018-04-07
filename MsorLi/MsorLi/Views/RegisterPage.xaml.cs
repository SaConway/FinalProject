using MsorLi.Models;
using MsorLi.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Text.RegularExpressions;

namespace MsorLi.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RegisterPage : ContentPage
	{
        //---------------------------------------------------
        // MEMBERS
        //---------------------------------------------------


        //---------------------------------------------------
        // FUNCTIONS
        //---------------------------------------------------

        public RegisterPage ()
		{
			InitializeComponent();
		}

        public async void SubmitBtnClicked(object sender, EventArgs e)
        {
            try
            {
                bool IsValid = await Validation();

                if (IsValid == false)
                {
                    await DisplayAlert("", "אימייל לא תקין. נסה שנית.", "אישור");
                    return;
                }

                User new_user = new User
                {
                    FirstName = firstName.Text,
                    LastName = lastName.Text,
                    Email = email.Text,
                    Password = Utilities.EncryptDecrypt.Encrypt(password.Text),
                    Phone = phoneNumber.Text,
                    Address = address.Text.Length > 0 ? city.Text + ", " + address.Text : city.Text,
                    Permission = "1"
                };

                await AzureUserService.DefaultManager.UploadToServer(new_user, new_user.Id);
                await Navigation.PopToRootAsync();
            }
            catch (Exception)
            {
                
            }
        }

        private void Event_TextChanged(object sender, TextChangedEventArgs e)
        {
            Entry entry = sender as Entry;
            String val = entry.Text;
            int len = val.Length;

            bool IsVisable = false;

            if (len > 0)
            {
                IsVisable = true;
                entry.Margin = new Thickness(0, 15, 0, 0);
            }
            else
            {
                IsVisable = false;
                entry.Margin = new Thickness(0, 50, 0, 0);
            }

            var placeholder = entry.Placeholder.ToString();

            switch (placeholder)
            {
                case "שם פרטי":
                    {
                        firstNameLabel.IsVisible = IsVisable;
                        entry.Margin = new Thickness(0, 0, 0, 0);
                        break;
                    }
                case "שם משפחה":
                    {
                        lastNameLabel.IsVisible = IsVisable;
                        entry.Margin = new Thickness(0, 0, 0, 0);
                        break;
                    }
                case "אימייל":
                    {
                        emailLabel.IsVisible = IsVisable;
                        break;
                    }
                case "סיסמה":
                    {
                        passwordLabel.IsVisible = IsVisable;
                        break;
                    }
                case "מס' טלפון":
                    {
                        phoneLabel.IsVisible = IsVisable;
                        break;
                    }
                case "עיר מגורים":
                    {
                        cityLabel.IsVisible = IsVisable;
                        entry.Margin = new Thickness(0, 0, 0, 0);
                        break;
                    }
                case "כתובת (אופציונלי)":
                    {
                        adressLabel.IsVisible = IsVisable;
                        entry.Margin = new Thickness(0, 0, 0, 0);
                        break;
                    }
            }

            if (firstNameLabel.IsVisible && lastNameLabel.IsVisible && emailLabel.IsVisible &&
                passwordLabel.IsVisible && phoneLabel.IsVisible && cityLabel.IsVisible)
            {
                SubmitBtn.IsEnabled = true;
                SubmitBtn.BackgroundColor = Color.FromHex("00BCD4");
            }
            else
            {
                SubmitBtn.IsEnabled = false;
                SubmitBtn.BackgroundColor = Color.FromHex("999999");
            }
        }

        private async Task<bool> Validation()
        {
            try
            {
                Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                Match match = regex.Match(email.Text);

                if (!match.Success)
                {
                    throw new Exception();
                }

                var b = await AzureUserService.DefaultManager.IsEmailExistAsync(email.Text);
                if (b)
                {
                    //Email exist
                    return false;
                }

                return true;
            }

            catch (Exception)
            {
                return false;
            }
        }
    }
}