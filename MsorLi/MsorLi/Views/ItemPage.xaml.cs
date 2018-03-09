using MsorLi.Models;
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
            InitializeComponent();

            title.Text = item.Title;
            image.Source = item.ImageUrl;
            description.Text = item.Description;
            condition.Text = item.Condition;
            location.Text = item.Location;
            contact_name.Text = item.ContactName;
            contact_number.Text = item.ContactNumber;
            date.Text = item.Date;
        }
    }
}
