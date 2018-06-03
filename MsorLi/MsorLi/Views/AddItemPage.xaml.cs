#region Usings
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
#endregion

namespace MsorLi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddItemPage : ContentPage
    {
        #region Members

        bool _isEditingItem = false;

        Dictionary<ImageSource, Tuple<ItemImage, byte[]>> _keyValues = 
            new Dictionary<ImageSource, Tuple<ItemImage, byte[]>>();

        Item _item = new Item();

        Boolean _isRunningItem = false;
        Boolean _runFrame = false;
        Object _lockObject = new Object();
        string _conditionString = "חדש";

        bool _firstAppearing = true;

        string _erea = "";

        #endregion

        //---------------------------------------------------
        // FUNCTIONS
        //---------------------------------------------------

        public AddItemPage()
        {
            try
            {
                InitializeComponent();
                this.Title = "הוספת מודעה";

                BindingContext = this;

                contactName.Text = Settings.UserFirstName + " " + Settings.UserLastName;
                contactNumber.Text = Settings.Phone;
                email.Text = Settings.Email;
                _erea = Settings.Erea;
                street.Text = Settings.Address != null && Settings.Address.Length > 0 ? Settings.Address : "";
            }
            catch { }
        }

        public AddItemPage(string itemId)
        {
            _item.Id = itemId;
        }

        public async Task EditItemInit(Item item, ObservableCollection<ItemImage> images)
        {
            try
            {
                _isEditingItem = true;
                InitializeComponent();
                InitializeCarouselView();

                this.Title = "עריכת מודעה";

                _item = item;

                await SetCategoris();
                await SetSubCategories(item.Category);
                await SetEreas();

                // Update item details

                category.SelectedItem = item.Category;
                description.Text = item.Description;


                switch (item.Condition)
                {
                    case "חדש":
                        //button.BackgroundColor = Color.Transparent;
                        //button.TextColor = Color.FromHex("19a4b4");
                        NewBtn.BackgroundColor = Color.FromHex("19a4b4");
                        NewBtn.TextColor = Color.FromHex("ffffff");
                        _conditionString = NewBtn.Text;
                        break;

                    case "משומש":
                        NewBtn.BackgroundColor = Color.Transparent;
                        NewBtn.TextColor = Color.FromHex("19a4b4");
                        UsedBtn.BackgroundColor = Color.FromHex("19a4b4");
                        UsedBtn.TextColor = Color.FromHex("ffffff");
                        _conditionString = UsedBtn.Text;
                        break;

                    default:
                        break;
                }

                EreaPicker.SelectedItem = item.Erea;
                contactName.Text = item.ContactName;
                contactNumber.Text = item.ContactNumber;
                subCategory.SelectedItem = item.SubCategory;

                foreach (var img in images)
                {
                    _keyValues.Add(img.Url, new Tuple<ItemImage, byte[]>(img, null));
                }

                var keyList = new List<ImageSource>(_keyValues.Keys);
                imagesView.ItemsSource = keyList;

                pickPictureButton.IsEnabled = _keyValues.Count == Constants.MAX_NUM_OF_IMAGES ? false : true;
            }
            catch(NoConnectionException)
            {
                if (!NoConnctionPage.Loaded)
                {
                    await Navigation.PushAsync(new NoConnctionPage());
                }
            }
            catch (Exception) { }
        }

        protected async override void OnAppearing()
        {
            try
            {
                if (_firstAppearing && !_isEditingItem)
                {
                    _firstAppearing = false;

                    //get areas from server
                    await SetEreas();
                    EreaPicker.SelectedItem = _erea;

                    //get categories from server
                    await SetCategoris();

                }
            }
            catch (NoConnectionException)
            {
                if (!NoConnctionPage.Loaded)
                {
                    await Navigation.PushAsync(new NoConnctionPage());
                }
            }
            catch (Exception)
            {

            }
        }

        //---------------------------------------------------
        // EVENT FUNCTIONS
        //---------------------------------------------------

        private async void OnAddImageClick(object sender, EventArgs e)
        {
            try
            {
                lock (_lockObject)
                {
                    if (_isRunningItem)
                        return;
                    else
                        _isRunningItem = true;
                }

                if (_keyValues.Count == Constants.MAX_NUM_OF_IMAGES) return;

                pickPictureButton.IsEnabled = false;
                Stream imageStream = await DependencyService.Get<IPicturePicker>().GetImageStreamAsync();

                if (imageStream != null)
                {
                    var byt = ImageUpload.ReadFully(imageStream);
                    ImageSource imageSource = ImageSource.FromStream(() => new MemoryStream(byt));

                    if (_keyValues.Count == 0)
                    {
                        InitializeCarouselView();

                        _keyValues.Add(imageSource, new Tuple<ItemImage, byte[]>(new ItemImage { IsPriorityImage = true }, byt));
                    }
                    else
                    {
                        _keyValues.Add(imageSource, new Tuple<ItemImage, byte[]>(new ItemImage { IsPriorityImage = false }, byt));
                    }

                    //var keyList = new List<ImageSource>(_keyValues.Keys);
                    var collection = new List<MsorLi.Utilities.ImageHelper>();
                    foreach (var img in _keyValues.Keys)
                    {
                        collection.Add(new Utilities.ImageHelper { imageSource = img });
                    }
                    imagesView.ItemsSource = collection;
                }

                pickPictureButton.IsEnabled = _keyValues.Count == Constants.MAX_NUM_OF_IMAGES ? false : true;

                _isRunningItem = false;

            }
            catch (Exception)
            {
                _isRunningItem = false;
            }
        }

        private async void OnAddItemClick(object sender, EventArgs e)
        {
            try
            {
                lock (_lockObject)
                {
                    if (_isRunningItem)
                        return;
                    else
                        _isRunningItem = true;
                }

                if (!Validation())
                {
                    await DisplayAlert("", "אחד או יותר משדות החובה לא מולאו. יש למלא את כולן ולנסות בשנית.", "אישור");
                    return;
                }

                MyScrollView.IsEnabled = false;
                MyScrollView.Opacity = 0.5;
                MyFrame.IsVisible = true;
                _runFrame = true;
                UpdaterFrameAsync();


                //upload item into item table
                await UploadItem();

                if (_item.Id == null)
                    throw new Exception();
                    
                var t2 = UploadImages();

                await Task.WhenAll(t2);
                
                if (!_isEditingItem)
                {
                    // Update item counter

                    int _numOfItems = await AzureUserService.DefaultManager.UpdateNumOfItems(Settings.UserId, 1);
                    Settings.NumOfItems = _numOfItems.ToString();

                    DependencyService.Get<IMessage>().LongAlert("פרסום המוצר בוצע בהצלחה");

                    await Navigation.PopToRootAsync();
                }

                if (_isEditingItem)
                {
                    MessagingCenter.Send<AddItemPage>(this, "Updated Item");

                    MessagingCenter.Subscribe<ItemPage>(this, "Finished To Updated Item", async (s) => {

                        MessagingCenter.Unsubscribe<ItemPage>(this, "Finished To Updated Item");

                        MyFrame.IsVisible = false;
                        _runFrame = true;
                        DependencyService.Get<IMessage>().LongAlert("עדכון המוצר בוצע בהצלחה");
						await Navigation.PopToRootAsync();

                    });
                }

                _isRunningItem = false;

            }
            catch (NoConnectionException)
            {
                _isRunningItem = false;
                if (!NoConnctionPage.Loaded)
                {
                    await Navigation.PushAsync(new NoConnctionPage());
                }
            }

            catch (Exception)
            {
                _isRunningItem = false;

                await DisplayAlert("שגיאה", "לא ניתן להשלים את פעולת פרסום המוצר. נסה/י שנית מאוחר יותר.", "אישור");
				await Navigation.PopToRootAsync();
            }
        }

        private async void OnCategoryChanged(object sender, EventArgs e)
        {
            try
            {
                string Category = category.Items[category.SelectedIndex];
                await SetSubCategories(Category);
            }
            catch (NoConnectionException)
            {
                _isRunningItem = false;
                if (!NoConnctionPage.Loaded)
                {
                    await Navigation.PushAsync(new NoConnctionPage());
                }
            }
            catch (Exception)
            {

            }
        }

        private async void OnDeleteImg(object sender, TappedEventArgs e)
        {
            try
            {
                lock (_lockObject)
                {
                    if (_isRunningItem)
                        return;
                    else
                        _isRunningItem = true;
                }

                ImageSource key = (ImageSource)e.Parameter;

                Tuple<ItemImage, byte[]> tuple = _keyValues[key];

                if (tuple.Item2 == null)
                {
                    // Image was uploaded to DB and Blob,
                    // so we need to delete..

                    await AzureImageService.DefaultManager.DeleteImage(tuple.Item1);
                    await BlobService.DeleteImage(tuple.Item1.Url);
                }

                _keyValues.Remove(key);

                var collection = new List<MsorLi.Utilities.ImageHelper>();
                foreach (var img in _keyValues.Keys)
                {
                    collection.Add(new Utilities.ImageHelper { imageSource = img });
                }
                imagesView.ItemsSource = collection;

                if (_keyValues.Count == 0)
                {
                    // Hide images view

                    imagesView.Margin = new Thickness(0,0,0,0);
                    imagesView.HeightRequest = 0;
                }
                else if (_keyValues.Count == 1)
                {
                    // Update the img to be priority

                    foreach (var item in _keyValues.Values)
                    {
                        item.Item1.IsPriorityImage = true;
                    }
                }

                pickPictureButton.IsEnabled = _keyValues.Count == Constants.MAX_NUM_OF_IMAGES ? false : true;

                _isRunningItem = false;
            }
            catch (NoConnectionException)
            {
                _isRunningItem = false;
                if (!NoConnctionPage.Loaded)
                {
                    await Navigation.PushAsync(new NoConnctionPage());
                }
            }
            catch (Exception)
            {
                _isRunningItem = false;

                await DisplayAlert("שגיאה", "לא ניתן להשלים את הפעולה. נסה שנית.", "אישור");
            }
        }

        //---------------------------------------------------
        // PRIVATE FUNCTIONS
        //---------------------------------------------------

        private async Task UploadImages()
        {
            try
            {
                var tList = new List<Task>();
                foreach (var item in _keyValues)
                {
                    var ByteData = item.Value.Item2;

                    if (ByteData != null)
                    {
                        // A New Image

                        var Url = await BlobService.SaveImageInBlob(ByteData);

                        item.Value.Item1.Url = Url;
                        item.Value.Item1.ItemId = _item.Id;
                        item.Value.Item1.UserId = Settings.UserId;
                    }

                    item.Value.Item1.Category = category.Items[category.SelectedIndex];
                    item.Value.Item1.SubCategory = subCategory.Items[subCategory.SelectedIndex];
                    item.Value.Item1.Erea = EreaPicker.SelectedItem.ToString();
                    item.Value.Item1.Condition = _conditionString;

                    var t = UploadImageToDB(item.Value.Item1);

                    tList.Add(t);
                }

                await Task.WhenAll(tList);
            }
            catch (NoConnectionException)
            {
                _isRunningItem = false;
                if (!NoConnctionPage.Loaded)
                {
                    await Navigation.PushAsync(new NoConnctionPage());
                }
            }
            catch (Exception)
            {

            }
        }

        private async Task UploadItem()
        {
            try
            {
                _item.Category = category.Items[category.SelectedIndex];
                _item.SubCategory = subCategory.Items[subCategory.SelectedIndex];
                _item.NumOfImages = _keyValues.Count;
                _item.Description = description.Text;
                _item.Condition = _conditionString;
                _item.Erea = EreaPicker.SelectedItem.ToString();
                _item.Address = (street.Text != null && street.Text.Length > 0) ? street.Text.ToString() : "";
                _item.ViewCounter = 0;
                _item.ContactName = contactName.Text;
                _item.ContactNumber = contactNumber.Text;
                _item.UserId = Settings.UserId;
                _item.Email = (email.Text != null && email.Text.Length > 0) ? email.Text : "";

                if (!_isEditingItem)
                {
                    _item.Date = DateTime.Today.ToString("d");
                    _item.Time = DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString();
                }
            }
            catch (Exception)
            {

            }

            await AzureItemService.DefaultManager.UploadToServer(_item, _item.Id);
   
        }

        private async Task UploadImageToDB(ItemImage itemImage)
        {
            await AzureImageService.DefaultManager.UploadToServer(itemImage, itemImage.Id);
        }

        private void InitializeCarouselView()
        {
            try
            {
                imagesView.Margin = new Thickness(5, 60, 5, 0);
                imagesView.HeightRequest = 300;
            }
            catch (Exception)
            {

            }
        }

        private bool Validation()
        {
            try
            {
                if (category.SelectedIndex == -1 || _keyValues.Count == 0 ||
                description.Text.Length == 0 || 
                EreaPicker.SelectedIndex == -1 || contactName.Text.Length == 0 ||
                contactNumber.Text.Length == 0 || subCategory.SelectedIndex == -1)
                    return false;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
            
        }

        private async Task SetSubCategories(string category)
        {

            subCategory.IsEnabled = true;

            var SubCategories = await SubCategoryStorage.GetSubCategories(category);

            subCategory.Items.Clear();
            foreach (var item in SubCategories)
            {
                subCategory.Items.Add(item.Name);
            }

        }

        private async Task SetCategoris()
        {

            var categories = await CategoryStorage.GetCategories();

            category.Items.Clear();
            foreach (var c in categories)
            {
                if (!category.Items.Contains(c.Name))
                    category.Items.Add(c.Name);
            }


        }

        private async Task SetEreas()
        {

            var locations = await LocationStorage.GetLocations();

            foreach (var l in locations)
            {
                if (!EreaPicker.Items.Contains(l.Name))
                    EreaPicker.Items.Add(l.Name);
            }

        }

        private void ConditionNewClicked(object sender, TappedEventArgs e)
        {

            ConditionChange();
            NewBtn.BackgroundColor = Color.FromHex("19a4b4");
            NewBtn.TextColor = Color.FromHex("ffffff");
            _conditionString = NewBtn.Text;

        }
        private void ConditionUsedClicked(object sender, TappedEventArgs e)
        {
            ConditionChange();
            UsedBtn.BackgroundColor = Color.FromHex("19a4b4");
            UsedBtn.TextColor = Color.FromHex("ffffff");
            _conditionString = UsedBtn.Text;
        }

        private void ConditionChange()
        {
            switch (_conditionString)
            {
                case "חדש":
                    NewBtn.BackgroundColor = Color.Transparent;
                    NewBtn.TextColor = Color.FromHex("19a4b4");
                    break;

                case "משומש":
                    UsedBtn.BackgroundColor = Color.Transparent;
                    UsedBtn.TextColor = Color.FromHex("19a4b4");
                    break;

                default:
                    break;
            }
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