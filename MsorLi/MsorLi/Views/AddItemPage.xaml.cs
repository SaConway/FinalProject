using MsorLi.Models;
using MsorLi.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.IO;
using System.Text;

namespace MsorLi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddItemPage : ContentPage
    {
        //---------------------------------
        // MEMBERS
        //---------------------------------

        AzureService _azureService;
        Stream _s;

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
            _s = await DependencyService.Get<IPicturePicker>().GetImageStreamAsync();
            Stream _ss = _s;
            if (_s != null)
            {
                image.Source = ImageSource.FromStream(() => _s);
            }
            else
            {
                pickPictureButton.IsEnabled = true;
            }
        }

        // Submit button operation
        public async void OnAdd(object sender, EventArgs e)
        {
            var uploadedFilename = await BlobService.UploadFileAsync(_s);

            //var temp_item = new Item {  Title= name.Text, ImageUrl = url.Text };
            //await InsertToList(temp_item);
        }

        async Task InsertToList(Item item)
        {
            await _azureService.UploadItemToServer(item);
        }
    }
}
