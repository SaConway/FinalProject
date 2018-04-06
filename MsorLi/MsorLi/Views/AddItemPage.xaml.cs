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

        List<string> _categories = new List<string>();

        bool _myBoolean = true;
        bool _firstAppearing = true;

        //---------------------------------------------------
        // FUNCTIONS
        //---------------------------------------------------

        async protected override void OnAppearing()
        {
            // User is not logged in
            if (Settings.UserId == "" && _myBoolean)
            {
                _myBoolean = false;
                await Navigation.PushAsync(new LoginPage());
            }

            // User is not logged in and he is back from log in page
            else if (Settings.UserId == "" && !_myBoolean)
            {
                await Navigation.PopToRootAsync();
            }

            // User has just looged in
            else if (Settings.UserId != "" && !_myBoolean)
            {
                _myBoolean = true;
                await InitializeAsync();
            }

            // User is looged in and its his first appearing
            else if (Settings.UserId != "" && _firstAppearing)
            {
                _firstAppearing = false;
                await InitializeAsync();
            }
        }

        private async Task InitializeAsync()
        {
            AzureCategoryService azureCategory = AzureCategoryService.DefaultManager;
            _categories = await azureCategory.GetAllCategories();

            MyInitializeComponent();
        }

        private void MyInitializeComponent()
        {
            InitializeComponent();

            city.Text = Settings.Address;
            contactName.Text = Settings.UserFirstName + " " + Settings.UserLastName;
            contactNumber.Text = Settings.Phone;

            city.Margin = new Thickness(25, 15, 25, 0);
            contactName.Margin = new Thickness(25, 15, 25, 0);
            contactNumber.Margin = new Thickness(25, 15, 25, 0);

            cityLabel.IsVisible = true;
            contactNameLabel.IsVisible = true;
            contactNumberLabel.IsVisible = true;

            if (_categories != null)
            {
                foreach (var c in _categories)
                {
                    category.Items.Add(c);
                }
            }
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
            catch (Exception) {}
        }

        //Add Item button operation
        public async void OnAdd_Event(object sender, EventArgs e)
        {
            try
            {
                if (Validation() == false)
                {
                    await DisplayAlert("", "אחד או יותר משדות החובה לא מולאו. יש למלא את כולן ולנסות בשנית.", "אישור");
                    return;
                }

                // Save images in blob
                List<string> imageUrls = await SaveImagesInDB();

                // Create new item
                Item item = CreateNewItem(imageUrls.Count);

                // Upload item to data base
                await _azureItemService.UploadToServer(item, item.Id);

                // Create all item images
                List<ItemImage> itemImages = CreateItemImages(imageUrls, item.Id);

                List<Task> TaskList = new List<Task>();

                // Upload item images to data base
                foreach (var itemImage in itemImages)
                {
                    var task = UploadImageToTable(itemImage);
                    TaskList.Add(task);
                }
                await Task.WhenAll(TaskList);

                await Navigation.PopAsync();
                DependencyService.Get<IMessage>().LongAlert("פרסום המוצר בוצע בהצלחה");
            }

            catch (Exception)
            {
                await DisplayAlert("שגיאה", "לא ניתן להשלים את פעולת פרסום המוצר. נסה/י שנית מאוחר יותר.", "אישור");
                await Navigation.PopAsync();
            }
        }

        //---------------------------------------------------
        // PRIVATE FUNCTIONS
        //---------------------------------------------------

        private async Task UploadImageToTable(ItemImage itemImage)
        {
            await _azureImageService.UploadToServer(itemImage, itemImage.Id);
        }

        private bool Validation()
        {
            if (category.SelectedIndex == -1 ||
                _images.Count == 0 ||
                description.Text.Length == 0 ||
                condition.SelectedIndex == -1 ||
                city.Text.Length == 0 ||
                contactName.Text.Length == 0 ||
                contactNumber.Text.Length == 0)
            {
                return false;
            }

            return true;
        }

        private List<ItemImage> CreateItemImages(List<string> imageUrls , string id)
        {
            List<ItemImage> itemImages = new List<ItemImage>();

            for (int i = 0; i < imageUrls.Count; ++i)
            {
                if (i == 0)
                {
                    // First and Priority image
                    itemImages.Add(new ItemImage { Url = imageUrls[i], ItemId = id, IsPriorityImage = true });
                }
                else
                {
                    itemImages.Add(new ItemImage { Url = imageUrls[i], ItemId = id, IsPriorityImage = false });
                }
            }

            return itemImages;
        }

        private Item CreateNewItem(int numOfUrls)
        {
            var item = new Item
            {
                Category = category.Items[category.SelectedIndex],
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

            foreach (var imageData in _byteData)
            {
                byte[] resizedImage = ImageResizer.ResizeImage(imageData, 400, 400);

                //Insert Image to Blob server
                var imageUrl = await BlobService.UploadFileAsync(new MemoryStream(resizedImage));
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
            
            if (entry.Placeholder.ToString() == "תיאור מוצר")
            {
                descriptionLabel.IsVisible = IsVisable;
            }
            else if (entry.Placeholder.ToString() == "עיר מגורים")
            {
                cityLabel.IsVisible = IsVisable;
            }
            else if (entry.Placeholder.ToString() == "רחוב (אופציונלי)")
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