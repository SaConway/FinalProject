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

        public async Task<ObservableCollection<SavedItem>> GetAllSavedOfUser(string userId)
        {
            try
            {
                var savedItems = await _table
                    .Where(Saved => Saved.UserId == userId)
                    .ToEnumerableAsync();

                return new ObservableCollection<SavedItem>(savedItems);
            }

            catch (Exception) { }
            return null;
        }

        public async Task<bool> DeleteSavedItem(SavedItem savedItem)
        {
            try
            {
                await _table.DeleteAsync(savedItem);
                return true;
            }

            catch (Exception)
            {
                return false;
            }
        }

        // Return ItemSaved id if exist
        public async Task<string> IsItemSaved(string itemId, string userId)
        {
            try
            {
                var mySavedItem = await _table
                    .Where(Saved => Saved.ItemId == itemId && Saved.UserId == userId)
                    .ToListAsync();

                return mySavedItem.Count == 0 ? "" : mySavedItem[0].Id;
            }

            catch (Exception)
            {
                return "";
            }
        }

        public async Task<int> NumOfItemsSavedByUser(string userId)
        {
            try
            {
                var list = await _table
                    .Where(Saved => Saved.UserId == userId)
                    .ToListAsync();

                return list.Count;
            }

            catch (Exception)
            {
                return 0;
            }
        }
    }
}