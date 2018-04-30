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

namespace MsorLi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddItemPage : ContentPage
    {
        //---------------------------------------------------
        // MEMBERS
        //---------------------------------------------------

        List<byte[]> _byteData = new List<byte[]>();
        ObservableCollection<ImageSource> _images = new ObservableCollection<ImageSource>();
        
        //---------------------------------------------------
        // FUNCTIONS
        //---------------------------------------------------

        public AddItemPage()
        {
            InitializeComponent();

            city.Text = Settings.Address;
            contactName.Text = Settings.UserFirstName + " " + Settings.UserLastName;
            contactNumber.Text = Settings.Phone;

            var categories = CategoryStorage.GetCategories().Result;

            foreach (var c in categories)
            {
                category.Items.Add(c.Name);
            }
        }

        //---------------------------------------------------
        // EVENT FUNCTIONS
        //---------------------------------------------------

        public async void OnAddImageClick(object sender, EventArgs e)
        {
            try
            {
                if (_images.Count == Constants.MAX_NUM_OF_IMAGES) return;

                pickPictureButton.IsEnabled = false;
                Stream imageStream = await DependencyService.Get<IPicturePicker>().GetImageStreamAsync();

                if (imageStream != null)
                {
                    _byteData.Add(ImageUpload.ReadFully(imageStream));
                    ImageSource imageSource = ImageSource.FromStream(() => new MemoryStream(_byteData[_byteData.Count - 1]));

                    if (_images.Count == 0)
                    {
                        InitializeCarouselView();
                    }

                    _images.Add(imageSource);
                    imagesView.ItemsSource = _images;
                }

                pickPictureButton.IsEnabled = _images.Count == Constants.MAX_NUM_OF_IMAGES ? false : true;
            }
            catch (Exception) {}
        }

        public async void OnAddItemClick(object sender, EventArgs e)
        {
            try
            {
                if (Validation() == false)
                {
                    await DisplayAlert("", "אחד או יותר משדות החובה לא מולאו. יש למלא את כולן ולנסות בשנית.", "אישור");
                    return;
                }

                // Save images in blob
                List<string> imageUrls = await BlobService.SaveImagesInDB(_byteData);

                // Create new item
                Item item = CreateNewItem(imageUrls.Count);

                // Upload item to data base
                await AzureItemService.DefaultManager.UploadToServer(item, item.Id);

                // Update item counter
                int _numOfItems = await AzureUserService.DefaultManager.UpdateNumOfItems(Settings.UserId, 1);
                Settings.NumOfItems = _numOfItems.ToString();

                // Create all item images
                List<ItemImage> itemImages = CreateItemImages(imageUrls, item.Id, item.UserId);

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

        public async void OnCategoryChanged(object sender, EventArgs e)
        {
            subCategory.IsEnabled = true;

            var mainCategory = category.Items[category.SelectedIndex];

            var SubCategories = await SubCategoryStorage.GetSubCategories(mainCategory);

            subCategory.Items.Clear();
            foreach (var item in SubCategories)
            {
                subCategory.Items.Add(item.Name);
            }
        }

        //---------------------------------------------------
        // PRIVATE FUNCTIONS
        //---------------------------------------------------

        private async Task UploadImageToTable(ItemImage itemImage)
        {
            await AzureImageService.DefaultManager.UploadToServer(itemImage, itemImage.Id);
        }

        private void InitializeCarouselView()
        {
            // Update CarouselView attributes
            imagesView.Margin = new Thickness(5, 60, 5, 0);
            imagesView.HeightRequest = 300;
        }

        private bool Validation()
        {
            //if (category.SelectedIndex == -1 ||
            //    _images.Count == 0 ||
            //    description.Text.Length == 0 ||
            //    condition.SelectedIndex == -1 ||
            //    city.Text.Length == 0 ||
            //    contactName.Text.Length == 0 ||
            //    contactNumber.Text.Length == 0)
            //{
            //    return false;
            //}


            if (category.SelectedIndex == -1)
                return false;
            if (_images.Count == 0)
                return false;
            if (description.Text.Length == 0)
                return false;
            if (condition.SelectedIndex == -1)
                return false;
            if (city.Text.Length == 0)
                return false;
            if (contactName.Text.Length == 0)
                return false;
            if (contactNumber.Text.Length == 0)
                return false;
            
            return true;
        }

        private List<ItemImage> CreateItemImages(List<string> imageUrls , string id, string userId)
        {
            List<ItemImage> itemImages = new List<ItemImage>();

            for (int i = 0; i < imageUrls.Count; ++i)
            {
                if (i == 0)
                {
                    // First and Priority image
                    itemImages.Add(new ItemImage {
                        Url = imageUrls[i],
                        ItemId = id,
                        IsPriorityImage = true,
                        UserId = userId,
                        Category = category.Items[category.SelectedIndex],
                        SubCategory = subCategory.Items[subCategory.SelectedIndex],
                    });
                }
                else
                {
                    itemImages.Add(new ItemImage { Url = imageUrls[i], ItemId = id, IsPriorityImage = false, UserId = userId});
                }
            }
            return itemImages;
        }

        private Item CreateNewItem(int numOfUrls)
        {
            var item = new Item
            {
                Category = category.Items[category.SelectedIndex],
                SubCategory = subCategory.Items[subCategory.SelectedIndex],
                NumOfImages = numOfUrls,
                Description = description.Text,
                Condition = condition.SelectedItem.ToString(),
                Location = city.Text + ", " + street.Text,
                ViewCounter = 0,
                Date = DateTime.Today.ToString("d"),
                Time = DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString(),
                ContactName = contactName.Text,
                ContactNumber = contactNumber.Text,
                UserId = Settings.UserId
            };

            return item;
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