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

        public async Task<ObservableCollection<ItemImage>> GetImagesAsync(bool syncImages = false)
        {
            try
            {
                IEnumerable<ItemImage> Images = await _table.ToEnumerableAsync();
                return new ObservableCollection<ItemImage>(Images);
            }

            catch (Exception) {}
            return null;
        }
    }
}