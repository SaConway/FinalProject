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
                UserImg.Source = Settings.ImgUrl;
                logButton.Text = "התנתק";
                logImg.Source = "logout.png";
            }
            else
            {
                UserName.Text = "שלום אורח";
                UserImg.Source = "unknown-user.png";
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
                if (Settings.UserId != ""){
                    Settings.UserId = "";
					await Navigation.PopToRootAsync();
                    DependencyService.Get<IMessage>().LongAlert("בוצעה התנתקות מהמערכת");
                    Settings.ClearUserData();
                }
                else
                    await Navigation.PushAsync(new LoginPage());
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
    }
}