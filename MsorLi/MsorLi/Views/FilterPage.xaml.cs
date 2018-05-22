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

        bool _firstAppearing = true;
        string _erea = "";
        string _category = "";
        string _subCategory = "";

        //---------------------------------------------------
        // FUNCTIONS
        //---------------------------------------------------

        public FilterPage(string category, string subCategory, 
                    string condition, string erea)
		{
			InitializeComponent();

            ConditionPicker.SelectedItem = condition;
            _subCategory = subCategory;
            _erea = erea;
            _category = category;
        }

        protected async override void OnAppearing()
        {
            try
            {
                if (_firstAppearing)
                {
                    _firstAppearing = false;

                    // Categories

                    var categories = await Utilities.CategoryStorage.GetCategories();

                    foreach (var c in categories)
                    {
                        CategoryPicker.Items.Add(c.Name);
                    }

                    CategoryPicker.SelectedItem = _category;

                    // Locations

                    var ereas = await Utilities.LocationStorage.GetLocations();

                    foreach (var e in ereas)
                    {
                        EreaPicker.Items.Add(e.Name);
                    }

                    EreaPicker.SelectedItem = _erea;

                    if (CategoryPicker.SelectedIndex != -1 ||
                        ConditionPicker.SelectedIndex != -1 ||
                        EreaPicker.SelectedIndex != -1)
                    {
                        EnableSubmit();
                    }

                    if (_subCategory != "")
                    {
                        await SetSubCategories();
                        SubCategoryPicker.SelectedItem = _subCategory;
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        // Event Functions
        //---------------------------------------------------

        private async void OnCategoryChanged(object sender, EventArgs e)
        {
            await SetSubCategories();
        }

        private void OnConditionChanged(object sender, EventArgs e)
        {
            EnableSubmit();
        }

        private void OnEreaChanged(object sender, EventArgs e)
        {
            EnableSubmit();
        }

        private async void OnFilterClick(object sender, EventArgs e)
        {
            try
            {
                var category = CategoryPicker.SelectedIndex != -1 ? 
                    CategoryPicker.Items[CategoryPicker.SelectedIndex].ToString() : "";

                var subCategory = SubCategoryPicker.SelectedIndex != -1 ?
                    SubCategoryPicker.Items[SubCategoryPicker.SelectedIndex].ToString() : "";

                var condition = ConditionPicker.SelectedIndex != -1 ?
                    ConditionPicker.Items[ConditionPicker.SelectedIndex].ToString() : "";

                var erea = EreaPicker.SelectedIndex != -1 ?
                    EreaPicker.Items[EreaPicker.SelectedIndex].ToString() : "";

                var filterResult = new Tuple<string, string, string, string>
                    (category, subCategory, condition, erea);

                _subCategory = subCategory;

                await Navigation.PopAsync();
                MessagingCenter.Send<FilterPage, Tuple<string, string, string, string>>
                    (this, "Back From Filter", filterResult);
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
            EnableSubmit();
        }

        private void EnableSubmit()
        {
            SearchBtn.IsEnabled = true;
            SearchBtn.BackgroundColor = Color.FromHex("19a4b4");
        }
    }
}