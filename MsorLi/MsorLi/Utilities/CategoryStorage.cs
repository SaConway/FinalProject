using MsorLi.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MsorLi.Utilities
{
    class CategoryStorage
    {
        //---------------------------------
        // MEMBERS
        //---------------------------------

        private static List<Models.ItemCategory> _categories = null;

        //---------------------------------
        // FUNCTIONS
        //---------------------------------

        public static async Task<List<Models.ItemCategory>> GetCategories()
        {
            if (_categories == null)
            {
                await LoadCategories();
            }
            return _categories;
        }

        private static async Task LoadCategories()
        {
            _categories = new List<Models.ItemCategory>();
            _categories = await AzureCategoryService.DefaultManager.GetAllCategories();
        }
    }
}