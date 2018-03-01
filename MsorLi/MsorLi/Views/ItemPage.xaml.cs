using MsorLi.Models;
using Xamarin.Forms;

namespace MsorLi.Views
{
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
            InitializeComponent();
		
            title.Text = item.Title;
			image.Source = item.ImageUrl;
        }
    }
}
