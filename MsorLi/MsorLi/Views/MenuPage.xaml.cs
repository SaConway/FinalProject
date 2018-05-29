using System;
using Xamarin.Forms;
using MsorLi.Utilities;

namespace MsorLi.Views
{
    public partial class MenuPage : ContentPage
    {
        public MenuPage()
        {
            InitializeComponent();
            if (Session.IsLogged())
            {
                UserName.Text = "שלום " + Settings.UserFirstName;
                UserProfilePicture();
                logButton.Text = "התנתק";
                logImg.Source = "logout.png";
            }
            else
            {
                UserName.Text = "שלום אורח";
                UserImg.Source = "unknownuser.png";
                logButton.Text = "התחבר";
                logImg.Source = "login.png";
            }
        }

        private async void SavedItemsClickEvent(object sender, EventArgs e)
        {
            try
            {
                if(Session.IsLogged())
                {
                    var x = Settings.UserId;
                    await Navigation.PushAsync(new SavedItemsPage());
                    MessagingCenter.Send<MenuPage>(this, "FirstApearing");
                }
                else
                {
                    await Navigation.PushAsync(new LoginPage());

                    //when login is finish with success load save item page
                    MessagingCenter.Subscribe<LoginPage>(this, "Success", async (send) => {

                        MessagingCenter.Unsubscribe<LoginPage>(this, "Success");
                        await Navigation.PushAsync(new SavedItemsPage());
                        MessagingCenter.Send<MenuPage>(this, "FirstApearing");
                    });
                }
            }
            catch (Exception)
            {
                await DisplayAlert("שגיאה", "לא ניתן לטעון מוצרים. נסה שנית מאוחר יותר.", "אישור");
            }
        }

        private async void LogbuttonClickEvent(object sender, EventArgs e)
        {
            try
            {
                //if user is looged and pressed logout
                if (Session.IsLogged())
                {
                    await Navigation.PopToRootAsync();
					DependencyService.Get<IMessage>().LongAlert("בוצעה התנתקות מהמערכת");
                    Settings.ClearUserData();
                }
                else
                {
                    await Navigation.PushAsync(new LoginPage());
                    //user loged in success event
                    MessagingCenter.Subscribe<LoginPage>(this, "Success", (send) => {

                        MessagingCenter.Unsubscribe<LoginPage>(this, "Success");
                        UserProfilePicture();
                        UserName.Text = "שלום " + Settings.UserFirstName;
                    });
                }
            }
            catch (Exception)
            {
                await DisplayAlert("שגיאה", "לא ניתן להתנתק. נסה שנית מאוחר יותר.", "אישור");
            }
        }

        private async void ProfileClickEvent(object sender, EventArgs e)
        {
            try
            {
               //await Navigation.PushAsync(new ProfilePage());
                if (Session.IsLogged())
                {
                    var x = Settings.UserId;
                    await Navigation.PushAsync(new ProfilePage());
                }
                else
                {
                    await Navigation.PushAsync(new LoginPage());

                    //when login is finish with success load save item page
                    MessagingCenter.Subscribe<LoginPage>(this, "Success", async (send) => {

                        MessagingCenter.Unsubscribe<LoginPage>(this, "Success");
                        await Navigation.PushAsync(new ProfilePage());
                    });

                }
            }
            catch (Exception)
            {
                //await DisplayAlert("שגיאה", "לא ניתן להתנתק. נסה שנית מאוחר יותר.", "אישור");
            }
        }

        private async void HomeClickEvent(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PopToRootAsync();
            }
            catch (Exception)
            {

            }
        }

        private async void AboutClickEvent(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new AboutPage());
            }
            catch (Exception)
            {

            }
        }

        private async void AddItemClickEvent(object sender, EventArgs e)
        {
            try
            {
                if (Session.IsLogged())
                {
                    await Navigation.PushAsync(new AddItemPage());
                }
                else
                {
                    await Navigation.PushAsync(new LoginPage());

                    // If login is finish with success, load add item page
                    MessagingCenter.Subscribe<LoginPage>(this, "Success", async (send) =>
                    {
                        MessagingCenter.Unsubscribe<LoginPage>(this, "Success");
                        await Navigation.PushAsync(new AddItemPage());
                    });
                }
            }
            catch (Exception)
            {

            }
        }
        private void UserProfilePicture()
        {

            //if user doesnt have profile picture
            if (String.IsNullOrEmpty(Settings.ImgUrl))
                UserImg.Source = "unknownuser.png";
            else
                UserImg.Source = Settings.ImgUrl;
        }
    }
}