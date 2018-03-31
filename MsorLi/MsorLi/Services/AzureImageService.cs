using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
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
                    .OrderByDescending(ItemImage => ItemImage.CreatedAt)
                    .Where(itemImage => itemImage.IsPriorityImage == true)
                    .ToEnumerableAsync();

                return new ObservableCollection<ItemImage>(images);
            }

            catch (Exception) { }
            return null;
        }

        public async Task<List<string>> GetImageUrl(string itemId)
        {
            try
            {
                var imageUrl = await _table
                    .Where(i => i.ItemId == itemId && i.IsPriorityImage == true)
                    .Select(i => i.Url)
                    .ToListAsync();

                return imageUrl;
            }

            catch (Exception) { }
            return null;
        
        }
    }
}