﻿using System;
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

        public async Task<ObservableCollection<Item>> GetItemsAsync(bool syncItems = false)
        {
            try
            {
                IEnumerable<Item> items = await _table.ToEnumerableAsync();
                return new ObservableCollection<Item>(items);
            }

            catch (Exception) {}
            return null;
        }
    }
}