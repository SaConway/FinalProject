using MsorLi.Models;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MsorLi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemPage : ContentPage
    {
        //---------------------------------
        // MEMBERS
        //---------------------------------



        //---------------------------------
        // FUNCTIONS
        //---------------------------------

        public ItemPage(Item item)
        {
            try
            {
                InitializeComponent();
                UpdateItemDetails(item);
            }
            catch
            {

            }
        }

        private void UpdateItemDetails(Item item)
        {
            title.Text = item.Title;

            ObservableCollection<Models.Image> images = new ObservableCollection<Models.Image>();

            if (item.ImageUrl_1 != "")
            {
                Models.Image image = new Models.Image { ImageUrl = item.ImageUrl_1, ImageNumber = "1 of " + item.NumOfImages.ToString() };
                images.Add(image);
            }
            if (item.ImageUrl_2 != "")
            {
                Models.Image image = new Models.Image { ImageUrl = item.ImageUrl_2, ImageNumber = "2 of " + item.NumOfImages.ToString() };
                images.Add(image);
            }
            if (item.ImageUrl_3 != "")
            {
                Models.Image image = new Models.Image { ImageUrl = item.ImageUrl_3, ImageNumber = "3 of " + item.NumOfImages.ToString() };
                images.Add(image);
            }
            if (item.ImageUrl_4 != "")
            {
                Models.Image image = new Models.Image { ImageUrl = item.ImageUrl_4, ImageNumber = "4 of " + item.NumOfImages.ToString() };
                images.Add(image);
            }

            imagesView.ItemsSource = images;

            description.Text = item.Description;
            condition.Text = item.Condition;
            location.Text = item.Location;
            contact_name.Text = item.ContactName;
            contact_number.Text = item.ContactNumber;
            date.Text = item.Date;
        }
    }
}
