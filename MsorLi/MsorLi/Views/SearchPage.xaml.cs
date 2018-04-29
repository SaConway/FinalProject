using System;
using Xamarin.Forms;

namespace MsorLi.Views
{
    public partial class SearchPage : ContentPage
    {
        //---------------------------------------------------
        // MEMBERS
        //---------------------------------------------------


        //---------------------------------------------------
        // FUNCTIONS
        //---------------------------------------------------

        public SearchPage()
        {
            InitializeComponent();

            var categories = Utilities.CategoryStorage.GetCategories().Result;

            foreach (var c in categories)
            {
                CategoryPicker.Items.Add(c.Name);
            }
        }

        // Event Functions

        public async void OnCategoryChanged(object sender, EventArgs e)
        {
            var subCategories = await Services.AzureSubCategoryService.DefaultManager.
                GetCategories(CategoryPicker.Items[CategoryPicker.SelectedIndex]);

            foreach (var sc in subCategories)
            {
                SubCategoryPicker.Items.Add(sc.Name);
            }

            SubCategoryPicker.IsEnabled = true;
            SearchBtn.IsEnabled = true;
        }

        public async void OnSearchClick(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception)
            {

            }
        }
    }
}