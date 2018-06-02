using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MsorLi.Models;
using MsorLi.Utilities;

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
            if (await Connection.IsServerReachableAndRunning())
            {
                IEnumerable<ItemImage> images = await _table
                    .Where(itemImage => itemImage.ItemId == itemId)
                    .OrderByDescending(ItemImage => ItemImage.IsPriorityImage)
                    .ToEnumerableAsync();
                return new ObservableCollection<ItemImage>(images);
            }
            else
                throw new NoConnectionException();
        }


        public async Task<ObservableCollection<ItemImage>> GetAllPriorityImages
            (int pageIndex, string category, string subCategory, string condition, string erea)
        {
            try
            {
                IEnumerable<ItemImage> images = null;

                // All items
                if (category == "כל המוצרים" && subCategory == "" && condition == "" && erea == "")
                {
                    images = await _table
                    .OrderByDescending(ItemImage => ItemImage.CreatedAt)
                    .Where(itemImage => itemImage.IsPriorityImage == true)
                    .Skip(pageIndex * Constants.PAGE_SIZE).Take(Constants.PAGE_SIZE)
                    .ToEnumerableAsync();
                }
                // All items with condition
                else if (category == "כל המוצרים" && subCategory == "" && condition != "" && erea == "")
                {
                    images = await _table
                    .OrderByDescending(ItemImage => ItemImage.CreatedAt)
                    .Where(itemImage => itemImage.IsPriorityImage == true &&
                    itemImage.Condition == condition)
                    .Skip(pageIndex * Constants.PAGE_SIZE).Take(Constants.PAGE_SIZE)
                    .ToEnumerableAsync();
                }
                // All items with erea
                else if (category == "כל המוצרים" && subCategory == "" && condition == "" && erea != "")
                {
                    images = await _table
                    .OrderByDescending(ItemImage => ItemImage.CreatedAt)
                    .Where(itemImage => itemImage.IsPriorityImage == true &&
                    itemImage.Erea == erea)
                    .Skip(pageIndex * Constants.PAGE_SIZE).Take(Constants.PAGE_SIZE)
                    .ToEnumerableAsync();
                }
                // All items with condition and erea
                else if (category == "כל המוצרים" && subCategory == "" && condition != "" && erea != "")
                {
                    images = await _table
                    .OrderByDescending(ItemImage => ItemImage.CreatedAt)
                    .Where(itemImage => itemImage.IsPriorityImage == true &&
                    itemImage.Condition == condition && itemImage.Erea == erea)
                    .Skip(pageIndex * Constants.PAGE_SIZE).Take(Constants.PAGE_SIZE)
                    .ToEnumerableAsync();
                }
                // Category with sub category
                else if (category != "כל המוצרים" && subCategory != "" && condition == "" && erea == "")
                {
                    images = await _table
                    .OrderByDescending(ItemImage => ItemImage.CreatedAt)
                    .Where(itemImage => itemImage.IsPriorityImage == true &&
                    itemImage.Category == category && itemImage.SubCategory == subCategory)
                    .Skip(pageIndex * Constants.PAGE_SIZE).Take(Constants.PAGE_SIZE)
                    .ToEnumerableAsync();
                }
                // Category with sub category and erea
                else if (category != "כל המוצרים" && subCategory != "" && condition == "" && erea != "")
                {
                    images = await _table
                    .OrderByDescending(ItemImage => ItemImage.CreatedAt)
                    .Where(itemImage => itemImage.IsPriorityImage == true && itemImage.Erea == erea &&
                    itemImage.Category == category && itemImage.SubCategory == subCategory)
                    .Skip(pageIndex * Constants.PAGE_SIZE).Take(Constants.PAGE_SIZE)
                    .ToEnumerableAsync();
                }
                // Category with condition
                else if (category != "כל המוצרים" && subCategory == "" && condition != "" && erea == "")
                {
                    images = await _table
                    .OrderByDescending(ItemImage => ItemImage.CreatedAt)
                    .Where(itemImage => itemImage.IsPriorityImage == true && 
                    itemImage.Category == category && itemImage.Condition == condition)
                    .Skip(pageIndex * Constants.PAGE_SIZE).Take(Constants.PAGE_SIZE)
                    .ToEnumerableAsync();
                }
                // Category with erea
                else if (category != "כל המוצרים" && subCategory == "" && condition == "" && erea != "")
                {
                    images = await _table
                    .OrderByDescending(ItemImage => ItemImage.CreatedAt)
                    .Where(itemImage => itemImage.IsPriorityImage == true &&
                    itemImage.Category == category && itemImage.Erea == erea)
                    .Skip(pageIndex * Constants.PAGE_SIZE).Take(Constants.PAGE_SIZE)
                    .ToEnumerableAsync();
                }
                // Category with erea and condition
                else if (category != "כל המוצרים" && subCategory == "" && condition != "" && erea != "")
                {
                    images = await _table
                    .OrderByDescending(ItemImage => ItemImage.CreatedAt)
                    .Where(itemImage => itemImage.IsPriorityImage == true &&
                    itemImage.Category == category && itemImage.Erea == erea && itemImage.Condition == condition)
                    .Skip(pageIndex * Constants.PAGE_SIZE).Take(Constants.PAGE_SIZE)
                    .ToEnumerableAsync();
                }
                // Category with sub category and condition
                else if (category != "כל המוצרים" && subCategory != "" && condition != "" && erea == "")
                {
                    images = await _table
                    .OrderByDescending(ItemImage => ItemImage.CreatedAt)
                    .Where(itemImage => itemImage.IsPriorityImage == true && itemImage.Category == category && 
                    itemImage.SubCategory == subCategory && itemImage.Condition == condition)
                    .Skip(pageIndex * Constants.PAGE_SIZE).Take(Constants.PAGE_SIZE)
                    .ToEnumerableAsync();
                }
                // Category with sub category and condition and erea
                else if (category != "כל המוצרים" && subCategory != "" && condition != "" && erea != "")
                {
                    images = await _table
                    .OrderByDescending(ItemImage => ItemImage.CreatedAt)
                    .Where(itemImage => itemImage.IsPriorityImage == true && itemImage.Category == category &&
                    itemImage.SubCategory == subCategory && itemImage.Condition == condition && itemImage.Erea == erea)
                    .Skip(pageIndex * Constants.PAGE_SIZE).Take(Constants.PAGE_SIZE)
                    .ToEnumerableAsync();
                }
                // Only category
                else if (category != "כל המוצרים" && subCategory == "" && condition == "" && erea == "")
                {
                    images = await _table
                    .OrderByDescending(ItemImage => ItemImage.CreatedAt)
                    .Where(itemImage => itemImage.IsPriorityImage == true && itemImage.Category == category)
                    .Skip(pageIndex * Constants.PAGE_SIZE).Take(Constants.PAGE_SIZE)
                    .ToEnumerableAsync();
                }

                return images != null ? new ObservableCollection<ItemImage>(images) : null;
            }

            catch (Exception)
            {
                return null;
            }
        }

        public async Task<ObservableCollection<ItemImage>> GetAllImgByUserId(string userId)
        {
            if (await Connection.IsServerReachableAndRunning())
            {
                IEnumerable<ItemImage> images = await _table
                    .OrderByDescending(ItemImage => ItemImage.CreatedAt)
                    .Where(itemImage => itemImage.IsPriorityImage == true && itemImage.UserId == userId)
                    .ToEnumerableAsync();

                return new ObservableCollection<ItemImage>(images);
            }
            else
                throw new NoConnectionException();

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
            if (await Connection.IsServerReachableAndRunning())
                await _table.DeleteAsync(itemImage);
            else
                throw new NoConnectionException();

        }

		public async Task<int> NumOfItems(string category, string subCategory,
            string condition, string erea)
        {
            try
            {
                IEnumerable<ItemImage> images = null;

                // All items
                if (category == "כל המוצרים" && subCategory == "" && condition == "" && erea == "")
                {
                    images = await _table
                    .OrderByDescending(ItemImage => ItemImage.CreatedAt)
                    .Where(itemImage => itemImage.IsPriorityImage == true)
                    .ToEnumerableAsync();
                }
                // All items with condition
                else if (category == "כל המוצרים" && subCategory == "" && condition != "" && erea == "")
                {
                    images = await _table
                    .OrderByDescending(ItemImage => ItemImage.CreatedAt)
                    .Where(itemImage => itemImage.IsPriorityImage == true &&
                    itemImage.Condition == condition)
                    .ToEnumerableAsync();
                }
                // All items with erea
                else if (category == "כל המוצרים" && subCategory == "" && condition == "" && erea != "")
                {
                    images = await _table
                    .OrderByDescending(ItemImage => ItemImage.CreatedAt)
                    .Where(itemImage => itemImage.IsPriorityImage == true &&
                    itemImage.Erea == erea)
                    .ToEnumerableAsync();
                }
                // All items with condition and erea
                else if (category == "כל המוצרים" && subCategory == "" && condition != "" && erea != "")
                {
                    images = await _table
                    .OrderByDescending(ItemImage => ItemImage.CreatedAt)
                    .Where(itemImage => itemImage.IsPriorityImage == true &&
                    itemImage.Condition == condition && itemImage.Erea == erea)
                    .ToEnumerableAsync();
                }
                // Category with sub category
                else if (category != "כל המוצרים" && subCategory != "" && condition == "" && erea == "")
                {
                    images = await _table
                    .OrderByDescending(ItemImage => ItemImage.CreatedAt)
                    .Where(itemImage => itemImage.IsPriorityImage == true &&
                    itemImage.Category == category && itemImage.SubCategory == subCategory)
                    .ToEnumerableAsync();
                }
                // Category with sub category and erea
                else if (category != "כל המוצרים" && subCategory != "" && condition == "" && erea != "")
                {
                    images = await _table
                    .OrderByDescending(ItemImage => ItemImage.CreatedAt)
                    .Where(itemImage => itemImage.IsPriorityImage == true && itemImage.Erea == erea &&
                    itemImage.Category == category && itemImage.SubCategory == subCategory)
                    .ToEnumerableAsync();
                }
                // Category with condition
                else if (category != "כל המוצרים" && subCategory == "" && condition != "" && erea == "")
                {
                    images = await _table
                    .OrderByDescending(ItemImage => ItemImage.CreatedAt)
                    .Where(itemImage => itemImage.IsPriorityImage == true &&
                    itemImage.Category == category && itemImage.Condition == condition)
                    .ToEnumerableAsync();
                }
                // Category with erea
                else if (category != "כל המוצרים" && subCategory == "" && condition == "" && erea != "")
                {
                    images = await _table
                    .OrderByDescending(ItemImage => ItemImage.CreatedAt)
                    .Where(itemImage => itemImage.IsPriorityImage == true &&
                    itemImage.Category == category && itemImage.Erea == erea)
                    .ToEnumerableAsync();
                }
                // Category with erea and condition
                else if (category != "כל המוצרים" && subCategory == "" && condition != "" && erea != "")
                {
                    images = await _table
                    .OrderByDescending(ItemImage => ItemImage.CreatedAt)
                    .Where(itemImage => itemImage.IsPriorityImage == true &&
                    itemImage.Category == category && itemImage.Erea == erea && itemImage.Condition == condition)
                    .ToEnumerableAsync();
                }
                // Category with sub category and condition
                else if (category != "כל המוצרים" && subCategory != "" && condition != "" && erea == "")
                {
                    images = await _table
                    .OrderByDescending(ItemImage => ItemImage.CreatedAt)
                    .Where(itemImage => itemImage.IsPriorityImage == true && itemImage.Category == category &&
                    itemImage.SubCategory == subCategory && itemImage.Condition == condition)
                    .ToEnumerableAsync();
                }
                // Category with sub category and condition and erea
                else if (category != "כל המוצרים" && subCategory != "" && condition != "" && erea != "")
                {
                    images = await _table
                    .OrderByDescending(ItemImage => ItemImage.CreatedAt)
                    .Where(itemImage => itemImage.IsPriorityImage == true && itemImage.Category == category &&
                    itemImage.SubCategory == subCategory && itemImage.Condition == condition && itemImage.Erea == erea)
                    .ToEnumerableAsync();
                }
                // Only category
                else if (category != "כל המוצרים" && subCategory == "" && condition == "" && erea == "")
                {
                    images = await _table
                    .OrderByDescending(ItemImage => ItemImage.CreatedAt)
                    .Where(itemImage => itemImage.IsPriorityImage == true && itemImage.Category == category)
                    .ToEnumerableAsync();
                }
                // Only erea
                else if (category == "כל המוצרים" && subCategory == "" && condition == "" && erea != "")
                {
                    images = await _table
                    .OrderByDescending(ItemImage => ItemImage.CreatedAt)
                    .Where(itemImage => itemImage.IsPriorityImage == true && itemImage.Erea == erea)
                    .ToEnumerableAsync();
                }

                var item_list = new ObservableCollection<ItemImage>(images);
                return item_list.Count;
            }
            catch
            {
                return 0;
            }
        }
    }
}