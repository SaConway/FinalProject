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

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace MsorLi.Views
{
    public partial class AddItemPage : ContentPage
    {
        //---------------------------------------------------
        // MEMBERS
        //---------------------------------------------------

        AzureItemService _azureItemService = AzureItemService.DefaultManager;
        AzureImageService _azureImageService = AzureImageService.DefaultManager;
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

        async protected override void OnAppearing()
        {
            if (Settings._GeneralSettings != "True")
            {
                await Navigation.PushAsync(new LoginPage());
                MainPage m = new MainPage();
                m.get().CurrentPage = m.get().Children[0];
            }
        }

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
            catch (Exception) {}
        }

        //Add Item button operation
        public async void OnAdd_Event(object sender, EventArgs e)
        {
            try
            {
                // Save images in blob
                List<string> imageUrls = await SaveImagesInDB();

                // Create new item
                Item item = CreateNewItem(imageUrls.Count);

                // Upload item to data base
                await _azureItemService.UploadToServer(item, item.Id);

                // Create all item images
                List<ItemImage> itemImages = CreateItemImages(imageUrls, item.Id);

                // Upload item images to data base
                foreach (var itemImage in itemImages)
                {
                    await _azureImageService.UploadToServer(itemImage, itemImage.Id);
                }
            }

            catch {}
        }

        //---------------------------------------------------
        // PRIVATE FUNCTIONS
        //---------------------------------------------------

        private List<ItemImage> CreateItemImages(List<string> imageUrls , string id)
        {
            List<ItemImage> itemImages = new List<ItemImage>();

            foreach(var imageUrl in imageUrls)
            {
                itemImages.Add(new ItemImage { Url = imageUrl, ItemId = id });
            }

            return itemImages;
        }

        private Item CreateNewItem(int numOfUrls)
        {
            var item = new Item
            {
                Title = name.Text,
                NumOfImages = numOfUrls,
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