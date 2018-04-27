using MsorLi.Models;
using MsorLi.Services;
using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Text.RegularExpressions;
using System.IO;
using MsorLi.Utilities;

namespace MsorLi.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RegisterPage : ContentPage
	{
        //---------------------------------------------------
        // MEMBERS
        //---------------------------------------------------
        List<byte[]> _byteData = new List<byte[]>();
        ObservableCollection<ImageSource> _profileImage = new ObservableCollection<ImageSource>();

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
                List<string> imageUrls = null;

                // Save image in blob
                if (_profileImage.Count != 0)
                    imageUrls = await BlobService.SaveImagesInDB(_byteData);

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
                    Password = EncryptDecrypt.Encrypt(password.Text),
                    Phone = phoneNumber.Text,
                    Address = address.Text.Length > 0 ? city.Text + ", " + address.Text : city.Text,
                    Permission = "User",
                    NumOfItems = 0,
                    NumOfItemsUserLike = 0,
                    ImgUrl = imageUrls == null ? "" : imageUrls[0]
                };

                await AzureUserService.DefaultManager.UploadToServer(new_user, new_user.Id);
                await Navigation.PopToRootAsync();
            }
            catch (Exception ex)
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

        public async void PickImageButton_Event(object sender, EventArgs e)
        {
            try
            {
                if (_profileImage.Count == Constants.MAX_NUM_OF_PROFILE_IMAGES) return;

                pickPictureButton.IsEnabled = false;
                Stream imageStream = await DependencyService.Get<IPicturePicker>().GetImageStreamAsync();

                if (imageStream != null)
                {
                    _byteData.Add(ImageUpload.ReadFully(imageStream));
                    ImageSource imageSource = ImageSource.FromStream(() => new MemoryStream(_byteData[_byteData.Count - 1]));

                    if (_profileImage.Count == 0)
                    {
                        InitializeCarouselView();
                    }

                    _profileImage.Add(imageSource);
                    imagesView.ItemsSource = _profileImage;
                }

                pickPictureButton.IsEnabled = _profileImage.Count == Constants.MAX_NUM_OF_PROFILE_IMAGES ? false : true;
            }
            catch (Exception) { }
        }

        private void InitializeCarouselView()
        {
            // Update CarouselView attributes
            imagesView.Margin = new Thickness(5, 60, 5, 0);
            imagesView.HeightRequest = 300;
        }
    }
}