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
            description.Text = item.Description;
            condition.Text = item.Condition;
            location.Text = item.Location;
            view_counter.Text = item.ViewCounter.ToString();
            contact_name.Text = item.ContactName;
            contact_number.Text = item.ContactNumber;
            date.Text = item.Date;
            time.Text = item.Time;
        }
    }
}
