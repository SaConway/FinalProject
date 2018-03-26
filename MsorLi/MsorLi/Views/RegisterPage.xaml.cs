using MsorLi.Models;
using MsorLi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MsorLi.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RegisterPage : ContentPage
	{
        //---------------------------------------------------
        // MEMBERS
        //---------------------------------------------------

        AzureUserService _azureUserService = new AzureUserService();

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
                // Validation client and server side:
                // Email exist or not
                // Sent Encrypt password

                User new_user = new User
                {
                    FirstName = firstName.Text,
                    LastName = lastName.Text,
                    Email = email.Text,
                    Password = password.Text,
                    Phone = phoneNumber.Text,
                    Address = address.Text,
                    Permission = "1"
                };

                await _azureUserService.UploadToServer(new_user, new_user.Id);
            }
            catch
            {
                // catch!!!
            }
        }

        public void LoginBtnClicked(object sender, EventArgs e)
        {
            // Go Back to Login page
        }
    }
}