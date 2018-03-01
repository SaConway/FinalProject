using MsorLi.Models;
using MsorLi.Services;
using System;
using System.Threading.Tasks;
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

                //image.Source = ImageSource.FromStream(() => _s);
            }
            else
            {
                pickPictureButton.IsEnabled = true;
            }
        }

        // Add Item button operation
        public async void OnAdd(object sender, EventArgs e)
        {
            var uploadedFilename = await BlobService.UploadFileAsync(_imageStream);
            var imageUrl = "https://msorli.blob.core.windows.net/images/" + uploadedFilename;

            var temp_item = new Item {  Title = name.Text, ImageUrl = imageUrl };
            await InsertToList(temp_item);
        }

        async Task InsertToList(Item item)
        {
            await _azureService.UploadItemToServer(item);
        }
    }
}
