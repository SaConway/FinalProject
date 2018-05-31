using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MsorLi.Models;


namespace MsorLi.Services
{
    public class AzureItemService : AzureService<Item>
    {
        //---------------------------------
        // MEMBERS
        //---------------------------------

        static AzureItemService _defaultInstance = new AzureItemService();

        public static AzureItemService DefaultManager
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

        public async Task<Item> GetItemAsync(string itemId)
        {
            try
            {
                Item item = await _table.LookupAsync(itemId);
                return item;
            }

            catch (Exception) { }
            return null;
        }

        public async Task DeleteItem(Item item)
        {
            await _table.DeleteAsync(item);
        }


        public async Task<int> NumOfItemsByUserId(string userId)
        {
            try
            {
                var list = await _table
                    .Where(Item => Item.UserId == userId)
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