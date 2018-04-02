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

                    Settings._GeneralSettings = ThisUser[0].Id;
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