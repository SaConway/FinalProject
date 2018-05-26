using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using FFImageLoading.Forms;
using MsorLi.Models;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;


namespace MsorLi.Views
{
    public partial class ImagePopUp : Rg.Plugins.Popup.Pages.PopupPage
    {
        //---------------------------------
        // MEMBERS
        //---------------------------------
        ObservableCollection<Models.Image> images; 

        public ImagePopUp(ObservableCollection<ItemImage> _images)
        {

            InitializeComponent();

            CarouselContainer.WidthRequest = Utilities.Constants.ScreenWidth;
            CarouselContainer.HeightRequest = Utilities.Constants.ScreenHeight / 2;

            images = new ObservableCollection<Models.Image>();

            for (int i = 0; i < _images.Count; ++i)
            {

                Models.Image image = new Models.Image { ImageUrl = _images[i].Url, ImageNumber = (i + 1).ToString() + " מתוך " + _images.Count.ToString() };

                images.Add(image);
            }

            imagesView.ItemsSource = images;

        }

        //---------------------------------
        // FUNCTIONS
        //---------------------------------

        protected override bool OnBackgroundClicked()
        {
            CloseAllPopup();

            return false;
        }

        private async void CloseAllPopup()
        {
            await Navigation.PopAllPopupAsync();
        }

    }
}
