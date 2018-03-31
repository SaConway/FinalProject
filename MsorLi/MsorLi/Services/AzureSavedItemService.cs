using MsorLi.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MsorLi.Services
{
    class AzureSavedItemService : AzureService<SavedItem>
    {
        //---------------------------------
        // MEMBERS
        //---------------------------------

        static AzureSavedItemService _defaultInstance = new AzureSavedItemService();

        public static AzureSavedItemService DefaultManager
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

        //public async Task<List<string>> GetAllCategories()
        //{
        //    try
        //    {
        //        var categories = await _table
        //            .Select(Category => Category.Name)
        //            .ToListAsync();

        //        return categories;
        //    }

        //    catch (Exception) { }
        //    return null;
        //}
    }
}