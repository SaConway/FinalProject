using MsorLi.Models;
using System;
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

        public async Task<ObservableCollection<string>> GetAllSavedOfUser(string userId)
        {
            try
            {
                var savedItems = await _table
                    .Where(Saved => Saved.UserId == userId)
                    .Select(Saved => Saved.ItemId)
                    .ToEnumerableAsync();

                return new ObservableCollection<string>(savedItems);
            }

            catch (Exception) { }
            return null;
        }
    }
}