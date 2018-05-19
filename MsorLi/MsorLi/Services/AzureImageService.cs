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
            try
            {
                IEnumerable<ItemImage> images = await _table
                    .Where(itemImage => itemImage.ItemId == itemId)
                    .OrderByDescending(ItemImage => ItemImage.IsPriorityImage)
                    .ToEnumerableAsync();
                return new ObservableCollection<ItemImage>(images);
            }

            catch (Exception) { }
            return null;
        }


        public async Task<ObservableCollection<ItemImage>> GetAllPriorityImages
            (int pageIndex, string category, string subCategory, string condition)
        {
            //var _TypeCategory = typeof(ItemImage);
            //var _PropCategory = _TypeCategory.GetProperty("Category");
            //var _ParamCategory = System.Linq.Expressions.Expression.Parameter(_TypeCategory, _PropCategory.Name);
            //var _LeftCategory = System.Linq.Expressions.Expression.PropertyOrField(_ParamCategory, _PropCategory.Name);
            //var _RightCategory = System.Linq.Expressions.Expression.Constant(category, _PropCategory.PropertyType);
            //var _BodyCategory = System.Linq.Expressions.Expression.Equal(_LeftCategory, _RightCategory);
            //var _WhereCategory = System.Linq.Expressions.Expression.Lambda<Func<ItemImage, bool>>(_BodyCategory, _ParamCategory);

            //var _TypeSubCategory = typeof(ItemImage);
            //var _PropSubCategory = _TypeSubCategory.GetProperty("SubCategory");
            //var _ParamSubCategory = System.Linq.Expressions.Expression.Parameter(_TypeSubCategory, _PropSubCategory.Name);
            //var _LeftSubCategory = System.Linq.Expressions.Expression.PropertyOrField(_ParamSubCategory, _PropSubCategory.Name);
            //var _RightSubCategory = System.Linq.Expressions.Expression.Constant(subCategory, _PropSubCategory.PropertyType);
            //var _BodySubCategory = System.Linq.Expressions.Expression.Equal(_LeftSubCategory, _RightSubCategory);
            //var _WhereSubCategory = System.Linq.Expressions.Expression.Lambda<Func<ItemImage, bool>>(_BodySubCategory, _ParamSubCategory);

            //var _TypeCondition = typeof(ItemImage);
            //var _PropCondition = _TypeCondition.GetProperty("Condition");
            //var _ParamCondition = System.Linq.Expressions.Expression.Parameter(_TypeCondition, _PropCondition.Name);
            //var _LeftCondition = System.Linq.Expressions.Expression.PropertyOrField(_ParamCondition, _PropCondition.Name);
            //var _RightCondition = System.Linq.Expressions.Expression.Constant(condition, _PropCondition.PropertyType);
            //var _BodyCondition = System.Linq.Expressions.Expression.Equal(_LeftCondition, _RightCondition);
            //var _WhereCondition = System.Linq.Expressions.Expression.Lambda<Func<ItemImage, bool>>(_BodyCondition, _ParamCondition);


            try
            {
                IEnumerable<ItemImage> images = null;

                // All items
                if (category == "כל המוצרים" && subCategory == "" && condition == "")
                {
                    images = await _table
                    .OrderByDescending(ItemImage => ItemImage.CreatedAt)
                    .Where(itemImage => itemImage.IsPriorityImage == true)
                    .Skip(pageIndex * Constants.PAGE_SIZE).Take(Constants.PAGE_SIZE)
                    .ToEnumerableAsync();
                }
                // All items with condition
                else if (category == "כל המוצרים" && subCategory == "" && condition != "")
                {
                    images = await _table
                    .OrderByDescending(ItemImage => ItemImage.CreatedAt)
                    .Where(itemImage => itemImage.IsPriorityImage == true &&
                    itemImage.Condition == condition)
                    .Skip(pageIndex * Constants.PAGE_SIZE).Take(Constants.PAGE_SIZE)
                    .ToEnumerableAsync();
                }
                // Category with sub category
                else if (category != "כל המוצרים" && subCategory != "" && condition == "")
                {
                    images = await _table
                    .OrderByDescending(ItemImage => ItemImage.CreatedAt)
                    .Where(itemImage => itemImage.IsPriorityImage == true &&
                    itemImage.Category == category && itemImage.SubCategory == subCategory)
                    .Skip(pageIndex * Constants.PAGE_SIZE).Take(Constants.PAGE_SIZE)
                    .ToEnumerableAsync();
                }
                // Category with condition
                else if (category != "כל המוצרים" && subCategory == "" && condition != "")
                {
                    images = await _table
                    .OrderByDescending(ItemImage => ItemImage.CreatedAt)
                    .Where(itemImage => itemImage.IsPriorityImage == true && 
                    itemImage.Category == category && itemImage.Condition == condition)
                    .Skip(pageIndex * Constants.PAGE_SIZE).Take(Constants.PAGE_SIZE)
                    .ToEnumerableAsync();
                }
                // Category with sub category and condition
                else if (category != "כל המוצרים" && subCategory != "" && condition != "")
                {
                    images = await _table
                    .OrderByDescending(ItemImage => ItemImage.CreatedAt)
                    .Where(itemImage => itemImage.IsPriorityImage == true && itemImage.Category == category && 
                    itemImage.SubCategory == subCategory && itemImage.Condition == condition)
                    .Skip(pageIndex * Constants.PAGE_SIZE).Take(Constants.PAGE_SIZE)
                    .ToEnumerableAsync();
                }
                // Only category
                else if (category != "כל המוצרים" && subCategory == "" && condition == "")
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

		public async Task<int> NumOfItems(string category, string subCategory, string condition)
        {
            try
            {
                IEnumerable<ItemImage> images = null;

                // All items
                if (category == "כל המוצרים" && subCategory == "" && condition == "")
                {
                    images = await _table
                    .OrderByDescending(ItemImage => ItemImage.CreatedAt)
                    .Where(itemImage => itemImage.IsPriorityImage == true)
                    .ToEnumerableAsync();
                }
                // All items with condition
                else if (category == "כל המוצרים" && subCategory == "" && condition != "")
                {
                    images = await _table
                    .OrderByDescending(ItemImage => ItemImage.CreatedAt)
                    .Where(itemImage => itemImage.IsPriorityImage == true &&
                    itemImage.Condition == condition)
                    .ToEnumerableAsync();
                }
                // Category with sub category
                else if (category != "כל המוצרים" && subCategory != "" && condition == "")
                {
                    images = await _table
                    .OrderByDescending(ItemImage => ItemImage.CreatedAt)
                    .Where(itemImage => itemImage.IsPriorityImage == true &&
                    itemImage.Category == category && itemImage.SubCategory == subCategory)
                    .ToEnumerableAsync();
                }
                // Category with condition
                else if (category != "כל המוצרים" && subCategory == "" && condition != "")
                {
                    images = await _table
                    .OrderByDescending(ItemImage => ItemImage.CreatedAt)
                    .Where(itemImage => itemImage.IsPriorityImage == true &&
                    itemImage.Category == category && itemImage.Condition == condition)
                    .ToEnumerableAsync();
                }
                // Category with sub category and condition
                else if (category != "כל המוצרים" && subCategory != "" && condition != "")
                {
                    images = await _table
                    .OrderByDescending(ItemImage => ItemImage.CreatedAt)
                    .Where(itemImage => itemImage.IsPriorityImage == true && itemImage.Category == category &&
                    itemImage.SubCategory == subCategory && itemImage.Condition == condition)
                    .ToEnumerableAsync();
                }
                // Only category
                else if (category != "כל המוצרים" && subCategory == "" && condition == "")
                {
                    images = await _table
                    .OrderByDescending(ItemImage => ItemImage.CreatedAt)
                    .Where(itemImage => itemImage.IsPriorityImage == true && itemImage.Category == category)
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