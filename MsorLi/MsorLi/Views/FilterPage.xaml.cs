using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MsorLi.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FilterPage : ContentPage
	{
        //---------------------------------------------------
        // MEMBERS
        //---------------------------------------------------

        string _subCategory = "";

        //---------------------------------------------------
        // FUNCTIONS
        //---------------------------------------------------

        public FilterPage(string category, string subCategory)
		{
			InitializeComponent ();

            var categories = Utilities.CategoryStorage.GetCategories().Result;

            foreach (var c in categories)
            {
                CategoryPicker.Items.Add(c.Name);
            }

            CategoryPicker.SelectedItem = category;
            _subCategory = subCategory;
        }

        protected async override void OnAppearing()
        {
            if (_subCategory != "")
            {
                await SetSubCategories();
                SubCategoryPicker.SelectedItem = _subCategory;
            }
        }

        // Event Functions
        //---------------------------------------------------

        private async void OnCategoryChanged(object sender, EventArgs e)
        {
            await SetSubCategories();
        }

        private async void OnFilterClick(object sender, EventArgs e)
        {
            try
            {
                var category = CategoryPicker.Items[CategoryPicker.SelectedIndex].ToString();
                var subCategory = SubCategoryPicker.SelectedIndex != -1 ? SubCategoryPicker.Items[SubCategoryPicker.SelectedIndex].ToString() : "";
                var filterResult = new Tuple<string, string>(category, subCategory);

                _subCategory = subCategory;

                await Navigation.PopAsync();
                MessagingCenter.Send<FilterPage, Tuple<string, string>>(this, "Back From Filter", filterResult);
            }
            catch (Exception)
            {

            }
        }

        private async void OnCancelClick(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PopAsync();
            }
            catch (Exception)
            {

            }
        }

        // Private Functions
        //---------------------------------------------------

        private async Task SetSubCategories()
        {
            var subCategories = await Services.AzureSubCategoryService.DefaultManager.
                GetCategories(CategoryPicker.Items[CategoryPicker.SelectedIndex]);

            SubCategoryPicker.Items.Clear();
            foreach (var sc in subCategories)
            {
                SubCategoryPicker.Items.Add(sc.Name);
            }

            SubCategoryPicker.IsEnabled = true;
            SearchBtn.IsEnabled = true;

            SearchBtn.IsEnabled = true;
            SearchBtn.BackgroundColor = Color.FromHex("19a4b4");
        }
    }
}