using MsorLi.Models;
using MsorLi.Services;
using System;
using Xamarin.Forms;
using System.IO;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace MsorLi.Views
{
    public partial class AddItemPage : ContentPage
    {
        //---------------------------------------------------
        // MEMBERS
        //---------------------------------------------------

        AzureService _azureService = AzureService.DefaultManager;
        List<byte[]> _byteData = new List<byte[]>();
        ObservableCollection<ImageSource> _images = new ObservableCollection<ImageSource>();
        const int MAX_NUM_OF_IMAGES = 4;

        //---------------------------------------------------
        // FUNCTIONS
        //---------------------------------------------------

        // C-TOR
        public AddItemPage()
        {
            InitializeComponent();
        }

        //---------------------------------------------------
        // EVENT FUNCTIONS
        //---------------------------------------------------

        public async void PickImageButton_Event(object sender, System.EventArgs e)
        {
            try
            {
                if (_images.Count == MAX_NUM_OF_IMAGES) return;

                pickPictureButton.IsEnabled = false;
                Stream imageStream = await DependencyService.Get<IPicturePicker>().GetImageStreamAsync();

                if (imageStream != null)
                {
                    _byteData.Add(ReadFully(imageStream));
                    ImageSource imageSource = ImageSource.FromStream(() => new MemoryStream(_byteData[_byteData.Count - 1]));

                    if (_images.Count == 0)
                    {
                        InitializeCarouselView();
                    }

                    _images.Add(imageSource);
                    imagesView.ItemsSource = _images;
                }

                pickPictureButton.IsEnabled = _images.Count == 4 ? false : true;
            }
            catch (Exception)
            {

            }
        }

        //Add Item button operation
        public async void OnAdd_Event(object sender, EventArgs e)
        {
            try
            {
                // Save images in data base
                List<string> imageUrls = await SaveImagesInDB();

                // Create new item
                Item item = CreateNewItem(imageUrls);

                // Upload item to data base
                await _azureService.UploadItemToServer(item);
            }
            catch
            {

            }
        }

        //---------------------------------------------------
        // PRIVATE FUNCTIONS
        //---------------------------------------------------

        private Item CreateNewItem(List<string> imageUrls)
        {
            var item = new Item
            {
                Title = name.Text,
                NumOfImages = imageUrls.Count,
                ImageUrl_1 = imageUrls.Count >= 1 ? imageUrls[0] : "",
                ImageUrl_2 = imageUrls.Count >= 2 ? imageUrls[1] : "",
                ImageUrl_3 = imageUrls.Count >= 3 ? imageUrls[2] : "",
                ImageUrl_4 = imageUrls.Count >= 4 ? imageUrls[3] : "",
                Description = description.Text,
                Condition = condition.SelectedItem.ToString(),
                Location = city.Text + ", " + street.Text,
                ViewCounter = 0,
                Date = DateTime.Today.ToString("d"),
                Time = DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString(),
                ContactName = contactName.Text,
                ContactNumber = contactNumber.Text
            };

            return item;
        }

        private async Task<List<string>> SaveImagesInDB()
        {
            List<string> imageUrls = new List<string>();

            foreach (var byteData in _byteData)
            {
                //Insert Image to Blob server
                var imageUrl = await BlobService.UploadFileAsync(new MemoryStream(byteData));
                imageUrls.Add("https://msorli.blob.core.windows.net/images/" + imageUrl);
            }

            return imageUrls;
        }

        private void InitializeCarouselView()
        {
            // Update CarouselView attributes
            imagesView.Margin = new Thickness(5, 60, 5, 0);
            imagesView.HeightRequest = 300;
        }

        //Convert from Stream to array of bytes
        private byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

        //If user inserted new info to one of the entries, make the label visable
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