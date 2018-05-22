using MsorLi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MsorLi.Services
{
    class AzureLocationService : AzureService<Location>
    {
        //---------------------------------
        // MEMBERS
        //---------------------------------

        static AzureLocationService _defaultInstance = new AzureLocationService();

        public static AzureLocationService DefaultManager
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

        public async Task<List<Location>> GetLocations()
        {
            try
            {
                var locations = await _table
                    .OrderBy(location => location.Order)
                    .ToListAsync();

                return locations;
            }

            catch (Exception)
            {
                return null;
            }
        }
    }
}