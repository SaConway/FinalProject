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

        ObservableCollection<ImageSource> _profileImage =
            new ObservableCollection<ImageSource>();

        bool _firstAppearing = true;
        Boolean _runFrame = false;


        //---------------------------------------------------
        // FUNCTIONS
        //---------------------------------------------------

        public RegisterPage ()
		{
            InitializeComponent();
		}

        protected async override void OnAppearing()
        {
            try
            {
                if (_firstAppearing)
                {
                    _firstAppearing = false;

                    // Set locations to the location picker

                    var locations = await LocationStorage.GetLocations();

                    foreach (var l in locations)
                    {
                        if (!LocationPicker.Items.Contains(l.Name))
                            LocationPicker.Items.Add(l.Name);
                    }
                }
            }
            catch (Exception)
            {

            }
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
                MyFrame.IsVisible = true;
                _runFrame = true;
                UpdaterFrameAsync();


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
                new_user.Erea = LocationPicker.SelectedItem.ToString();
                new_user.Address = (address.Text != null && address.Text.Length > 0) ? address.Text : "";
                new_user.Permission = "User";
                new_user.NumOfItems = 0;
                new_user.NumOfItemsUserLike = 0;
                new_user.ImgUrl = profileURL;

                await AzureUserService.DefaultManager.UploadToServer(new_user, new_user.Id);
                DependencyService.Get<IMessage>().LongAlert("ההרשמה בוצעה בהצלחה");
                MyFrame.IsVisible = false;
                _runFrame = true;
                await Navigation.PopToRootAsync();
            }
            catch (Exception)
            {
            }
        }

        private void Event_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                CheckToEnableSubmit();
            }
            catch (Exception)
            {

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

        private void OnLocationChanged(object sender, EventArgs e)
        {
            try
            {
                CheckToEnableSubmit();
            }
            catch (Exception)
            {

            }
        }

        private void CheckToEnableSubmit()
        {
            if (firstName.Text != null && firstName.Text.Length > 0 &&
                lastName.Text != null && lastName.Text.Length > 0 &&
                email.Text != null && email.Text.Length > 0 &&
                password.Text != null && password.Text.Length > 0 &&
                phoneNumber.Text != null && phoneNumber.Text.Length > 0 &&
                LocationPicker.SelectedIndex != -1)
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

        private void InitializeCarouselView()
        {
            // Update CarouselView attributes
            imagesView.Margin = new Thickness(5, 60, 5, 0);
            imagesView.HeightRequest = 300;
        }
        private async Task UpdaterFrameAsync()
        {
            while (_runFrame)
            {
                switch (FrameLabel.Text)
                {
                    case "אנא המתן.  ":
                        FrameLabel.Text = "אנא המתן.. ";
                        break;
                    case "אנא המתן.. ":
                        FrameLabel.Text = "אנא המתן...";
                        break;
                    case "אנא המתן...":
                        FrameLabel.Text = "אנא המתן.  ";
                        break;
                }

                await Task.Delay(500);
            }
        }
    }
}