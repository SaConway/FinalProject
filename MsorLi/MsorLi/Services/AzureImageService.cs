using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MsorLi.Models;


namespace MsorLi.Services
{
    public class AzureImageService : AzureService<ItemImage>
    {
        //---------------------------------
        // MEMBERS
        //---------------------------------

        static AzureImageService _defaultInstance = new AzureImageService();

        public static AzureImageService DefaultManager
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

        public async Task<ObservableCollection<ItemImage>> GetItemImages(string itemId)
        {
            try
            {
                IEnumerable<ItemImage> images = await _table
                    .Where(itemImage => itemImage.ItemId == itemId)
                    .ToEnumerableAsync();
                return new ObservableCollection<ItemImage>(images);
            }

            catch (Exception) { }
            return null;
        }

        public async Task<ObservableCollection<ItemImage>> GetAllPriorityImages()
        {
            try
            {
                IEnumerable<ItemImage> images = await _table
                    .ToEnumerableAsync();
                return new ObservableCollection<ItemImage>(images);
            }

            catch (Exception) { }
            return null;
        }
    }
}