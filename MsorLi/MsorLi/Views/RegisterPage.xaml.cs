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
                if (IsValid == false)
                {
                    await DisplayAlert("", "אימייל לא תקין. נסה שנית.", "אישור");
                    return;
                }

                string profileURL = "";

                // Save image in blob
                if (_profileImage.Count != 0)
                {
                    var v = await BlobService.SaveImagesInDB(_byteData);
                    if (v != null && v.Count > 0)
                        profileURL = v[0];
                }

                User new_user = new User();
                new_user.FirstName = firstName.Text;
                new_user.LastName = lastName.Text;
                new_user.Email = email.Text;
                new_user.Password = EncryptDecrypt.Encrypt(password.Text);
                new_user.Phone = phoneNumber.Text;
                new_user.Address = (address.Text != null && address.Text.Length > 0) ? city.Text + ", " + address.Text : city.Text;
                new_user.Permission = "User";
                new_user.NumOfItems = 0;
                new_user.NumOfItemsUserLike = 0;
                new_user.ImgUrl = profileURL;

                await AzureUserService.DefaultManager.UploadToServer(new_user, new_user.Id);
                await Navigation.PopToRootAsync();
            }
            catch (Exception)
            {
            }
        }

        private void Event_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (firstName.Text != null && lastName.Text != null && email.Text != null &&
                password.Text != null && phoneNumber.Text != null && city.Text != null)
            {
                SubmitBtn.IsEnabled = true;
                SubmitBtn.BackgroundColor = Color.FromHex("19a4b4");
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