using MsorLi.Models;
using MsorLi.Services;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.IO;

namespace MsorLi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddItemPage : ContentPage
    {
        //---------------------------------
        // MEMBERS
        //---------------------------------

        AzureService _azureService;
        Stream _imageStream;

        //---------------------------------
        // FUNCTIONS
        //---------------------------------

        public AddItemPage()
        {
            InitializeComponent();
            _azureService = AzureService.DefaultManager;
        }

        public async void PickPhotoButtonClicked(object sender, System.EventArgs e)
        {
            pickPictureButton.IsEnabled = false;
            _imageStream = await DependencyService.Get<IPicturePicker>().GetImageStreamAsync();

            if (_imageStream != null)
            {
                // This line will cause an Exeption - probably because it's a stream that can be read once

                image.Source = ImageSource.FromStream(() => _imageStream);

                image.IsVisible = true;
            }
            else
            {
                pickPictureButton.IsEnabled = true;
            }
        }

        // Add Item button operation
        public async void OnAdd(object sender, EventArgs e)
        {
            //Insert Image to Blob server
            var uploadedFilename = await BlobService.UploadFileAsync(_imageStream);
            var imageUrl = "https://msorli.blob.core.windows.net/images/" + uploadedFilename;

            //Create new Item
            var item = new Item
            {
                Title = name.Text,
                ImageUrl = image.ToString(),
                Description = description.Text,
                Condition = condition.SelectedItem.ToString(),
                Location = city.Text + ", " + street.Text,
                ViewCounter = 0,
                Date = DateTime.Today.ToString("d"),
                Time = DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString(),
                ContactName = contactName.Text,
                ContactNumber = contactNumber.Text
            };

            await _azureService.UploadItemToServer(item);
        }

        // If user inserted new info to one of the entries, make the label visable
        private void NameTextChangedEvent(object sender, EventArgs e)
        {
            Entry entry = sender as Entry;

            bool IsVisable = false;

            if (entry.Text.Length > 0)
            {
                IsVisable = true;
                entry.Margin = new Thickness(25, 15, 25, 0);
            }
            else
            {
                IsVisable = false;
                entry.Margin = new Thickness(25, 50, 25, 0);
            }

            if (entry.Placeholder.ToString() == "שם מוצר")
            {
                nameLabel.IsVisible = IsVisable;
            }
            else if (entry.Placeholder.ToString() == "תיאור מוצר")
            {
                descriptionLabel.IsVisible = IsVisable;
            }
            else if (entry.Placeholder.ToString() == "עיר מגורים")
            {
                cityLabel.IsVisible = IsVisable;
            }
            else if (entry.Placeholder.ToString() == "רחוב")
            {
                streetLabel.IsVisible = IsVisable;
            }
            else if (entry.Placeholder.ToString() == "שם איש קשר")
            {
                contactNameLabel.IsVisible = IsVisable;
            }
            else if (entry.Placeholder.ToString() == "טלפון ליצירת קשר")
            {
                contactNumberLabel.IsVisible = IsVisable;
            }
        }
    }
}