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

        public async Task<ObservableCollection<ItemImage>> GetAllPriorityImages(string category)
        {
            try
            {
                IEnumerable<ItemImage> images = null;

                if (category == "הכל")
                {
                    images = await _table
                    .OrderByDescending(ItemImage => ItemImage.CreatedAt)
                    .Where(itemImage => itemImage.IsPriorityImage == true)
                    .ToEnumerableAsync();
                }
                else
                {
                    images = await _table
                    .OrderByDescending(ItemImage => ItemImage.CreatedAt)
                    .Where(itemImage => itemImage.IsPriorityImage == true && itemImage.Category == category)
                    .ToEnumerableAsync();
                }
                

                return images != null ? new ObservableCollection<ItemImage>(images) : null;
            }

            catch (Exception) { }
            return null;
        }

        public async Task<ObservableCollection<ItemImage>> GetAllImgByUserId(string userId)
        {
            try
            {
                IEnumerable<ItemImage> images = await _table
                    .OrderByDescending(ItemImage => ItemImage.CreatedAt)
                    .Where(itemImage => itemImage.IsPriorityImage == true && itemImage.UserId == userId)
                    .ToEnumerableAsync();

                return new ObservableCollection<ItemImage>(images);
            }

            catch (Exception) { }
            return null;
        }

        public async Task<string> GetImageUrl(string itemId)
        {
            try
            {
                var imageUrl = await _table
                    .Where(i => i.ItemId == itemId && i.IsPriorityImage == true)
                    .Select(i => i.Url)
                    .ToListAsync();

                if (imageUrl.Count == 0) return null;
                return imageUrl[0];
            }

            catch (Exception) { }
            return null;
        
        }

        public async Task DeleteImage(ItemImage itemImage)
        {
            await _table.DeleteAsync(itemImage);
        }
    }
}