using MsorLi.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using MsorLi.Models;

namespace MsorLi.Utilities
{
    class SubCategoryStorage
    {
        //---------------------------------
        // MEMBERS
        //---------------------------------

        private static Dictionary<string, List<ItemSubCategory>> _subCategories = 
            new Dictionary<string, List<ItemSubCategory>>();

        //---------------------------------
        // FUNCTIONS
        //---------------------------------

        public static async Task<List<ItemSubCategory>> GetSubCategories(string mainCategory)
        {
            if (!_subCategories.ContainsKey(mainCategory))
            {
                await LoadSubCategories(mainCategory);
            }
            return _subCategories[mainCategory];
        }

        private static async Task LoadSubCategories(string mainCategory)
        {
            if (await Connection.IsServerReachableAndRunning())
            {
                var subCategories = await AzureSubCategoryService.DefaultManager.GetCategories(mainCategory);

                _subCategories.Add(mainCategory, subCategories);
            }
            else
                throw new NoConnectionException();
        }
    }
}