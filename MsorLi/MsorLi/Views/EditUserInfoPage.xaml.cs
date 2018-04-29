using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MsorLi.Models;
using MsorLi.Services;
using MsorLi.Utilities;
using Xamarin.Forms;

namespace MsorLi.Views
{
    public partial class EditUserInfoPage : ContentPage
    {


        //---------------------------------------------------
        // MEMBERS
        //---------------------------------------------------
        List<byte[]> _byteData = new List<byte[]>();
        ObservableCollection<ImageSource> _profileImage = new ObservableCollection<ImageSource>();
        bool _password_change = false;
        //---------------------------------------------------
        // FUNCTIONS
        //---------------------------------------------------

        public EditUserInfoPage()
        {
            InitializeComponent();
            firstName.Text = Settings.UserFirstName;
            lastName.Text = Settings.UserLastName;
            email.Text = Settings.Email;
            phoneNumber.Text = Settings.Phone;
            city.Text = Settings.Address;
            NewPhotoLabel.IsVisible = false;
        }

        public async void SubmitBtnClicked(object sender, EventArgs e)
        {
            try
            {
                bool IsValid = await Validation();
                List<string> imageUrl = null;
        

                if (IsValid == false)
                {
                    await DisplayAlert("", "אימייל לא תקין. נסה שנית.", "אישור");
                    return;
                }

                if (!CheckPassword())
                {
                    await DisplayAlert("", "סיסמא לא תקינה. נסה שנית.", "אישור");
                    _password_change = false;
                    return;
                }

                // Save images in blob
                if (_profileImage.Count > 0)
                {
                    //add new photo to blob
                    imageUrl = await BlobService.SaveImagesInDB(_byteData);
                    //delete old photo

                    await BlobService.DeleteImage(Settings.ImgUrl);
                }

                User new_user = new User
                {
                    Id = Settings.UserId,
                    FirstName = firstName.Text,
                    LastName = lastName.Text,
                    Email = email.Text,
                    Phone = phoneNumber.Text,
                    Address = city.Text,
                    Permission = "1",
                    NumOfItems = int.Parse( Settings.NumOfItems),
                    NumOfItemsUserLike = int.Parse(Settings.NumOfItemsUserLike),
                    ImgUrl = _profileImage.Count > 0 ? imageUrl[0] : Settings.ImgUrl,
                };

                if (!_password_change)
                    new_user.Password = Settings.Password;
                else
                    new_user.Password = Utilities.EncryptDecrypt.Encrypt(new_password_first.Text);
                            

                new_user = await AzureUserService.DefaultManager.UpdateUser(new_user);
                Settings.UpdateUserInfo(new_user);
                await Navigation.PopAsync();
            }
            catch (Exception)
            {
                await DisplayAlert("שגיאה", "לא ניתן להשלים את הפעולה. נסה שנית.", "אישור");
            }
        }

        private bool CheckPassword()
        {
            if(old_password.Text != null && new_password_first.Text != null && new_password_second.Text != null)
            {
                _password_change = true;

                if (old_password.Text == Utilities.EncryptDecrypt.Decrypt(Settings.Password) &&
                   new_password_first.Text == new_password_second.Text)
                {
                    return true;
                }
                else return false;

            }
            _password_change = false;
            return true;
        }

        private async System.Threading.Tasks.Task<bool> Validation()
        {
            try
            {
                System.Text.RegularExpressions.Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                Match match = regex.Match(email.Text);

                if (!match.Success)
                {
                    throw new Exception();
                }
                if (email.Text != Settings.Email)
                {
                    var emailCheck = await AzureUserService.DefaultManager.IsEmailExistAsync(email.Text);
                    if (emailCheck)
                    {
                        //Email exist
                        return false;
                    }
                }
                return true;
            }

            catch (Exception)
            {
                return false;
            }
        }
         

        public async Task UpdateProfilePicAsync(object sender, System.EventArgs e)
        {
            if(_profileImage.Count > 0 )
            {
                ProfilePictureBtn.Text = "שינוי תמונת פרופיל";
                NewPhotoLabel.IsVisible = false;
                _profileImage.Clear();
                imagesView.IsVisible = false;
            }
            else
            {
                if (await PickImageButton_Event())
                {
                    imagesView.IsVisible = true;
                    ProfilePictureBtn.Text = "בטל העלאת תמונה";
                    NewPhotoLabel.IsVisible = true;
                }
            }
        }
    

        public async Task<bool> PickImageButton_Event()
        {
            try
            {
                if (_profileImage.Count == Constants.MAX_NUM_OF_PROFILE_IMAGES) return false;

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
                    return true;
                }
                return false;
            }
            catch (Exception) { return false; }
        }


        private void InitializeCarouselView()
        {
            // Update CarouselView attributes
            imagesView.Margin = new Thickness(5, 60, 5, 0);
            imagesView.HeightRequest = 300;
        }
    }
}
