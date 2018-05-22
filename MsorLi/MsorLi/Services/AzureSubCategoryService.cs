using MsorLi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MsorLi.Services
{
    class AzureSubCategoryService : AzureService<ItemSubCategory>
    {
        //---------------------------------
        // MEMBERS
        //---------------------------------

        static AzureSubCategoryService _defaultInstance = new AzureSubCategoryService();

        public static AzureSubCategoryService DefaultManager
        {
            get
            {
                return _defaultInstance;
            }
            private set
            {
                _defaultInstance = value;
            }
        }

        //---------------------------------
        // FUNCTIONS
        //---------------------------------

        public async Task<List<ItemSubCategory>> GetCategories(string mainCategory)
        {
            try
            {
                var subCategories = await _table
                    .Where(Sub => Sub.MainCategory == mainCategory)
                    .OrderBy(Sub => Sub.Order)
                    .ToListAsync();

                return subCategories;
            }

            catch (Exception)
            {
                return null;
            }
        }
    }
}