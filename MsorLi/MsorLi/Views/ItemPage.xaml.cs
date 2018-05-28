using MsorLi.Models;
using MsorLi.Services;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MsorLi.Utilities;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.Generic;
using Plugin.Messaging;
using FFImageLoading.Forms;
using Rg.Plugins.Popup.Extensions;
using FFImageLoading.Transformations;

namespace MsorLi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemPage : ContentPage, INotifyPropertyChanged
    {
        //---------------------------------
        // MEMBERS
        //---------------------------------

        public bool _saveItem = false;
        public bool _itemWasSaved = false;
        public bool _unSaveItem = false;

        Item _item = new Item();
        string _userId = Utilities.Settings.UserId;
        ObservableCollection<ItemImage> _images = new ObservableCollection<ItemImage>();

        //list of items form the same user
        public ObservableCollection<ItemImage> AllImages = new ObservableCollection<ItemImage>();
        Boolean _isRunningItem = false;
        Object _lockObject = new Object();

        string _savedId = "";

        bool _DoInitialization = true;

        //---------------------------------
        // FUNCTIONS
        //---------------------------------

        // INITIALIZE FUNCTIONS
        //----------------------------------------------------------

        // c-tor
        public ItemPage(string itemId)
        {
            _item.Id = itemId;

            MessagingCenter.Subscribe<AddItemPage>(this, "Updated Item", async (sender) => {

                MessagingCenter.Unsubscribe<AddItemPage>(this, "Updated Item");

                var task1 = SetItemAsync();
                var task2 = SetItemImagesAsync();

                await Task.WhenAll(task1, task2);

                UpdateItemDetails();
                UpdateItemImages();

                MessagingCenter.Send<ItemPage>(this, "Finished To Updated Item");

            });
        }

        protected override async void OnAppearing()
        {
            if (_DoInitialization)
            {
                _DoInitialization = false;

                InitializeComponent();

                MyScrollView.IsVisible = false;
                MyScrollView.Opacity = 0;

                MyActivityIndicator.IsRunning = true;
                MyActivityIndicator.IsVisible = true;

                await InitializeAsync();
            }
        }

        async Task InitializeAsync()
        {
            var task1 = SetItemAsync();
            var task2 = SetItemImagesAsync();
            var task3 = SetItemSavedAsync();

            await Task.WhenAll(task1, task2, task3);

            //get more items from same user
            await GetUserItems(_item.UserId); 

            MyInitializeComponent();
        }

        async void MyInitializeComponent()
        {
            // Update item images

            imagesView.HeightRequest = (double)(Constants.ScreenHeight / 2.5);

            UpdateItemImages();

            // Update saved item details

            if (_savedId != "" && Utilities.Settings.UserId != null)
            {
                // Item is saved by the user

                _itemWasSaved = true;
                UnSaveStack.IsVisible = true;
            }
            else
            {
                // Item is not saved by the user
                
                SaveStack.IsVisible = true;
            }

            // Update item details
            UpdateItemDetails();

            // Show Delete item btn and Update item ?

            if (_userId == _item.UserId || Utilities.Settings.Permission == "Admin")
            {
                ToolbarItems.Add(new ToolbarItem("", "edit.png", async () =>
                {
                    await UpdateItem();
                }));

                ToolbarItems.Add(new ToolbarItem("", "delete_item.png", async () =>
                {
                    await DeleteItem();
                }));
            }

            // Hide Activity Indicator
            await MyActivityIndicator.FadeTo(0, 100);
            MyActivityIndicator.IsRunning = false;
            MyActivityIndicator.IsVisible = false;

            // Show View
            MyScrollView.IsVisible = true;
            await MyScrollView.FadeTo(1, 100);

            await ItemList.ScrollToAsync(StackUserItems.Children[StackUserItems.Children.Count - 1], ScrollToPosition.MakeVisible, true);

        }

        protected async override void OnDisappearing()
        {
            //Save Item
            if (_saveItem && !_itemWasSaved && _item.Id != null)
            {
                await AzureSavedItemService.DefaultManager.UploadToServer(new SavedItem { ItemId = _item.Id, UserId = _userId }, null);
                UpdateLikeCounter(1);
            }
            //Unsave Item
            else if (_unSaveItem && _itemWasSaved && _item.Id != null)
            {
                await AzureSavedItemService.DefaultManager.DeleteSavedItem(new SavedItem { Id = _savedId });
                UpdateLikeCounter(-1);
                MessagingCenter.Send<ItemPage, string>(this, "Item Unsaved", _item.Id);
            }
            else
            {
                MessagingCenter.Send<ItemPage>(this, "Back From Item Page");
            }
        }

        // EVENT FUNCTIONS
        //----------------------------------------------------------

        // Save Button
        private async void SaveButtonClick(object sender, EventArgs e)
        {
            try
            {
                if (Utilities.Settings.UserId == "")
                {
                    // User is Not loged in
                    await Navigation.PushAsync(new LoginPage());
                }
                else
                {  // User is allowed to save Item

                    if (SaveStack.IsVisible == true)
                    {
                        // Change from save to unsave

                        UnSaveStack.Opacity = 0;
                        await SaveStack.FadeTo(0, 100);
                        SaveStack.IsVisible = false;
                        UnSaveStack.IsVisible = true;
                        await UnSaveStack.FadeTo(1);

                        _saveItem = true;
                        _unSaveItem = false;
                    }
                    else
                    {
                        // Change from unsave to save

                        SaveStack.Opacity = 0;
                        await UnSaveStack.FadeTo(0, 100);
                        UnSaveStack.IsVisible = false;
                        SaveStack.IsVisible = true;
                        await SaveStack.FadeTo(1);

                        _unSaveItem = true;
                        _saveItem = false;
                    }
                }
            }

            catch (Exception)
            {
                await DisplayAlert("שגיאה", "לא ניתן לשמור מוצר מבוקש. נסה שנית.", "אישור");
            }
        }

        // Share button
        private void ShareButtonClick(object sender, EventArgs e)
        {
            DependencyService.Get<Utilities.IShare>().Share("מוצר למסירה, דרך אפליקציית מסור-לי.",
                 "מוצר למסירה, דרך אפליקציית מסור--לי. להלן פרטי המוצר ודרכי התקשרות:" + Environment.NewLine + Environment.NewLine
                     + "קטגוריה: " + _item.Category + Environment.NewLine
                     + "מיקום: " + _item.Erea + Environment.NewLine
                     + "מצב מוצר: " + _item.Condition + Environment.NewLine
                     + "תיאור: " + _item.Description + Environment.NewLine
                     + "שם איש קשר: " + _item.ContactName + Environment.NewLine
                     + "טלפון ליצירת קשר: " + _item.ContactNumber + Environment.NewLine + Environment.NewLine
                     + "למגוון מוצרים נוספים אנא הורד את אפליקציית מסור-לי.",
                    _images[0].Url
                );
        }

        // Report Button
        private async void OnReportClick(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new ReportItemPage(_item.Id));
            }
            catch
            {

            }
        }

        // Waze Buuton
        private void OnWazeClick(object sender, EventArgs e)
        {
            var location = _item.Address.Length > 0 ? 
                _item.Erea + ", " + _item.Address : _item.Erea;

            DependencyService.Get<IWaze>().Navigate(location);
        }

        // Call Button
        private async void CallButtonClick(object sender, EventArgs e)
        {
            try
            {
                var phoneDialer = CrossMessaging.Current.PhoneDialer;
                if (phoneDialer.CanMakePhoneCall)
                    phoneDialer.MakePhoneCall(_item.ContactNumber);
                else throw new Exception();
            }
            catch (Exception)
            {
                await DisplayAlert("שגיאה", "לא ניתן להשלים שיחה זו.", "אישור");
            }
        }

        // PRIVATE FUNCTIONS
        //----------------------------------------------------------

        private async Task DeleteItem()
        {
            try
            {
                var answer = await DisplayAlert("", "האם ברצונך למחוק מודעה זו?", "כן", "לא");

                if (!answer)
                {
                    return;
                }

                List<Task> list = new List<Task>();
                var task1 = AzureItemService.DefaultManager.DeleteItem(_item);
                list.Add(task1);

                foreach (var img in _images)
                {
                    var task2 = AzureImageService.DefaultManager.DeleteImage(img);
                    list.Add(task2);

                    var task3 = BlobService.DeleteImage(img.Url);
                    list.Add(task3);
                }

                int num_of_items = await AzureUserService.DefaultManager.UpdateNumOfItems(Utilities.Settings.UserId, -1);
                Utilities.Settings.NumOfItems = num_of_items.ToString();

                await Task.WhenAll(list);

                MessagingCenter.Send<ItemPage>(this, "Item Deleted");
                await Navigation.PopAsync();
                DependencyService.Get<IMessage>().LongAlert("המודעה נמחקה");
            }
            catch (Exception)
            {
                await DisplayAlert("שגיאה", "לא ניתן להשלים את הפעולה. נסה שנית.", "אישור");
            }
        }

        private async Task UpdateItem()
        {
            try
            {
                AddItemPage addItemPage = new AddItemPage(_item.Id);
                await Navigation.PushAsync(addItemPage);
                await addItemPage.EditItemInit(_item, _images);
            }
            catch (Exception)
            {

            }
        }

        private void UpdateItemImages()
        {
            ObservableCollection<Models.Image> images = new ObservableCollection<Models.Image>();

            for (int i = 0; i < _images.Count; ++i)
            {
                Models.Image image = new Models.Image { ImageUrl = _images[i].Url, ImageNumber = (i + 1).ToString() + " מתוך " + _images.Count.ToString() };
                images.Add(image);
            }

            imagesView.ItemsSource = images;
        }

        private void UpdateItemDetails()
        {
            title.Text = _item.Category;
            subCategory.Text = _item.SubCategory;
            description.Text = _item.Description;
            condition.Text = _item.Condition;
            location.Text = _item.Address.Length > 0 ? _item.Erea + ", " + _item.Address : _item.Erea;
            contact_name.Text = _item.ContactName;
            contact_number.Text = _item.ContactNumber;
            date.Text = _item.Date;
            email.Text = (_item.Email.Length > 0) ? _item.Email : "מידע חסר";
        }

        private async void UpdateLikeCounter(int prefix)
        {
            int _numOfLikedItem = await AzureUserService.DefaultManager.UpdateNumOfItemsLiked(MsorLi.Utilities.Settings.UserId, prefix);
            Utilities.Settings.NumOfItemsUserLike = _numOfLikedItem.ToString();
            MessagingCenter.Send<ItemPage>(this, "Update Like Counter");
        }

        private async Task SetItemAsync()
        {
            try
            {
                _item = await AzureItemService.DefaultManager.GetItemAsync(_item.Id);
            }
            catch(Exception){}
        }

        private async Task SetItemImagesAsync()
        {
            try
            {
                _images = await AzureImageService.DefaultManager.GetItemImages(_item.Id);
            }
            catch(Exception){}
        }

        private async Task SetItemSavedAsync()
        {
            try
            {
                _savedId = await AzureSavedItemService.DefaultManager.IsItemSaved(_item.Id, _userId);
            }
            catch(Exception){}
        }




        // Items List From same User FUNCTIONS
        //----------------------------------------------------------

        //Get User Items (By User ID)
        private async Task GetUserItems(string userId)
        {
            try
            {
                AllImages = await AzureImageService.DefaultManager.GetAllImgByUserId(userId);

                if (AllImages.Count > 1)
                {
                    ItemList.IsVisible = true;
                    UserLabel.Text = "מוצרים נוספים של המוסר";
                    ShowImages();
                }
                else
                {
                    ItemList.IsVisible = false;
                    UserLabel.IsVisible = false;
                }
            }
            catch (Exception)
            {
                await DisplayAlert("שגיאה", "שגיאה בקבלת מידע מהשרת", "אישור");

            }
        }

        private void ShowImages()
        {
            StackUserItems.Children.Clear();
            for (int i = 0; i < AllImages.Count; i++)
            {
                if (AllImages[i].ItemId == _item.Id)
                    continue;

                var image = new CachedImage
                {
                    Source = AllImages[i].Url,
                    WidthRequest = Utilities.Constants.ScreenWidth / 2,
                    HeightRequest = Utilities.Constants.ScreenWidth / 2,
                    DownsampleToViewSize = true
                };

                image.Transformations.Add(new RoundedTransformation(15));
                var tap = new TapGestureRecognizer();
                tap.CommandParameter = AllImages[i].ItemId;

                //image tap function loading the item page 
                tap.Tapped += async (s, e) =>
                {
                    try
                    {
                        var item = (CachedImage)s;
                        var gets = item.GestureRecognizers;
                        // To prevent double tap on images
                        lock (_lockObject)
                        {
                            if (_isRunningItem)
                                return;
                            else
                                _isRunningItem = true;
                        }

                        string itemId = (string)((TapGestureRecognizer)gets[0]).CommandParameter;

                        if (itemId != "")
                            await Navigation.PushAsync(new ItemPage(itemId));

                        _isRunningItem = false;
                    }
                    catch (Exception)
                    {
                        await DisplayAlert("שגיאה", "לא ניתן לטעון עמוד מבוקש.", "אישור");
                    }
                };

                image.GestureRecognizers.Add(tap);
                StackUserItems.Children.Add(image);
            }
        }

        //pop up event for image preview click

        private async void OpenPopUp()
        {
            try
            {
                var page = new ImagePopUp(_images);
                await Navigation.PushPopupAsync(page);
            }
            catch (Exception){
            }

        }
    }
}