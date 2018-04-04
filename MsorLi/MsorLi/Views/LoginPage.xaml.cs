using MsorLi.Models;
using MsorLi.Services;
using System;
using Xamarin.Forms;
using System.IO;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms.Xaml;
using MsorLi.Utilities;


namespace MsorLi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        AzureUserService _azureUserService = AzureUserService.DefaultManager;

        public LoginPage()
        {
            InitializeComponent();
        }


        // EVENT FUNCTIONS
        //----------------------------------------------------------

        // For android only, return to item list
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
                string email = Email.Text;
                string password = Password.Text;

                ObservableCollection<User> ThisUser = await _azureUserService.LoginAsync(email, password);

                if (ThisUser != null)
                {
                    // Success

                    Settings.UserId = ThisUser[0].Id;
                    Settings.UserFirstName = ThisUser[0].FirstName;
                    Settings.UserLastName = ThisUser[0].LastName;
                    Settings.ImgUrl = ThisUser[0].ImgUrl;
                    Settings.Email = ThisUser[0].Email;
                    Settings.Phone = ThisUser[0].Phone;
                    Settings.Address = ThisUser[0].Address;
                    Settings.Permission = ThisUser[0].Permission;

                    await Navigation.PopAsync();
                }

                else
                {
                    // User not registerd
                }
                
            }
            catch (Exception) {}

        }
        private async void RegBtnClicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new RegisterPage());
            }
            catch (Exception) {}
        }
    }
}